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
    public class InfraBuilder
    {
        public async Task CreateNewInfrastructure(string env, IWindowForm form)
        {
            try
            {
                Level1Builder level1 = new Level1Builder(env, form);
                await level1.Teardown();
                var response = await level1.Creat();
                InfraEntitiesServices service = new InfraEntitiesServices();
                service.SaveInfraEntities(response);
                Level2Builder level2 = new Level2Builder(response, env, form);
                await level2.Creat();
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }
    }
}
