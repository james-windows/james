using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    internal class NotificationOutput : BasicOutput, ISurviveable
    {
        public NotifyIcon LastIcon { get; set; }

        [DataMember]
        [ComponentField("Period for displaying notification [ms]")]
        public int Timeperiod { get; set; } = 5000;

        [DataMember]
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

        public override void Display(string output)
        {
            var icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = output,
                BalloonTipTitle = "James-Workflow: " + ParentWorkflow.Title,
                Visible = true
            };
            icon.ShowBalloonTip(Timeperiod);
            LastIcon?.Dispose();
            LastIcon = icon;
        }
    }
}