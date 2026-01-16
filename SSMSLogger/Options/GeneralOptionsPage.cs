using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SSMSLogger.Options
{
    [Guid("5F238789-E306-48AE-93D6-44FCC2EAAC80")]
    internal class GeneralOptionsPage : DialogPage
    {
        [Category("Logging")]
        [DisplayName("Log File Location")]
        [Description("Path to the log file where SQL executions will be recorded.")]
        public string LogFilePath { get; set; } = @"C:\temp\SSMSlog.log";

        [Category("Logging")]
        [DisplayName("Create Daily Log Files")]
        [Description("If enabled, a new log file will be created for each day.")]
        public bool CreateDailyLogFiles { get; set; } = false;

        private GeneralOptionsControl _control;

        protected override IWin32Window Window
        {
            get
            {
                if (_control == null)
                {
                    _control = new GeneralOptionsControl();
                    _control.LogFilePath = LogFilePath;
                    _control.CreateDailyLogFiles = CreateDailyLogFiles;
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
                _control.CreateDailyLogFiles = CreateDailyLogFiles;
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (_control != null)
            {
                LogFilePath = _control.LogFilePath;
                CreateDailyLogFiles = _control.CreateDailyLogFiles;
            }
            base.OnApply(e);
        }
    }
}
