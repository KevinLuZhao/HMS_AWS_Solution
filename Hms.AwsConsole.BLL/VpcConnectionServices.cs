using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;

namespace Hms.AwsConsole.BLL
{
    public class VpcConnectionServices
    {
        public string CreateVpcPeeringConnection(string peerVpcId, string vpcId, string environment, IWindowForm form)
        {
            var helper = new EC2Helper(environment, form);
            return helper.CreatePeeringConnection(peerVpcId, vpcId);
        }
    }
}
