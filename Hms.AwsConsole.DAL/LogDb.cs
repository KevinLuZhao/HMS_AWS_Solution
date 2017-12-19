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

        public List<Log> GetLogList(string env, string logType, string message)
        {
            var conditions = new List<DynamodbScanCondition>();
            if (!string.IsNullOrEmpty(logType))
            {
                conditions.Add(new DynamodbScanCondition()
                {
                    AttributeName = "LogType",
                    Operator = DynamodbScanOperator.EQ,
                    Value = logType
                });
            }
            if (!string.IsNullOrEmpty(env))
            {
                conditions.Add(new DynamodbScanCondition()
                {
                    AttributeName = "LogKey",
                    Operator = DynamodbScanOperator.EQ,
                    Value = env
                });
            }
            if (!string.IsNullOrEmpty(message))
            {
                conditions.Add(new DynamodbScanCondition()
                {
                    AttributeName = "Message",
                    Operator = DynamodbScanOperator.CONTAINS,
                    Value = message
                });
            }
            return helper.ScanTable("hms_logs", conditions);
        }
    }
}
