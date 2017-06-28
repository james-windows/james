using System;
using System.Drawing;
using System.Windows.Forms;
using James.HelperClasses;
using James.MyWindows;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    internal class NotificationOutput : BasicOutput, ISurviveable
    {
        public NotifyIcon LastIcon { get; set; }

        public bool CancelNotifyIcon { get; set; } = true;

        public void Cancel()
        {
            if (LastIcon != null && CancelNotifyIcon)
            {
                LastIcon.Visible = false;
                LastIcon = null;
            }
        }

        public override string GetSummary() => $"Displays notification";

        public override void Run(string[] output)
        {
            var icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = FormatString.InsertArguments(output),
                BalloonTipText = "James-Workflow: " + ParentWorkflow.Name,
                Visible = true
            };
            icon.ShowBalloonTip(1000);
            LastIcon = icon;
            MainWindow.GetInstance().HideWindow();
        }

        public override string GetDescription() => "Displays a message using the platform's notification api";
    }
}
