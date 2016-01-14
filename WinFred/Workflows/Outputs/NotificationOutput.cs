using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    internal class NotificationOutput : BasicOutput, ISurviveable
    {
        public NotifyIcon LastIcon { get; set; }

        [ComponentField("Period for displaying notification [ms]")]
        public int Timeperiod { get; set; } = 5000;

        public bool CancelNotifyIcon { get; set; } = true;

        public void Cancel()
        {
            if (LastIcon != null && CancelNotifyIcon)
            {
                LastIcon.Visible = false;
                LastIcon = null;
            }
        }

        public override string GetSummary() => $"Displays notification for {Timeperiod} ms";

        public override void Run(string[] output)
        {
            var icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = FormatStringToText(output),
                BalloonTipText = "James-Workflow: " + ParentWorkflow.Name,
                Visible = true
            };
            icon.ShowBalloonTip(Timeperiod);
            LastIcon?.Dispose();
            LastIcon = icon;
        }
    }
}