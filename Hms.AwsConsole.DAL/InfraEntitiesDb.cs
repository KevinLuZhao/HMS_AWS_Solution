using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Model;
using System.Collections.Generic;

namespace Hms.AwsConsole.DAL
{
    public class InfraEntitiesDb
    {
        private DynamoDBHelper<InfraEntities> helper;
        private string tableName = "hms_logs";
        public InfraEntitiesDb()
        {
            helper = new DynamoDBHelper<InfraEntities>();
        }
        public void Add(InfraEntities obj)
        {
            helper.CreateItem(tableName, obj);
        }

        public List<InfraEntities> GetLogList()
        {
            return helper.ScanTable(tableName);
        }
    }
}
