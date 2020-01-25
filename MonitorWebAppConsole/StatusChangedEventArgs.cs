using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorWebAppConsole
{
    public class StatusChangedEventArgs:EventArgs
    {
        public Monitor.Status OldStatus { get; set; }
        public Monitor.Status NewStatus { get; set; }
    }
}
