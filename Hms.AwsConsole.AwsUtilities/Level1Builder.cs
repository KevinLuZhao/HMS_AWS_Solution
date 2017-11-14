using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Hms.AwsConsole.AwsUtilities
{
    public class Level1Builder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;

        //Vpc vpc = null;
        //Subnet publicSubnet = null;
        //Subnet privateSubnet = null;
        //InternetGateway igw = null;
        //NatGateway ngw = null;

        const string STR_VPC = "VPC";
        const string STR_PUBLIC_SUBNET = "Public_Subnet";
        const string STR_PRIVATE_SUBNET = "Private_Subnet";
        const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        const string STR_NAT_GATEWAY = "NAT_Gateway";
        const string STR_PUBLIC_ROUTETABLE = "NAT_Gateway";
        //const string STR_PUBLIC_ROUTETABLE = "Public_Routetable";
        const string CIDR_VPC = "10.82.128.0/26";
        const string CIDR_PUBLIC_SUBNET = "10.82.128.0/27";
        const string CIDR_PRIVATE_SUBNET = "10.82.128.32/27";
        const string CIDR_ALL = "0.0.0.0/0";

        public Level1Builder(string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env, frm);
        }
        public async Task<IEnumerable<string>> CreatVPC()
        {
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
                var vpc = responseVpc.Vpc;

                var responsePublicSubnet = await ec2Helper.CreateSubnet(vpc.VpcId, CIDR_PUBLIC_SUBNET, STR_PUBLIC_SUBNET);
                var publicSubnet = responsePublicSubnet.Subnet;

                var responsePrivateSubnet = await ec2Helper.CreateSubnet(vpc.VpcId, CIDR_PRIVATE_SUBNET, STR_PRIVATE_SUBNET);
                var privateSubnet = responsePrivateSubnet.Subnet;
                /********************************************Internet Gateway********************************************/

                var responseIgw = await ec2Helper.CreateInternetGateway(vpc.VpcId, STR_INTERNET_GATEWAY);
                var igw = responseIgw.InternetGateway;

                /********************************************Nat Gateway********************************************/
                //var requestNgw = new CreateNatGatewayRequest()
                //{
                //    SubnetId = publicSubnetId,

                //};
                //var responsengw = await client.CreateNatGatewayAsync(requestNgw);
                //var ngwId = responseIgw.InternetGateway.InternetGatewayId;
                //AssignNameToResource(igwId, STR_NAT_GATEWAY);


                /********************************************Public Route Table********************************************/
                CreatePublicRouteTable(vpc.VpcId, publicSubnet.SubnetId, STR_PUBLIC_ROUTETABLE, igw.InternetGatewayId);

                await ec2Helper.LanchInstance("ami-4a8ba42f", publicSubnet.SubnetId, InstanceType.T2Micro, "hms_qa_keypair");
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TeardownExistingVPC()
        {
            Vpc existingVpc = ec2Helper.FindVpc(STR_VPC);

            if (existingVpc != null)
            {
                var publicSubnet = ec2Helper.FindSubnet(STR_PUBLIC_SUBNET);
                var privateSubnet = ec2Helper.FindSubnet(STR_PRIVATE_SUBNET);
                var publicRouteTable = ec2Helper.FindRouteTable(STR_PUBLIC_ROUTETABLE);
                var internetGateway = ec2Helper.FindInternetGateway(STR_INTERNET_GATEWAY);

                if (publicRouteTable != null && publicSubnet != null)
                {
                    ec2Helper.DisassociateRouteTableToSubnet(publicRouteTable, publicSubnet);
                    await ec2Helper.DeleteRouteTable(publicRouteTable);
                }

                if (internetGateway != null)
                {
                    await ec2Helper.DeleteInternetGateway(internetGateway, existingVpc.VpcId);
                }

                if (publicSubnet != null)
                {
                    await ec2Helper.DeleteSubnet(publicSubnet);
                }

                if (privateSubnet != null)
                {
                    await ec2Helper.DeleteSubnet(privateSubnet);
                }

                await ec2Helper.DeleteVpc(existingVpc);
            }
        }

        private RouteTable CreatePublicRouteTable(string vpcId, string subnetId, string name, string igwId)
        {
            var response = ec2Helper.CreateRouteTable(vpcId, name);
            ec2Helper.CreateRouteForRouteTable(igwId, response.RouteTableId);
            ec2Helper.AssociateRouteTableToSubnet(subnetId, response.RouteTableId);
            monitorForm.ShowCallbackMessage($"Public route table is created.");
            return response;
        }
    }
}
