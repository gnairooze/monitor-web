using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorWebAppConsole
{
    class Logger
    {
        public static string Filename { get; set; }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            StringBuilder bld = new StringBuilder();
            bld.AppendLine(message);
            bld.AppendLine();

            System.IO.File.AppendAllText(Filename, bld.ToString(), Encoding.UTF8);
        }
    }
}
