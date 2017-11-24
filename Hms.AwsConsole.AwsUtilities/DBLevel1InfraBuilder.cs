using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hms.AwsConsole.AwsUtilities
{
    public class DBLevel1InfraBuilder
    {
        IWindowForm monitorForm;
        Model.Environment environment;
        EC2Helper ec2Helper;
        DBInfraEntities entities = new DBInfraEntities();

        const string STR_VPC = "Database_VPC";
        const string STR_PUBLIC_SUBNET = "Public_Subnet";
        const string STR_DB_SUBNET_A = "Database_Subnet_A";
        const string STR_DB_SUBNET_B = "Database_Subnet_B";
        //const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        //const string STR_NAT_GATEWAY = "NAT_Gateway";
        //const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        //const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
        const string STR_RDS_SECURITY_GROUP = "Rds_Security_Group";
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

        public async Task<DBInfraEntities> Creat()
        {
            //Create VPC
            var vpcResponse = await ec2Helper.CreateVpc(STR_VPC, CIDR_VPC);
            entities.VpcId = vpcResponse.Vpc.VpcId;

            //Create Subnets
            //Subnet group has to include at least two subnets, otherwise, CreateDBSubnetGroup 
            //command will halt there forever
            var subnetResponseA = await ec2Helper.
                CreateSubnet(vpcResponse.Vpc.VpcId, STR_DB_SUBNET_A, CIDR_DB_SUBNET_A, "us-east-2a");
            entities.SubnetAId = subnetResponseA.Subnet.SubnetId;
            var subnetResponseB = await ec2Helper.
                CreateSubnet(vpcResponse.Vpc.VpcId, STR_DB_SUBNET_B, CIDR_DB_SUBNET_B, "us-east-2b");
            entities.SubnetBId = subnetResponseB.Subnet.SubnetId;

            //Create Security Groups
            var securityGroups = new List<string>();
            entities.DBSecurityGroupId = CreatRdsSeurityGroup();
            securityGroups.Add(entities.DBSecurityGroupId);

            RDSHelper rdsHelper = new RDSHelper(environment, "us-east-2");
            //Create DBSubnetGroup
            var dbSubnetGroupResponse = await rdsHelper.CreateDBSubnetGroup(
                new List<string>() { subnetResponseA.Subnet.SubnetId, subnetResponseB.Subnet.SubnetId });
            entities.DBSubnetGoupId = dbSubnetGroupResponse.DBSubnetGroupName;

            //Create RDS Instance
            var responseRdsInstance = await rdsHelper.CreatInstance(dbSubnetGroupResponse, securityGroups);
            entities.DBInstanceId = responseRdsInstance.DBInstanceIdentifier;

            return entities;
        }

        private string CreatRdsSeurityGroup()
        {
            var sgId = ec2Helper.CreateSecurityGroup(STR_RDS_SECURITY_GROUP, entities.VpcId, STR_RDS_SECURITY_GROUP);
            var lstRules = new List<SecurityRule>();
            SecurityRule rule = new SecurityRule()
            {
                Type = SecurityRuleType.MSSQL.Key,
                FromPort = 1433,
                ToPort = 1433,
                Protocol = "TCP",
                Source = "0.0.0.0/0",
                Description = "All internal instances for DB connection"
            };
            lstRules.Add(rule);
            ec2Helper.AssignRulesToSecurityGroup(sgId, lstRules);
            return sgId;
        }
    }
}
