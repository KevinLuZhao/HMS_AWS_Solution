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

        public List<Log> GetLogList(string logType, string logKey)
        {
            var conditions = new List<DynamodbScanCondition>();
            if (!string.IsNullOrEmpty(logType.ToString()))
            {
                conditions.Add(new DynamodbScanCondition()
                {
                    AttributeName = "LogType",
                    Operator = DynamodbScanOperator.EQ,
                    Value = logType
                });
            }
            if (!string.IsNullOrEmpty(logKey.ToString()))
            {
                conditions.Add(new DynamodbScanCondition()
                {
                    AttributeName = "LogKey",
                    Operator = DynamodbScanOperator.EQ,
                    Value = logKey
                });
            }
            return helper.ScanTable("hms_logs", conditions);
        }
    }
}
