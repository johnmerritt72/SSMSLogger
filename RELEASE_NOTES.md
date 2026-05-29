# SSMSLogger Release Notes

## 1.4.8 — 2026-05-29

### New: size-based log file rollover

Added the ability to start a new log file once the active file reaches a
configurable size, in addition to the existing daily-rotation option.

**Log File Mode** is now selected with radio buttons in
`Tools > Options > SSMSLogger > General`. The three modes are mutually exclusive:

- **Single log file** — all activity is appended to one file (default).
- **New log file daily** — a new file is created each day (`SSMSlog_yyyy-MM-dd.log`). Unchanged from prior versions.
- **New log file by size** — when the active file reaches **Max Log File Size (KB)** (default `5120` KB / 5 MB), it is archived to `SSMSlog-yyyy-MM-dd.log` (today's date) and a fresh file is started.

**Same-day rollovers** never overwrite an existing archive: if
`SSMSlog-yyyy-MM-dd.log` already exists, a numeric suffix is appended
(`SSMSlog-yyyy-MM-dd-1.log`, `-2`, and so on).

### Upgrade notes

- Users who previously enabled **Create Daily Log Files** are automatically
  migrated to the **New log file daily** mode — no reconfiguration needed.
- A rename that fails (for example, the file is locked) is recorded to the
  error log and logging continues to the existing file, so no log data is lost.

### Other

- Version bumped to 1.4.8 so the extension upgrades cleanly in SSMS.
- Updated `readme.md` to document the three log file modes.
