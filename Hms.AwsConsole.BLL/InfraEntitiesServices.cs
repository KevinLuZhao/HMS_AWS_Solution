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
        InfraEntitiesDb db = new InfraEntitiesDb();

        public void SaveInfraEntities(ApplicationInfraEntities item)
        {
            db.Save(item);
        }
    }
}
