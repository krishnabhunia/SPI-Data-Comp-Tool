"""Inventory exact restored NuGet metadata; do not infer rights or a project license.

Run after nuget restore. No network access or third-party Python packages are used.
"""

from __future__ import annotations

import argparse
import hashlib
from pathlib import Path, PurePosixPath
import re
import xml.etree.ElementTree as ET
import zipfile


ROOT = Path(__file__).resolve().parents[1]


def local_name(tag: str) -> str:
    return tag.rsplit("}", 1)[-1]


def text(node: ET.Element | None) -> str:
    return "" if node is None else "".join(node.itertext()).strip()


def field(metadata: ET.Element, name: str) -> ET.Element | None:
    return next((item for item in metadata if local_name(item.tag) == name), None)


def safe_component(value: str, label: str) -> str:
    if not re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9_.+\-]*", value):
        raise ValueError(f"Invalid NuGet {label}: {value!r}")
    return value


def package_file(directory: Path, relative: str) -> Path:
    # NuGet manifests can use either separator. Never read outside the package,
    # including through a symbolic link or a Windows drive/UNC path.
    relative = relative.replace("\\", "/")
    parts = PurePosixPath(relative)
    if not relative or parts.is_absolute() or ":" in relative or ".." in parts.parts:
        raise ValueError(f"Unsafe package license path: {relative!r}")
    result = (directory / Path(*parts.parts)).resolve()
    if not result.is_relative_to(directory.resolve()) or not result.is_file():
        raise ValueError(f"Missing or unsafe package license file: {relative!r}")
    return result


def read_notice(path: Path) -> str:
    raw = path.read_bytes()
    if raw.startswith((b"\xff\xfe", b"\xfe\xff")):
        return raw.decode("utf-16")
    return raw.decode("utf-8-sig")


def read_nuspec(directory: Path, package_name: str) -> bytes:
    """NuGet.exe may preserve only the nupkg, not a loose nuspec manifest."""
    maximum_bytes = 1048576
    specs = [entry for entry in directory.iterdir() if entry.suffix.casefold() == ".nuspec"]
    if len(specs) > 1:
        raise ValueError(f"Expected one nuspec for {package_name}; found {len(specs)}")
    if specs:
        spec = package_file(directory, specs[0].name)
        if spec.stat().st_size > maximum_bytes:
            raise ValueError(f"Oversized nuspec for {package_name}")
        return spec.read_bytes()
    archives = [entry for entry in directory.iterdir()
                if entry.name.casefold() == f"{package_name}.nupkg".casefold()]
    if len(archives) != 1:
        raise ValueError(f"No loose nuspec or unique restored nupkg for {package_name}")
    archive = package_file(directory, archives[0].name)
    with zipfile.ZipFile(archive) as package:
        # Read only a root manifest; never extract archive-controlled paths.
        manifests = [entry for entry in package.infolist()
                     if not entry.is_dir() and "/" not in entry.filename and "\\" not in entry.filename
                     and ":" not in entry.filename and entry.filename.casefold().endswith(".nuspec")]
        if len(manifests) != 1:
            raise ValueError(f"Expected one root nuspec inside {package_name}.nupkg; found {len(manifests)}")
        entry = manifests[0]
        if entry.file_size > maximum_bytes:
            raise ValueError(f"Oversized nuspec inside {package_name}.nupkg")
        with package.open(entry) as source:
            content = source.read(maximum_bytes + 1)
        if len(content) > maximum_bytes or len(content) != entry.file_size:
            raise ValueError(f"Inconsistent nuspec size inside {package_name}.nupkg")
        return content


def read_package(packages_root: Path, package: ET.Element) -> dict[str, object]:
    package_id = safe_component(package.get("id", ""), "id")
    version = safe_component(package.get("version", ""), "version")
    name = f"{package_id}.{version}"
    directories = [entry for entry in packages_root.iterdir()
                   if entry.is_dir() and entry.name.casefold() == name.casefold()]
    if len(directories) != 1:
        raise ValueError(f"Expected one restored package directory for {name}; found {len(directories)}")
    directory = directories[0]
    if not directory.resolve().is_relative_to(packages_root.resolve()):
        raise ValueError(f"Package directory escapes restore root: {name}")
    spec_bytes = read_nuspec(directory, name)
    root = ET.fromstring(spec_bytes)
    metadata = next((item for item in root if local_name(item.tag) == "metadata"), None)
    if metadata is None:
        raise ValueError(f"Package {name} has no metadata")
    if text(field(metadata, "id")).casefold() != package_id.casefold() or text(field(metadata, "version")) != version:
        raise ValueError(f"Restored metadata does not match requested package {name}")
    license_node = field(metadata, "license")
    license_value = text(license_node)
    license_type = "" if license_node is None else license_node.get("type", "")
    license_url = text(field(metadata, "licenseUrl"))
    files: list[tuple[str, str]] = []
    if license_type == "file":
        path = package_file(directory, license_value)
        files.append((license_value.replace("\\", "/"), read_notice(path)))
    elif license_type and license_type != "expression":
        raise ValueError(f"Unsupported license metadata type for {name}: {license_type}")
    # Many older packages include a top-level notice even with licenseUrl/expression.
    # Preserve those notices as well; do not download mutable external license URLs.
    for path in sorted(directory.iterdir(), key=lambda item: item.name.casefold()):
        if path.is_file() and re.search(r"license|licence|notice|eula", path.name, re.I) and path.suffix.lower() in ("", ".txt", ".md", ".rtf"):
            if not any(path.resolve() == package_file(directory, name).resolve() for name, _ in files):
                checked = package_file(directory, path.name)
                files.append((path.name, read_notice(checked)))
    return {
        "id": package_id, "version": version,
        "scope": "Build only" if package.get("developmentDependency", "").lower() == "true" else "Application dependency",
        "authors": text(field(metadata, "authors")),
        "copyright": text(field(metadata, "copyright")),
        "license": f"{license_type}: {license_value}" if license_value else "URL only" if license_url else "Not declared; review required",
        "license_url": license_url, "project_url": text(field(metadata, "projectUrl")),
        "nuspec_sha256": hashlib.sha256(spec_bytes).hexdigest(), "files": files,
    }


def one_line(value: object) -> str:
    return str(value).replace("|", "\\|").replace("\r", " ").replace("\n", " ")


def generate(packages_config: Path, packages_root: Path) -> str:
    config = ET.parse(packages_config).getroot()
    packages = [entry for entry in config if local_name(entry.tag) == "package"]
    keys = [(entry.get("id", "").casefold(), entry.get("version", "")) for entry in packages]
    if len(keys) != len(set(keys)):
        raise ValueError("Duplicate package/version entries in packages.config")
    if not packages:
        raise ValueError("packages.config contains no packages")
    records = sorted((read_package(packages_root, package) for package in packages), key=lambda item: str(item["id"]).casefold())
    lines = [
        "# Third-party dependency notices", "",
        "Generated from the exact restored NuGet packages listed in packages.config. This inventory includes build-time tooling; it is not a claim that every listed package is shipped as a separate file.", "",
        "Package metadata and included notices are reproduced below. An SPDX expression or URL is recorded as declared by its package; missing or URL-only terms still require review. This document does not grant a license for SPI or establish commercial entitlements or publication rights.", "",
        "EPPlus use must match the owner's applicable entitlement and the vendor's terms. Setting LicenseContext.Commercial in code does not prove that entitlement.", "",
        "| Package | Version | Scope | Declared license metadata |",
        "| --- | --- | --- | --- |",
    ]
    for record in records:
        lines.append("| " + " | ".join(one_line(record[key]) for key in ("id", "version", "scope", "license")) + " |")
    for record in records:
        lines.extend(["", f"## {record['id']} {record['version']}", ""])
        for key, label in (("authors", "Authors"), ("copyright", "Copyright"), ("license_url", "Declared license URL"), ("project_url", "Project URL"), ("nuspec_sha256", "Nuspec SHA-256")):
            if record[key]:
                lines.append(f"- {label}: {one_line(record[key])}")
        for name, content in record["files"]:
            # Indentation preserves any Markdown or backtick fences in the original notice.
            lines.extend(["", f"### Included notice: {one_line(name)}", ""])
            lines.extend("    " + line for line in content.splitlines())
    return "\n".join(lines).rstrip() + "\n"


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--packages-config", type=Path, default=ROOT / "SPI Data Comp Tool" / "packages.config")
    parser.add_argument("--packages-root", type=Path, default=ROOT / "packages")
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = generate(args.packages_config, args.packages_root)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(result, encoding="utf-8")
    print(f"Generated notices for {len(ET.parse(args.packages_config).getroot())} restored NuGet packages.")


if __name__ == "__main__":
    main()
