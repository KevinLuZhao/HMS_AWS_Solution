using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hms.AwsConsole.Interfaces;

namespace Hms.AwsConsole.AwsUtilities
{
    public class DBLevel1InfraBuilder
    {
        IWindowForm monitorForm;
        string environment;
        EC2Helper ec2Helper;

        const string STR_VPC = "Database_VPC";
        const string STR_PUBLIC_SUBNET = "Public_Subnet";
        const string STR_PRIVATE_SUBNET = "Private_Subnet";
        const string STR_INTERNET_GATEWAY = "Internet_Gateway";
        const string STR_NAT_GATEWAY = "NAT_Gateway";
        const string STR_PUBLIC_ROUTETABLE = "Public_RouteTable";
        const string STR_PRIVATE_ROUTETABLE = "Private_Routetable";
        const string CIDR_VPC = "10.82.128.0/26";
        const string CIDR_PUBLIC_SUBNET = "10.82.128.0/27";
        const string CIDR_PRIVATE_SUBNET = "10.82.128.32/27";
        const string CIDR_ALL = "0.0.0.0/0";

        public DBLevel1InfraBuilder(string env, IWindowForm frm)
        {
            monitorForm = frm;
            environment = env;
            ec2Helper = new EC2Helper(env, frm);
        }

        public async Task Creat()
        {
            //EC2Helper helper = new EC2Helper()
        }
    }
}
