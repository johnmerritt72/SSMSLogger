# Size-Based Log File Rollover — Design

**Date:** 2026-05-29
**Status:** Approved

## Summary

Add a third log-file mode to the SSMSLogger extension: rolling over to a new
log file once the active file reaches a configurable size in KB. The three
modes (single file, new file daily, new file by size) are mutually exclusive
and selected via radio buttons in the Options UI.

## Motivation

The extension currently supports a single static log file or a new file per
day. Long-running SSMS sessions with heavy query activity can produce very
large single-day files. A size-based rollover keeps individual files
manageable while preserving all history through dated archives.

## Options Model

`GeneralOptionsPage` exposes a single enum property that backs the radio group:

```csharp
public enum LogFileMode { Single, Daily, Size }
```

| Property            | Type        | Default        | Notes                                              |
|---------------------|-------------|----------------|----------------------------------------------------|
| `LogFilePath`       | string      | `C:\temp\SSMSlog.log` | Unchanged.                                  |
| `LogFileMode`       | LogFileMode | `Single`       | Backs the radio group.                             |
| `MaxLogFileSizeKB`  | int         | `5120`         | Only used when `LogFileMode == Size`. 5120 KB = 5 MB. |
| `CreateDailyLogFiles` | bool      | `false`        | **Hidden/legacy.** Retained only for migration.    |

### Backward-compatibility migration

`DialogPage` persists public properties to the registry. Existing users may
have `CreateDailyLogFiles = true` saved from a prior version. On load, if
`LogFileMode == Single` **and** `CreateDailyLogFiles == true`, treat the mode
as `Daily`. This runs when the page/control initializes so no one loses their
daily-logging setting on upgrade.

## Options UI (`GeneralOptionsControl`)

Replace the single "Create Daily Log Files" checkbox with a radio group:

- ◉ **Single log file** (default)
- ○ **New log file daily** — produces `SSMSlog_yyyy-MM-dd.log` (underscore, existing behavior)
- ○ **New log file by size** — enables the **Max Log File Size (KB)** textbox

The KB textbox is enabled only when "by size" is selected; otherwise disabled.
The existing version label remains.

## Path Resolution

Extract the path logic currently duplicated in `OpenSqlLogFile` and
`LogDiagnostic` into a single helper:

```csharp
private string ResolveLogFilePath(GeneralOptionsPage options)
```

- `Single`: returns `LogFilePath` as-is.
- `Daily`: returns `{dir}\{name}_{yyyy-MM-dd}{ext}` (unchanged behavior).
- `Size`: returns `LogFilePath` as-is (the active file; rollover renames it).

## Rollover Logic

Rollover is evaluated in `LogDiagnostic` (the write path), before each append:

1. Only when `LogFileMode == Size`.
2. If the active file exists and its length `>= MaxLogFileSizeKB * 1024`, roll over.
3. Rename the active file to `{dir}\SSMSlog-{yyyy-MM-dd}{ext}` (today's date, **dash** separator).
4. **Collision:** if that name exists, try `SSMSlog-{yyyy-MM-dd}-1{ext}`,
   `-2`, etc. until an unused name is found. Never overwrites.
5. After rename, the next `AppendAllText` recreates the active file fresh.

`OpenSqlLogFile` opens whatever `ResolveLogFilePath` returns (the current
active file).

## Error Handling

The rename is wrapped in try/catch routed to the existing `LogError`. If the
rename fails (e.g., file locked), logging continues to the existing file —
no data loss, no crash. This matches the existing swallow-and-continue style
of `LogDiagnostic`.

## Versioning

Bump `1.4.7` → `1.4.8` so SSMS upgrades the installed VSIX:

- `Properties/AssemblyInfo.cs`: `AssemblyVersion` and `AssemblyFileVersion` → `1.4.8.0`
- `source.extension.vsixmanifest`: `Identity Version` → `1.4.8`

## Documentation

Update `readme.md`:
- Features: mention size-based rollover alongside daily rotation.
- Configuration: describe the three modes and the Max Log File Size (KB) setting.

## Testing

No test project exists in this VSIX solution. Verification:
- Build the solution and confirm a clean compile.
- Runtime rollover behavior (rename, collision suffixes, mode switching) is
  validated manually in SSMS by the user.

## Out of Scope

- Time-based rollover other than daily.
- Compression or pruning of archived files.
- Configurable archive naming format.
