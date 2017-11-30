using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.BLL
{
    public class ApplicationsInfraBuilder
    {
        public async Task CreateNewInfrastructure(int level, string env, IWindowForm form)
        {
            try
            {
                InfraEntitiesServices service = new InfraEntitiesServices();
                var entities = service.GetApplicationInfraEntities(env);
                //If need to build infra level1, or build infra level2 but no existing level1 infra, 
                if (level == 1 || (level==2 && entities == null))
                {
                    ApplicationsLevel1Builder level1 = new ApplicationsLevel1Builder(env, form);
                    await level1.Destroy(entities);
                    entities = await level1.Creat();
                    service.SaveApplicationInfraEntities(entities);
                }

                if (level == 2)
                {
                    ApplicationsLevel2Builder level2 = new ApplicationsLevel2Builder(entities, env, form);
                    await level2.Creat();
                }
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }
    }
}
