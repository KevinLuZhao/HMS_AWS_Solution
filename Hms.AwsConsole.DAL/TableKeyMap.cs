using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.DAL
{
    internal class TableKeyMap
    {
        internal static string GetPrimaryKeyName(string tableName)
        {
            switch (tableName)
            {
                case "hms_application_infra_entities":
                case "hms_db_infra_entities":
                case "hms_instances":
                    return "Environment";
                case "hms_logs":
                    return "Id";
                default:
                    return null;
            }
        }
    }
}
