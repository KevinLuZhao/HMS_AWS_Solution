using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.RDS;
using Amazon.RDS.Model;
using Hms.AwsConsole.Model;

namespace Hms.AwsConsole.AwsUtilities
{
    public class RDSHelper
    {
        public Model.Environment Environment { get; }
        private AmazonRDSClient client;
        string region;

        public RDSHelper(Model.Environment profile, string region)
        {
            try
            {
                Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
                client = new AmazonRDSClient(credentials,  AwsCommon.GetRetionEndpoint(region));
                this.Environment = profile;
                this.region = region;
                //monitorForm = frm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AwsRdsInstance GetRDSInstance()
        {
            var lstInstance = client.DescribeDBInstances().DBInstances;
            foreach (var instance in lstInstance)
            {
                if (instance.DBSubnetGroup.DBSubnetGroupName.IndexOf(this.Environment.ToString()) >= 0)
                {
                    AwsRdsInstance objInstance = new AwsRdsInstance
                    {
                        DBInstanceIdentifier = instance.DBInstanceIdentifier,
                        DBInstanceArn = instance.DBInstanceArn,
                        RdsEnvinronment = this.Environment,
                        Status = instance.DBInstanceStatus,
                        MultiAZ = instance.MultiAZ
                    };
                    return objInstance;
                }
            }
            return null;
        }


        public async Task StopRdsInstance(string instanceIdentifier)
        {
            var instance = GetRDSInstance();
            var isMultiAZ = instance.MultiAZ;
            if (isMultiAZ)
            {
                ModifyDBInstanceRequest request = new ModifyDBInstanceRequest();
                request.MultiAZ = false;
                request.ApplyImmediately = true;
                request.DBInstanceIdentifier = instanceIdentifier;
                try
                {
                    var response = await client.ModifyDBInstanceAsync(request);
                    isMultiAZ = response.DBInstance.MultiAZ;
                    while (isMultiAZ)
                    {
                        System.Threading.Thread.Sleep(30000);
                        isMultiAZ = GetRDSInstance().MultiAZ;
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }


            var stopRequest = new StopDBInstanceRequest()
            {
                DBInstanceIdentifier = instanceIdentifier
            };
            await client.StopDBInstanceAsync(stopRequest);
        }

        public async Task StartRdsInstance(string instanceIdentifier)
        {
            var startRequest = new StartDBInstanceRequest()
            {
                DBInstanceIdentifier = instanceIdentifier
            };
            await client.StartDBInstanceAsync(startRequest);

            ModifyDBInstanceRequest request = new ModifyDBInstanceRequest();
            request.MultiAZ = true;
            request.DBInstanceIdentifier = instanceIdentifier;
            var response = await client.ModifyDBInstanceAsync(request);
        }

        public async Task<DBInstance> CreatInstance(DBSubnetGroup dbSubnetGroup)
        {
            var request = new CreateDBInstanceRequest()
            {

                DBInstanceIdentifier = $"HMS-RDS-{Environment}-DBInstance",
                DBInstanceClass = "db.t2.micro",
                Domain = "safemail.local",
                Engine = "sqlserver-ex",
                MasterUsername = "sa",
                MasterUserPassword = "Password123",
                DBSubnetGroupName = dbSubnetGroup.DBSubnetGroupName,
                MultiAZ = false,
                LicenseModel = "license-included"
            };
            //var response = await client.CreateDBInstanceAsync(request);
            //return response.DBInstance;
            try
            {
                var response = client.CreateDBInstance(request);
                return response.DBInstance;
            }
            catch (Exception ex)
            {
                /*
AWS encrypts the data on the servers that host EC2 instances and provide encryption of data-in-transit from EC2 instances and on to EBS storage

The LicenseModel parameter is required for configurations with multiple options.

RDS does not support creating a DB instance with the following combination: 
DBInstanceClass=db.t2.micro, Engine=sqlserver-ee, EngineVersion=13.00.4451.0.v1, LicenseModel=license-included. 
For supported combinations of instance class and database engine version, see the documentation.

"The parameter MasterUserPassword is not a valid password. Only printable ASCII characters besides '/', '@', '\"', ' ' may be used."

                The parameter AllocatedStorage must be provided and must not be null.
                 */
                throw ex;
            }
        }

        public async Task<DBSubnetGroup> CreateDBSubnetGroup(List<string> subnetIds)
        {
            var request = new CreateDBSubnetGroupRequest()
            {
                DBSubnetGroupName = FormatresourceName("DBSubnetGroup"),
                DBSubnetGroupDescription ="For safemail database",      //Not allow to be null!
                SubnetIds = subnetIds,
                Tags = new List<Tag> { new Tag() { Key = "Name", Value = FormatresourceName("DBSubnetGroup") } }
            };
            //var response = await client.CreateDBSubnetGroupAsync(request);
            try
            {
                var response = client.CreateDBSubnetGroup(request);
                return response.DBSubnetGroup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string FormatresourceName(string name)
        {
            return $"HMS_RDS_{Environment}_{name}";
        }
    }
}
