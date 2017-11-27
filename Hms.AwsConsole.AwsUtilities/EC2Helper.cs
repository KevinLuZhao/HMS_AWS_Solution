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
        IWindowForm monitorForm;
        string environment;
        const string CIDR_ALL = "0.0.0.0/0";

        public EC2Helper(string env, IWindowForm frm)
        {
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
            client = new AmazonEC2Client(credentials, Amazon.RegionEndpoint.USEast2);
            this.environment = env;
            monitorForm = frm;
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
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == FormatresourceName(resourceTypeName)) >= 0)
                {
                    ret = item;
                    break;
                }
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
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == FormatresourceName(resourceTypeName)) >= 0)
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
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == FormatresourceName(resourceTypeName)) >= 0)
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
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == FormatresourceName(resourceTypeName)) >= 0)
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
                    System.Threading.Thread.Sleep(30000);
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
        internal string CreateRouteTable(string vpcId, string resourceTypeName)
        {
            CreateRouteTableRequest request = new CreateRouteTableRequest()
            {
                VpcId = vpcId
            };
            var response = client.CreateRouteTable(request);
            AssignNameToResource(response.RouteTable.RouteTableId, resourceTypeName);
            return response.RouteTable.RouteTableId;
        }

        internal bool CreateRouteForRouteTable(string gatewayId, string natGatewayId, string destinationCidrBlock, string routeTableId)
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

        internal RouteTable FindRouteTable(string resourceTypeName)
        {
            RouteTable ret = null;
            var response = client.DescribeRouteTables();
            foreach (var item in response.RouteTables)
            {
                if (item.Tags.FindIndex(o => o.Key == "Name" && o.Value == FormatresourceName(resourceTypeName)) >= 0)
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
                    return response.RouteTables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal async Task<string> DeleteRouteTable(string routeTableId)
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
        /************************************************* Instance ************************************************/
        public async Task<List<Instance>> LaunchInstances(
            string resourceTypeName, string subnetId, string amiId, string keyPairName, List<string> mySGIds,
            InstanceType instanceType, int max, int min, string userData = "", string privateIp = null)
        {
            //List<string> groups = new List<string>() { mySG.GroupId };
            var eni = new InstanceNetworkInterfaceSpecification()
            {
                DeviceIndex = 0,
                SubnetId = subnetId,
                //Groups = groups,
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
                InstanceType = instanceType,
                SecurityGroupIds = mySGIds,
                MinCount = min,
                MaxCount = max,
                KeyName = keyPairName,
                NetworkInterfaces = enis,
                UserData = userData
            };

            var response = await client.RunInstancesAsync(launchRequest);
            if (response.Reservation.Instances.Count == 1)
            {
                AssignNameToResource(response.Reservation.Instances[0].InstanceId, resourceTypeName);
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
                    System.Threading.Thread.Sleep(30000);
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

        internal async Task<string> DeleteSecurityGoup(string sgId)
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

        internal void AssociateRouteTableToSubnet(string subnetId, string routeTableId)
        {
            AssociateRouteTableRequest request = new AssociateRouteTableRequest()
            {
                RouteTableId = routeTableId,
                SubnetId = subnetId
            };
            client.AssociateRouteTable(request);
        }

        internal void DisassociateRouteTableToSubnet(string routeTableId, string subnetId)
        {
            var routeTable = FindRouteTableByID(routeTableId);
            var request = new DisassociateRouteTableRequest()
            {
                AssociationId = routeTable.Associations.Find(o => o.SubnetId == subnetId).RouteTableAssociationId
            };
            client.DisassociateRouteTable(request);
        }

        internal void DisassociateAddress(string ip)
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

        private void AssignNameToResource(string resourceId, string resourTypeName)
        {
            string resourceName = FormatresourceName(resourTypeName);
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

        public string CreatePeeringConnection(string peerVpcId, string vpcId)
        {
            var request = new CreateVpcPeeringConnectionRequest()
            {
                PeerVpcId = peerVpcId,
                VpcId = vpcId
            };
            var response = client.CreateVpcPeeringConnection(request);
            return response.VpcPeeringConnection.VpcPeeringConnectionId;
        }

        private string FormatresourceName(string name)
        {
            return $"HMS_{environment}_{name}";
        }
    }
}
