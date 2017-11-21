using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class SecurityRuleType
    {
        private SecurityRuleType(string key, string value) { Value = value; Key = key; }

        public string Key { get; set; }
        public string  Value { get; set; }

        public static SecurityRuleType CustomTCP { get { return new SecurityRuleType("customtcp", "Custom TCP Rule"); } }
        public static SecurityRuleType CustomUDP { get { return new SecurityRuleType("customudp", "Custom UDP Rule"); } }
        public static SecurityRuleType CustomICMP { get { return new SecurityRuleType("customicmp", "Custom ICMP Rule - IPv4"); } }
        public static SecurityRuleType SSH { get { return new SecurityRuleType("ssh", "SSH"); } }
        public static SecurityRuleType SMTP { get { return new SecurityRuleType("smtp", "SMTP"); } }
        public static SecurityRuleType HTTP { get { return new SecurityRuleType("http", "HTTP"); } }
        public static SecurityRuleType HTTPS { get { return new SecurityRuleType("https", "HTTPS"); } }
        public static SecurityRuleType RDP { get { return new SecurityRuleType("rdp", "RDP"); } }
        public static SecurityRuleType MSSQL { get { return new SecurityRuleType("mssql", "MS SQL"); } }
        public static SecurityRuleType AllTraffic { get { return new SecurityRuleType("-1", "All Traffic"); } }
    }
}
