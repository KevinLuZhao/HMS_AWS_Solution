using System;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hms.AwsConsole.BLL
{
    public class DBLevel1InfraBuilder
    {
        IWindowForm monitorForm;
        Model.Environment environment;
        string region = "us-east-2";
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
            entities.Environment = env.ToString();
            ec2Helper = new EC2Helper(env.ToString());
        }

        public async Task<DBInfraEntities> Creat()
        {
            try
            {
                //Create VPC
                var vpcResponse = await ec2Helper.CreateVpc(STR_VPC, CIDR_VPC);
                entities.VpcId = vpcResponse;

                //Create Subnets
                //Subnet group has to include at least two subnets, otherwise, CreateDBSubnetGroup 
                //command will halt there forever
                var subnetResponseA = await ec2Helper.
                    CreateSubnet(entities.VpcId, STR_DB_SUBNET_A, CIDR_DB_SUBNET_A, "us-east-2a");
                entities.SubnetAId = subnetResponseA;
                var subnetResponseB = await ec2Helper.
                    CreateSubnet(entities.VpcId, STR_DB_SUBNET_B, CIDR_DB_SUBNET_B, "us-east-2b");
                entities.SubnetBId = subnetResponseB;

                //Create Security Groups
                var securityGroups = new List<string>();
                entities.DBSecurityGroupId = await CreatRdsSeurityGroup();
                securityGroups.Add(entities.DBSecurityGroupId);

                RDSHelper rdsHelper = new RDSHelper(environment, region);
                //Create DBSubnetGroup
                var dbSubnetGroupResponse = await rdsHelper.CreateDBSubnetGroup(
                    new List<string>() { entities.SubnetAId, entities.SubnetBId });
                entities.DBSubnetGoupId = dbSubnetGroupResponse;

                //Create RDS Instance
                var responseRdsInstance = await rdsHelper.CreatInstance(dbSubnetGroupResponse, securityGroups);
                entities.DBInstanceId = responseRdsInstance;

                return entities;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                var service = new InfraEntitiesServices();
                service.SaveDbInfraEntities(entities);
            }
        }

        public async Task<string> Delete(DBInfraEntities entities)
        {
            var service = new InfraEntitiesServices();
            RDSHelper rdsHelper = new RDSHelper(environment, "us-east-2");
            try
            {
                try
                {
                    await rdsHelper.DeleteRDSInstance(entities.DBInstanceId);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "DBInstanceNotFound")
                    {; }
                    else
                    {
                        throw ex;
                    }
                }
                entities.DBInstanceId = null;
                await ec2Helper.DeleteSecurityGoup(entities.DBSecurityGroupId);
                entities.DBSecurityGroupId = null;
                await ec2Helper.DeleteSubnet(entities.SubnetAId);
                entities.DBSubnetGoupId = null;
                service.DeleteDbInfraEntities(environment.ToString());
                return "Success";
            }
            catch (Exception ex)
            {
                service.SaveDbInfraEntities(entities);
                return ex.Message;
            }
        }

        private async Task<string> CreatRdsSeurityGroup()
        {
            var sgId = await ec2Helper.CreateSecurityGroup(STR_RDS_SECURITY_GROUP, entities.VpcId, STR_RDS_SECURITY_GROUP);
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
            await ec2Helper.AssignRulesToSecurityGroup(sgId, lstRules);
            return sgId;
        }
    }
}
