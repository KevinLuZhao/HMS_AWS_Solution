using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Model;
using System.Collections.Generic;

namespace Hms.AwsConsole.DAL
{
    public class LogDb
    {
        private DynamoDBHelper<Log> helper;
        private string tableName = "hms_logs";
        public LogDb()
        {
            helper = new DynamoDBHelper<Log>();
        }
        public void Add(Log obj)
        {
            helper.CreateItem(tableName, obj);
        }

        public List<Log> GetLogList()
        {
            return helper.ScanTable("hms_logs");
        }
    }
}
