using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
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
            listenThread = new Thread(Listen)
            {
                IsBackground = true
            };
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
                            string keyword = message.Trim().Split('/').First();
                            var splits = message.Replace(keyword, "").Trim('/').Split('/');
                            WorkflowManager.Instance.RunApiTrigger(keyword, splits);

                            if (keyword == "workflow")
                            {
                                ChecksForNewImportedWorkflow(splits);
                            }
                        }
                        else
                        {
                            MyWindows.MainWindow.GetInstance().OnHotKeyHandler(new Shortcut.Shortcut() { HotKey = Config.Instance.ShortcutManagerSettings.JamesHotKey.HotKey });
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
            string workflowPath = $@"{Config.WorkflowFolderLocation}\{splits[0]}";
            if (Directory.Exists(workflowPath))
            {
                if (WorkflowManager.Instance.LoadWorkflow(workflowPath))
                {
                    WorkflowManager.Instance.LoadKeywordTriggers();
                    var icon = new NotifyIcon
                    {
                        Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "James.exe"),
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipTitle = $"workflow: {splits[0]}",
                        BalloonTipText = "was imported successfully!",
                        Visible = true
                    };
                    icon.ShowBalloonTip(1000);
                }
            }
        }
    }
}