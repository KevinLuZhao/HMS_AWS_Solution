using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.DAL;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.BLL
{
    public class DBInstanceServices
    {
        RDSHelper helper;
        string environment;

        public DBInstanceServices(string env, string region)
        {
            helper = new RDSHelper((Model.Environment)Enum.Parse(typeof(Model.Environment),env), region);
            environment = env.ToString();
        }

        public AwsRdsInstance GetDBInstance()
        {
            var db = new GeneralDb<DBInfraEntities>("hms_db_infra_entities");
            string instanceIdentifier = db.GetItem(environment).DBInstanceId;
            return helper.FindRDSInstance(instanceIdentifier);
        }
    }
}
