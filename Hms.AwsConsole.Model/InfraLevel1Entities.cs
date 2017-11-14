﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class InfraEntities
    {
        public string Environment { get; set; }
        public string VpcId { get; set; }
        public string PublicSubnetId { get; set; }
        public string PrivateSubnetId { get; set; }
        public string InternetGatewayId { get; set; }
        public string NatGatewayId { get; set; }
        public string PublicRouteTableId { get; set; }
        public string PrivateRouteTableId { get; set; }
    }
}
