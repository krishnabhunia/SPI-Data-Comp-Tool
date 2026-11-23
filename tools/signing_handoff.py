"""Validate an exact same-run signing handoff and updater-compatible final distribution."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil


def portable_paths(version):
    if not re.fullmatch(r"\d+\.\d+\.\d+(?:-beta\d{3}|-manual)?", version):
        raise ValueError("Invalid distribution version")
    return {f"portable/SPI_Data_Comp_Tool_{version}.exe",
            f"portable/SPI_Data_Comp_Tool_{version}.exe.config",
            "portable/PNID_Reports/Source/instrument.txt",
            "portable/PNID_Reports/Target_IPEP/instrument.txt"}


def handoff_paths(version):
    return portable_paths(version) | {"x64/SPI Data Comp Tool.exe", "x64/SPI Data Comp Tool.exe.config", "THIRD_PARTY_NOTICES.md"}


def check_files(root, expected):
    root = Path(root)
    if not root.is_dir() or root.is_symlink():
        raise ValueError("Missing or linked distribution root")
    entries = list(root.rglob("*"))
    if any(p.is_symlink() for p in entries):
        raise ValueError("Linked artifact entries are forbidden")
    actual = {p.relative_to(root).as_posix() for p in entries if p.is_file()}
    if actual != set(expected):
        raise ValueError(f"Unexpected artifact files: missing={sorted(set(expected)-actual)}, extra={sorted(actual-set(expected))}")
    if any((root / p).stat().st_size > 256 * 1024 * 1024 for p in expected):
        raise ValueError("Artifact file exceeds 256 MiB limit")
    for anchor in (p for p in expected if p.endswith("instrument.txt")):
        if (root / anchor).stat().st_size:
            raise ValueError("Portable compatibility anchors must be empty")


def digest(path):
    with Path(path).open("rb") as stream:
        value = hashlib.sha256()
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            value.update(block)
        return value.hexdigest()


def metadata(version, sha, run_id, attempt):
    portable_paths(version)
    if not re.fullmatch(r"[a-f0-9]{40}", sha) or not str(run_id).isdigit() or not str(attempt).isdigit():
        raise ValueError("Invalid build provenance")
    return {"schema": 1, "version": version, "source_sha": sha, "run_id": str(run_id), "run_attempt": str(attempt)}


def prepare(dist, x64, output, version, sha, run_id, attempt):
    record = metadata(version, sha, run_id, attempt)
    dist, x64, output = Path(dist), Path(x64), Path(output)
    check_distribution(dist, version)
    if output.exists():
        raise ValueError("Handoff output must be a fresh directory")
    output.mkdir(parents=True)
    for name in sorted(handoff_paths(version)):
        source = x64 / Path(name).name if name.startswith("x64/") else dist / name
        if source.is_symlink() or not source.is_file():
            raise ValueError("Missing or linked handoff source")
        target = output / name
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, target)
    check_files(output, handoff_paths(version))
    record["sha256"] = {p: digest(output / p) for p in sorted(handoff_paths(version))}
    (output / "handoff.json").write_text(json.dumps(record, indent=2) + "\n", encoding="utf-8")


def verify(root, version, sha, run_id, attempt):
    root = Path(root)
    check_files(root, handoff_paths(version) | {"handoff.json"})
    if (root / "handoff.json").stat().st_size > 16384:
        raise ValueError("Oversized handoff manifest")
    record = json.loads((root / "handoff.json").read_text(encoding="utf-8"))
    expected = metadata(version, sha, run_id, attempt)
    expected["sha256"] = {p: digest(root / p) for p in sorted(handoff_paths(version))}
    if record != expected:
        raise ValueError("Handoff provenance or SHA-256 mismatch")
    print(f"Verified exact same-run handoff: {sha}, run {run_id}/{attempt}")


def check_distribution(root, version):
    check_files(root, portable_paths(version) | {f"windows-x64/SPI_Data_Comp_Tool_{version}.exe", "THIRD_PARTY_NOTICES.md"})


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=("prepare", "verify", "distribution"))
    parser.add_argument("--root", type=Path, required=True)
    parser.add_argument("--version", required=True)
    parser.add_argument("--sha")
    parser.add_argument("--run-id")
    parser.add_argument("--attempt")
    parser.add_argument("--dist", type=Path)
    parser.add_argument("--x64", type=Path)
    args = parser.parse_args()
    if args.command == "prepare":
        prepare(args.dist, args.x64, args.root, args.version, args.sha, args.run_id, args.attempt)
    elif args.command == "verify":
        verify(args.root, args.version, args.sha, args.run_id, args.attempt)
    else:
        check_distribution(args.root, args.version)
        print("Final distribution has the exact updater-compatible platform layout")


if __name__ == "__main__":
    main()
