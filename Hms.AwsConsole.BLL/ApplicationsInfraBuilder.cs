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
                ApplicationsLevel1Builder level1 = new ApplicationsLevel1Builder(env, form);
                await level1.Teardown();
                var response = await level1.Creat();
                ApplicationsLevel2Builder level2 = new ApplicationsLevel2Builder(response, env, form);
                await level2.Creat();
                InfraEntitiesServices service = new InfraEntitiesServices();
                service.SaveApplicationInfraEntities(response);
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }
    }
}
