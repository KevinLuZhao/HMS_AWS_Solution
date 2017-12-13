using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class Log
    {
        public string Id { get; set; }
        //public LogType LogType { get; set; }
        public string LogType { get; set; }
        public string LogKey { get; set; }
        public string Message { get; set; }
        public string CategoryKey { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }
}
