using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;
using Amazon.EC2.Model;

namespace Hms.AwsConsole.AwsUtilities
{
    public class Level2Builder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;

        public Level2Builder(string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env, frm);
        }

        public async Task Creat()
        {
            //return Task<T>;
        }

        //private async Task<Instance> LaunchWebServer()
        //{
        //    ec2Helper.LaunchInstances()
        //}
    }
}
