"""Per-user MSI: WiX source, build and CI install test, and how the updater recognises an MSI copy."""
import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"
WXS = ROOT / "windows" / "msi" / "SPI_Data_Comp_Tool.wxs"
WORKFLOW = (ROOT / ".github" / "workflows" / "build.yml").read_text()
NS = {"w": "http://wixtoolset.org/schemas/v4/wxs"}


class WixSource(unittest.TestCase):
    def setUp(self):
        self.root = ET.parse(WXS).getroot()
        self.package = self.root.find("w:Package", NS)

    def test_per_user_package_with_a_fixed_upgrade_code(self):
        self.assertEqual(self.package.get("Scope"), "perUser")
        self.assertEqual(self.package.get("UpgradeCode"), "{B361868A-E052-46BA-BB36-F0DDF1D47CD9}")
        self.assertEqual(self.package.get("Version"), "$(var.Version)")
        upgrade = self.package.find("w:MajorUpgrade", NS)
        # the in-app updater can choose an older version or the normal build of a beta's number
        self.assertEqual(upgrade.get("AllowDowngrades"), "yes")
        self.assertIsNone(upgrade.get("AllowSameVersionUpgrades"))  # implied by AllowDowngrades (WIX0035)
        self.assertEqual(self.package.find("w:MediaTemplate", NS).get("EmbedCab"), "yes")

    def test_requires_dotnet_48_like_the_setup(self):
        launch = self.package.find("w:Launch", NS)
        self.assertEqual(launch.get("Condition"), 'Installed OR (NETFX4RELEASE AND NETFX4RELEASE >= "#528040")')
        search = self.package.find("w:Property[@Id='NETFX4RELEASE']/w:RegistrySearch", NS)
        self.assertEqual((search.get("Root"), search.get("Key"), search.get("Name"), search.get("Type")),
                         ("HKLM", "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", "Release", "raw"))
        self.assertIn("528040", (ROOT / "windows" / "installer" / "SPI_Data_Comp_Tool.iss").read_text())

    def test_shortcut_never_collides_with_the_setup(self):
        shortcut = self.package.find(".//w:Shortcut", NS)
        self.assertEqual(shortcut.get("Name"), "SPI Data Comparison Tool (MSI)")
        self.assertNotIn("SPI Data Comparison Tool (MSI)", (ROOT / "windows" / "installer" / "SPI_Data_Comp_Tool.iss").read_text())

    def test_installs_into_its_own_local_app_data_folder(self):
        local = self.package.find("w:StandardDirectory[@Id='LocalAppDataFolder']", NS)
        folder = local.find("w:Directory/w:Directory", NS)
        self.assertEqual(folder.get("Id"), "INSTALLFOLDER")
        self.assertEqual(folder.get("Name"), "SPI Data Comp Tool (MSI)")

    def test_same_files_as_the_setup_and_registry_key_paths(self):
        files = {f.get("Source") for f in self.package.iter("{%s}File" % NS["w"])}
        self.assertEqual(files, {"$(var.SourceDir)\\SPI Data Comp Tool.exe", "$(var.SourceDir)\\SPI Data Comp Tool.exe.config",
                                 "$(var.SourceDir)\\THIRD_PARTY_NOTICES.md"})
        for component in self.package.iter("{%s}Component" % NS["w"]):
            keys = [r for r in component.findall("w:RegistryValue", NS) if r.get("KeyPath") == "yes"]
            self.assertEqual(len(keys), 1, component.get("Id"))
            self.assertEqual(keys[0].get("Root"), "HKCU")
        guids = [c.get("Guid") for c in self.package.iter("{%s}Component" % NS["w"])]
        self.assertEqual(len(guids), len(set(guids)))
        self.assertTrue(all(re.fullmatch(r"\{[0-9A-F]{8}(-[0-9A-F]{4}){3}-[0-9A-F]{12}\}", g) for g in guids))

    def test_updater_reads_the_location_the_msi_registers(self):
        registration = self.package.find(".//w:Component[@Id='Registration']", NS)
        value = registration.find("w:RegistryValue[@Name='InstallLocation']", NS)
        self.assertEqual(value.get("Value"), "[INSTALLFOLDER]")
        installer = (APP / "UpdateInstaller.cs").read_text(encoding="utf-8-sig")
        self.assertIn('MsiRegistryKey = @"' + value.get("Key") + '"', installer)
        self.assertIn('key.GetValue("InstallLocation")', installer)
        self.assertIn("Both the MSI and the setup installer claim this folder", installer)
        self.assertIn("result.Kind = UpdateInstallKind.Msi", installer)


class BuildAndTest(unittest.TestCase):
    def test_wix_is_pinned_and_beta_versions_pack_as_numbers(self):
        script = (ROOT / "tools" / "build_msi.ps1").read_text()
        self.assertIn("$WixVersion = '5.0.2'", script)
        self.assertIn("dotnet tool install wix --version $WixVersion --tool-path $tools", script)
        self.assertIn("$numeric = ($Version -split '-')[0]", script)
        self.assertIn('-d "Version=$numeric"', script)
        self.assertIn("'D0-CF-11-E0-A1-B1-1A-E1'", script)

    def test_ci_builds_and_installs_the_msi_after_the_setup_test(self):
        windows = WORKFLOW.split("  windows-build:", 1)[1].split("\n  sign:", 1)[0]
        order = [windows.index(n) for n in ("- name: Test the installer", "- name: Build per-user MSI",
                                            "- name: Test the MSI", "- name: Build Microsoft Store package (MSIX)")]
        self.assertEqual(order, sorted(order))
        test = (ROOT / "tools" / "test_windows_msi.ps1").read_text()
        for text in ("'/qn'", "--check-install-kind', 'msi'", "@('--check-assemblies')", "@('--check-ui')", "@('--check-startup')",
                     "MSI reinstall of the same version", "@('/x'", "The MSI uninstall left the exe behind",
                     "The MSI uninstall left its registration behind", "The Start menu shortcut is missing"):
            self.assertIn(text, test)

    def test_updater_applies_an_msi_without_elevation(self):
        worker = (APP / "UpdateApplyWorker.cs").read_text(encoding="utf-8-sig")
        apply = worker.split("private static void ApplyMsi(", 1)[1].split("internal static void VerifyApplicationVersion", 1)[0]
        self.assertIn('Path.Combine(Environment.SystemDirectory, "msiexec.exe")', apply)
        self.assertIn("/passive /norestart", apply)
        self.assertIn("UseShellExecute = false", apply)
        self.assertNotIn("runas", apply)
        self.assertIn("RequireMsiPackage(msi)", apply)
        self.assertIn("current.Kind != UpdateInstallKind.Msi", apply)
        self.assertIn("settings.Rollback()", apply)
        self.assertIn('manifest.Kind != UpdateInstallKind.Installed && manifest.IsMachineInstall', worker)
        contracts = (APP / "UpdateContracts.cs").read_text(encoding="utf-8-sig")
        self.assertIn("internal enum UpdateInstallKind { Installed, Portable, Msi }", contracts)
        self.assertIn('FirstMsiVersion = ParseKnown("4.6.0-beta001")', contracts)
        self.assertIn("Version.CompareTo(FirstMsiVersion) >= 0", contracts)

    def test_single_exe_updates_write_a_self_contained_user_config(self):
        worker = (APP / "UpdateApplyWorker.cs").read_text(encoding="utf-8-sig")
        self.assertEqual(worker.count("RunsWithoutConfiguration(stage, manifest, version)"), 2)
        preferences = (APP / "UpdatePreferences.cs").read_text(encoding="utf-8-sig")
        self.assertIn("if (declareSettingsSections) DeclareSettingsSections(xml.Root);", preferences)
        self.assertIn('allowExeDefinition", "MachineToLocalUser"', preferences)
        checks = (ROOT / "tests" / "UpdateApplyChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("OpenMappedExeConfiguration", checks)


if __name__ == "__main__":
    unittest.main()
