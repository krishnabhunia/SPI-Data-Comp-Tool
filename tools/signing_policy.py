"""Fail-closed release eligibility. No credentials are read or provisioned here."""
import os
import re
from urllib.parse import urlsplit


def plan(mode, event, ref, protected, thumbprint="", store="CurrentUser", timestamp="", isolated=""):
    mode = mode or "off"
    if mode not in ("off", "store"):
        raise ValueError("WINDOWS_SIGNING_MODE must be off or store")
    # Even same-repository PRs must never receive a signing runner or identity.
    if mode == "off" or event == "pull_request":
        return {"enabled": "false", "status": "unsigned"}
    if event != "push" or ref != "refs/heads/master" or protected != "true":
        raise ValueError("Signing requires a protected master push")
    if isolated != "true":
        raise ValueError("Confirm external release-only runner isolation before enabling signing")
    if not re.fullmatch(r"[0-9a-fA-F]{40}", thumbprint):
        raise ValueError("An exact 40-hex certificate thumbprint is required")
    if store not in ("CurrentUser", "LocalMachine"):
        raise ValueError("Certificate store must be CurrentUser or LocalMachine")
    url = urlsplit(timestamp)
    # Timestamp URLs are public configuration, never authentication endpoints.
    if (url.scheme not in ("http", "https") or not url.hostname or url.username or url.password
            or url.query or url.fragment or any(c in timestamp for c in '\r\n" $`')):
        raise ValueError("Use a public HTTP(S) RFC 3161 URL without credentials, queries or shell characters")
    return {"enabled": "true", "status": "signed"}


def main():
    result = plan(os.getenv("WINDOWS_SIGNING_MODE", ""), os.getenv("GITHUB_EVENT_NAME", ""),
                  os.getenv("GITHUB_REF", ""), os.getenv("REF_PROTECTED", ""),
                  os.getenv("WINDOWS_SIGNING_THUMBPRINT", ""), os.getenv("WINDOWS_SIGNING_STORE", "CurrentUser") or "CurrentUser",
                  os.getenv("WINDOWS_SIGNING_TIMESTAMP_URL", ""), os.getenv("WINDOWS_SIGNING_RUNNER_ISOLATED", ""))
    with open(os.environ["GITHUB_OUTPUT"], "a", encoding="utf-8") as output:
        for key, value in result.items():
            output.write(f"{key}={value}\n")
    print("Signing policy: " + ("eligible; verification required before publication" if result["enabled"] == "true"
                               else "unsigned; SmartScreen warning is not fixed"))


if __name__ == "__main__":
    main()
