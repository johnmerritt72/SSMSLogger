# SSMSLogger

SSMSLogger is an extension for SQL Server Management Studio (SSMS) that automatically logs all SQL queries executed from query windows to a configurable log file. This is useful for auditing, troubleshooting, or tracking SQL activity performed by users in SSMS.

## Features
- Logs every SQL statement executed from SSMS query windows.
- Configurable log file location via the Options UI.
- Option to create a new log file for each day (daily log rotation).
- Simple menu entry under the Tools menu for quick access to options.
- No external dependencies—runs entirely within SSMS as a VSIX extension.

## Installation
1. Build the solution to generate the `.vsix` file.
2. Double-click the `.vsix` file to install the extension into SSMS (version 17 or later).
3. Restart SSMS if required.

## Configuration
- Go to `Tools > Options > SSMSLogger > General` in SSMS.
- Set the desired log file location (e.g., `C:\temp\SSMSlog.log`).
- Optionally enable daily log files.

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
