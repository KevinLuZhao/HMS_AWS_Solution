using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.AwsUtilities
{
    public class AwsCommon
    {
        public static Amazon.RegionEndpoint GetRetionEndpoint(string region)
        {
            switch (region)
            {
                case "us-east-1":
                    return Amazon.RegionEndpoint.USEast1;
                case "us-east-2":
                    return Amazon.RegionEndpoint.USEast2;
                case "us-west-1":
                    return Amazon.RegionEndpoint.USWest1;
                case "us-west-2":
                    return Amazon.RegionEndpoint.USWest2;
                case "ca-central-1":
                    return Amazon.RegionEndpoint.CACentral1;
                default:
                    return Amazon.RegionEndpoint.USEast2;
            }
        }

        public static string FormatResourceName(string name, string environment)
        {
            return $"HMS_{environment}_{name}";
        }
    }
}
