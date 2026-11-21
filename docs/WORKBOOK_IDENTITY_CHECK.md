# Workbook identity check

**Comparison > Workbook identity check** answers one question: **are two Excel workbooks identical?** It shows the answer on screen, lists every difference and writes a report with five sheets.

The other comparison tasks are unchanged. Use this table to choose between the three Excel tasks.

| SrNo. | Task | Use it when | Report |
|---|---|---|---|
| 1 | Excel comparison | You want the original comparison with its rules and advanced column mapping | As before |
| 2 | Excel Sheet Audit | You want to audit one worksheet of each file with very large data in mind | Summary and Data Differences |
| 3 | Workbook identity check | You want to know whether two workbooks are identical, sheet by sheet, including headers, formatting and layout | Summary, Headers, Data Differences, Format Differences, Layout & Other |

## How to use it

| SrNo. | Step | What to do |
|---|---|---|
| 1 | Choose the workbooks | Use **Browse...** for the Source workbook (File A) and the Target workbook (File B). `.xlsx` and `.xlsm` files are accepted. The task reads the sheet names and the header row and shows, for example, `Loaded: 1 sheet, 7 columns, 21 data rows`. |
| 2 | Choose the sheets | Keep **All sheets (paired by name)**, or choose one sheet in both files. One sheet on one side and all sheets on the other is refused. |
| 3 | Choose how rows are matched | **By row position** compares row N of File A with row N of File B. **By primary key** pairs rows that have the same key, whatever their order. |
| 4 | Tick the key | With a primary key, tick the key column in each list. Tick several columns for a combined key. Each list shows the order: `key part 1`, `key part 2`. Part 1 of Source pairs with part 1 of Target, so the two files may use different column names. Names are matched ignoring case. |
| 5 | Set the options | Header row, number tolerance, difference thresholds, and whether to compare cell formatting, sheet layout and formulas. |
| 6 | Choose the report file | An `.xlsx` file that is neither of the two workbooks. Tick **Open the report when it is finished** if you want it opened. |
| 7 | Check or compare | **Check inputs** only checks the fields. **Compare and create report** checks them, compares and writes the report. **Cancel** stops the work. |

A missing or wrong input is marked beside its field and written to the log; no message box appears. The only question the task asks is whether an existing report file may be replaced, and the default answer is No.

## Options

| SrNo. | Option | Meaning | Default |
|---|---|---|---|
| 1 | Header row | The row that holds the column names. Change it and leave the field: the key lists are read again. | 1 |
| 2 | Number tolerance | Two numbers that differ by no more than this are treated as equal. | 0 |
| 3 | Difference thresholds | For every number difference the report says whether it is greater than each threshold. Up to 8 numbers, separated by commas. | 0, 0.01, 0.1, 1 |
| 4 | Compare cell formatting | Font, alignment, number format, fill and borders. | On |
| 5 | Compare sheet layout | Column widths, row heights, merged cells, freeze panes, hidden rows and columns, filter, comments, hyperlinks, data validation and conditional formatting. | On |
| 6 | Compare formulas | The formula text of each cell. Formulas are not recalculated. | On |

Numbers are typed with a dot as the decimal separator, for example `0.005`.

## The result

The result appears below the inputs on the same page.

| SrNo. | Part | What it shows |
|---|---|---|
| 1 | Verdict | `IDENTICAL - no differences found`, or `NOT IDENTICAL - N of 18 checks found differences` |
| 2 | Row matching | Whether rows were paired by position or by which key columns |
| 3 | Open report / Open folder | Opens the report file, or its folder with the file selected |
| 4 | Summary tab | The 18 checks, each MATCH, DIFF or SKIPPED, with the number of differences |
| 5 | Headers tab | One row per column: name, position, font and alignment in both files |
| 6 | Data Differences tab | One row per cell that differs, with both values, the kind of difference and the threshold columns |
| 7 | Format Differences tab | Formatting that differs, grouped by column and property |
| 8 | Layout & Other tab | Sheet-level checks such as widths, merged cells and freeze panes |

The tabs show the same rows as the sheets of the report. A long list shows its first 1,000 rows on screen; the report holds all of them. If you edit an input after a comparison, a line says that the result belongs to the earlier inputs.

## What is safe

| SrNo. | Point | Behaviour |
|---|---|---|
| 1 | Input workbooks | Opened read-only and shared, so a workbook that is open in Excel can be compared. They are never changed. |
| 2 | Report file | Written completely first and only then put in place. A failed or cancelled run leaves an existing report as it was. |
| 3 | Background work | Reading and comparing run in the background. The inputs are locked meanwhile. Closing the task while it works is refused until the work has stopped. |
| 4 | Other programs | No Excel installation and no Python is needed. |

## Limits

- `.xlsx` and `.xlsm` only.
- Text such as `1,200`, `₹500` or `10%` is not read as a number.
- A formula that was never calculated in Excel has no saved result and compares as blank; the Summary says so.
- Charts, pictures and macros are not compared.

The rules of the comparison itself are described in [PROJECT_GUIDE.md](PROJECT_GUIDE.md), section "Workbook identity check (engine)".
