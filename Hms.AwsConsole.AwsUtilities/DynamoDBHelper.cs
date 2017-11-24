using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.AwsUtilities
{
    public class DynamoDBHelper<T>
    {
        //public string Environment { get; }
        private AmazonDynamoDBClient client;

        public DynamoDBHelper()
        {
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials("safemail");
            client = new AmazonDynamoDBClient(
                credentials,
                AwsCommon.GetRetionEndpoint("us-east-2"));
        }

        //public DynamoDBHelper(Model.Environment profile, string region)
        //{
        //    Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials(profile.ToString());
        //    //this.Environment = profile.ToString();
        //    client = new AmazonDynamoDBClient(credentials, AwsCommon.GetRetionEndpoint("us-east-2"));
        //}

        public void CreateItem(string tableName, T tableInstance)
        {
            Table table = Table.LoadTable(client, tableName);
            var dicTable = new Dictionary<string, AttributeValue>();
            foreach (var prop in tableInstance.GetType().GetProperties())
            {
                DynamoDBEntry entry = new Primitive
                {
                    Value = prop.GetValue(tableInstance, null)
                };
                dicTable.Add(prop.Name, new AttributeValue(prop.GetValue(tableInstance).ToString()));
            }
            //var doc = new Document(dicTable);
            var request = new PutItemRequest(tableName, dicTable);
            client.PutItem(request);
        }

        public long GetTableSize(string tableName)
        {
            return client.DescribeTable(tableName).Table.ItemCount;
        }

        public List<T> QueryItems(string tableName, string keyName, string keyValueName, string AttributeValues)
        {
            var retValue = new List<T>();
            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = keyName + " = " + keyValueName,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {keyValueName, new AttributeValue { S =  AttributeValues }}
                }
            };

            var response = client.Query(request);

            foreach (var item in response.Items)
            {
                retValue.Add(ConvertTableItemToInstance(item));
            }
            return retValue;
        }

        public List<T> ScanTable(string tableName)
        {
            var retValue = new List<T>();
            //var client = new AmazonDynamoDBClient(CredentiaslManager.GetDynamoDbCredential(), AwsCommon.GetRetionEndpoint("us-east-2"));
            var request = new ScanRequest(tableName);
            var response = client.Scan(request);
            foreach (var item in response.Items)
            {
                retValue.Add(ConvertTableItemToInstance(item));
            }
            return retValue;
        }

        //Single key, key type is string
        public T GetItemByKey(string tableName, string primaryKeyName, string primaryKeyValue)
        {
            var request = new GetItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() { { primaryKeyName, new AttributeValue { S = primaryKeyValue } } },
            };
            var response = client.GetItem(request);

            if (response.Item.Count == 0)
                return default(T);
            var item = response.Item;
            return ConvertTableItemToInstance(item);
        }

        public void DeleteItem(string tableName, string primaryKeyName, string primaryKeyValue)
        {
            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() { { primaryKeyName, new AttributeValue { S = primaryKeyValue } } },
            };
            client.DeleteItem(request);
        }

        //private Dictionary<string, AttributeValue> CovertInstanceToTableItem(T instance)
        //{
        //    var dicTable = new Dictionary<string, AttributeValue>();
        //    foreach (var prop in instance.GetType().GetProperties())
        //    {
        //        DynamoDBEntry entry = new Primitive
        //        {
        //            Value = prop.GetValue(instance, null)
        //        };

        //        var attributeValue = new AttributeValue();
        //        string propertyType = prop.GetType().ToString();
        //        switch (propertyType)
        //        {
        //            case "System.string":
        //                attributeValue.S = prop.GetValue(instance).ToString();
        //                break;
        //            case "Stystem.int":
        //                attributeValue.n = prop.GetValue(instance).ToString();
        //                break;
        //        }
        //        dicTable.Add(prop.Name, new AttributeValue(prop.GetValue(instance).ToString()));
        //    }
        //    return dicTable;
        //}

        private T ConvertTableItemToInstance(Dictionary<string, AttributeValue> tableItem)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            foreach (var prop in typeof(T).GetProperties())
            {
                try
                {
                    string propertyType;
                    if (prop.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        propertyType = "System.Enum";
                    }
                    else
                    {
                        propertyType = prop.PropertyType.FullName;
                    }
                    switch (propertyType)
                    {
                        case "System.String":
                            prop.SetValue(obj, tableItem[prop.Name].S);
                            break;
                        case "System.Int32":
                            prop.SetValue(obj, int.Parse(tableItem[prop.Name].S));
                            break;
                        case "System.Enum":
                            prop.SetValue(obj, Enum.Parse(prop.PropertyType, tableItem[prop.Name].S));
                            break;
                        case "System.DateTime":
                            prop.SetValue(obj, DateTime.Parse(tableItem[prop.Name].S));
                            break;
                        default:
                            prop.SetValue(obj, tableItem[prop.Name].S);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Found error while tranfer key {prop.Name} of TableItem. Error message:{ex.Message}");
                }
            }
            return obj;
        }
    }
}
