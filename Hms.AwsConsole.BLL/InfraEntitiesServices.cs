using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.DAL;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.BLL
{
    public class InfraEntitiesServices
    {
        GeneralDb<ApplicationInfraEntities> applicationEntitiesDb = 
            new GeneralDb<ApplicationInfraEntities>("hms_application_infra_entities");

        GeneralDb<DBInfraEntities> DbEntitiesDb =
            new GeneralDb<DBInfraEntities>("hms_db_infra_entities");

        public ApplicationInfraEntities GetApplicationInfraEntities(string environment)
        {
            return applicationEntitiesDb.GetItem(environment);
        }

        public void SaveApplicationInfraEntities(ApplicationInfraEntities item)
        {
            applicationEntitiesDb.Save(item);
        }

        public void DeleteApplicationInfraEntities(string environment)
        {
            applicationEntitiesDb.DeleteItem(environment);
        }

        public DBInfraEntities GetDbInfraEntities(string environment)
        {
            return DbEntitiesDb.GetItem(environment);
        }

        public void SaveDbInfraEntities(DBInfraEntities item)
        {
            DbEntitiesDb.Save(item);
        }

        public void DeleteDbInfraEntities(string environment)
        {
            DbEntitiesDb.DeleteItem(environment);
        }
    }
}
