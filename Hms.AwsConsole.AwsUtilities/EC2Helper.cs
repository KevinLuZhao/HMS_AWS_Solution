using System;
using Amazon.EC2;
using Amazon.EC2.Model;
//using Amazon.EC2.Util;
using Hms.AwsConsole.Interfaces;
using Hms.AwsConsole.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hms.AwsConsole.AwsUtilities
{
    public class EC2Helper
    {
        private AmazonEC2Client client;
        string environment;
        const string CIDR_ALL = "0.0.0.0/0";

        public EC2Helper(string env)
        {
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
            client = new AmazonEC2Client(credentials, Amazon.RegionEndpoint.USEast2);
            this.environment = env;
        }
        /************************************************* VPC ************************************************/
        public async Task<string> CreateVpc(string resourceTypeName, string cidr)
        {
            CreateVpcRequest requestVPC = new CreateVpcRequest(cidr);
            var responseVPC = await client.CreateVpcAsync(requestVPC);
            string vpcId = responseVPC.Vpc.VpcId;
            AssignNameToResource(vpcId, resourceTypeName);
            return responseVPC.Vpc.VpcId;
        }

        internal Vpc FindVpc(string resourceTypeName)
        {
            Vpc ret = null;
            var response = client.DescribeVpcs();
            foreach (var item in response.Vpcs)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && 
                o.Value == AwsCommon.FormatResourceName(resourceTypeName, environment)) >= 0)
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }

        public async Task<List<AwsVpc>> GetVPCList()
        {
            var request = new DescribeVpcsRequest();
            var response = await client.DescribeVpcsAsync(request);
            var ret = new List<AwsVpc>();
            foreach (var vpc in response.Vpcs)
            {
                if (vpc.IsDefault)
                {
                    continue;
                }
                ret.Add(new AwsVpc()
                {
                    CidrBlock = vpc.CidrBlock,
                    Name = vpc.Tags.Find(o => o.Key == "Name").Value,
                    State = vpc.State,
                    VpcId = vpc.VpcId
                });
            }
            return ret;
        }

        public async Task<string> DeleteVpc(string vpcId)
        {

            try
            {
                var request = new DeleteVpcRequest(vpcId);
                await client.DeleteVpcAsync(request);
                return $"Delete vpc {vpcId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete vpc {vpcId}. Error message: {ex.Message}";
            }
        }

        /************************************************* Subnet ************************************************/
        public async Task<string> CreateSubnet(
            string vpcId, string resourceTypeName, string cidr, string az = null)
        {
            CreateSubnetRequest request = new CreateSubnetRequest(vpcId, cidr);
            if (az != null)
                request.AvailabilityZone = az;
            var response = await client.CreateSubnetAsync(request);
            string publicSubnetId = response.Subnet.SubnetId;
            AssignNameToResource(publicSubnetId, resourceTypeName);
            return response.Subnet.SubnetId;
        }

        internal Subnet FindSubnet(string resourceTypeName)
        {
            Subnet ret = null;
            //DescribeSubnetsRequest request = new DescribeSubnetsRequest();
            var response = client.DescribeSubnets();
            foreach (var item in response.Subnets)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && 
                o.Value == AwsCommon.FormatResourceName(resourceTypeName, environment)) >= 0)
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }

        internal Subnet FindSubnetByID(string id)
        {
            var request = new DescribeSubnetsRequest()
            {
                SubnetIds = new List<string>() { id }
            };
            //DescribeSubnetsRequest request = new DescribeSubnetsRequest();
            var response = client.DescribeSubnets(request);
            return response.Subnets[0];
        }

        public async Task<string> DeleteSubnet(string subnetId)
        {
            try
            {
                var request = new DeleteSubnetRequest(subnetId);
                await client.DeleteSubnetAsync(request);
                return $"Delete subnet {subnetId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete subnet {subnetId}. Error message: {ex.Message}";
            }
        }
        /************************************************* Gateway ************************************************/
        public async Task<string> CreateInternetGateway(string vpcId, string resourceTypeName)
        {
            var response = await client.CreateInternetGatewayAsync();
            var igwId = response.InternetGateway.InternetGatewayId;
            AssignNameToResource(igwId, resourceTypeName);

            AttachInternetGatewayRequest requestIgw = new AttachInternetGatewayRequest()
            {
                InternetGatewayId = igwId,
                VpcId = vpcId
            };
            var responseAttachIgw = await client.AttachInternetGatewayAsync(requestIgw);
            //monitorForm.ShowCallbackMessage($"{igwId} is attached to {vpcId}.");
            return response.InternetGateway.InternetGatewayId;
        }

        internal InternetGateway FindInternetGateway(string resourceTypeName)
        {
            InternetGateway ret = null;
            //var request = new DescribeInternetGatewaysRequest();
            var response = client.DescribeInternetGateways();
            foreach (var item in response.InternetGateways)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && 
                o.Value == AwsCommon.FormatResourceName(resourceTypeName, environment)) >= 0)
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }

        public async Task<string> DeleteInternetGateway(string igwId, string vpcId)
        {
            try
            {
                var requestDetach = new DetachInternetGatewayRequest()
                {
                    InternetGatewayId = igwId,
                    VpcId = vpcId
                };
                client.DetachInternetGateway(requestDetach);

                var request = new DeleteInternetGatewayRequest() { InternetGatewayId = igwId };
                await client.DeleteInternetGatewayAsync(request);
                return $"Delete internet gateway {igwId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete internet gateway {igwId}. Error message: {ex.Message}";
            }
        }

        public async Task<string> CreateNatGateway(string subnetId, string allocationId, string resourceTypeName)
        {
            var request = new CreateNatGatewayRequest()
            {
                SubnetId = subnetId,
                AllocationId = allocationId
            };
            var response = await client.CreateNatGatewayAsync(request);
            //bool createFinished = false;
            while (true)
            {
                await Task.Delay(10000);
                var natGateway = FindNatGatewayById(response.NatGateway.NatGatewayId);
                if (natGateway.State == NatGatewayState.Available)
                {
                    break;
                }
            }
            AssignNameToResource(response.NatGateway.NatGatewayId, resourceTypeName);

            return response.NatGateway.NatGatewayId;
        }

        internal NatGateway FindNatGateway(string resourceTypeName)
        {
            NatGateway ret = null;
            var request = new DescribeNatGatewaysRequest();
            var response = client.DescribeNatGateways(request);
            foreach (var item in response.NatGateways)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && 
                o.Value == AwsCommon.FormatResourceName(resourceTypeName, environment)) >= 0)
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }

        internal NatGateway FindNatGatewayById(string ngwId)
        {
            try
            {
                NatGateway ret = null;
                var request = new DescribeNatGatewaysRequest()
                {
                    NatGatewayIds = new List<string>() { ngwId }
                };
                ret = client.DescribeNatGateways(request).NatGateways[0];
                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> DeleteNatGateway(string ngwId, string vpcId)
        {
            try
            {
                NatGateway ngw = FindNatGatewayById(ngwId);
                var request = new DeleteNatGatewayRequest() { NatGatewayId = ngwId };
                await client.DeleteNatGatewayAsync(request);
                while (ngw.State != NatGatewayState.Deleted)
                {
                    await Task.Delay(10000);
                    ngw = FindNatGateway(ngwId);
                    if (ngw != null)
                    {
                        break;
                    }
                }
                return $"Delete NAT gateway {ngwId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete NAT gateway {ngwId}. Error message: {ex.Message}";
            }
            //monitorForm.ShowCallbackMessage(
            //    $"NAT Gateway {ngw.NatGatewayId}|{(ngw.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }
        /************************************************* Route Table ************************************************/
        public string CreateRouteTable(string vpcId, string resourceTypeName)
        {
            CreateRouteTableRequest request = new CreateRouteTableRequest()
            {
                VpcId = vpcId
            };
            var response = client.CreateRouteTable(request);
            AssignNameToResource(response.RouteTable.RouteTableId, resourceTypeName);
            return response.RouteTable.RouteTableId;
        }

        public bool CreateRouteForRouteTable(string gatewayId, string natGatewayId, string destinationCidrBlock, string routeTableId)
        {
            CreateRouteRequest request = new CreateRouteRequest();
            if (!string.IsNullOrEmpty(gatewayId))
                request.GatewayId = gatewayId;
            if (!string.IsNullOrEmpty(natGatewayId))
                request.NatGatewayId = natGatewayId;
            request.DestinationCidrBlock = destinationCidrBlock;
            request.RouteTableId = routeTableId;

            return client.CreateRoute(request).Return;
        }

        public async Task<List<string>> GetRouteTablesByVpc(string vpcId)
        {
            var ret = new List<string>();
            var request = new DescribeRouteTablesRequest();
            var response = await client.DescribeRouteTablesAsync(request);
            foreach (var routeTable in response.RouteTables)
            {
                if (routeTable.VpcId == vpcId && routeTable.Tags.Find(o => o.Key == "Name") != null)
                    ret.Add(routeTable.RouteTableId);
            }
            return ret;
        }

        internal RouteTable FindRouteTable(string resourceTypeName)
        {
            RouteTable ret = null;
            var response = client.DescribeRouteTables();
            foreach (var item in response.RouteTables)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == AwsCommon.FormatResourceName(resourceTypeName, environment)) >= 0)
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }

        internal RouteTable FindRouteTableByID(string routeTableId)
        {
            var request = new DescribeRouteTablesRequest() { RouteTableIds = new List<string>() { routeTableId } };
            try
            {
                var response = client.DescribeRouteTables(request);
                if (response.RouteTables != null && response.RouteTables.Count > 0)
                {
                    return response.RouteTables[0];}
                else
                {
                    return null;
                }
            }
            catch (Amazon.Runtime.AmazonServiceException ex)
            {
                if (ex.ErrorCode == "InvalidRouteTableID.NotFound")
                    return null;
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> DeleteRouteTable(string routeTableId)
        {
            try
            {
                var request = new DeleteRouteTableRequest() { RouteTableId = routeTableId };
                await client.DeleteRouteTableAsync(request);
                return $"Delete routetable {routeTableId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete routetable {routeTableId}. Error message: {ex.Message}";
            }
            //monitorForm.ShowCallbackMessage(
            //    $"Route Table {routeTable.RouteTableId}|{(routeTable.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }

        public bool CreateRouteForRouteTable(
           string peeringConnection,
           string gatewayId,
           string natGatewayId,
           string destinationCidrBlock,
           string routeTableId)
        {
            var request = new CreateRouteRequest();
            if (!string.IsNullOrEmpty(gatewayId))
                request.GatewayId = gatewayId;
            if (!string.IsNullOrEmpty(natGatewayId))
                request.NatGatewayId = natGatewayId;
            if (!string.IsNullOrEmpty(peeringConnection))
                request.VpcPeeringConnectionId = peeringConnection;
            request.DestinationCidrBlock = destinationCidrBlock;
            request.RouteTableId = routeTableId;

            return client.CreateRoute(request).Return;
        }

        public async Task DeleteRouteForRouteTable(string destinationCidrBlock, string routeTableId)
        {
            try
            {
                var request = new DeleteRouteRequest()
                {
                    DestinationCidrBlock = destinationCidrBlock,
                    RouteTableId = routeTableId
                };

                await client.DeleteRouteAsync(request);
            }
            catch (Exception ex)
            {
                //Check if destinationCidrBlock doesn't exist, should skip
                throw ex;
            }
        }
        /************************************************* Instance ************************************************/
        public async Task<AwsAppInstance> LaunchSingleInstance(
            string resourceTypeName, string subnetId, string amiId, string keyPairName, List<string> mySGIds,
            string instanceType, string userData = "", string privateIp = null)
        {
            AwsAppInstance awsInstance = null;
            //List<string> groups = new List<string>() { mySG.GroupId };
            var eni = new InstanceNetworkInterfaceSpecification()
            {
                DeviceIndex = 0,
                SubnetId = subnetId,
                //Groups = groups,
                Groups = mySGIds,
                AssociatePublicIpAddress = true
            };
            if (privateIp != null)
            {
                eni.PrivateIpAddress = privateIp;
            }
            List<InstanceNetworkInterfaceSpecification> enis = new List<InstanceNetworkInterfaceSpecification>() { eni };

            var launchRequest = new RunInstancesRequest()
            {
                ImageId = amiId,
                InstanceType = InstanceType.FindValue(instanceType),
                //Network interfaces and an instance-level security groups may not be specified on the same request
                //SecurityGroupIds = mySGIds,
                MinCount = 1,
                MaxCount = 1,
                KeyName = keyPairName,
                NetworkInterfaces = enis,
                UserData = userData
            };

            var response = await client.RunInstancesAsync(launchRequest);
            if (response.Reservation.Instances.Count > 0)
            {
                var instance = response.Reservation.Instances[0];
                AssignNameToResource(instance.InstanceId, resourceTypeName);
                DynamoDBHelper<AwsAppInstance> dynamoDbHelper = new DynamoDBHelper<AwsAppInstance>();
                awsInstance = new AwsAppInstance()
                {
                    Environment = environment,
                    InstanceId = instance.InstanceId,
                    Name = resourceTypeName,
                    PublicIP = instance.PublicIpAddress,
                    PrivateIP = instance.PrivateIpAddress
                };
                dynamoDbHelper.CreateItem("hms_instances", awsInstance);
            }
            return awsInstance;
        }

        //Consider to return InstanceId instead of Instance
        public async Task<List<Instance>> LaunchInstances(
            string resourceTypeName, string subnetId, string amiId, string keyPairName, List<string> mySGIds,
            string instanceType, int max, int min, string userData = "", string privateIp = null)
        {
            //List<string> groups = new List<string>() { mySG.GroupId };
            var eni = new InstanceNetworkInterfaceSpecification()
            {
                DeviceIndex = 0,
                SubnetId = subnetId,
                //Groups = groups,
                Groups = mySGIds,
                AssociatePublicIpAddress = true
            };
            if (privateIp != null)
            {
                eni.PrivateIpAddress = privateIp;
            }
            List<InstanceNetworkInterfaceSpecification> enis = new List<InstanceNetworkInterfaceSpecification>() { eni };
            
            var launchRequest = new RunInstancesRequest()
            {
                ImageId = amiId,
                InstanceType = InstanceType.FindValue(instanceType),
                //Network interfaces and an instance-level security groups may not be specified on the same request
                //SecurityGroupIds = mySGIds,
                MinCount = min,
                MaxCount = max,
                KeyName = keyPairName,
                NetworkInterfaces = enis,
                UserData = userData
            };

            var response = await client.RunInstancesAsync(launchRequest);
            if (response.Reservation.Instances.Count == 1)
            {
                var instance = response.Reservation.Instances[0];
                AssignNameToResource(response.Reservation.Instances[0].InstanceId, resourceTypeName);
                DynamoDBHelper<AwsAppInstance> dynamoDbHelper = new DynamoDBHelper<AwsAppInstance>();
                var awsInstance = new AwsAppInstance()
                {
                    Environment = environment,
                    InstanceId = instance.InstanceId,
                    Name = resourceTypeName,
                    PublicIP = instance.PublicIpAddress,
                    PrivateIP = instance.PrivateIpAddress
                };
                dynamoDbHelper.CreateItem("hms_instances", awsInstance);
            }
            else
            {
                int counter = 0;
                foreach (var instance in response.Reservation.Instances)
                {
                    AssignNameToResource(instance.InstanceId, resourceTypeName + counter.ToString());

                    DynamoDBHelper<AwsAppInstance> dynamoDbHelper = new DynamoDBHelper<AwsAppInstance>();
                    var awsInstance = new AwsAppInstance()
                    {
                        Environment = environment,
                        InstanceId = instance.InstanceId,
                        Name = resourceTypeName + counter.ToString(),
                        PublicIP = instance.PublicIpAddress,
                        PrivateIP = instance.PrivateIpAddress
                    };
                    dynamoDbHelper.CreateItem("hms_instances", awsInstance);
                    counter++;
                }
            }

            return response.Reservation.Instances;
        }

        public async Task<List<AwsAppInstance>> GetInstancesListByNames(List<string> instanceNames)
        {
            var ret = new List<AwsAppInstance>();
            var request = new DescribeInstancesRequest();
            var response = await client.DescribeInstancesAsync(request);
            foreach (var reservation in response.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    if (instance.Tags.Find(o => o.Key == "Name") == null)
                        continue;
                    var instanceName = instance.Tags.Find(o => o.Key == "Name").Value;
                    if (!instanceNames.Contains(instanceName))
                        continue;
                    ret.Add(TranslateInstance(instance, instance.Tags.Find(o=>o.Key=="Name").Value));
                }
            }
            return ret;
        }

        public async Task<List<AwsAppInstance>> GetInstancesListByIds(List<string> instanceIds)
        {
            var ret = new List<AwsAppInstance>();
            var request = new DescribeInstancesRequest()
            {
                InstanceIds = instanceIds
            };
            var response = await client.DescribeInstancesAsync(request);
            foreach (var reservation in response.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    ret.Add(TranslateInstance(instance, instance.Tags.Find(o => o.Key == "Name").Value));
                }
            }
            return ret;
        }

        public async Task<string> DeleteInstances(List<string> lstInstanceIds)
        {

            try
            {
                var request = new TerminateInstancesRequest(lstInstanceIds);
                var response = await client.TerminateInstancesAsync(request);
                bool allTerminated = false;
                //Need to find all instances and check status.
                while (!allTerminated)
                {
                    await Task.Delay(30000);
                    allTerminated = true;
                    foreach (var instance in response.TerminatingInstances)
                    {
                        if (instance.CurrentState.Name != "terminated")
                        {
                            allTerminated = false;
                            break;
                        }
                    }
                }
                return $"Delete EC2 instances {string.Join(", ", lstInstanceIds.ToArray())} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete EC2 instances {string.Join(", ", lstInstanceIds.ToArray())} . Error message: {ex.Message}";
            }
        }

        private AwsAppInstance TranslateInstance(Instance instance, string name)
        {
            return new AwsAppInstance()
            {
                Environment = environment,
                InstanceId = instance.InstanceId,
                Name = name,
                PublicIP = instance.PublicIpAddress,
                PrivateIP = instance.PrivateIpAddress,
                state = instance.State.Name
            };
        }

        public List<ImageModel> GetImageList()
        {
            var ret = new List<ImageModel>();
            DescribeImagesRequest request = new DescribeImagesRequest() { Owners = new List<string> { "157799504602" } };
            var response = client.DescribeImages(request);
            foreach (var image in response.Images)
            {
                ret.Add(new ImageModel { AmiId = image.ImageId, Name = image.Name });
            }
            return ret;
        }

        public ImageModel FindAMIByName(string name)
        {
            DescribeImagesRequest request = new DescribeImagesRequest() { Owners = new List<string> { "157799504602" } };
            var response = client.DescribeImages(request);
            //var image = response.Images.Find(o=>o.Tags.FindIndex(p=>p.Key=="Name" && p.Value==name)>=0);
            var image = response.Images.Find(o => o.Name == name);
            if (image != null)
            {
                var ret = new ImageModel()
                {
                    AmiId = image.ImageId,
                    Name = image.Name
                };
                return ret;
            }
            else
                return null;
        }
        /************************************************* Security Group ************************************************/
        public async Task<string> CreateSecurityGroup(string groupName, string vpcId, string resourceTypeName)
        {
            CreateSecurityGroupRequest request = new CreateSecurityGroupRequest()
            {
                GroupName = groupName,
                VpcId = vpcId,
                Description = "HMS RDS Security Group"
            };
            var response = await client.CreateSecurityGroupAsync(request);
            AssignNameToResource(response.GroupId, resourceTypeName);
            return response.GroupId;
        }

        public async Task<string> DeleteSecurityGoup(string sgId)
        {
            try
            {
                var request = new DeleteSecurityGroupRequest(sgId);
                await client.DeleteSecurityGroupAsync(request);
                return $"Delete routetable {sgId} successfully!";
            }
            catch (Exception ex)
            {
                return $"Faied to delete routetable {sgId}. Error message: {ex.Message}";
            }
        }

        /*************************************************Associate************************************************/

        public void AssociateRouteTableToSubnet(string subnetId, string routeTableId)
        {
            AssociateRouteTableRequest request = new AssociateRouteTableRequest()
            {
                RouteTableId = routeTableId,
                SubnetId = subnetId
            };
            client.AssociateRouteTable(request);
        }

        public void DisassociateRouteTableToSubnet(string routeTableId, string subnetId)
        {
            var routeTable = FindRouteTableByID(routeTableId);
            if (routeTable != null)
            {
                var request = new DisassociateRouteTableRequest()
                {
                    AssociationId = routeTable.Associations.Find(o => o.SubnetId == subnetId).RouteTableAssociationId
                };
                client.DisassociateRouteTable(request);
            }
        }

        public void DisassociateAddress(string ip)
        {
            string associationId = null;
            DescribeAddressesRequest request1 = new DescribeAddressesRequest();
            var response = client.DescribeAddresses(request1);
            foreach (var address in response.Addresses)
            {
                if (address.PublicIp == ip)
                {
                    associationId = address.AssociationId;
                    break;
                }
            }
            if (associationId != null)
            {
                var request2 = new DisassociateAddressRequest() { AssociationId = associationId };
                client.DisassociateAddress(request2);
            }
        }

        //internal List<Hms.AwsConsole.Model.Image> GetAMIs()
        //{
        //    var response = client.DescribeImages();
        //    var rets = new List<Model.Image>();
        //    foreach (var image in response.Images)
        //    {
        //        rets.Add(
        //            new Model.Image()
        //            {
        //                ImageId = image.ImageId,
        //                Name = image.Name
        //            }
        //            );
        //    }
        //    return rets;
        //}

        private void AssignNameToResource(string resourceId, string resourceTypeName)
        {
            string resourceName = AwsCommon.FormatResourceName(resourceTypeName, environment);
            CreateTagsRequest reqCreateTag = new CreateTagsRequest();
            reqCreateTag.Resources = new List<string>();
            reqCreateTag.Resources.Add(resourceId);
            reqCreateTag.Tags = new List<Tag>();
            reqCreateTag.Tags.Add(new Tag("Name", resourceName));

            client.CreateTags(reqCreateTag);
            //monitorForm.ShowCallbackMessage($"Resource {resourceId} is created, name: {resourceName}");
        }

        public async Task AssignRulesToSecurityGroup(string securityGroupId, List<SecurityRule> rules)
        {
            IpRange ips = new IpRange()
            {

            };
            var lstIPPermission = new List<IpPermission>();
            foreach (var rule in rules)
            {
                var ipPermission = new IpPermission()
                {
                    FromPort = rule.FromPort,
                    ToPort = rule.ToPort,
                    IpProtocol = rule.Protocol,
                    Ipv4Ranges = new List<IpRange>() { new IpRange() { CidrIp = rule.Source, Description = rule.Description } }
                };
                lstIPPermission.Add(ipPermission);
            }
            var ingressRequest = new AuthorizeSecurityGroupIngressRequest();
            ingressRequest.GroupId = securityGroupId;
            ingressRequest.IpPermissions.AddRange(lstIPPermission);

            try
            {
                var ingressResponse = await client.AuthorizeSecurityGroupIngressAsync(ingressRequest);
            }
            catch (AmazonEC2Exception ex)
            {
                // Check the ErrorCode to see if the rule already exists
                if ("InvalidPermission.Duplicate" == ex.ErrorCode)
                {
                    //Console.WriteLine("An RDP rule for: {0} already exists.", ipRange);
                }
                else
                {
                    // The exception was thrown for another reason, so re-throw the exception
                    throw;
                }
            }
        }
        /*************************************************Peering Connection************************************************/
        public async Task<string> CreatePeeringConnection(string accepterVpc, string requesterVpc, string name)
        {
            var request = new CreateVpcPeeringConnectionRequest()
            {
                PeerOwnerId = "157799504602 ",
                PeerVpcId = accepterVpc,
                VpcId = requesterVpc
            };
            var response = await client.CreateVpcPeeringConnectionAsync(request);
            AssignNameToResource(response.VpcPeeringConnection.VpcPeeringConnectionId, name);
            return response.VpcPeeringConnection.VpcPeeringConnectionId;
        }

        public async Task<string> AcceptPeeringConnection(string vpcPeeringConnectionId)
        {
            var request = new AcceptVpcPeeringConnectionRequest()
            {
                VpcPeeringConnectionId = vpcPeeringConnectionId
            };
            var response = await client.AcceptVpcPeeringConnectionAsync(request);
            return response.VpcPeeringConnection.VpcPeeringConnectionId;
        }

        public async Task<AwsPeeringConnection> GetPeeringConnection(string name)
        {
            var request = new DescribeVpcPeeringConnectionsRequest();
            var response = await client.DescribeVpcPeeringConnectionsAsync(request);
            var connection = response.VpcPeeringConnections.
                FindAll(o => (o.Tags.FindIndex(p => p.Key == "Name" && p.Value == name) >= 0)).
                Find(o => o.Status.Code == VpcPeeringConnectionStateReasonCode.Active);
            if (connection != null)
            {
                var connectionModel = new AwsPeeringConnection()
                {
                    AccepterVpc = connection.AccepterVpcInfo.VpcId,
                    RequesterVpc = connection.RequesterVpcInfo.VpcId,
                    VpcPeeringConnectionId = connection.VpcPeeringConnectionId,
                    Status = connection.Status.Code.Value
                };
                return connectionModel;
            }
            else
                return null;
        }

        public async Task<string> DeletePeeringConnection(string peeringConnectionId)
        {
            try
            {
                var request = new DeleteVpcPeeringConnectionRequest()
                {
                    VpcPeeringConnectionId = peeringConnectionId
                };
                var response = await client.DeleteVpcPeeringConnectionAsync(request);
                return $"VPC Peering Connection {peeringConnectionId} is deleted successfully";
            }
            catch (Exception ex)
            {
                return $"Error -- {ex.Message}";
            }
        }
    }
}
