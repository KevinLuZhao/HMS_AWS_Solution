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
                var level1 = new DBLevel1InfraBuilder((Model.Environment)Enum.Parse(typeof(Model.Environment), env), form);
                var response = await level1.Creat();
                var service = new InfraEntitiesServices();
                service.SaveDbInfraEntities(response);
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }

        public async Task Destory(string env, IWindowForm form)
        {
            try
            {
                var builder = new DBLevel1InfraBuilder((Model.Environment)Enum.Parse(typeof(Model.Environment), env), form);
                //var response = await level1.Creat();
                var service = new InfraEntitiesServices();
                var dbInfraEntities = service.GetDbInfraEntities(env);
                
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack trace: " + ex.StackTrace, LogType.Error, env);
            }
        }

        public void GetDBInstanceStatus(string env, string instanceIdentifier)
        {
            RDSHelper helper = new RDSHelper((Model.Environment)Enum.Parse(typeof(Model.Environment), env), "us-east-2");
            helper.FindRDSInstance(instanceIdentifier);
        }
    }
}
