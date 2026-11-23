"""Automatic x.y.z versioning for CI.

  x = major update, y = minor update, z = bug / error fix.

Pull request  -> beta  vX.Y.Z-betaNNN (pre-release). NNN counts from 001 for each X.Y.Z across all
                 PRs; every new pushed commit gets the next number, a re-run of the same commit keeps
                 its number, and the number is reserved by tagging the PR's head commit (a failed
                 build's number is skipped).
Merge/push to master -> stable vX.Y.Z (latest release).

X.Y.Z = latest stable tag bumped by the change type. With no stable tag yet, the version in
Properties/AssemblyInfo.cs counts as the last release.

Change type (first match wins):
  1. PR label  release:major | release:minor | release:patch
  2. "[major]" / "[minor]" / "[patch]" in the PR title
  3. A line starting "BREAKING CHANGE:" in title/body, or a type ending in "!" -> major
  4. Type Feature / feat                                                  -> minor
  5. Anything else (Fix, Docs, CI, Refactor, Chore, no type ...)          -> patch

Commands (run from the repository root):
  python tools/release.py plan [--reserve]    writes version, tag, prerelease, bump to $GITHUB_OUTPUT
  python tools/release.py stamp VERSION       writes the version into Properties/AssemblyInfo.cs
  python tools/release.py comment STATE       prints the text CI keeps on the PR; STATE is beta, stable
                                              or outdated (a normal version of the same number came out
                                              while the beta was building, so the beta is not published)
Environment for plan: EVENT_NAME, PR_TITLE, PR_BODY, PR_LABELS (comma list), PR_NUMBER, HEAD_SHA,
  GITHUB_REPOSITORY, GH_TOKEN (to find the merged PR of a push).
Environment for comment: GITHUB_REPOSITORY, TAG, HEAD_SHA.

The version is never typed into a PR title: CI writes it on the PR (one comment, kept up to date)
and in the name of the Publish check.
"""
import json
import os
import re
import subprocess
import sys
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLY_INFO = ROOT / "SPI Data Comp Tool" / "Properties" / "AssemblyInfo.cs"
TAG_RE = re.compile(r"^v(\d+)\.(\d+)\.(\d+)(?:-beta(\d{3}))?$")
TYPE_RE = re.compile(r"^\s*(?:claude\s*:\s*)?([A-Za-z]+)(\([^)]*\))?(!)?\s*:", re.IGNORECASE)


# ---------- pure helpers (unit-tested) ----------

def parse_tag(tag):
    m = TAG_RE.match(tag.strip())
    if not m:
        return None
    x, y, z, beta = m.groups()
    return (int(x), int(y), int(z), int(beta) if beta else None)


def latest_stable(tags, baseline):
    stable = [p[:3] for p in map(parse_tag, tags) if p and p[3] is None]
    return max(stable) if stable else baseline


def bump_kind(title, body="", labels=()):
    labels = {l.strip().lower() for l in labels if l.strip()}
    for kind in ("major", "minor", "patch"):
        if f"release:{kind}" in labels:
            return kind
    low = (title or "").lower()
    for kind in ("major", "minor", "patch"):
        if f"[{kind}]" in low:
            return kind
    # a breaking-change note is a line that starts with "BREAKING CHANGE:" (Conventional Commits footer);
    # merely mentioning the words, e.g. in a PR description that explains these rules, does not count
    if re.search(r"^\s*BREAKING[ -]CHANGE\s*:", (title or "") + "\n" + (body or ""), re.MULTILINE):
        return "major"
    m = TYPE_RE.match(title or "")
    if m:
        word, bang = m.group(1).lower(), m.group(3)
        if bang or word == "breaking":
            return "major"
        if word in ("feature", "feat"):
            return "minor"
    return "patch"


def bump(version, kind):
    x, y, z = version
    if kind == "major":
        return (x + 1, 0, 0)
    if kind == "minor":
        return (x, y + 1, 0)
    return (x, y, z + 1)


def beta_number(tags, version, head_tags):
    """Number already on this commit for this version, else the next free one."""
    for t in head_tags:
        p = parse_tag(t)
        if p and p[:3] == version and p[3] is not None:
            return p[3], False
    used = [p[3] for p in map(parse_tag, tags) if p and p[:3] == version and p[3] is not None]
    n = max(used, default=0) + 1
    if n > 999:
        raise SystemExit(f"beta numbers for {fmt(version)} are used up (999)")
    return n, True


def fmt(version, beta=None):
    s = "{}.{}.{}".format(*version)
    return f"{s}-beta{beta:03d}" if beta is not None else s


def sort_key(tag):
    """Updaters rank x.y.z-betaNNN below x.y.z."""
    p = parse_tag(tag)
    return (p[0], p[1], p[2], 0 if p[3] is not None else 1, p[3] or 0)


def baseline_from_assembly_info(text):
    m = re.search(r'AssemblyFileVersion\("(\d+)\.(\d+)\.(\d+)', text)
    if not m:
        raise SystemExit("AssemblyFileVersion not found")
    return tuple(int(v) for v in m.groups())


def stamp_text(text, version):
    """Set AssemblyVersion / AssemblyFileVersion to x.y.z.0 and AssemblyInformationalVersion to the full name."""
    p = parse_tag("v" + version)
    if not p:
        raise SystemExit(f"bad version {version}")
    numeric = "{}.{}.{}.0".format(*p[:3])
    text = re.sub(r'\[assembly: AssemblyVersion\("[^"]*"\)\]', f'[assembly: AssemblyVersion("{numeric}")]', text)
    text = re.sub(r'\[assembly: AssemblyFileVersion\("[^"]*"\)\]', f'[assembly: AssemblyFileVersion("{numeric}")]', text)
    info = f'[assembly: AssemblyInformationalVersion("{version}")]'
    if "AssemblyInformationalVersion(" in text:
        text = re.sub(r'\[assembly: AssemblyInformationalVersion\("[^"]*"\)\]', info, text)
    else:
        text = text.rstrip("\n") + "\n" + info + "\n"
    return text


COMMENT_MARKER = "<!-- spi-version -->"


def release_assets(version):
    """(what it is, file name) of the files CI puts on a release page."""
    return [
        ("64-bit installer", f"SPI_Data_Comp_Tool_{version}_x64_setup.exe"),
        ("Portable + installer (zip)", f"SPI_Data_Comp_Tool_{version}.zip"),
        ("Checksums (SHA-256)", f"SPI_Data_Comp_Tool_{version}.zip.sha256"),
    ]


def signing_notice(status):
    if status == "unsigned":
        return ("**Windows signing: unsigned.** No trusted signing identity was used; "
                "the SmartScreen warning is not fixed. SHA-256 checksums verify download integrity, not publisher identity.")
    if status == "signed":
        return ("**Windows signing: signed and verified.** Authenticode publisher signatures and RFC 3161 timestamps "
                "were verified for this build. SmartScreen reputation is independent; warnings can still appear.")
    if status == "signed-app-and-setup":
        return ("**Windows signing: application and installer signed.** The SSL.com backend verified signatures "
                "and timestamps on the portable app, installed app and outer installer. Its uninstaller and temporary "
                "setup copies are not covered by this backend. SmartScreen warnings can still appear.")
    raise ValueError("Unknown Windows signing status")


def comment_body(state, repo, tag, sha, signing="unsigned"):
    """The text CI keeps on the pull request, so the version is read there and not typed into the title."""
    p = parse_tag(tag)
    if not p:
        raise SystemExit(f"bad tag {tag}")
    version, stable, short = tag[1:], "v" + fmt(p[:3]), sha[:7]
    base = f"https://github.com/{repo}/releases"
    lines = [COMMENT_MARKER]
    if state == "outdated":
        lines += [
            f"### No beta for commit `{short}`",
            "",
            f"`{stable}` was released while this build was running, so `{tag}` was **not published** "
            "(a beta must not appear after the normal version with the same number).",
            "",
            "Re-run the workflow (Re-run all jobs) or push a new commit to get the next beta.",
        ]
        return "\n".join(lines) + "\n"
    if state == "beta":
        lines += [f"### Beta version: `{tag}`", "", f"Built from commit `{short}` · pre-release · [release page]({base}/tag/{tag})"]
    elif state == "stable":
        lines += [f"### Released as `{tag}`", "", f"Merged to master as commit `{short}` · latest release · [release page]({base}/tag/{tag})"]
    else:
        raise SystemExit(f"unknown comment state {state}")
    lines += [
        "",
        signing_notice(signing),
        "",
        "**Requires Microsoft .NET Framework 4.8 or later.** Install the runtime before updating an older portable copy, "
        "or use the installer, which checks the prerequisite before changing application files. "
        "[Runtime download](https://dotnet.microsoft.com/download/dotnet-framework/net48).",
        "",
        "| SrNo. | Download | File |", "|---|---|---|",
    ]
    for i, (what, name) in enumerate(release_assets(version), 1):
        lines.append(f"| {i} | {what} | [{name}]({base}/download/{tag}/{name}) |")
    if state == "beta":
        lines += [
            "",
            "The normal version is generated when this PR is merged to master. Its number is worked out "
            "at merge time, so it can differ from this beta's number if another PR is merged first.",
        ]
    return "\n".join(lines) + "\n"


# ---------- git / GitHub ----------

def git(*args):
    return subprocess.run(["git", *args], cwd=ROOT, capture_output=True, text=True, check=True).stdout


def merged_pr(sha):
    repo, token = os.environ.get("GITHUB_REPOSITORY"), os.environ.get("GH_TOKEN")
    if not repo or not token:
        return None
    req = urllib.request.Request(f"https://api.github.com/repos/{repo}/commits/{sha}/pulls",
                                 headers={"Authorization": f"Bearer {token}", "Accept": "application/vnd.github+json"})
    try:
        with urllib.request.urlopen(req, timeout=30) as r:
            pulls = json.load(r)
    except Exception as exc:  # network trouble must not block a release; fall back to the commit message
        print(f"::warning::could not look up the merged PR: {exc}")
        return None
    merged = [p for p in pulls if p.get("merged_at")]
    return merged[0] if merged else None


def plan(reserve):
    event = os.environ.get("EVENT_NAME", "workflow_dispatch")
    head = os.environ.get("HEAD_SHA") or git("rev-parse", "HEAD").strip()
    title, body = os.environ.get("PR_TITLE", ""), os.environ.get("PR_BODY", "")
    labels = os.environ.get("PR_LABELS", "").split(",")
    pr_number = os.environ.get("PR_NUMBER", "") if event == "pull_request" else ""
    if event == "push":
        pr = merged_pr(head)
        if pr:
            pr_number = str(pr.get("number", ""))
            title, body = pr.get("title", ""), pr.get("body") or ""
            labels = [l["name"] for l in pr.get("labels", [])]
        else:
            msg = git("log", "-1", "--format=%B", head)
            title, body = msg.splitlines()[0] if msg else "", msg
    tags = [t for t in git("tag", "--list", "v*").split() if parse_tag(t)]
    base = latest_stable(tags, baseline_from_assembly_info(ASSEMBLY_INFO.read_text(encoding="utf-8-sig")))
    kind = bump_kind(title, body, labels)
    version = bump(base, kind)
    if event == "pull_request":
        head_tags = git("tag", "--points-at", head).split()
        for _ in range(5):
            n, new = beta_number(tags, version, head_tags)
            tag = "v" + fmt(version, n)
            if not (reserve and new):
                break
            try:
                git("tag", tag, head)
                git("push", "origin", f"refs/tags/{tag}")
                break
            except subprocess.CalledProcessError:
                # another PR took this number first: refresh and try the next one
                subprocess.run(["git", "tag", "-d", tag], cwd=ROOT, capture_output=True)
                git("fetch", "--tags", "--force", "origin")
                tags = [t for t in git("tag", "--list", "v*").split() if parse_tag(t)]
        else:
            raise SystemExit("could not reserve a beta number")
        full, pre = fmt(version, n), "true"
    elif event == "push":
        full, pre = fmt(version), "false"
    else:  # manual run: show what the next release would be, publish nothing
        full, pre = fmt(version) + "-manual", "true"
    out = {"version": full, "tag": "v" + full, "prerelease": pre, "bump": kind, "base": fmt(base), "head": head, "pr": pr_number}
    print(json.dumps(out, indent=1))
    gh_out = os.environ.get("GITHUB_OUTPUT")
    if gh_out:
        with open(gh_out, "a", encoding="utf-8") as f:
            for k, v in out.items():
                f.write(f"{k}={v}\n")


def main(argv):
    if len(argv) >= 2 and argv[1] == "plan":
        plan("--reserve" in argv)
    elif len(argv) == 3 and argv[1] == "stamp":
        text = ASSEMBLY_INFO.read_text(encoding="utf-8-sig")
        ASSEMBLY_INFO.write_text(stamp_text(text, argv[2].replace("-manual", "")), encoding="utf-8-sig")
        print(f"stamped {argv[2]}")
    elif len(argv) == 3 and argv[1] == "comment":
        sys.stdout.write(comment_body(argv[2], os.environ["GITHUB_REPOSITORY"], os.environ["TAG"], os.environ["HEAD_SHA"], os.environ.get("SIGNING_STATUS", "unsigned")))
    elif len(argv) == 2 and argv[1] == "notes":
        sys.stdout.write((ROOT / "docs/RELEASE_PREREQUISITES.md").read_text(encoding="utf-8"))
        sys.stdout.write("\n\n## Windows publisher signatures\n\n" + signing_notice(os.environ.get("SIGNING_STATUS", "unsigned")) + "\n")
    else:
        print(__doc__)
        return 2
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
