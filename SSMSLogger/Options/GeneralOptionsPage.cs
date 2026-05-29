using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SSMSLogger.Options
{
    /// <summary>
    /// Determines how the log file rolls over to a new file.
    /// </summary>
    public enum LogFileMode
    {
        /// <summary>A single log file is used and never rolled over.</summary>
        Single,
        /// <summary>A new log file is created for each day.</summary>
        Daily,
        /// <summary>A new log file is created once the active file reaches a configured size.</summary>
        Size
    }

    [Guid("5F238789-E306-48AE-93D6-44FCC2EAAC80")]
    internal class GeneralOptionsPage : DialogPage
    {
        [Category("Logging")]
        [DisplayName("Log File Location")]
        [Description("Path to the log file where SQL executions will be recorded.")]
        public string LogFilePath { get; set; } = @"C:\temp\SSMSlog.log";

        [Category("Logging")]
        [DisplayName("Log File Mode")]
        [Description("How the log file rolls over: a single file, a new file each day, or a new file once it reaches the configured size.")]
        public LogFileMode LogFileMode { get; set; } = LogFileMode.Single;

        [Category("Logging")]
        [DisplayName("Max Log File Size (KB)")]
        [Description("When Log File Mode is Size, a new log file is started once the active file reaches this many kilobytes.")]
        public int MaxLogFileSizeKB { get; set; } = 5120;

        // Legacy setting retained only so existing saved preferences can be migrated to LogFileMode.
        [Browsable(false)]
        public bool CreateDailyLogFiles { get; set; } = false;

        private GeneralOptionsControl _control;

        /// <summary>
        /// Resolves the effective mode, migrating the legacy <see cref="CreateDailyLogFiles"/> setting
        /// for users who enabled daily logging before the mode option existed.
        /// </summary>
        private LogFileMode EffectiveMode()
        {
            if (LogFileMode == LogFileMode.Single && CreateDailyLogFiles)
            {
                LogFileMode = LogFileMode.Daily;
                CreateDailyLogFiles = false;
            }
            return LogFileMode;
        }

        protected override IWin32Window Window
        {
            get
            {
                if (_control == null)
                {
                    _control = new GeneralOptionsControl();
                    _control.LogFilePath = LogFilePath;
                    _control.LogFileMode = EffectiveMode();
                    _control.MaxLogFileSizeKB = MaxLogFileSizeKB;
                }
                return _control;
            }
        }

        protected override void OnActivate(System.ComponentModel.CancelEventArgs e)
        {
            base.OnActivate(e);
            if (_control != null)
            {
                _control.LogFilePath = LogFilePath;
                _control.LogFileMode = EffectiveMode();
                _control.MaxLogFileSizeKB = MaxLogFileSizeKB;
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (_control != null)
            {
                LogFilePath = _control.LogFilePath;
                LogFileMode = _control.LogFileMode;
                MaxLogFileSizeKB = _control.MaxLogFileSizeKB;
            }
            base.OnApply(e);
        }
    }
}
