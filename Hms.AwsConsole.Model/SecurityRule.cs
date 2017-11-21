using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class SecurityRule
    {
        public string Type { get; set; }
        public string Protocol { get; set; }
        public int FromPort { get; set; }
        public int ToPort { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
    }
}
