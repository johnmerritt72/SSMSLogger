using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SSMSLogger.Options;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace SSMSLogger
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SSMSLoggerPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(SSMSLogger.Options.GeneralOptionsPage), "SSMSLogger", "General", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]

    //[ProvideAutoLoad("34A715A0-6587-11D3-B21B-00C04F6852DA", PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SSMSLoggerPackage : AsyncPackage
    {
        /// <summary>
        /// SSMSLoggerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3a408c50-c8c4-4930-b09e-b938b9e9ee39";

        private DTE2 _dte;
        private CommandEvents _queryCommandEvents;
        private const int OpenLogCommandId = 0x0200;
        private static readonly Guid CommandSet = new Guid(PackageGuidString);

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            try
            {
                _dte = (DTE2)await GetServiceAsync(typeof(DTE));
                if (_dte != null)
                {
                    Events2 events2 = (Events2)_dte.Events;
                    _queryCommandEvents = events2.get_CommandEvents(null, 0);
                    _queryCommandEvents.BeforeExecute += QueryCommandEvents_BeforeExecute;
                }
                OleMenuCommandService commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (commandService != null)
                {
                    var openLogCommandID = new CommandID(CommandSet, OpenLogCommandId);
                    var openLogMenuItem = new MenuCommand(OpenSqlLogFile, openLogCommandID);
                    commandService.AddCommand(openLogMenuItem);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "InitializeAsync");
            }
        }

        private void OpenSqlLogFile(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            GeneralOptionsPage options = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            string logFilePath = options.LogFilePath;
            if (options.CreateDailyLogFiles)
            {
                string dir = Path.GetDirectoryName(logFilePath);
                string file = Path.GetFileNameWithoutExtension(logFilePath);
                string ext = Path.GetExtension(logFilePath);
                string datedFile = $"{file}_{DateTime.Now:yyyy-MM-dd}{ext}";
                logFilePath = Path.Combine(dir, datedFile);
            }
            try
            {
                if (!File.Exists(logFilePath))
                {
                    MessageBox.Show($"Log file not found: {logFilePath}", "SSMSLogger", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _dte.ItemOperations.OpenFile(logFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open log file: {ex.Message}", "SSMSLogger", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogError(Exception ex, string context)
        {
            try
            {
                string errorLogDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SSMSLogger");
                Directory.CreateDirectory(errorLogDir);
                string errorLogPath = Path.Combine(errorLogDir, "error.log");
                string message = $"[{DateTime.Now}] Context: {context}\r\n{ex}\r\n----------------------\r\n";
                System.IO.File.AppendAllText(errorLogPath, message);
            }
            catch { /* Swallow any errors in error logging */ }
        }

        private void LogDiagnostic(string message, GeneralOptionsPage options, bool showDateTime = true)
        {
            try
            {
                string logFilePath = options.LogFilePath;
                if (options.CreateDailyLogFiles)
                {
                    string dir = Path.GetDirectoryName(logFilePath);
                    string file = Path.GetFileNameWithoutExtension(logFilePath);
                    string ext = Path.GetExtension(logFilePath);
                    string datedFile = $"{file}_{DateTime.Now:yyyy-MM-dd}{ext}";
                    logFilePath = Path.Combine(dir, datedFile);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                string logdate = showDateTime ? $"[{DateTime.Now}] " : "";
                string fullMessage = $"{logdate}{message}\r\n";
                System.IO.File.AppendAllText(logFilePath, fullMessage);
            }
            catch { /* Swallow any errors in diagnostic logging */ }
        }

        private void QueryCommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            GeneralOptionsPage options = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            var commandName = "";
            try
            {
                commandName = _dte.Commands.Item(Guid, ID)?.Name;
            }
            catch { }
            try
            {
                if (commandName != null && commandName == "Query.Execute")
                {
                    Document doc = _dte.ActiveDocument;
                    string activeDocInfo = doc != null ? $"{doc.Name}" : "";
                    if (doc != null)
                    {
                        TextDocument textDoc = doc.Object("TextDocument") as TextDocument;
                        if (textDoc != null)
                        {
                            string sqlText = GetActiveQuery();
                            LogDiagnostic($"--------------------------------- [{activeDocInfo}] ---------------------------------", options);
                            LogDiagnostic($"{sqlText}\r\n", options, false);
                        }
                        else
                        {
                            LogDiagnostic("QueryCommandEvents_BeforeExecute: TextDocument is null", options);
                        }
                    }
                    else
                    {
                        LogDiagnostic("QueryCommandEvents_BeforeExecute: ActiveDocument is null", options);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "QueryCommandEvents_BeforeExecute");
            }
        }

        private string GetActiveQuery()
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // Make sure we're on the main thread

            Document activeDocument = _dte.ActiveDocument;
            if (activeDocument == null)
            {
                return ""; // No active document
            }

            TextDocument textDoc = activeDocument.Object("TextDocument") as TextDocument;
            if (textDoc == null)
            {
                return ""; // Not a text document
            }

            string sqlToLog;

            // Check for a selection
            if (!textDoc.Selection.IsEmpty)
            {
                // The user executed a specific selection
                sqlToLog = textDoc.Selection.Text;
            }
            else
            {
                // No selection, log the entire document
                EditPoint start = textDoc.StartPoint.CreateEditPoint();
                sqlToLog = start.GetText(textDoc.EndPoint);
            }

            return sqlToLog;
        }

        #endregion
    }
}
