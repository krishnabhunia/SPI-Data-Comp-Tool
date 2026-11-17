"""Static contract checks for the legacy .NET Framework WinForms auth entry point.

These are not substitutes for interactive Windows/Entra integration testing.
"""

import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class AuthenticationContracts(unittest.TestCase):
    def test_authentication_disabled_until_configured(self):
        config = ET.parse(APP / "App.config").getroot()
        keys = {
            e.attrib["key"]: e.attrib["value"]
            for e in config.findall("./appSettings/add")
        }
        self.assertEqual(
            keys,
            {
                "Auth.Windows.AllowedUserSids": "",
                "Auth.Windows.AllowedGroupSids": "",
                "Auth.Entra.TenantId": "",
                "Auth.Entra.ClientId": "",
                "Auth.Entra.AllowedUserObjectIds": "",
            },
        )

    def test_windows_requires_real_token_and_explicit_sids(self):
        source = (APP / "AuthenticationService.cs").read_text()
        for fragment in (
            "WindowsIdentity.GetCurrent()",
            "identity.IsAuthenticated",
            "identity.User",
            "new WindowsPrincipal(identity)",
            "new SecurityIdentifier(value)",
            "allowedGroups.Any(principal.IsInRole)",
            "allowedUsers.Count == 0 && allowedGroups.Count == 0",
        ):
            self.assertIn(fragment, source)

    def test_entra_requires_approved_tenant_and_user_identity(self):
        source = (APP / "AuthenticationService.cs").read_text()
        for fragment in (
            "PublicClientApplicationBuilder.Create",
            ".WithAuthority(AzureCloudInstance.AzurePublic",
            '.WithRedirectUri("http://localhost")',
            ".WithUseEmbeddedWebView(false)",
            "allowedUsers.Count == 0",
            "result.TenantId",
            "actualTenantId != tenantId",
            "result.Account.HomeAccountId.ObjectId",
            "!allowedUsers.Contains(actualObjectId)",
        ):
            self.assertIn(fragment, source)

    def test_legacy_shared_password_and_login_bypass_removed(self):
        ui = (APP / "MainForm.cs").read_text()
        designer = (APP / "MainForm.Designer.cs").read_text()
        program = (APP / "Program.cs").read_text()
        debug = (APP / "Common_Data.cs").read_text()
        self.assertIn("AuthenticationService.SignInWithWindows()", ui)
        self.assertIn("AuthenticationService.SignInWithEntraAsync", ui)
        self.assertIn("ToggleMenuStripForms(false)", ui)
        self.assertIn("userSettingsToolStripMenuItem.Enabled = boolEnableStatus", ui)
        self.assertNotIn('tb_password.Text ==', ui)
        self.assertNotIn('tb_password.Text = "', ui)
        self.assertNotIn("tb_password", designer)
        self.assertNotIn("tb_username", designer)
        self.assertIn("btn_EntraLogin_Click", designer)
        self.assertNotIn("Environment.MachineName", ui)
        self.assertEqual(program.count("Application.Run("), 1)
        self.assertIn("Application.Run(new MainForm())", program)
        self.assertNotIn('GetEnvironmentVariable("SPI_DATA_COMP_DEBUG")', debug)

    def test_debug_ui_is_disabled_for_release_builds(self):
        debug = (APP / "Common_Data.cs").read_text()
        self.assertRegex(debug, re.compile(r"#if DEBUG\s+return Debugger\.IsAttached;\s+#else\s+return false;\s+#endif"))
        for filename in APP.glob("*.cs"):
            source = filename.read_text(encoding="utf-8-sig")
            if filename.name == "Common_Data.cs":
                self.assertEqual(source.count("Debugger.IsAttached"), 1)
            else:
                self.assertNotIn("Debugger.IsAttached", source, str(filename))

    def test_msal_package_and_project_reference_match(self):
        packages = ET.parse(APP / "packages.config").getroot()
        package_versions = {p.attrib["id"]: p.attrib["version"] for p in packages}
        self.assertEqual(package_versions["Microsoft.Identity.Client"], "4.92.0")
        self.assertEqual(package_versions["Microsoft.IdentityModel.Abstractions"], "8.14.0")
        project = (APP / "SPI Data Comp Tool.csproj").read_text()
        self.assertIn('<Compile Include="AuthenticationService.cs" />', project)
        for package, version in package_versions.items():
            if package in ("Microsoft.Identity.Client", "Microsoft.IdentityModel.Abstractions", "System.Formats.Asn1"):
                self.assertIn("packages\\" + package + "." + version, project)


if __name__ == "__main__":
    unittest.main()
