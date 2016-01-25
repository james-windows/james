using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        static void Listen()
        {
            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream("james"))
                {
                    server.WaitForConnection();
                    using (StreamReader reader = new StreamReader(server))
                    {
                        string message = reader.ReadLine().Trim('/');
                        if (message != null)
                        {
                            WorkflowManager.Instance.RunApiTrigger(message.Split('/').FirstOrDefault(), message.Split('/'));
                        }
                    }
                }
            }
        }
    }
}
