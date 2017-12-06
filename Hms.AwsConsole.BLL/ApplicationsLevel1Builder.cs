using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hms.AwsConsole.BLL
{
    public class ApplicationsLevel1Builder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;
        ApplicationInfraEntities entities = new ApplicationInfraEntities();

        const string STR_VPC = "VPC";
        const string STR_PUBLIC_SUBNET = "Public_Subnet";
        const string STR_PRIVATE_SUBNET = "Private_Subnet";
        const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        const string STR_NAT_GATEWAY = "NAT_Gateway";
        const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
        const string STR_PUBLIC_SECURITYGROUP = "Public_SecurityGroup";
        const string STR_PRIVATE_SECURITYGROUP = "Private_SecurityGroup";
        const string STR_JUMPBOX_SECURITYGROUP = "Jumpbox_SecurityGroup";
        const string CIDR_VPC = "10.82.128.0/26";
        const string CIDR_PUBLIC_SUBNET = "10.82.128.0/27";
        const string CIDR_PRIVATE_SUBNET = "10.82.128.32/27";
        const string CIDR_ALL = "0.0.0.0/0";

        public ApplicationsLevel1Builder(string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env);
        }
        public async Task<ApplicationInfraEntities> Creat()
        {
            entities.Environment = environment;
            /*//A good way to gernerate the VPC, but the problem is no way to know when it finish.
            LaunchVPCWithPublicSubnetRequest request = new LaunchVPCWithPublicSubnetRequest()
            {
                VPCName = "VPC_Safemail",
                VPCCidrBlock = "10.82.128.0/26",
                PublicSubnetCiderBlock = "10.82.128.0/27",
                //PrivateSubnetCiderBlock= "10.82.128.32/27",
                ProgressCallback = CreateCallBack,

            };
            var Network = VPCUtilities.LaunchVPCWithPublicSubnet(client, request);*/
            try
            {
                var responseVpc = await ec2Helper.CreateVpc(STR_VPC, CIDR_VPC);
                //var vpc = responseVpc.Vpc;
                entities.VpcId = responseVpc;
                ShowCreatedMessage(entities.VpcId, STR_VPC);
                /******************************************** Security Group ********************************************/
                var responsePublicSG = await CreatePublicSecurityGroup();
                entities.PublicSecurityGroupId = responsePublicSG;
                ShowCreatedMessage(entities.PublicSecurityGroupId, STR_PUBLIC_SECURITYGROUP);
                //Later
                var responsePrivateSG = await CreatePrivateSecurityGroup();
                entities.PrivateSecurityGroupId = responsePrivateSG;

                var responsePublicSubnet = await ec2Helper.CreateSubnet(entities.VpcId, STR_PUBLIC_SUBNET, CIDR_PUBLIC_SUBNET);
                entities.PublicSubnetId = responsePublicSubnet;
                ShowCreatedMessage(entities.PublicSubnetId, STR_PUBLIC_SUBNET);

                var responsePrivateSubnet = await ec2Helper.CreateSubnet(entities.VpcId, STR_PRIVATE_SUBNET, CIDR_PRIVATE_SUBNET);
                entities.PrivateSubnetId = responsePrivateSubnet;
                ShowCreatedMessage(entities.PrivateSubnetId, STR_PRIVATE_SUBNET);
                /******************************************** Internet Gateway ********************************************/
                var responseIgw = await ec2Helper.CreateInternetGateway(entities.VpcId, STR_INTERNET_GATEWAY);
                entities.InternetGatewayId = responseIgw;
                ShowCreatedMessage(entities.InternetGatewayId, STR_INTERNET_GATEWAY);
                /******************************************** Nat Gateway ********************************************/
                var responseNgw = await ec2Helper.CreateNatGateway(entities.PrivateSubnetId, "eipalloc-bf81d491", STR_NAT_GATEWAY);
                entities.NatGatewayId = responseNgw;
                ShowCreatedMessage(entities.NatGatewayId, STR_NAT_GATEWAY);
                /******************************************** Public Route Table ********************************************/
                var responsePublicRoutetable = CreatePublicRouteTable(entities.VpcId, entities.PublicSubnetId, responseIgw);
                entities.PublicRouteTableId = responsePublicRoutetable;
                ShowCreatedMessage(entities.PublicRouteTableId, STR_PUBLIC_ROUTETABLE);
                /******************************************** Private Route Table ********************************************/
                var responsePrivateRouteTable = CreatePrivateRouteTable(entities.VpcId, entities.PrivateSubnetId, responseNgw);
                entities.PrivateRouteTableId = responsePrivateRouteTable;
                ShowCreatedMessage(entities.PrivateRouteTableId, STR_PRIVATE_ROUTETABLE);

                return entities;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                var serivce = new InfraEntitiesServices();
                serivce.SaveApplicationInfraEntities(entities);
            }
        }

        private void ShowCreatedMessage(string resourceId, string resourceTypeName)
        {
            monitorForm.ShowCallbackMessage($"Resource {resourceId} is created, resource type: {STR_PUBLIC_SUBNET}");
        }

        public async Task Destroy(ApplicationInfraEntities entities)
        {
            var serivce = new InfraEntitiesServices();
            try
            {
                /*Notice the order:
                Before delete a Internet Gateway must disaasociate the public IP, or Elastic IP from VPC, 
                otherwise may get error: Network vpc-80fa87ef has some mapped public address(es). Please unmap those public address(es) before detaching the gateway.
                Before disassociate the address, you must delete the Nat gateway which associate with the Elastic IP
                Otherwise may get error: You do not have permission to access the specified resource. 
                */
                string response;
                if (entities.PublicRouteTableId != null && entities.PublicSubnetId != null)
                {
                    ec2Helper.DisassociateRouteTableToSubnet(entities.PublicRouteTableId, entities.PublicSubnetId);
                    response = await ec2Helper.DeleteRouteTable(entities.PublicRouteTableId);
                    monitorForm.ShowCallbackMessage($"{STR_PUBLIC_ROUTETABLE}: {response}");
                    entities.PublicRouteTableId = null;
                }

                if (entities.PrivateRouteTableId != null && entities.PrivateSubnetId != null)
                {
                    ec2Helper.DisassociateRouteTableToSubnet(entities.PrivateRouteTableId, entities.PrivateSubnetId);
                    response = await ec2Helper.DeleteRouteTable(entities.PrivateRouteTableId);
                    monitorForm.ShowCallbackMessage($"{STR_PRIVATE_ROUTETABLE}: {response}");
                    entities.PrivateRouteTableId = null;
                }

                if (entities.NatGatewayId != null)
                {
                    monitorForm.ShowCallbackMessage($"{STR_NAT_GATEWAY}: Delete begin...");
                    response = await ec2Helper.DeleteNatGateway(entities.NatGatewayId, entities.VpcId);
                    monitorForm.ShowCallbackMessage($"{STR_NAT_GATEWAY}: {response}");
                    entities.NatGatewayId = null;
                }

                //Elastice IP for NAT gateway
                ec2Helper.DisassociateAddress("18.220.208.101");

                if (entities.InternetGatewayId != null)
                {
                    response = await ec2Helper.DeleteInternetGateway(entities.InternetGatewayId, entities.VpcId);
                    monitorForm.ShowCallbackMessage($"{STR_INTERNET_GATEWAY}: {response}");
                    entities.InternetGatewayId = null;
                }

                if (entities.PublicSubnetId != null)
                {
                    response = await ec2Helper.DeleteSubnet(entities.PublicSubnetId);
                    monitorForm.ShowCallbackMessage($"{STR_PUBLIC_SUBNET}: {response}");
                    entities.PublicSubnetId = null;
                }

                if (entities.PrivateSubnetId != null)
                {
                    response = await ec2Helper.DeleteSubnet(entities.PrivateSubnetId);
                    monitorForm.ShowCallbackMessage($"{STR_PRIVATE_SUBNET}: {response}");
                    entities.PrivateSubnetId = null;
                }

                //await ec2Helper.DeleteSecurityGoup()

                response = await ec2Helper.DeleteVpc(entities.VpcId);
                monitorForm.ShowCallbackMessage($"{STR_VPC}: {response}");
                serivce.DeleteApplicationInfraEntities(environment);
            }
            catch (Exception ex)
            {
                //Save break point.
                serivce.SaveApplicationInfraEntities(entities);
                throw ex;
            }
        }

        private string CreatePublicRouteTable(string vpcId, string subnetId, string igwId)
        {
            var response = ec2Helper.CreateRouteTable(vpcId, STR_PUBLIC_ROUTETABLE);
            ec2Helper.CreateRouteForRouteTable(igwId, "", CIDR_ALL, response);
            ec2Helper.AssociateRouteTableToSubnet(subnetId, response);
            //monitorForm.ShowCallbackMessage($"Public route table is created.");
            return response;
        }

        private string CreatePrivateRouteTable(string vpcId, string subnetId, string ngwId)
        {
            var response = ec2Helper.CreateRouteTable(vpcId, STR_PRIVATE_ROUTETABLE);
            ec2Helper.CreateRouteForRouteTable("", ngwId, CIDR_ALL, response);
            ec2Helper.AssociateRouteTableToSubnet(subnetId, response);
            //monitorForm.ShowCallbackMessage($"Private route table is created.");
            return response;
        }

        private async Task<string> CreateJumpboxSecurityGroup()
        {
            var sgId = await ec2Helper.CreateSecurityGroup(STR_JUMPBOX_SECURITYGROUP, entities.VpcId, STR_JUMPBOX_SECURITYGROUP);
            var lstRules = new List<SecurityRule>();
            SecurityRule rule = new SecurityRule()
            {
                Type = SecurityRuleType.RDP.Key,
                FromPort = 3389,
                ToPort = 3389,
                Protocol = "TCP",
                Source = CIDR_ALL,
                Description = "All internal instances for DB connection"
            };
            lstRules.Add(rule);
            await ec2Helper.AssignRulesToSecurityGroup(sgId, lstRules);
            return sgId;
        }

        private async Task<string> CreatePublicSecurityGroup()
        {
            var sgId = await ec2Helper.CreateSecurityGroup(STR_PUBLIC_SECURITYGROUP, entities.VpcId, STR_PUBLIC_SECURITYGROUP);
            var lstRules = new List<SecurityRule>();
            SecurityRule rule = new SecurityRule()
            {
                Type = SecurityRuleType.RDP.Key,
                FromPort = 3389,
                ToPort = 3389,
                Protocol = "TCP",
                Source = CIDR_VPC,
                Description = "Local RDP Connection"
            };
            lstRules.Add(rule);
            rule = new SecurityRule()
            {
                Type = SecurityRuleType.HTTP.Key,
                FromPort = 80,
                ToPort = 80,
                Protocol = "TCP",
                Source = CIDR_ALL,
                Description = ""
            };
            lstRules.Add(rule);
            rule = new SecurityRule()
            {
                Type = SecurityRuleType.HTTPS.Key,
                FromPort = 443,
                ToPort = 443,
                Protocol = "TCP",
                Source = CIDR_ALL,
                Description = ""
            };
            lstRules.Add(rule);
            await ec2Helper.AssignRulesToSecurityGroup(sgId, lstRules);
            return sgId;
        }

        private async Task<string> CreatePrivateSecurityGroup()
        {
            var sgId = await ec2Helper.CreateSecurityGroup(STR_PUBLIC_SECURITYGROUP, entities.VpcId, STR_PUBLIC_SECURITYGROUP);
            var lstRules = new List<SecurityRule>();
            SecurityRule rule = new SecurityRule()
            {
                Type = SecurityRuleType.RDP.Key,
                FromPort = 3389,
                ToPort = 3389,
                Protocol = "TCP",
                Source = CIDR_VPC,
                Description = "Local RDP Connection"
            };
            lstRules.Add(rule);
            rule = new SecurityRule()
            {
                Type = SecurityRuleType.HTTP.Key,
                FromPort = 80,
                ToPort = 80,
                Protocol = "TCP",
                Source = CIDR_ALL,
                Description = ""
            };
            lstRules.Add(rule);
            rule = new SecurityRule()
            {
                Type = SecurityRuleType.HTTPS.Key,
                FromPort = 443,
                ToPort = 443,
                Protocol = "TCP",
                Source = CIDR_ALL,
                Description = ""
            };
            lstRules.Add(rule);
            await ec2Helper.AssignRulesToSecurityGroup(sgId, lstRules);
            return sgId;
        }
    }
}
