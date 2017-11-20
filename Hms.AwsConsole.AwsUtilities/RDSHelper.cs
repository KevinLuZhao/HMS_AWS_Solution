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

        public RDSHelper(Model.Environment profile, string region)
        {
            try
            {
                Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
                client = new AmazonRDSClient(credentials, Amazon.RegionEndpoint.USEast2);
                this.Environment = profile;
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
                
                DBInstanceIdentifier = FormatresourceName("DBInstance"),
                DBInstanceClass = "db.t2.micro",
                Domain = "safemail.local",
                Engine = "sqlserver-ee",
                MasterUsername = "sa",
                MasterUserPassword = "P@ssw0rd",
                DBSubnetGroupName = dbSubnetGroup.DBSubnetGroupName,
                MultiAZ = false 
            };
            var response = await client.CreateDBInstanceAsync(request);
            return response.DBInstance;
        }

        public async Task<DBSubnetGroup> CreateDBSubnetGroup(List<string> subnetIds)
        {
            var request = new CreateDBSubnetGroupRequest()
            {
                DBSubnetGroupName = FormatresourceName("DBSubnetGroup"),
                SubnetIds = subnetIds
            };
            var response = await client.CreateDBSubnetGroupAsync(request);
            return response.DBSubnetGroup;
        }

        private string FormatresourceName(string name)
        {
            return $"HMS_RDS_{Environment}_{name}";
        }
    }
}
