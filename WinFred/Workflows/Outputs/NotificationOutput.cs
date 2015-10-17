using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using James.HelperClasses;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    class NotificationOutput : BasicOutput, ISurviveable
    {
        public NotifyIcon LastIcon { get; set; } = null;

        [DataMember]
        [ComponentField("Timeout between each tick [ms]")]
        public int Timeout { get; set; } = 5000;
        [DataMember]
        public bool CancelNotifyIcon { get; set; } = true;

        public override string GetSummary() => $"Displays notification for {Timeout} ms";

        public override void Display(string output)
        {
            NotifyIcon icon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = output,
                BalloonTipTitle = "James-Workflow: " + ParentWorkflow.Title,
                Visible = true
            };
            icon.ShowBalloonTip(Timeout);
            LastIcon?.Dispose();
            LastIcon = icon;
        }

        public void Cancel()
        {
            if (LastIcon != null && CancelNotifyIcon)
            {
                LastIcon.Visible = false;
                LastIcon = null;
            }
        }
    }
}
