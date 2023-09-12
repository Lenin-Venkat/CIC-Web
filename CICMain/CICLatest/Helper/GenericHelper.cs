using CICLatest.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CICLatest.Helper
{
    public class GenericHelper
    {
        public static int GetRegNo(int RegNum, string formValue, AzureStorageConfiguration azureConfig)
        {
            int tempMax = 0;

            if (formValue == "Edit")
            {
                tempMax = RegNum;
            }
            else
            {
                var res = AzureTablesData.GetAllEntityWithContinuationToken(azureConfig.StorageAccount, azureConfig.StorageKey1, "cicform");
                var firstPageObj = JObject.Parse(res.Data);

                var jTokens = new List<JToken>();
                var nextPartitionKey = res.NextPartitionKey;
                var nextRowKey = res.NextRowKey;

                IEnumerable<JToken> jtok = firstPageObj["value"];
                while (nextPartitionKey != null && nextRowKey != null)
                {
                    var response = AzureTablesData.GetEntitybyNextRowPartition(azureConfig.StorageAccount, azureConfig.StorageKey1, "cicform", res.NextPartitionKey, res.NextRowKey);
                    var nextPageObj = JObject.Parse(response.Data);
                    var concatResult = jtok.Concat(nextPageObj["value"]);
                    jtok = concatResult;

                    nextPartitionKey = response.NextPartitionKey;
                    nextRowKey = response.NextRowKey;
                }
                jTokens.AddRange(jtok.ToList());
                //var finalObj = new JObject();

                int cntJson = jTokens.Count();
                int tempRegNo;

                if (cntJson != 0)
                {
                    tempMax = (int)jTokens[0]["ProjectRegistrationNo"];
                }


                for (int i = 0; i < cntJson; i++)
                {
                    tempRegNo = (int)jTokens[i]["ProjectRegistrationNo"];

                    if (tempRegNo > tempMax)
                    {
                        tempMax = tempRegNo;
                    }
                }
                tempMax++;
            }

            return tempMax;

        }

        public static long GetFormRegNo(long RegNum, string formValue, AzureStorageConfiguration azureConfig)
        {
            long tempMax = 0;

            if (formValue == "Edit")
            {
                tempMax = RegNum;
            }
            else
            {
                var res = AzureTablesData.GetAllEntityWithContinuationToken(azureConfig.StorageAccount, azureConfig.StorageKey1, "cicform1");
                var firstPageObj = JObject.Parse(res.Data);

                var jTokens = new List<JToken>();
                var nextPartitionKey = res.NextPartitionKey;
                var nextRowKey = res.NextRowKey;

                IEnumerable<JToken> jtok = firstPageObj["value"];
                while (nextPartitionKey != null && nextRowKey != null)
                {
                    var response = AzureTablesData.GetEntitybyNextRowPartition(azureConfig.StorageAccount, azureConfig.StorageKey1, "cicform1", res.NextPartitionKey, res.NextRowKey);
                    var nextPageObj = JObject.Parse(response.Data);
                    var concatResult = jtok.Concat(nextPageObj["value"]);
                    jtok = concatResult;

                    nextPartitionKey = response.NextPartitionKey;
                    nextRowKey = response.NextRowKey;
                }
                jTokens.AddRange(jtok.ToList());
                //var finalObj = new JObject();

                int cntJson = jTokens.Count();
                int tempRegNo;

                if (cntJson != 0)
                {
                    tempMax = (int)jTokens[0]["FirmRegistrationNo"];
                }


                for (int i = 0; i < cntJson; i++)
                {
                    tempRegNo = (int)jTokens[i]["FirmRegistrationNo"];

                    if (tempRegNo > tempMax)
                    {
                        tempMax = tempRegNo;
                    }
                }
                tempMax++;
            }

            return tempMax;

        }
    }
}
