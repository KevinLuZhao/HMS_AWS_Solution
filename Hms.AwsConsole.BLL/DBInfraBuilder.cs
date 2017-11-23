using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.BLL
{
    public class DBInfraBuilder
    {
        public async Task CreateNewInfrastructure(string env, IWindowForm form)
        {
            try
            {
                //Level1Builder level1 = new Level1Builder(env, form);
                //await level1.Teardown();
                //var response = await level1.Creat();
                //InfraEntitiesServices service = new InfraEntitiesServices();
                //service.SaveInfraEntities(response);
                //Level2Builder level2 = new Level2Builder(response, env, form);
                //await level2.Creat();
                DBLevel1InfraBuilder level1 = 
                    new DBLevel1InfraBuilder((Model.Environment)Enum.Parse(typeof(Model.Environment), env), form);
                var response = await level1.Creat();
                InfraEntitiesServices service = new InfraEntitiesServices();
                service.SaveDbInfraEntities(response);
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }
    }
}
