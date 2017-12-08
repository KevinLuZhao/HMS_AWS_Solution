using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class AwsAppInstance
    {
        public string Environment { get; set; }
        public string Name { get; set; }
        public string InstanceId { get; set; }
        public string PublicIP { get; set; }
        public string PrivateIP { get; set; }
        public string state { get; set; }
    }
}
