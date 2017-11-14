using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class Regions
    {
        public static List<KeyValuePair<string, string>> GetRegionList()
        {
            List<KeyValuePair<string, string>> lstRegions = new List<KeyValuePair<string, string>>();
            lstRegions.Add(new KeyValuePair<string, string>("us-east-1", "US East (N. Virginia)"));
            lstRegions.Add(new KeyValuePair<string, string>("us-east-2", "US East (Ohio)"));
            lstRegions.Add(new KeyValuePair<string, string>("us-west-1", "US West (N. California)"));
            lstRegions.Add(new KeyValuePair<string, string>("us-west-2", "US West (Oregon)"));
            lstRegions.Add(new KeyValuePair<string, string>("ca-central-1", "Canada (Central)"));
            return lstRegions;
        }
    }
}
