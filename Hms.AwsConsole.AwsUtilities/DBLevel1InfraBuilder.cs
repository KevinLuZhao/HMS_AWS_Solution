using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;

namespace Hms.AwsConsole.AwsUtilities
{
    public class DBLevel1InfraBuilder
    {
        IWindowForm monitorForm;
        Model.Environment environment;
        EC2Helper ec2Helper;

        const string STR_VPC = "Database_VPC";
        const string STR_PUBLIC_SUBNET = "Public_Subnet";
        const string STR_DB_SUBNET_A = "Database_Subnet_A";
        const string STR_DB_SUBNET_B = "Database_Subnet_B";
        const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        const string STR_NAT_GATEWAY = "NAT_Gateway";
        const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
        const string CIDR_VPC = "10.82.128.64/26";
        //const string CIDR_PUBLIC_SUBNET = "10.82.128.0/27";
        const string CIDR_DB_SUBNET_A = "10.82.128.64/28";
        const string CIDR_DB_SUBNET_B = "10.82.128.80/28";
        const string CIDR_ALL = "0.0.0.0/0";

        public DBLevel1InfraBuilder(Model.Environment env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env.ToString(), frm);
        }

        public async Task Creat()
        {
            var vpcResponse = await ec2Helper.CreateVpc(STR_VPC, CIDR_VPC);
            //Subnet group has to include at least two subnets, otherwise, CreateDBSubnetGroup 
            //command will halt there forever
            var subnetResponseA = await ec2Helper.
                CreateSubnet(vpcResponse.Vpc.VpcId, STR_DB_SUBNET_A, CIDR_DB_SUBNET_A, "us-east-2a");
            var subnetResponseB = await ec2Helper.
                CreateSubnet(vpcResponse.Vpc.VpcId, STR_DB_SUBNET_B, CIDR_DB_SUBNET_B, "us-east-2b");

            RDSHelper rdsHelper = new RDSHelper(environment, "us-east-2");
            var dbSubnetGroupResponse = await rdsHelper.CreateDBSubnetGroup(
                new List<string>() { subnetResponseA.Subnet.SubnetId, subnetResponseB.Subnet.SubnetId });
            await rdsHelper.CreatInstance(dbSubnetGroupResponse);  
        }
    }
}
