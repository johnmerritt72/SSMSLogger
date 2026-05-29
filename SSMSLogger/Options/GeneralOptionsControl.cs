using System;
using System.Windows.Forms;
using System.Reflection;

namespace SSMSLogger.Options
{
    public class GeneralOptionsControl : UserControl
    {
        private TextBox txtLogFilePath;
        private Label lblLogFilePath;
        private GroupBox grpLogFileMode;
        private RadioButton rdoSingle;
        private RadioButton rdoDaily;
        private RadioButton rdoSize;
        private Label lblMaxLogFileSizeKB;
        private TextBox txtMaxLogFileSizeKB;
        private Label lblVersion;

        public GeneralOptionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblLogFilePath = new Label { Text = "Log File Location:", AutoSize = true, Top = 10, Left = 10 };
            txtLogFilePath = new TextBox { Top = 30, Left = 10, Width = 300 };

            grpLogFileMode = new GroupBox { Text = "Log File Mode", Top = 65, Left = 10, Width = 300, Height = 110 };
            rdoSingle = new RadioButton { Text = "Single log file", AutoSize = true, Top = 20, Left = 10 };
            rdoDaily = new RadioButton { Text = "New log file daily", AutoSize = true, Top = 45, Left = 10 };
            rdoSize = new RadioButton { Text = "New log file by size", AutoSize = true, Top = 70, Left = 10 };
            grpLogFileMode.Controls.Add(rdoSingle);
            grpLogFileMode.Controls.Add(rdoDaily);
            grpLogFileMode.Controls.Add(rdoSize);

            lblMaxLogFileSizeKB = new Label { Text = "Max Log File Size (KB):", AutoSize = true, Top = 185, Left = 10 };
            txtMaxLogFileSizeKB = new TextBox { Top = 205, Left = 10, Width = 100 };

            // Version label
            lblVersion = new Label
            {
                AutoSize = true,
                Top = 240,
                Left = 10,
                Text = $"SSMSLogger Version: {GetVersion()}"
            };

            Controls.Add(lblLogFilePath);
            Controls.Add(txtLogFilePath);
            Controls.Add(grpLogFileMode);
            Controls.Add(lblMaxLogFileSizeKB);
            Controls.Add(txtMaxLogFileSizeKB);
            Controls.Add(lblVersion);

            this.Width = 350;
            this.Height = 280;

            rdoSize.CheckedChanged += (s, e) => UpdateSizeEnabled();
            UpdateSizeEnabled();
        }

        private void UpdateSizeEnabled()
        {
            bool sizeMode = rdoSize.Checked;
            lblMaxLogFileSizeKB.Enabled = sizeMode;
            txtMaxLogFileSizeKB.Enabled = sizeMode;
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString() : "Unknown";
        }

        public string LogFilePath
        {
            get => txtLogFilePath.Text;
            set => txtLogFilePath.Text = value;
        }

        public LogFileMode LogFileMode
        {
            get
            {
                if (rdoSize.Checked) return LogFileMode.Size;
                if (rdoDaily.Checked) return LogFileMode.Daily;
                return LogFileMode.Single;
            }
            set
            {
                rdoSingle.Checked = value == LogFileMode.Single;
                rdoDaily.Checked = value == LogFileMode.Daily;
                rdoSize.Checked = value == LogFileMode.Size;
                UpdateSizeEnabled();
            }
        }

        public int MaxLogFileSizeKB
        {
            get
            {
                return int.TryParse(txtMaxLogFileSizeKB.Text, out int kb) && kb > 0 ? kb : 5120;
            }
            set => txtMaxLogFileSizeKB.Text = value.ToString();
        }
    }
}
