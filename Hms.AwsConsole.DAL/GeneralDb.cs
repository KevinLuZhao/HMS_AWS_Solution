using Hms.AwsConsole.AwsUtilities;
using Hms.AwsConsole.Model;
using System.Collections.Generic;

namespace Hms.AwsConsole.DAL
{
    public class GeneralDb<T>
    {
        private DynamoDBHelper<T> helper;
        private string tableName;
        public GeneralDb(string tableName)
        {
            helper = new DynamoDBHelper<T>();
            this.tableName = tableName;
        }
        public void Save(T obj)
        {
            helper.CreateItem(tableName, obj);
        }

        public List<T> GetList(List<DynamodbScanCondition> hmsScanConditions)
        {
            return helper.ScanTable(tableName, hmsScanConditions);
        }

        public T GetItem(string keyValue)
        {
            return helper.GetItemByKey(tableName, TableKeyMap.GetPrimaryKeyName(tableName), keyValue);
        }

        public void DeleteItem(string keyValue)
        {
            helper.DeleteItem(tableName, TableKeyMap.GetPrimaryKeyName(tableName), keyValue);
        }
    }
}
