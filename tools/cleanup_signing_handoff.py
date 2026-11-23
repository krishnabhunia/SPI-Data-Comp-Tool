"""Delete only this run's nonsecret signing input; never touch another run's artifacts."""
import json
import os
import re
import urllib.request


def main():
    repo, run, attempt, sha = (os.environ[k] for k in ("GITHUB_REPOSITORY", "GITHUB_RUN_ID", "GITHUB_RUN_ATTEMPT", "GITHUB_SHA"))
    if not re.fullmatch(r"[\w.-]+/[\w.-]+", repo) or not run.isdigit() or not attempt.isdigit() or not re.fullmatch(r"[a-f0-9]{40}", sha):
        raise ValueError("Invalid cleanup provenance")
    base = f"https://api.github.com/repos/{repo}"
    headers = {"Authorization": "Bearer " + os.environ["GH_TOKEN"], "Accept": "application/vnd.github+json", "User-Agent": "SPI-signing-cleanup"}

    def request(path, method="GET"):
        with urllib.request.urlopen(urllib.request.Request(base + path, headers=headers, method=method), timeout=30) as response:
            return json.load(response) if method == "GET" else None

    state = request(f"/actions/runs/{run}")
    if state["head_sha"] != sha or state["event"] != "push" or state["head_branch"] != "master":
        raise ValueError("Refusing cleanup outside the exact master release run")
    name = f"spi-signing-input-{run}-{attempt}"
    targets = []
    for page in range(1, 101):
        items = request(f"/actions/runs/{run}/artifacts?per_page=100&page={page}")["artifacts"]
        targets.extend(a["id"] for a in items if a["name"] == name)
        if len(items) < 100:
            break
    else:
        raise ValueError("Artifact pagination incomplete")
    for artifact_id in targets:
        request(f"/actions/artifacts/{int(artifact_id)}", "DELETE")
    print(f"Removed {len(targets)} temporary signing input artifact(s) from run {run}/{attempt}")


if __name__ == "__main__":
    main()
