"""Exercise exact package provenance and safe inclusion of package license files."""

import importlib.util
from pathlib import Path
import tempfile
import unittest
import zipfile


ROOT = Path(__file__).resolve().parents[1]
SPEC = importlib.util.spec_from_file_location("third_party_notices", ROOT / "tools" / "generate_third_party_notices.py")
NOTICES = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(NOTICES)


class ThirdPartyNotices(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.packages = self.root / "packages"
        self.packages.mkdir()
        self.config = self.root / "packages.config"

    def package(self, name, version="1.0.0", license_metadata='<license type="expression">MIT</license>', extra=""):
        directory = self.packages / f"{name}.{version}"
        directory.mkdir()
        (directory / f"{name}.nuspec").write_text(
            f'<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"><metadata><id>{name}</id>'
            f'<version>{version}</version><authors>Fixture author</authors>{license_metadata}{extra}</metadata></package>'
        )
        return directory

    def test_preserves_exact_metadata_and_included_terms_in_stable_order(self):
        self.config.write_text('<packages><package id="Zed" version="1.0.0" developmentDependency="true"/><package id="Alpha" version="1.0.0"/></packages>')
        self.package("Zed", license_metadata='<licenseUrl>https://example.invalid/fixture-terms</licenseUrl>')
        alpha = self.package("Alpha", license_metadata='<license type="file">licenses\\terms.txt</license>')
        (alpha / "licenses").mkdir()
        (alpha / "licenses/terms.txt").write_text("Synthetic package terms\nCopyright fixture author\n")
        (alpha / "NOTICE.txt").write_text("Synthetic attribution notice\n")
        result = NOTICES.generate(self.config, self.packages)
        self.assertLess(result.index("| Alpha |"), result.index("| Zed |"))
        self.assertIn("| Zed | 1.0.0 | Build only | URL only |", result)
        self.assertIn("    Synthetic package terms", result)
        self.assertIn("    Synthetic attribution notice", result)
        self.assertIn("Nuspec SHA-256:", result)
        self.assertEqual(result, NOTICES.generate(self.config, self.packages))

    def test_missing_restore_or_wrong_version_fails_instead_of_omitting_dependency(self):
        self.config.write_text('<packages><package id="Alpha" version="2.0.0"/></packages>')
        self.package("Alpha")
        with self.assertRaises(ValueError):
            NOTICES.generate(self.config, self.packages)

    def test_legacy_restore_reads_manifest_from_nupkg_without_extracting_it(self):
        self.config.write_text('<packages><package id="Alpha" version="1.0.0"/></packages>')
        directory = self.package("Alpha")
        spec = directory / "Alpha.nuspec"
        expected = NOTICES.generate(self.config, self.packages)
        with zipfile.ZipFile(directory / "Alpha.1.0.0.nupkg", "w") as archive:
            archive.writestr("Alpha.nuspec", spec.read_bytes())
            archive.writestr("../../not-extracted.txt", "Synthetic unsafe unrelated entry")
        spec.unlink()
        self.assertEqual(NOTICES.generate(self.config, self.packages), expected)
        self.assertFalse((self.root / "not-extracted.txt").exists())
        self.assertFalse(spec.exists())

    def test_nupkg_manifest_must_be_unique_at_root_and_match_identity(self):
        self.config.write_text('<packages><package id="Alpha" version="1.0.0"/></packages>')
        directory = self.package("Alpha")
        spec = directory / "Alpha.nuspec"
        content = spec.read_bytes()
        spec.unlink()
        archive_path = directory / "Alpha.1.0.0.nupkg"
        for entries in (
            [("nested/Alpha.nuspec", content)],
            [("Alpha.nuspec", content), ("ALPHA.NUSPEC", content)],
            [("Alpha.nuspec", content.replace(b"<id>Alpha</id>", b"<id>Other</id>"))],
        ):
            with self.subTest(entries=[name for name, _ in entries]):
                with zipfile.ZipFile(archive_path, "w") as archive:
                    for name, data in entries:
                        archive.writestr(name, data)
                with self.assertRaises(ValueError):
                    NOTICES.generate(self.config, self.packages)

    def test_directory_metadata_mismatch_is_rejected(self):
        self.config.write_text('<packages><package id="Alpha" version="1.0.0"/></packages>')
        directory = self.package("Alpha")
        spec = directory / "Alpha.nuspec"
        spec.write_text(spec.read_text().replace("<version>1.0.0", "<version>9.0.0"))
        with self.assertRaises(ValueError):
            NOTICES.generate(self.config, self.packages)

    def test_unknown_license_is_visible_and_not_invented(self):
        self.config.write_text('<packages><package id="Alpha" version="1.0.0"/></packages>')
        self.package("Alpha", license_metadata="")
        self.assertIn("Not declared; review required", NOTICES.generate(self.config, self.packages))

    def test_license_path_cannot_escape_restored_package(self):
        for path in ("../../private.txt", "..\\private.txt", "/private.txt", "C:\\private.txt", "\\\\host\\share\\private.txt"):
            with self.subTest(path=path), self.assertRaises(ValueError):
                NOTICES.package_file(self.packages, path)

    def test_duplicate_or_unsafe_package_identifiers_are_rejected(self):
        self.config.write_text('<packages><package id="Alpha" version="1.0.0"/><package id="alpha" version="1.0.0"/></packages>')
        with self.assertRaises(ValueError):
            NOTICES.generate(self.config, self.packages)
        with self.assertRaises(ValueError):
            NOTICES.safe_component("../outside", "id")


if __name__ == "__main__":
    unittest.main()
