using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hms.AwsConsole.BLL
{
    public class ApplicationsLevel2Builder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;
        ApplicationInfraEntities entities;

        const string STR_JUMPBOX_INSTANCE = "Jumpbox";
        const string STR_WEBSERVER_INSTANCE = "HMS-WEB-SERVER";
        const string STR_SERVICES_INSTANCE = "HMS-SERVICES-SERVER";
        const string STR_EMAILFILTER_INSTANCE = "HMS-EMAILFILTER-SERVER";

        public ApplicationsLevel2Builder(ApplicationInfraEntities entities, string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env);
            this.entities = entities;
        }

        public async Task<ApplicationInfraEntities> Creat()
        {
            try
            {
                var response = await LaunchJumpbox();
                entities.Instances = new List<string>();
                entities.Instances.Clear();
                entities.Instances.Add(response);
                monitorForm.ShowCallbackMessage($"{STR_JUMPBOX_INSTANCE} is created");
                response = await LaunchWebServer();
                entities.Instances.Add(response);
                monitorForm.ShowCallbackMessage($"{STR_WEBSERVER_INSTANCE} is created");
                response = await LaunchServicesServer();
                entities.Instances.Add(response);
                monitorForm.ShowCallbackMessage($"{STR_SERVICES_INSTANCE} is created");
                response = await LaunchEmailFilterServer();
                entities.Instances.Add(response);
                monitorForm.ShowCallbackMessage($"{STR_EMAILFILTER_INSTANCE} is created");
                return entities;
            }
            catch (Exception ex)
            {
                LogServices.WriteLog(ex.Message + " Stack Trace: " + ex.StackTrace, 
                    Model.LogType.Error, GlobalVariables.Enviroment.ToString());
                return entities;
                //throw ex;
            }
            finally
            {
                var serivce = new InfraEntitiesServices();
                serivce.SaveApplicationInfraEntities(entities);
            }
        }

        public async Task Destroy(ApplicationInfraEntities entities)
        {
            if (entities.Instances == null)
                return;

            await ec2Helper.DeleteInstances(entities.Instances);
            var serivce = new InfraEntitiesServices();
            entities.Instances = null;
            serivce.SaveApplicationInfraEntities(entities);
        }

        public async Task<List<AwsAppInstance>> GetAllAppInstances(ApplicationInfraEntities entities)
        {
            if (entities.Instances == null)
                return null;
            return await ec2Helper.GetInstancesListByIds(entities.Instances);
        }

        private async Task<string> LaunchJumpbox()
        {
            string str = @"<powershell>
  set-ExecutionPolicy remotesigned -force
  iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex
  choco install -y vcredist2015
  choco install -y dotnet4.5
  choco install -y mysql.workbench
  choco install -y putty.install
  choco install -y googlechrome
  choco install -y awscli
</powershell>";
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str);
            string userData = Convert.ToBase64String(encbuff);
            var response = await ec2Helper.LaunchSingleInstance(STR_JUMPBOX_INSTANCE,
                entities.PublicSubnetId, "ami-5d99b938", "hms_qa_keypair",
                new List<string> { entities.PublicSecurityGroupId}, "T2.Micro", userData);
            return response.InstanceId;
        }

        private async Task<string> LaunchWebServer()
        {
            var response = await ec2Helper.LaunchSingleInstance(STR_WEBSERVER_INSTANCE,
                entities.PublicSubnetId, "ami-986b42fd", "hms_qa_keypair",
                new List<string> { entities.PublicSecurityGroupId }, "T2.Micro");
            return response.InstanceId;
        }

        private async Task<string> LaunchServicesServer()
        {
            var response = await ec2Helper.LaunchSingleInstance(STR_SERVICES_INSTANCE,
                entities.PrivateSubnetId, "ami-40684125", "hms_qa_keypair",
                new List<string> { entities.PrivateSecurityGroupId }, "T2.Medium");
            return response.InstanceId;
        }

        private async Task<string> LaunchEmailFilterServer()
        {
            var response = await ec2Helper.LaunchSingleInstance(STR_EMAILFILTER_INSTANCE,
                entities.PrivateSubnetId, "ami-9b6b42fe", "hms_qa_keypair", null, "T2.Medium");
            return response.InstanceId;
        }

        //private async Task<Instance> LaunchEmailServiceServer()
        //{
        //    var response = await ec2Helper.LaunchInstances(
        //        entities.PublicSubnetId, "ami-3f4a645a", "hms_qa_keypair",
        //        null, InstanceType.T2Micro, 1, 1);
        //    return response;
        //}
    }
}
