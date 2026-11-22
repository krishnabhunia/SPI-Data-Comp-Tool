# Excel Sheet Audit — Design A

Open **Compare workbooks** and choose **Values and formatting** (one pair). This is a native C# Windows Forms workflow, separate from the value comparison (**Values**). It uses the application's existing .NET Framework 4.8 and Open XML SDK dependency; it does not install Python, launch Excel, execute formulas/macros, or alter the older comparison engine.

## Run an audit

1. Choose **File A** and **File B** (`.xlsx` or `.xlsm`). Load each file, then select its worksheet and header row. Click **Load** again after changing either selection. Header rows may differ between files.
2. Choose **No key · Row by row**, or **Primary key** and add the corresponding key columns in their intended component order. Column names are resolved ignoring case. Column-name whitespace is not silently trimmed.
3. Select the checks: **Values and data types**, **Headers and column order**, **Cell formatting**, and **Worksheet layout**. At least one check must be selected. Unselected checks are reported as not checked.
4. Choose a separate `.xlsx` report path, an existing temporary folder with sufficient free space, and the maximum retained findings per section. **Validate** performs a full input/key scan, not an equality check. **Compare sheets** performs validation again and produces the result.
5. Review **Data Differences** and **Summary & other differences**. The grids page through retained findings in groups of 500. Totals include differences omitted from the detail preview/report by the configured limits.
6. Select **Export report**. Replacing an existing report requires explicit confirmation. **Open report** opens only a successfully saved output.

Changing comparison inputs or rules clears stale results. Loading, validation, comparison and export run on a background worker. **Cancel** requests safe cancellation; the page remains protected against closing until cleanup finishes. Navigating to another workspace task does not cancel the operation or reset its state. Existing Ocean, Berry, Forest and Midnight themes apply to this page.

## Matching rules

| Choice | Example | Behaviour |
|---|---|---|
| No key | No columns selected | Compare logical row positions below each selected header row; preserve original Excel coordinates. Do not sort, infer keys, or skip interior blank positions. |
| Single key | File A `a1` → File B `b1` | Match the stored key values even if record order or key-column names differ. |
| Composite key | `a2` → `b2`, then `a3` → `b3` | Match the complete ordered tuple. Each component may repeat individually; the full tuple must be unique in its file. |

Both files must supply the same number of key components. Each selected column must exist unambiguously and may appear only once in its mapping. Every data record requires non-null, non-whitespace key components; formulas and Excel errors are not accepted as primary keys. Duplicate complete keys on either side stop the run; there is no silent fallback to positional comparison and no partial report is exported.

Key and ordinary values retain their type, text case and whitespace. Number `1` differs from text `"1"`; text `"001"` keeps its leading zeros; `abc` differs from `ABC`. Equivalent stored numeric representations such as `1.00` and `1e0` compare equal without converting all values to binary floating point. Composite keys use length-prefixed typed values, not a joined delimiter string or a hash as proof of equality.

Non-key columns pair by case-insensitive header name. Explicitly mapped key columns may have different names. Header name/case/order differences remain separate findings when header checks are enabled. Case-ambiguous headers and populated columns without headers are rejected. Unpaired columns are identified in header findings and prevent a complete value/style match claim; the engine does not guess renamed non-key columns.

## Exactly two report worksheets

| Sheet | Contents |
|---|---|
| `Summary` | Files, selected worksheets/header rows, matching rules, enabled checks, counts, completeness, retained/omitted details, header differences, formatting differences, layout differences and scope warnings. |
| `Data Differences` | Changed values, data types, saved formulas, and records present only in one file. Each finding includes source/target addresses, column names, mapped key when applicable and both values. |

Both detail tables use these columns: **No., Category, Difference, Primary key, File A cell, File B cell, File A column, File B column, File A value, File B value, Details**. They have filters, frozen headings, simple colours, centered/middle-aligned wrapped text and readable fixed column widths. Excel cannot automatically calculate a true fit-to-content width through this writer; lengthy text remains available in the cell/formula bar.

Every exported value is literal text, including strings beginning with `=`, `+`, `-` or `@`. Findings do not become executable formulas. A single report cell over Excel's 32,767-character limit is visibly shortened with `[TRUNCATED]`. An unmatched record is one finding with a bounded descriptive row preview, not a complete second copy of the input dataset.

The result says **Matches under selected rules**, **Differences found**, or **Not fully checked**. It never asserts byte-for-byte workbook identity or pixel-perfect rendered equivalence. Validation alone cannot produce a match verdict or be exported as a completed comparison.

## Checked scope and limitations

| Area | Treatment |
|---|---|
| Worksheet selection | Content comparison applies to the selected worksheet pair and the table starting at the chosen header rows, not all workbook tabs or title rows above the table. Worksheet names/order are metadata findings when layout is enabled. |
| Values | Exact stored types, text, booleans, Excel errors, numbers, supported dates and saved formula definitions. Ordinary formulas are compared as saved text; Excel calculation is not performed and stale cached results are not proof of equality. |
| Headers | Header count, names, case, order, types and enabled formatting. |
| Formatting | Resolved font properties, size, emphasis, colour, alignment, wrapping, number format, fills, borders and protection properties for mapped cells. Workbook-local style indexes are not compared directly. |
| Layout | Supported row/column dimensions and visibility, outline/collapse state, merged ranges, panes, default dimensions and worksheet visibility. |
| Unsupported rendering | Rich-text runs, conditional-formatting effects, special shared/array/data-table formulas, unresolved theme colours and locale-specific formats may produce explicit incomplete-scope warnings. |
| Not an entire workbook audit | Charts, images, macros, comments, external connections, print settings, pivots and all Excel extension features are not certified equal. Inputs are not saved, macros are not executed and external connections are not refreshed. |

An incomplete-scope warning is not a discovered data difference; it is kept in `Summary`, and prevents a full-match claim even when the compared fields have no findings. Warnings may be discovered during streaming, so the final result collects them after the scan as well as during input loading.

## Resource and file safety

- Worksheet cells are streamed, and the shared-string table and its offset index are spooled to a task-owned temporary location. Keyed comparisons spool rows into 64 disk partitions and load one guarded partition at a time. Positional comparison streams both worksheets in logical row order.
- Default retained detail limits are **100,000 findings and an estimated 64 MiB per section**, independently for data and other findings. The form allows changing the finding count up to 1,000,000. Full counts continue after the detail limit; omitted details are explicitly disclosed in the screen and report.
- A keyed partition has a **128 MiB default serialized/estimated-memory guard**. This is a guard, not a measured process-memory guarantee. Wide records, repeated large strings, skewed partitions or unsuitable input sizes may fail with actionable guidance rather than attempting an unbounded allocation. Use the 64-bit application for larger supported worksheets.
- Metadata, styles and row text have additional bounded-reader checks. Workbook XML/ZIP parsing still uses the Open XML packaging stack. This is not a hardened decompression-bomb sandbox, a guarantee of processing arbitrary Excel files, or a solution for the earlier 500 GB CSV datasets.
- Temporary data can be significantly larger than the compressed workbook; available space must cover shared strings and keyed row snapshots. Cancellation/failure disposes readers and removes task-owned working files. Unrelated temporary files are never recursively removed.
- Input streams are read-only. The report path is validated against both canonical input paths; file links and reparse-point ancestors (including linked folders/junctions) on the input/output paths are rejected before export. This is not a complete physical hard-link identity check or protection against concurrent path substitution. Use regular paths, not aliases, and avoid concurrent writers when selecting an output.
- Export writes a complete same-directory temporary workbook and only then publishes it by move/replacement. A new destination created by another writer is not overwritten without explicit overwrite permission. Failure or cancellation before publication preserves an existing report. Do not edit the same destination concurrently: an explicitly approved replacement is not a compare-and-swap against other writers.

## Implementation and verification

| File | Responsibility |
|---|---|
| `ExcelSheetAuditForm.cs` | Design A controls, validation feedback, background worker/cancellation, paged previews and export confirmation. |
| `ExcelAuditContracts.cs` | Captured options, typed cells, findings, separate bounded detail sections and completeness state. |
| `ExcelAuditReader.cs` | Read-only streamed Open XML reader, disk shared strings and semantic styles/metadata. |
| `ExcelAuditEngine.cs` | Schema/key validation, positional or partitioned typed-key matching and finding classification. |
| `ExcelAuditReportWriter.cs` | Literal-text, atomic two-sheet report output. |
| `tests/ExcelAuditChecks.cs` | Temporary workbook fixtures, real engine/export assertions and new-form smoke coverage. |

Windows entry point: `"SPI Data Comp Tool.exe" --check-excel-audit <log-file>`. It is wired into `.github/workflows/build.yml`, alongside existing build, workspace, theme, form-opening and installer checks. Python source contracts run through `python -m unittest discover -s tests -p 'test_*.py' -v`. Read the exact commit's Actions results; configured tests are not a claim that the run has passed. Representative customer workbooks, high-DPI layout, keyboard/accessibility and large-file performance still require Windows acceptance testing.
