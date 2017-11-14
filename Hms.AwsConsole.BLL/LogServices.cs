using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.DAL;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.BLL
{
    public class LogServices
    {
        static LogDb db = new LogDb();
        public static void WriteLog(string message, LogType logType, string logKey)
        {
            Log log = new Log()
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                LogType = logType,
                Date = DateTime.Now,
                User = System.Environment.UserDomainName + "\\" + System.Environment.UserName,
                CategoryKey = "SafeArrival Admin Toolkits",
                LogKey = logKey
            };
            db.Add(log);
        }

        public static List<Log> GetLogList()
        {
            List<Log> ret = db.GetLogList();
            ret.Sort((a, b) => b.Date.CompareTo(a.Date));
            return ret.Take(100).ToList();
        }
    }
}
