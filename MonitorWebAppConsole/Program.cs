using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorWebAppConsole
{
    class Program
    {
        static Monitor _Monitor = new Monitor();
        static void Main(string[] args)
        {
            initLogger();

            initWebClient();
            
            initMonitor();

            initAlert();

            _Monitor.Start();

            Console.WriteLine("Monitor Sites In-Progress (do not close) ...");
            Console.ReadLine();
        }

        private static void initLogger()
        {
            Logger.Filename = Properties.Settings.Default.LogFilePath;
        }

        private static void initAlert()
        {
            Alert.AlertFromMail = Properties.Settings.Default.AlertFromEmail;
            Alert.AlertFromName = Properties.Settings.Default.AlertFromName;
            Alert.AlertFromPassword = Properties.Settings.Default.AlertFromPassword;
            Alert.AlertToMail = Properties.Settings.Default.AlertToEmail;
            Alert.AlertToName = Properties.Settings.Default.AlertToName;
            Alert.DemoMode = Properties.Settings.Default.DemoMode;
        }

        private static void initMonitor()
        {
            _Monitor.StatusChanged += _Monitor_StatusChanged;
            _Monitor.CheckNow += _Monitor_CheckNow;
            _Monitor.Interval = Properties.Settings.Default.Interval;
        }

        private static void initWebClient()
        {
            WebClient.Timeout = Properties.Settings.Default.Timeout;
        }

        private static void _Monitor_CheckNow(object sender, EventArgs e)
        {
            var method = Properties.Settings.Default.Method;
            var url = Properties.Settings.Default.URL;
            var mediatype = Properties.Settings.Default.MediaType;
            var payload = Properties.Settings.Default.PayLoad;
            
            var task = WebClient.ExecuteAsync(method, url, mediatype, payload);

            task.Wait();

            _Monitor.SiteStatus = (task.Result) ? Monitor.Status.OK : Monitor.Status.Error;
        }

        private static void _Monitor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if(e.OldStatus == Monitor.Status.NotSet)
            {
                Logger.Log($"{DateTime.Now} - Monitor Started");
                
                Alert.SendMail(Alert.ComposeMailSubject(Alert.AlertType.MonitorStarted), Alert.ComposeMailBody(Alert.AlertType.MonitorStarted));
            }
            else if(e.OldStatus == e.NewStatus)
            {
                Logger.Log($"{DateTime.Now} - No status change");
            }
            else if (e.OldStatus == Monitor.Status.OK && e.NewStatus == Monitor.Status.Error)
            {
                Logger.Log($"{DateTime.Now} - Site is down");

                Alert.SendMail(Alert.ComposeMailSubject(Alert.AlertType.SiteDown), Alert.ComposeMailBody(Alert.AlertType.SiteDown));
            }
            else if (e.OldStatus == Monitor.Status.Error && e.NewStatus == Monitor.Status.OK)
            {
                Logger.Log($"{DateTime.Now} - Site is up");

                Alert.SendMail(Alert.ComposeMailSubject(Alert.AlertType.SiteUp), Alert.ComposeMailBody(Alert.AlertType.SiteUp));
            }
        }
    }
}
