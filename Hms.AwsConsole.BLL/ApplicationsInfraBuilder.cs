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
        public async Task CreateNewInfrastructure(string env, IWindowForm form)
        {
            try
            {
                InfraEntitiesServices service = new InfraEntitiesServices();
                var entities = service.GetApplicationInfraEntities(env);
                ApplicationsLevel1Builder level1 = new ApplicationsLevel1Builder(env, form);
                await level1.Destroy(entities);
                var response = await level1.Creat();
                service.SaveApplicationInfraEntities(response);
                ApplicationsLevel2Builder level2 = new ApplicationsLevel2Builder(response, env, form);
                await level2.Creat();
                
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }
    }
}
