# Comparison measurements and operating limits

## Reproducible measurements

Run the built application with `--measure-comparison <output.json>`. It generates only synthetic tables and calls the same `ComparisonRows` matcher used by the retained comparison paths. It checks every matched row's original occurrence and all column values, and requires the expected matched/unmatched counts. It does not connect to a database, open a user file or persist application settings.

| Case | Rows per side | Columns | Distinct reference keys | Purpose |
| --- | ---: | ---: | ---: | --- |
| `unique-10000` | 10,000 | 16 | 10,000 | Many different quoted text keys |
| `duplicates-50000` | 50,000 | 16 | 500 | Cached duplicate occurrence queues and exhaustion |
| `wide-10000` | 10,000 | 128 | 1,000 | Wider snapshots with 2,560,000 cells across both sides |

Every case runs after a small warm-up. JSON records fixture generation time separately from matching plus verification, managed memory above the pre-fixture baseline, process working set, process lifetime peak working set, process bitness, processor count, operating system, CLR version and installed-framework Release value. The lifetime peak is not an isolated per-case peak. Managed memory is a sample that includes fixtures, cache allocations and garbage not yet collected; it is not an allocation profiler.

The `Framework 4.8 compatibility and measurements` workflow records these cases for Any CPU and x64 on both Windows runner images and uploads the JSON with its test logs. Use the artifact from the exact commit for numeric evidence. Runner load, runtime, bitness and input shape affect measurements, so there is no fixed millisecond pass threshold. A failed correctness assertion fails the check.

These measurements exclude database reads and Excel report creation. They establish repeatable matching evidence for the stated shapes, not an end-to-end throughput claim or a maximum supported dataset size. Full report content remains covered by the executable correctness/integration suites.

## Limits

Database/List/legacy Excel comparison snapshots are held in memory, and reports require additional allocations. Width, text lengths, number of differences and repeated-key distribution can change memory use substantially. There is no universal file-size limit that the current implementation guarantees. Use the x64 build for large inputs, run representative smaller samples first, and reserve additional memory for report generation.

The separate Excel Sheet Audit has explicit budgets: at default settings it retains at most 100,000 details and 64 MiB of estimated detail payload per section, and allows 128 MiB of estimated partition payload. These protect those specific buffers; they do not guarantee total process memory. Its reader rejects rows over 8 MiB of text and enforces worksheet coordinates within 1,048,576 rows and 16,384 columns. Counts and truncation/incomplete indicators must be reviewed when details or feature coverage are limited. These audit budgets do not apply to the legacy comparison engine or workbook identity task.

To establish an operational limit, record the tested version, Windows/runtime, bitness, available RAM, source/target dimensions, key distribution, text-length distribution, elapsed read/compare/export times, peak memory and resulting workbook counts. Confirm the report opens correctly in the intended spreadsheet application. Do not extrapolate these bounded synthetic cases to arbitrary gigabyte-sized workbooks.
