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
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials(profile.ToString());
            this.Environment = profile;
            client = new AmazonRDSClient(credentials, AwsCommon.GetRetionEndpoint(region));
            //client = new AmazonRDSClient(
            //    CredentiaslManager.GetCredential(profile),
            //    AwsCommon.GetRetionEndpoint(region));
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

        public async Task CreatInstance()
        {
            CreateDBSubnetGroupRequest req0 = new CreateDBSubnetGroupRequest()
            {
                 DBSubnetGroupName = "HmsRdsSub",
                  //SubnetIds
            };
            CreateDBSubnetGroupRequest req1 = new CreateDBSubnetGroupRequest { };
            var request = new CreateDBInstanceRequest()
            {
                
            };
        }
    }
}
