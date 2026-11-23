"""Microsoft Store (MSIX) package: manifest, logos, build script, CI install test, release asset and
the rule that a Store copy never updates itself (docs/MICROSOFT_STORE.md)."""
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"
MANIFEST = ROOT / "windows" / "msix" / "AppxManifest.xml"
WORKFLOW = (ROOT / ".github" / "workflows" / "build.yml").read_text()
NS = {
    "f": "http://schemas.microsoft.com/appx/manifest/foundation/windows10",
    "uap": "http://schemas.microsoft.com/appx/manifest/uap/windows10",
    "uap3": "http://schemas.microsoft.com/appx/manifest/uap/windows10/3",
    "desktop": "http://schemas.microsoft.com/appx/manifest/desktop/windows10",
    "rescap": "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities",
}


def step(name):
    return WORKFLOW.split(f"- name: {name}", 1)[1].split("\n      - ", 1)[0]


class ManifestTests(unittest.TestCase):
    def setUp(self):
        self.text = MANIFEST.read_text(encoding="utf-8")
        self.root = ET.fromstring(self.text)

    def test_identity_comes_from_tokens_so_partner_center_values_are_never_committed(self):
        identity = self.root.find("f:Identity", NS)
        self.assertEqual(identity.get("Name"), "$IdentityName$")
        self.assertEqual(identity.get("Publisher"), "$Publisher$")
        self.assertEqual(identity.get("Version"), "$Version$")
        self.assertEqual(identity.get("ProcessorArchitecture"), "x64")
        self.assertEqual(self.root.find("f:Properties/f:PublisherDisplayName", NS).text, "$PublisherDisplayName$")

    def test_desktop_full_trust_app_with_the_installed_exe_name(self):
        app = self.root.find("f:Applications/f:Application", NS)
        self.assertEqual(app.get("Executable"), "SPI Data Comp Tool.exe")
        self.assertEqual(app.get("EntryPoint"), "Windows.FullTrustApplication")
        self.assertIsNotNone(self.root.find("f:Capabilities/rescap:Capability[@Name='runFullTrust']", NS))
        family = self.root.find("f:Dependencies/f:TargetDeviceFamily", NS)
        self.assertEqual(family.get("Name"), "Windows.Desktop")

    def test_execution_alias_used_by_the_ci_install_test(self):
        alias = self.root.find(".//desktop:ExecutionAlias", NS)
        self.assertEqual(alias.get("Alias"), "spi-data-comp-tool.exe")
        self.assertIn("Microsoft\\WindowsApps\\spi-data-comp-tool.exe", WORKFLOW)

    def test_every_referenced_logo_is_drawn_by_the_build_script(self):
        script = (ROOT / "tools" / "build_msix.ps1").read_text()
        expected = {"StoreLogo.png": 50, "Square44x44Logo.png": 44, "Square150x150Logo.png": 150}
        for name, px in expected.items():
            self.assertIn("Assets\\" + name, self.text)
            self.assertIn(f"@('{name}', {px},", script)
        self.assertIn("'SPI Data Comp Tool/app.ico'", script)
        self.assertEqual(sorted(set(__import__("re").findall(r"Assets\\(\w+\.png)", self.text))), sorted(expected))
        # no committed images: the repository keeps only the app's own generated images
        self.assertFalse((ROOT / "windows" / "msix" / "Assets").exists())


class BuildScriptTests(unittest.TestCase):
    SCRIPT = (ROOT / "tools" / "build_msix.ps1").read_text()

    def test_store_version_has_four_parts_ending_in_zero(self):
        self.assertIn("$packageVersion = \"$numeric.0\"", self.SCRIPT)
        self.assertIn("($Version -split '-')[0]", self.SCRIPT)

    def test_same_files_as_the_installer_and_no_loose_libraries(self):
        self.assertIn("'.pdb', '.xml', '.application', '.manifest'", self.SCRIPT)
        self.assertIn("-Filter *.dll", self.SCRIPT)

    def test_values_are_escaped_and_every_token_replaced(self):
        self.assertIn("[Security.SecurityElement]::Escape", self.SCRIPT)
        self.assertIn("Unreplaced token in the manifest", self.SCRIPT)
        for token in ("$IdentityName$", "$Publisher$", "$PublisherDisplayName$", "$Version$"):
            self.assertIn(token, self.SCRIPT)

    def test_package_is_validated_and_read_back(self):
        self.assertIn("pack /o /h SHA256", self.SCRIPT)
        self.assertIn("unpack /o", self.SCRIPT)
        self.assertIn("The packed manifest has the wrong version", self.SCRIPT)

    def test_store_package_is_never_signed_by_the_script(self):
        self.assertNotIn("signtool", self.SCRIPT.lower())


class WorkflowTests(unittest.TestCase):
    def test_store_package_is_built_after_the_installer_test_and_before_upload(self):
        windows = WORKFLOW.split("  windows-build:", 1)[1].split("\n  release:", 1)[0]
        order = [windows.index(n) for n in (
            "- name: Test the installer", "- name: Build Microsoft Store package (MSIX)",
            "- name: Test the Store package", "uses: actions/upload-artifact@v6")]
        self.assertEqual(order, sorted(order))

    def test_identity_comes_from_repository_variables(self):
        build = step("Build Microsoft Store package (MSIX)")
        for name in ("MSIX_IDENTITY_NAME", "MSIX_PUBLISHER", "MSIX_PUBLISHER_DISPLAY_NAME"):
            self.assertIn(f"vars.{name}", build)
        self.assertNotIn("secrets.", build)
        self.assertIn('-OutFile "dist/store/SPI_Data_Comp_Tool_${ver}_x64.msix"', build)

    def test_install_test_signs_only_a_throwaway_copy_and_cleans_up(self):
        test = step("Test the Store package (install, run, uninstall)")
        self.assertIn("New-SelfSignedCertificate", test)
        self.assertIn("-NotAfter (Get-Date).AddDays(1)", test)
        self.assertIn("$test = Join-Path $env:RUNNER_TEMP", test)
        self.assertIn("Add-AppxPackage -Path $test", test)
        self.assertIn("'--check-package', 'packaged'", test)
        self.assertIn("'--check-package', \"`\"$plain`\"\", 'unpackaged'", test)
        self.assertIn("@('--check-assemblies')", test)
        self.assertIn("Remove-AppxPackage", test)
        finally_block = test.split("finally {", 1)[1]
        self.assertIn("Cert:\\LocalMachine\\TrustedPeople", finally_block)
        self.assertIn("Cert:\\CurrentUser\\My", finally_block)

    def test_release_keeps_the_msix_out_of_the_updater_zip(self):
        publish = WORKFLOW.split("- name: Publish release", 1)[1]
        self.assertLess(publish.index('mv "dist/store/${msix_name}" "${msix_name}"'), publish.index('zip -qr'))
        self.assertLess(publish.index("rmdir dist/store"), publish.index('zip -qr'))
        self.assertIn('"${msix_name}" "${msix_name}.sha256"', publish)
        # the updater's checksum file keeps listing only the zip and the installer
        self.assertIn('sha256sum "${zip_name}" "${setup_name}" > "${zip_name}.sha256"', publish)


class StoreCopyNeverSelfUpdatesTests(unittest.TestCase):
    def test_packaged_detection_uses_the_windows_package_api_safely(self):
        source = (APP / "PackagedApp.cs").read_text()
        self.assertIn("GetCurrentPackageFullName", source)
        self.assertIn("AppModelErrorNoPackage = 15700", source)
        self.assertIn("catch (EntryPointNotFoundException)", source)
        self.assertIn('<Compile Include="PackagedApp.cs" />', (APP / "SPI Data Comp Tool.csproj").read_text(encoding="utf-8-sig"))

    def test_updater_refuses_to_install_into_a_store_copy(self):
        installer = (APP / "UpdateInstaller.cs").read_text(encoding="utf-8-sig")
        self.assertIn("detectionError = PackagedApp.IsPackaged ? PackagedApp.StoreUpdateReason : error;", installer)
        can_install = installer.split("public bool CanInstall(out string reason)", 1)[1]
        self.assertLess(can_install.index("if (detectionError != null)"), can_install.index("IsApplying"))

    def test_self_test_mode_checks_identity_and_updater(self):
        selftest = (APP / "SelfTest.cs").read_text(encoding="utf-8-sig")
        self.assertIn('"--check-package"', selftest)
        self.assertIn("reason == PackagedApp.StoreUpdateReason", selftest)

    def test_documented(self):
        doc = (ROOT / "docs" / "MICROSOFT_STORE.md").read_text()
        for text in ("MSIX_IDENTITY_NAME", "Product identity", "runFullTrust", "never updates itself"):
            self.assertIn(text, doc)
        self.assertIn("docs/MICROSOFT_STORE.md", (ROOT / "README.md").read_text())


if __name__ == "__main__":
    unittest.main()
