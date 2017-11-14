using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class AwsRdsInstance
    {
        public string DBInstanceIdentifier { get; set; }
        public string DBInstanceArn { get; set; }
        public Environment RdsEnvinronment { get; set; }
        public string Status { get; set; }
        public bool MultiAZ { get; set; }
    }
}
