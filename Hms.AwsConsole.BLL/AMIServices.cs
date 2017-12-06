using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.Model;
using Hms.AwsConsole.AwsUtilities;

namespace Hms.AwsConsole.BLL
{
    public class AMIServices
    {
        public List<ImageModel> GetAMIs(string env)
        {
            EC2Helper helper = new EC2Helper(env.ToString());
            return helper.GetImageList();
        }
    }
}
