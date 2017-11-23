using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;
using Amazon.EC2.Model;
using Hms.AwsConsole.Model;
using Amazon.EC2;

namespace Hms.AwsConsole.AwsUtilities
{
    public class ApplicationsLevel2Builder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;
        ApplicationInfraEntities entities;

        public ApplicationsLevel2Builder(ApplicationInfraEntities entities, string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env, frm);
            this.entities = entities;
        }

        public async Task Creat()
        {
            await LaunchJumpbox();
            await LaunchWebServer();
        }

        private async Task<Instance> LaunchJumpbox()
        {
            var response = await ec2Helper.LaunchInstances(
                entities.PublicSubnetId, "ami-5d99b938", "hms_qa_keypair",
                new List<string> { entities.PublicSecurityGroupId}, 
                InstanceType.T2Micro, 1, 1, "JumpboxUserData.xml");
            return response;
        }

        private async Task<Instance> LaunchWebServer()
        {
            var response = await ec2Helper.LaunchInstances(
                entities.PublicSubnetId, "ami-3f4a645a", "hms_qa_keypair",
                new List<string> { entities.PublicSecurityGroupId },
                InstanceType.T2Micro, 1, 1);
            return response;
        }

        private async Task<Instance> LaunchServicesServer()
        {
            var response = await ec2Helper.LaunchInstances(
                entities.PrivateSubnetId, "ami-3f4a645a", "hms_qa_keypair",
                null, InstanceType.T2Micro, 1, 1);
            return response;
        }

        private async Task<Instance> LaunchEmailServiceServer()
        {
            var response = await ec2Helper.LaunchInstances(
                entities.PublicSubnetId, "ami-3f4a645a", "hms_qa_keypair",
                null, InstanceType.T2Micro, 1, 1);
            return response;
        }
    }
}
