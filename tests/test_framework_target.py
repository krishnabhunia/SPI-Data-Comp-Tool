"""Deployment contracts: the shipped binary, package metadata and installer require one runtime."""

import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"
MSBUILD = {"m": "http://schemas.microsoft.com/developer/msbuild/2003"}


class FrameworkTarget(unittest.TestCase):
    def test_release_configuration_and_installer_require_the_adopted_framework(self):
        project = ET.parse(APP / "SPI Data Comp Tool.csproj")
        targets = project.findall("m:PropertyGroup/m:TargetFrameworkVersion", MSBUILD)
        self.assertEqual([target.text for target in targets], ["v4.8"])
        config = ET.parse(APP / "App.config")
        runtimes = config.findall("startup/supportedRuntime")
        self.assertEqual(len(runtimes), 1)
        self.assertEqual(runtimes[0].attrib, {"version": "v4.0", "sku": ".NETFramework,Version=v4.8"})
        self.assertEqual({package.get("targetFramework") for package in ET.parse(APP / "packages.config").getroot()}, {"net48"})
        installer = (ROOT / "windows/installer/SPI_Data_Comp_Tool.iss").read_text()
        self.assertIn("Release >= 528040", installer)
        self.assertIn("needs Microsoft .NET Framework 4.8 or later", installer)

    def test_compatibility_gate_tests_shipped_target_without_silently_retargeting_it(self):
        script = (ROOT / "tools/check_framework_compatibility.ps1").read_text()
        self.assertNotIn("/p:TargetFrameworkVersion=", script)
        self.assertNotIn(".SetAttribute('sku'", script)
        self.assertIn("--check-updates", script)
        self.assertIn("--measure-comparison", script)
        self.assertIn("if ($process.ExitCode -ne 0", script)


if __name__ == "__main__":
    unittest.main()
