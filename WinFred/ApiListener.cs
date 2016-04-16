using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using James.Workflows;

namespace James
{
    public class ApiListener
    {
        private static ApiListener _apiListener;
        private static readonly object SingeltonLock = new object();

        public static ApiListener Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _apiListener ?? (_apiListener = new ApiListener());
                }
            }
        }

        private readonly Thread listenThread;
        private ApiListener()
        {
            listenThread = new Thread(Listen);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        


        /// Listens for incoming messages over the NamedPipes
        /// </summary>
        static void Listen()
        {
            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream("james"))
                {
                    server.WaitForConnection();
                    using (StreamReader reader = new StreamReader(server))
                    {
                        string message = reader.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            var splits = message.Trim('/').Split('/');
                            WorkflowManager.Instance.RunApiTrigger(splits[0], splits);

                            if (splits[0] == "workflow")
                            {
                                ChecksForNewImportedWorkflow(splits);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check for new imported workflow to be added
        /// </summary>
        /// <param name="splits"></param>
        private static void ChecksForNewImportedWorkflow(string[] splits)
        {
            string workflowPath = $@"{Config.WorkflowFolderLocation}\{splits[1]}";
            if (splits[0] == "workflow" && Directory.Exists(workflowPath))
            {
                if (WorkflowManager.Instance.LoadWorkflow(workflowPath))
                {
                    WorkflowManager.Instance.LoadKeywordTriggers();
                    var icon = new NotifyIcon
                    {
                        Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipTitle = $"workflow: {splits[1]}",
                        BalloonTipText = "was imported successfully!",
                        Visible = true
                    };
                    icon.ShowBalloonTip(1000);
                }
            }
        }
    }
}