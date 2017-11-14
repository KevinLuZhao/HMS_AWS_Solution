using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Interfaces;

namespace Hms.AwsConsole.BLL
{
    public class InfraBuilder
    {
        public async Task CreateNewInfrastructure(string env, IWindowForm form)
        {
            Level1Builder level1 = new Level1Builder(env, form);
            await level1.TeardownExistingVPC();
            await level1.CreatVPC();
        }
    }
}
