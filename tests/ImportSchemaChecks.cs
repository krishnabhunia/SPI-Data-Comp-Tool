using System;
using System.Data;
using System.IO;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Candidate import validation without touching a user's files or preferences.</summary>
    internal static class ImportSchemaChecks
    {
        public static void Run(Action<string> passed)
        {
            foreach (string invalid in new[]
            {
                "null",
                "{}",
                "[null]",
                "[{\"profileName\":\"Incomplete\"}]",
                "[{\"profileName\":\"Incomplete\",\"dbSRC\":{}}]",
                "[{\"profileName\":\"Wrong type\",\"dbSRC\":[],\"dbTRG\":{}}]",
                "[{\"profileName\":\"Wrong type\",\"dbSRC\":{},\"dbTRG\":null}]",
            })
            {
                bool rejected = false;
                try { ProfileStore.ParseJson(invalid); }
                catch (InvalidDataException) { rejected = true; }
                Require(rejected, "An incomplete or malformed profile document was accepted.");
            }
            var validProfiles = ProfileStore.ParseJson("[{\"profileName\":\"Synthetic\",\"dbSRC\":{},\"dbTRG\":{}}]");
            var caseProfiles = ProfileStore.ParseJson("[{\"profileName\":\"Synthetic\",\"DBSRC\":{},\"DBTRG\":{}}]");
            Require(validProfiles.Count == 1 && caseProfiles.Count == 1 && ProfileStore.ParseJson("[]").Count == 0,
                "An existing valid profile format was rejected.");
            passed("Profile JSON requires explicit Source/Target objects before constructor defaults can mask missing fields.");

            using (var rules = ExampleRules())
            {
                User_Settings.ValidateRulesCandidate(rules);
                rules.Tables[0].Rows[0]["RuleType"] = "3";
                ExpectInvalidRules(rules);
                foreach (string name in new[] { "ConditionSourceColumn", "ConditionTargetColumn", "ConditionSourceValue", "ConditionTargetValue" })
                    rules.Tables[0].Columns.Add(name);
                User_Settings.ValidateRulesCandidate(rules);
                rules.Tables[0].Columns.Remove("Target");
                ExpectInvalidRules(rules);
            }
            using (var onlySrno = new DataSet())
            {
                var table = onlySrno.Tables.Add("Rules");
                table.Columns.Add("srno");
                table.Rows.Add("1");
                ExpectInvalidRules(onlySrno);
            }
            using (var empty = ExampleRules())
            {
                empty.Tables[0].Rows.Clear();
                ExpectInvalidRules(empty);
            }
            passed("Rule candidates require evaluator columns; type-3 rules require four positional condition fields; valid type-1/type-3 formats remain accepted.");
        }

        private static DataSet ExampleRules()
        {
            var rules = new DataSet();
            var table = rules.Tables.Add("Rules");
            foreach (string name in new[] { "srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname" })
                table.Columns.Add(name);
            table.Rows.Add("1", "Synthetic source", "Synthetic target", "1", "COLUMN_A", "COLUMN_B");
            return rules;
        }

        private static void ExpectInvalidRules(DataSet rules)
        {
            bool rejected = false;
            try { User_Settings.ValidateRulesCandidate(rules); }
            catch (InvalidOperationException) { rejected = true; }
            Require(rejected, "A rules candidate missing the evaluator's required schema was accepted.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
