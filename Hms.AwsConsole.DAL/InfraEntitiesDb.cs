using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Model;
using System.Collections.Generic;

namespace Hms.AwsConsole.DAL
{
    public class InfraEntitiesDb
    {
        private DynamoDBHelper<InfraEntities> helper;
        private string tableName = "infra_entities";
        public InfraEntitiesDb()
        {
            helper = new DynamoDBHelper<InfraEntities>();
        }
        public void Save(InfraEntities obj)
        {
            helper.CreateItem(tableName, obj);
        }

        public List<InfraEntities> GetList()
        {
            return helper.ScanTable(tableName);
        }

        //public InfraEntities GetItem(Environment env)
        //{
        //    return helper.g
        //}
    }
}
