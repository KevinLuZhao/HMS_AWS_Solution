using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;
using Amazon.EC2.Model;
using Hms.AwsConsole.Model;

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
        const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
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
        public async Task<InfraEntities> Creat()
        {
            InfraEntities entities = new InfraEntities();
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
                var vpc = responseVpc.Vpc;
                entities.VpcId = vpc.VpcId;

                var responsePublicSubnet = await ec2Helper.CreateSubnet(vpc.VpcId, STR_PUBLIC_SUBNET, CIDR_PUBLIC_SUBNET);
                var publicSubnet = responsePublicSubnet.Subnet;
                entities.PublicSubnetId = publicSubnet.SubnetId;

                var responsePrivateSubnet = await ec2Helper.CreateSubnet(vpc.VpcId, STR_PRIVATE_SUBNET, CIDR_PRIVATE_SUBNET);
                var privateSubnet = responsePrivateSubnet.Subnet;
                entities.PrivateSubnetId = privateSubnet.SubnetId;

                /******************************************** Internet Gateway ********************************************/
                var responseIgw = await ec2Helper.CreateInternetGateway(vpc.VpcId, STR_INTERNET_GATEWAY);
                var igw = responseIgw.InternetGateway;
                entities.InternetGatewayId = igw.InternetGatewayId;

                /******************************************** Nat Gateway ********************************************/
                var responseNgw = await ec2Helper.CreateNatGateway(privateSubnet.SubnetId, "eipalloc-bf81d491", STR_NAT_GATEWAY);
                var ngw = responseNgw.NatGateway;
                entities.NatGatewayId = ngw.NatGatewayId;

                /******************************************** Public Route Table ********************************************/
                var publicRouteTable = CreatePublicRouteTable(vpc.VpcId, publicSubnet.SubnetId, igw.InternetGatewayId);
                entities.PublicRouteTableId = publicRouteTable.RouteTableId;

                /******************************************** Private Route Table ********************************************/
                var privateRouteTable = CreatePrivateRouteTable(vpc.VpcId, privateSubnet.SubnetId, ngw.NatGatewayId);
                entities.PrivateRouteTableId = privateRouteTable.RouteTableId;

                return entities;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Teardown()
        {
            Vpc existingVpc = ec2Helper.FindVpc(STR_VPC);

            if (existingVpc != null)
            {
                var publicSubnet = ec2Helper.FindSubnet(STR_PUBLIC_SUBNET);
                var privateSubnet = ec2Helper.FindSubnet(STR_PRIVATE_SUBNET);
                var publicRouteTable = ec2Helper.FindRouteTable(STR_PUBLIC_ROUTETABLE);
                var internetGateway = ec2Helper.FindInternetGateway(STR_INTERNET_GATEWAY);
                var natGateway = ec2Helper.FindNatGateway(STR_NAT_GATEWAY);
                var privateRouteTable = ec2Helper.FindRouteTable(STR_PRIVATE_ROUTETABLE);

                /*Notice the order:
                 Before delete a Internet Gateway must disaasociate the public IP, or Elastic IP from VPC, 
                 otherwise may get error: Network vpc-80fa87ef has some mapped public address(es). Please unmap those public address(es) before detaching the gateway.
                 Before disassociate the address, you must delete the Nat gateway which associate with the Elastic IP
                 Otherwise may get error: You do not have permission to access the specified resource. 
                 */

                if (publicRouteTable != null && publicSubnet != null)
                {
                    ec2Helper.DisassociateRouteTableToSubnet(publicRouteTable, publicSubnet);
                    await ec2Helper.DeleteRouteTable(publicRouteTable);
                }

                if (privateRouteTable != null && privateSubnet != null)
                {
                    ec2Helper.DisassociateRouteTableToSubnet(privateRouteTable, privateSubnet);
                    await ec2Helper.DeleteRouteTable(privateRouteTable);
                }

                if (natGateway != null)
                {
                    await ec2Helper.DeleteNatGateway(natGateway, existingVpc.VpcId);
                }

                ec2Helper.DisassociateAddress("18.220.208.101");

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

        private RouteTable CreatePublicRouteTable(string vpcId, string subnetId, string igwId)
        {
            var response = ec2Helper.CreateRouteTable(vpcId, STR_PUBLIC_ROUTETABLE);
            ec2Helper.CreateRouteForRouteTable(igwId, response.RouteTableId);
            ec2Helper.AssociateRouteTableToSubnet(subnetId, response.RouteTableId);
            monitorForm.ShowCallbackMessage($"Public route table is created.");
            return response;
        }

        private RouteTable CreatePrivateRouteTable(string vpcId, string subnetId, string ngwId)
        {
            var response = ec2Helper.CreateRouteTable(vpcId, STR_PRIVATE_ROUTETABLE);
            response.Routes.Add(new Route { DestinationCidrBlock = "0.0.0.0/0", NatGatewayId = ngwId });
            ec2Helper.AssociateRouteTableToSubnet(subnetId, response.RouteTableId);
            monitorForm.ShowCallbackMessage($"Private route table is created.");
            return response;
        }
    }
}
