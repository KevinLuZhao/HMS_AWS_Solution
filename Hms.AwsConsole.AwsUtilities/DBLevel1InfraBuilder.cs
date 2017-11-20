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
        const string STR_DB_SUBNET = "Database_Subnet";
        const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        const string STR_NAT_GATEWAY = "NAT_Gateway";
        const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
        const string CIDR_VPC = "10.82.128.64/28";
        //const string CIDR_PUBLIC_SUBNET = "10.82.128.0/27";
        const string CIDR_DB_SUBNET = "10.82.128.64/28";
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
            var subnetResponse = await ec2Helper.CreateSubnet(vpcResponse.Vpc.VpcId, STR_DB_SUBNET, CIDR_DB_SUBNET);

            RDSHelper rdsHelper = new RDSHelper(environment, "us-east-2");
            var dbSubnetGroupResponse = await rdsHelper.CreateDBSubnetGroup(new List<string>() { subnetResponse.Subnet.SubnetId });
            await rdsHelper.CreatInstance(dbSubnetGroupResponse);  
        }
    }
}
