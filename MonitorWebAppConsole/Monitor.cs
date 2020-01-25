using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorWebAppConsole
{
    public class Monitor
    {
        public enum Status
        {
            NotSet = 0,
            OK = 1,
            Error = 2
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<EventArgs> CheckNow;

        private Status _SiteStatus = Status.NotSet;
        private Timer _T;
        public Status SiteStatus 
        {
            get
            {
                return _SiteStatus;
            }
            set
            {
                var oldStatus = _SiteStatus;
                _SiteStatus = value;

                Logger.Log($"old status: {oldStatus}");
                Logger.Log($"new status: {_SiteStatus}");
                
                OnStatusChanged(new StatusChangedEventArgs()
                {
                    OldStatus = oldStatus,
                    NewStatus = _SiteStatus
                });
            }
        }

        /// <summary>
        /// Interval in minutes
        /// </summary>
        public int Interval { get; set; }
        public void OnStatusChanged(StatusChangedEventArgs e)
        {
            Logger.Log("Status Changed event triggered");

            EventHandler<StatusChangedEventArgs> handler = StatusChanged;

            if(handler != null)
            {
                handler(this, e);
            }
        }
    
        public void Start()
        {
            _T = new Timer(TimerCallback, null, 0, Interval * 1000 * 60);
        }

        private void TimerCallback(Object o)
        {
            Logger.Log("Check Now event triggered");

            EventHandler<EventArgs> handler = CheckNow;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }

            Logger.Log($"Waiting for {Interval} minute(s)");
            Logger.Log("--------------------------------------");
        }
    }
}
