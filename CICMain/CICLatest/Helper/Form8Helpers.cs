using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using CICLatest.Models;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Controllers;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using System.Collections.Generic;
using CICLatest.Contracts;

namespace CICLatest.Helper
{
    public class Form8Helpers
    {
        public string accessToken = "";
        public string ClientId;
        public string ClientSecret;
        public string AadTenantId;
        public string Authority;
        public string BCURL;
        public string TokenURL;
        public readonly IAppSettingsReader _appSettingsReader;

        public Form8Helpers(IAppSettingsReader appSettingsReader) 
        {
            _appSettingsReader = appSettingsReader;
            ClientId = _appSettingsReader.Read("ClientId");
            ClientSecret = _appSettingsReader.Read("ClientSecret");
            AadTenantId = _appSettingsReader.Read("AadTenantId");
            Authority = _appSettingsReader.Read("Authority");
            BCURL = _appSettingsReader.Read("BCURL");
            TokenURL = _appSettingsReader.Read("TokenURL");
        }
       
        public void PostReleaseUpdates(SaveModelForm8 model, string StorageName, string StorageKey)
        {
            string invoiceNo;
            string id;
          
            //AK
            id = CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceNo, model.PartitionKey, model.FormName);
            model.InvoiceNo = invoiceNo;
            CreateInvoiceLineItemERP(id, Convert.ToDecimal(model.TotalProjectCost));


            // data into BC
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform8", model.RowKey, out jsonData1);//Get data
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
                UpdateProjectDetails(myJObject1, i, model.RowKey, invoiceNo,model.ProjectType);

        }
        private string CreateInvoiceERP(string cust, string AppNo, out string invoiceNo, string partitionKey, string FormName)
        {
            string istr = "";
            invoiceNo = "";
            GetAccessToken();
            try
            {
                var data1 = JObject.FromObject(new
                {
                    customerNumber = cust,
                    externalDocumentNumber = AppNo,
                    levyInvoice = true,
                    partitionKey = partitionKey,
                    formName = FormName
                });
                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = BCURL + "/cicSalesOrders";
                    HttpResponseMessage response = httpClient.PostAsync(@u, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string str = response.Content.ReadAsStringAsync().Result;

                        JObject myJObject = JObject.Parse(str);
                        istr = (string)myJObject["id"];
                        invoiceNo = (string)myJObject["number"];
                    }
                }
                return istr;
            }
            catch (Exception e)
            { string s = e.Message; }

            return "";
        }

        private void CreateInvoiceLineItemERP(string istr, decimal TotalProjectCost)
        {
            GetAccessToken();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data2 = JObject.FromObject(new
                    {
                        lineType = "G/L Account",
                        lineObjectNumber = "1000/006",
                        description = "Construction Levy",
                        unitPrice = TotalProjectCost,
                        quantity = 1,
                        discountAmount = 0,
                        discountPercent = 0
                    });
                    var json2 = JsonConvert.SerializeObject(data2);
                    var data3 = new StringContent(json2, Encoding.UTF8, "application/json");
                    string uitemline = BCURL + "/cicSalesOrders(" + istr + ")/cicSalesOrderLines";
                    HttpResponseMessage response1 = httpClient.PostAsync(@uitemline, data3).Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        string str = response1.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception e)
            { string s = e.Message; }

        }

        //AK Project Details
        private string UpdateProjectDetails(JObject myJObject, int i, string rowkey, string invoiceNo, string ProjectType)
        {

            string custno = (string)myJObject["value"][i]["CustNo"];
            string istr = "";
            try
            {
                var data1 = JObject.FromObject(new
                {
                    customerNo = (string)myJObject["value"][i]["CustNo"],
                    invoiceNo = invoiceNo,
                    certificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    customerCategory = (string)myJObject["value"][i]["CategoryId"],
                    grade = (string)myJObject["value"][i]["Grade"],
                    projectNumber = rowkey,
                    contractSum = (decimal)myJObject["value"][i]["ContractVAlue"],
                    levy = (decimal)myJObject["value"][i]["LevyPaybale"],
                    levyAmount = (decimal)myJObject["value"][i]["TotalProjectCost"],
                    classification = (string)myJObject["value"][i]["OwnerCategoryId"],
                    levyPaymentOption = (string)myJObject["value"][i]["LevyPaymentOptions"],
                    timeFrameForPaymentOfLevy = (string)myJObject["value"][i]["TimeFrameoption"],
                    projectDetails = (string)myJObject["value"][i]["ProjectTite"],
                    prcContactName = (string)myJObject["value"][i]["AuthrisedFirstName"],
                    prcContactSurname = (string)myJObject["value"][i]["AuthorisedSurname"],
                    prcDesignation = (string)myJObject["value"][i]["Designation"],
                    prcEmail = (string)myJObject["value"][i]["AuthorisedEmail"],
                    prcMobilePhoneNo = (string)myJObject["value"][i]["AuthorisedMobile"],
                    prcPhoneNo = (string)myJObject["value"][i]["AuthorisedTelePhone"],
                    pocContactName = (string)myJObject["value"][i]["FirstName"],
                    pocContactSurname = (string)myJObject["value"][i]["Surname"],
                    pocEmail = (string)myJObject["value"][i]["EmailAdress"],
                    pocMobilePhoneNo = (string)myJObject["value"][i]["MobileNo"],
                    pocPhoneNo = (string)myJObject["value"][i]["Telephone"],
                    awardDate = ((DateTime)myJObject["value"][i]["DateofAward"]).ToString("yyyy-MM-dd"),
                    startDate = ((DateTime)myJObject["value"][i]["ProposedCommencmentDate"]).ToString("yyyy-MM-dd"),
                    completionDate = ((DateTime)myJObject["value"][i]["ProposedCompleteDate"]).ToString("yyyy-MM-dd"),
                    tradeName = (string)myJObject["value"][i]["Oraganization"],
                    typeofApplication = ProjectType
                });

                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = BCURL + "/customersContract";
                    HttpResponseMessage response = httpClient.PostAsync(@u, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string str = response.Content.ReadAsStringAsync().Result;

                        JObject myProjJObject = JObject.Parse(str);
                        istr = (string)myProjJObject["id"];
                    }
                }


                //updating Blob Physical Address
                using (var httpClient = new HttpClient())
                {
                    string BCUrl2 = BCURL + "/customersContract(" + istr + ")/pocPhysicalAddress";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PhysicalAddress"], "text/plain", accessToken));
                    t.Wait();
                }

                //updating Blob Postal Address
                using (var httpClient = new HttpClient())
                {
                    string BCUrl2 = BCURL + "/customersContract(" + istr + ")/pocPostalAddress";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PostalAddress"], "text/plain", accessToken));
                    t.Wait();
                }


                return custno;
            }
            catch
            { return ""; }

        }

        static async Task<HttpResponseMessage> PatchData(Uri u, string json, string appType, string accessToken)
        {
            HttpClient client1 = new HttpClient();
            client1.DefaultRequestHeaders.Clear();
            client1.DefaultRequestHeaders.Add("If-Match", "*");
            client1.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent c = new StringContent(json, Encoding.UTF8, appType);

            var method = "PATCH";
            var httpVerb = new HttpMethod(method);
            var httpRequestMessage =
                new HttpRequestMessage(httpVerb, u)
                {
                    Content = c
                };

            var response = await client1.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseCode = response.StatusCode;
                var responseJson = response.Content.ReadAsStringAsync();
            }
            return response;
        }

        public string GetAccessToken()
        {
            //Get new token from Azure for BC
            string url = TokenURL;
            Dictionary<string, string> requestBody = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials" },
                    {"client_id" , ClientId },
                    {"client_secret", ClientSecret },
                    {"scope", @"https://api.businesscentral.dynamics.com/.default" }
                };

            var content = new FormUrlEncodedContent(requestBody);
            HttpClient client = new HttpClient();
            var response = client.PostAsync(url, content);
            var rescontent = response.Result.Content.ReadAsStringAsync();

            dynamic jsonresult = JsonConvert.DeserializeObject(rescontent.Result);
            accessToken = jsonresult.access_token;
            return accessToken;
        }
    }
}
