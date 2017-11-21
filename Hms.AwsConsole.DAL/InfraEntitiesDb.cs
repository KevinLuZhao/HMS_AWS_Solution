using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Model;
using System.Collections.Generic;

namespace Hms.AwsConsole.DAL
{
    public class InfraEntitiesDb
    {
        private DynamoDBHelper<ApplicationInfraEntities> helper;
        private string tableName = "infra_entities";
        public InfraEntitiesDb()
        {
            helper = new DynamoDBHelper<ApplicationInfraEntities>();
        }
        public void Save(ApplicationInfraEntities obj)
        {
            helper.CreateItem(tableName, obj);
        }

        public List<ApplicationInfraEntities> GetList()
        {
            return helper.ScanTable(tableName);
        }

        //public InfraEntities GetItem(Environment env)
        //{
        //    return helper.g
        //}
    }
}
