"""Source-level guards; these do not replace Windows/SQL Server/Oracle integration tests."""

import unittest
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (ROOT / name).read_text(encoding="utf-8-sig")


class SqlScriptSafetyContracts(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.executor = source("SafeSqlScriptExecutor.cs")
        cls.execute_form = source("Execute Excel Scripts Form.cs")
        cls.generator = source("Generate IUD Script Form.cs")

    def test_executor_is_in_winforms_build(self) -> None:
        project = ET.fromstring(source("SPI Data Comp Tool.csproj"))
        namespace = {"m": "http://schemas.microsoft.com/developer/msbuild/2003"}
        compile_names = [item.get("Include") for item in project.findall(".//m:Compile", namespace)]
        self.assertIn("SafeSqlScriptExecutor.cs", compile_names)

    def test_write_batch_is_validated_before_database_open(self) -> None:
        script = self.executor.split("internal static SqlScriptExecutionResult Execute(", 1)[1]
        self.assertLess(script.index("SqlScriptSafety.ValidateWrite(sql)"),
                        script.index("connection.Open()"))
        self.assertIn("transaction.Commit()", script)
        self.assertIn("transaction.Rollback()", script)
        self.assertIn("cancellationToken.ThrowIfCancellationRequested()", script)

    def test_destructive_preview_requires_typed_consent(self) -> None:
        self.assertIn('string.Equals(confirmation.Text, "EXECUTE", StringComparison.Ordinal)', self.executor)
        self.assertIn("AcceptButton = previewOnly", self.executor)
        self.assertIn("new SqlScriptPreviewForm(previewTarget, scripts)", self.execute_form)
        self.assertIn("if (previewForm.ShowDialog(this) != DialogResult.OK)", self.execute_form)

    def test_spreadsheet_sql_cannot_use_old_autocommit_loop(self) -> None:
        handler = self.execute_form.split("private void btn_Execute_Click(", 1)[1]
        handler = handler.split("private string CurrentConnectionFingerprint()", 1)[0]
        self.assertNotIn("common_Data.ExecuteNonQuery(", handler)
        self.assertIn("SafeSqlScriptExecutor.Execute(", handler)
        self.assertNotIn("Executing script :", handler)
        self.assertNotIn("strConnectionString}", handler)

    def test_select_exports_require_select_only(self) -> None:
        self.assertIn("SqlScriptSafety.ValidateSelect(script)", self.execute_form)
        self.assertIn("SqlScriptSafety.ValidateSelect(strQuery)", self.execute_form)
        self.assertIn('"FOR" && words[i + 1].Text == "UPDATE"', self.executor)
        self.assertIn('"NEXTVAL"', self.executor)

    def test_reject_multiple_sql_statements_and_comments(self) -> None:
        self.assertIn('throw new ArgumentException("Multiple statements are not permitted.")', self.executor)
        self.assertIn('throw new ArgumentException("SQL comments are not supported', self.executor)
        self.assertIn('word.Text == "WHERE" && word.Depth == 0', self.executor)

    def test_reconnect_required_if_target_or_credentials_change(self) -> None:
        self.assertIn("RequireFreshDatabaseConnection();", self.execute_form)
        self.assertIn("testedConnectionFingerprint = CurrentConnectionFingerprint()", self.execute_form)
        self.assertIn("tb_SRC_Password.Text, tb_DefaultPort.Text.Trim()", self.execute_form)

    def test_script_generation_escapes_values_and_requires_where(self) -> None:
        self.assertIn("SqlScriptSafety.SqlLiteral(dr[x.ColumnName])", self.generator)
        self.assertIn("SqlScriptSafety.SqlWhereEquals(x.ToString(), dr[x.ToString()])", self.generator)
        self.assertIn("SqlScriptSafety.RequireSafeIdentifier(tb_TableName.Text)", self.generator)
        self.assertIn("Unrestricted UPDATE/DELETE generation is blocked.", self.generator)
        self.assertNotIn('strUpdateTable.Replace("N\'\'", "NULL")', self.generator)

    def test_local_audit_omits_sql_and_credentials(self) -> None:
        audit = self.executor.split("internal static class SqlScriptAudit", 1)[1]
        audit = audit.split("internal static class SafeSqlScriptExecutor", 1)[0]
        self.assertIn("LocalApplicationData", audit)
        self.assertIn("SHA256.Create()", audit)
        self.assertIn('phase, fingerprint, scriptCount.ToString(), rows.ToString()', audit)
        self.assertNotIn("connectionString", audit)
        self.assertNotIn("CommandText", audit)


if __name__ == "__main__":
    unittest.main()
