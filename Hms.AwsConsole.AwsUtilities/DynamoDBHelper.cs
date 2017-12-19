using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Hms.AwsConsole.Model;
using System;
using System.Collections.Generic;

namespace Hms.AwsConsole.AwsUtilities
{
    public class DynamoDBHelper<T>
    {
        //public string Environment { get; }
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;

        public DynamoDBHelper()
        {
            try
            {
                Amazon.Runtime.AWSCredentials credentials = new StoredProfileAWSCredentials("safemail");
                client = new AmazonDynamoDBClient(
                    credentials,
                    AwsCommon.GetRetionEndpoint("us-east-2"));

                context = new DynamoDBContext(client);
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
            Type type = typeof(T);
            foreach (var prop in tableInstance.GetType().GetProperties())
            {
                DynamoDBEntry entry = new Primitive
                {
                    Value = prop.GetValue(tableInstance, null)
                };
                //dicTable.Add(prop.Name, new AttributeValue(prop.GetValue(tableInstance)));
                var propVal = prop.GetValue(tableInstance);
                if (propVal != null)
                {
                    switch (prop.PropertyType.Name)
                    {
                        case "String":
                            dicTable.Add(prop.Name, new AttributeValue(prop.GetValue(tableInstance).ToString()));
                            break;
                        //case "Int32":
                        //    prop.SetValue(obj, int.Parse(tableItem[prop.Name].S));
                        //    break;
                        //case "Enum":
                        //    prop.SetValue(obj, Enum.Parse(prop.PropertyType, tableItem[prop.Name].S));
                        //    break;
                        //case "DateTime":
                        //    prop.SetValue(obj, DateTime.Parse(tableItem[prop.Name].S));
                        //    break;
                        case "List`1":
                            dicTable.Add(prop.Name, new AttributeValue((List<string>)prop.GetValue(tableInstance)));
                            break;
                        default:
                            dicTable.Add(prop.Name, new AttributeValue(prop.GetValue(tableInstance).ToString()));
                            break;
                    }
                }                
            }
            //var doc = new Document(dicTable);
            var request = new PutItemRequest(tableName, dicTable);
            client.PutItem(request);
        }

        public long GetTableSize(string tableName)
        {
            return client.DescribeTable(tableName).Table.ItemCount;
        }

        //public List<T> QueryItems(
        //    string tableName, 
        //    List<KeyValuePair<string, object>> keyExpressionParams, 
        //    List<KeyValuePair<string, object>> filterExpressionParams)
        //{
        //    var retValue = new List<T>();

        //    List<string> lstKeyExpressions = new List<string>();
        //    foreach (var param in keyExpressionParams)
        //    {
        //        lstKeyExpressions.Add()
        //    }
        //    string.Join()
        //    var request = new QueryRequest
        //    {
        //        TableName = tableName,
        //        KeyConditionExpression = keyName + " = " + keyValueName,
        //        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        //        {
        //            {keyValueName, new AttributeValue { S =  AttributeValues }}
        //        }
        //    };

        //    var response = client.Query(request);

        //    foreach (var item in response.Items)
        //    {
        //        retValue.Add(ConvertTableItemToInstance(item));
        //    }
        //    return retValue;
        //}

        public List<T> ScanTable(string tableName, List<DynamodbScanCondition> hmsScanConditions = null)
        {
            var retValue = new List<T>();
            ScanResponse response;
            if (hmsScanConditions != null)
            {
                var filterConditions = new Dictionary<string, Condition>();
                foreach (var hmsCondition in hmsScanConditions)
                {
                    List<AttributeValue> val = new List<AttributeValue>();
                    val.Add(new AttributeValue(hmsCondition.Value.ToString()));
                    Condition condition = new Condition()
                    {
                        AttributeValueList = val,
                        ComparisonOperator = ComparisonOperator.FindValue(hmsCondition.Operator.ToString())
                    };
                    filterConditions.Add(hmsCondition.AttributeName, condition);
                }
                //var request = new ScanRequest(tableName);
                response = client.Scan(tableName, filterConditions);
            }
            else
            {
                response = client.Scan(tableName, new Dictionary<string, Condition>());
            }
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
                //ReturnConsumedCapacity.
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
                    //For the case that instance has a property but table doesn't have yet.
                    if (!tableItem.ContainsKey(prop.Name))
                    {
                        continue;
                    }
                    string propertyType;
                    if (prop.PropertyType.BaseType.Name == "Enum")
                    {
                        propertyType = "Enum";
                    }
                    else
                    {
                        propertyType = prop.PropertyType.Name;
                    }
                    switch (propertyType)
                    {
                        case "String":
                            prop.SetValue(obj, tableItem[prop.Name].S);
                            break;
                        case "Int32":
                            prop.SetValue(obj, int.Parse(tableItem[prop.Name].S));
                            break;
                        case "Enum":
                            prop.SetValue(obj, Enum.Parse(prop.PropertyType, tableItem[prop.Name].S));
                            break;
                        case "DateTime":
                            prop.SetValue(obj, DateTime.Parse(tableItem[prop.Name].S));
                            break;
                        case "List`1":
                            prop.SetValue(obj, tableItem[prop.Name].SS);
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
