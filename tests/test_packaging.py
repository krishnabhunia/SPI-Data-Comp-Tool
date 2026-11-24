"""Static contracts for packaging: one-exe build (Costura) and the 64-bit installer."""

import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
PROJ = REPO / "SPI Data Comp Tool"
ISS = REPO / "windows" / "installer" / "SPI_Data_Comp_Tool.iss"


def text(path: Path) -> str:
    return path.read_text(encoding="utf-8-sig")


class Packaging(unittest.TestCase):
    def test_costura_and_fody_versions_match_their_imports(self) -> None:
        versions = {p.get("id"): p.get("version") for p in ET.fromstring(text(PROJ / "packages.config"))}
        project = text(PROJ / "SPI Data Comp Tool.csproj")
        self.assertIn(rf"..\packages\Costura.Fody.{versions['Costura.Fody']}\build\Costura.Fody.props", project)
        self.assertIn(rf"..\packages\Costura.Fody.{versions['Costura.Fody']}\build\Costura.Fody.targets", project)
        self.assertIn(rf"..\packages\Fody.{versions['Fody']}\build\Fody.targets", project)

    def test_costura_weaver_is_on(self) -> None:
        weavers = ET.fromstring(text(PROJ / "FodyWeavers.xml"))
        self.assertIsNotNone(weavers.find("Costura"))

    def test_x64_release_is_a_real_64_bit_build(self) -> None:
        project = text(PROJ / "SPI Data Comp Tool.csproj")
        x64 = project.split("'Release|x64'", 1)[1].split("</PropertyGroup>", 1)[0]
        self.assertIn("<PlatformTarget>x64</PlatformTarget>", x64)
        self.assertIn("<OutputPath>bin\\x64\\Release\\</OutputPath>", x64)

    def test_installer_is_64_bit_with_a_stable_app_id(self) -> None:
        iss = text(ISS)
        self.assertRegex(iss, r"(?m)^AppId=\{\{DF6AFF7A-0A5A-4F1E-BF54-F5301F101D26\}$")   # never change: upgrades rely on it
        self.assertIn("ArchitecturesInstallIn64BitMode=x64compatible", iss)
        self.assertIn("ArchitecturesAllowed=x64compatible", iss)

    def test_installer_ships_application_configuration_and_dependency_notices(self) -> None:
        files = text(ISS).split("[Files]", 1)[1].split("[", 1)[0]
        sources = re.findall(r'Source: "([^"]+)"', files)
        self.assertEqual(sources, [r"{#SourceDir}\{#ExeName}", r"{#SourceDir}\{#ExeName}.config",
                                   r"{#SourceDir}\THIRD_PARTY_NOTICES.md"])

    def test_portable_is_one_exe_without_config_or_compatibility_files(self) -> None:
        workflow = text(REPO / ".github/workflows/build.yml")
        package = workflow.split("- name: Package portable build", 1)[1].split("- name: Portable build is one exe", 1)[0]
        self.assertNotIn('Copy-Item "$src/PNID_Reports"', package)
        self.assertNotIn("instrument.txt", package)
        self.assertNotIn(".exe.config", package)
        self.assertIn('Copy-Item "$src/SPI Data Comp Tool.exe" -Destination "$dst/$name.exe"', package)
        self.assertIn("portable/ must hold exactly one exe", package)
        self.assertIn("Retired SQL templates leaked into the build output", package)
        self.assertIn("Retired SQL templates leaked into the 64-bit build output", workflow)
        self.assertIn("Retired SQL templates or portable compatibility anchors leaked into the clean install", text(REPO / "tools/test_windows_installer.ps1"))
        alone = workflow.split("- name: Portable build is one exe", 1)[1].split("\n      - name:", 1)[0]
        for check in ("'--check-assemblies'", "'--check-ui'", "'--check-install-kind', 'portable'"):
            self.assertIn(check, alone)
        self.assertIn("The portable exe must run without a .config", alone)


if __name__ == "__main__":
    unittest.main()
