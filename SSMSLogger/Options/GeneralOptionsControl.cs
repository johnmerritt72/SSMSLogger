using System;
using System.Windows.Forms;

namespace SSMSLogger.Options
{
    public class GeneralOptionsControl : UserControl
    {
        private TextBox txtLogFilePath;
        private CheckBox chkCreateDailyLogFiles;
        private Label lblLogFilePath;
        private Label lblCreateDailyLogFiles;

        public GeneralOptionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblLogFilePath = new Label { Text = "Log File Location:", AutoSize = true, Top = 10, Left = 10 };
            txtLogFilePath = new TextBox { Top = 30, Left = 10, Width = 300 };

            lblCreateDailyLogFiles = new Label { Text = "Create Daily Log Files:", AutoSize = true, Top = 70, Left = 10 };
            chkCreateDailyLogFiles = new CheckBox { Top = 90, Left = 10 };

            Controls.Add(lblLogFilePath);
            Controls.Add(txtLogFilePath);
            Controls.Add(lblCreateDailyLogFiles);
            Controls.Add(chkCreateDailyLogFiles);

            this.Width = 350;
            this.Height = 130;
        }

        public string LogFilePath
        {
            get => txtLogFilePath.Text;
            set => txtLogFilePath.Text = value;
        }

        public bool CreateDailyLogFiles
        {
            get => chkCreateDailyLogFiles.Checked;
            set => chkCreateDailyLogFiles.Checked = value;
        }
    }
}
