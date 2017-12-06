using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class AwsVpc
    {
        public string VpcId { get; set; }
        public string Name { get; set; }
        public string CidrBlock { get; set; }
        public string State { get; set; }
    }
}
