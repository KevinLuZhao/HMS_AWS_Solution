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

        /*************************************************Create************************************************/
        public EC2Helper(string env, IWindowForm frm)
        {
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
            client = new AmazonEC2Client(credentials, Amazon.RegionEndpoint.USEast2);
            this.environment = env;
            monitorForm = frm;
        }

        internal async Task<CreateVpcResponse> CreateVpc(string resourceTypeName, string cidr)
        {
            CreateVpcRequest requestVPC = new CreateVpcRequest(cidr);
            var responseVPC = await client.CreateVpcAsync(requestVPC);
            string vpcId = responseVPC.Vpc.VpcId;
            AssignNameToResource(vpcId, resourceTypeName);
            return responseVPC;
        }

        internal async Task<CreateSubnetResponse> CreateSubnet(
            string vpcId, string resourceTypeName, string cidr, string az = null)
        {
            CreateSubnetRequest request = new CreateSubnetRequest(vpcId, cidr);
            if (az != null)
                request.AvailabilityZone = az;
            var response = await client.CreateSubnetAsync(request);
            string publicSubnetId = response.Subnet.SubnetId;
            AssignNameToResource(publicSubnetId, resourceTypeName);
            return response;
        }

        internal async Task<CreateInternetGatewayResponse> CreateInternetGateway(string vpcId, string resourceTypeName)
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
            monitorForm.ShowCallbackMessage($"{igwId} is attached to {vpcId}.");
            return response;
        }

        internal async Task<CreateNatGatewayResponse> CreateNatGateway(string subnetId, string allocationId, string resourceTypeName)
        {
            var request = new CreateNatGatewayRequest()
            {
                SubnetId = subnetId,
                AllocationId = allocationId
            };
            var response = await client.CreateNatGatewayAsync(request);
            AssignNameToResource(response.NatGateway.NatGatewayId, resourceTypeName);

            return response;
        }

        internal RouteTable CreateRouteTable(string vpcId, string resourceTypeName)
        {
            CreateRouteTableRequest request = new CreateRouteTableRequest()
            {
                VpcId = vpcId
            };
            var response = client.CreateRouteTable(request);
            AssignNameToResource(response.RouteTable.RouteTableId, resourceTypeName);
            return response.RouteTable;
        }

        internal void CreateRouteForRouteTable(string gatewayId, string routeTableId)
        {
            CreateRouteRequest request = new CreateRouteRequest()
            {
                GatewayId = gatewayId,
                DestinationCidrBlock = CIDR_ALL,
                RouteTableId = routeTableId
            };
            var response = client.CreateRoute(request);
        }

        public async Task<Instance> LaunchInstances(
            string subnetId, string amiId, string keyPairName, SecurityGroup mySG,
            InstanceType instanceType, int max, int min, string privateIp = null)
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
                MinCount = min,
                MaxCount = max,
                KeyName = keyPairName,
                NetworkInterfaces = enis
            };

            var response = await client.RunInstancesAsync(launchRequest);
            return response.Reservation.Instances[0];
        }

        public void CreateSecurityGroup(string groupName, string vpcId)
        {
            CreateSecurityGroupRequest request = new CreateSecurityGroupRequest()
            {
                GroupName = groupName,
                VpcId = vpcId
            };
            var response = client.CreateSecurityGroup(request);
        }
        /*************************************************Find************************************************/
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
        /*************************************************Delete************************************************/
        internal async Task DeleteVpc(Vpc vpc)
        {
            var request = new DeleteVpcRequest(vpc.VpcId);
            await client.DeleteVpcAsync(request);
            monitorForm.ShowCallbackMessage(
                $"Subnet {vpc.VpcId}|{(vpc.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }

        internal async Task DeleteSubnet(Subnet subnet)
        {
            var request = new DeleteSubnetRequest(subnet.SubnetId);
            await client.DeleteSubnetAsync(request);
            monitorForm.ShowCallbackMessage(
                $"Subnet {subnet.SubnetId}|{(subnet.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }

        internal async Task DeleteInternetGateway(InternetGateway igw, string vpcId)
        {
            var requestDetach = new DetachInternetGatewayRequest()
            {
                InternetGatewayId = igw.InternetGatewayId,
                VpcId = vpcId
            };
            client.DetachInternetGateway(requestDetach);

            var request = new DeleteInternetGatewayRequest() { InternetGatewayId = igw.InternetGatewayId };
            await client.DeleteInternetGatewayAsync(request);
            monitorForm.ShowCallbackMessage(
                $"Internet Gateway {igw.InternetGatewayId}|{(igw.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }

        internal async Task DeleteNatGateway(NatGateway ngw, string vpcId)
        {
            var request = new DeleteNatGatewayRequest() { NatGatewayId = ngw.NatGatewayId };
            await client.DeleteNatGatewayAsync(request);
            monitorForm.ShowCallbackMessage(
                $"NAT Gateway {ngw.NatGatewayId}|{(ngw.Tags.Find(o => o.Key == "Name")).Value} is deleted");
        }

        internal async Task DeleteRouteTable(RouteTable routeTable)
        {
            var request = new DeleteRouteTableRequest() { RouteTableId = routeTable.RouteTableId };
            await client.DeleteRouteTableAsync(request);
            monitorForm.ShowCallbackMessage(
                $"Route Table {routeTable.RouteTableId}|{(routeTable.Tags.Find(o => o.Key == "Name")).Value} is deleted");
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

        internal void DisassociateRouteTableToSubnet(RouteTable routeTable, Subnet subnet)
        {
            var request = new DisassociateRouteTableRequest()
            {
                AssociationId = routeTable.Associations.Find(o => o.SubnetId == subnet.SubnetId).RouteTableAssociationId
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
            if (associationId !=null)
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

        private void AssignNameToResource(string resource, string name)
        {
            string resourceName = FormatresourceName(name);
            CreateTagsRequest reqCreateTag = new CreateTagsRequest();
            reqCreateTag.Resources = new List<string>();
            reqCreateTag.Resources.Add(resource);
            reqCreateTag.Tags = new List<Tag>();
            reqCreateTag.Tags.Add(new Tag("Name", resourceName));

            client.CreateTags(reqCreateTag);
            monitorForm.ShowCallbackMessage($"Resource {resource} is created, name: {resourceName}");
        }

        private string FormatresourceName(string name)
        {
            return $"HMS_{environment}_{name}";
        }
    }
}
