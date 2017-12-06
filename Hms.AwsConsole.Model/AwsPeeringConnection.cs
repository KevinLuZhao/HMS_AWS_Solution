using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class AwsPeeringConnection
    {
        public string VpcPeeringConnectionId { get; set; }
        public string AccepterVpc { get; set; }
        public string RequesterVpc { get; set; }
        public string Status { get; set; }
    }
}
