# SSMSLogger

SSMSLogger is an extension for SQL Server Management Studio (SSMS) that automatically logs all SQL queries executed from query windows to a configurable log file. This is useful for auditing, troubleshooting, or tracking SQL activity performed by users in SSMS.

## Features
- Logs every SQL statement executed from SSMS query windows.
- Configurable log file location via the Options UI.
- Three log file modes (mutually exclusive):
  - **Single log file** — all activity goes to one file.
  - **New log file daily** — a new file is created each day (`SSMSlog_yyyy-MM-dd.log`).
  - **New log file by size** — when the active file reaches a configurable size (KB), it is archived to `SSMSlog-yyyy-MM-dd.log` and a fresh file is started. If that dated name already exists (multiple rollovers in one day), a numeric suffix is added (`SSMSlog-yyyy-MM-dd-1.log`).
- Simple menu entry under the Tools menu for quick access to options.
- No external dependencies�runs entirely within SSMS as a VSIX extension.

## Installation
1. Build the solution to generate the `.vsix` file.
2. Double-click the `.vsix` file to install the extension into SSMS (version 17 or later).
3. Restart SSMS if required.

## Configuration
- Go to `Tools > Options > SSMSLogger > General` in SSMS.
- Set the desired log file location (e.g., `C:\temp\SSMSlog.log`).
- Choose a **Log File Mode**: Single log file, New log file daily, or New log file by size.
- When using **New log file by size**, set **Max Log File Size (KB)** (default `5120`, i.e. 5 MB) to control when the active file is archived and a new one started.

## Usage
- Execute any SQL query in a query window.
- The executed SQL and basic document info will be appended to the configured log file.
- Each log entry includes a timestamp and the name of the document.

## Limitations
- Due to SSMS extensibility limitations, the extension cannot reliably capture the database server or database name for each query.
- Only queries executed from SSMS query windows are logged.
- The extension relies on the DTE automation model, which may not expose all query execution events in all SSMS versions.

## Troubleshooting
- If no log file is created, ensure the extension is enabled and the log file path is writable.
- Check the error log at `C:\temp\SSMSLogger_error.log` for diagnostic messages.
- The extension may require you to open the Options UI or use the menu command once per SSMS session to initialize.

## Contributing
Contributions are welcome! Please open issues or pull requests on GitHub.

## License
This project is licensed under the MIT License.
