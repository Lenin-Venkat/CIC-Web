using CICLatest.Helper;
using CICLatest.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    public class PaymentController : Controller
    {
        static string formNo;
        private readonly AzureStorageConfiguration _azureConfig;
        static string StorageName = "";
        static string StorageKey = "";
        private IHostingEnvironment Environment;
        private readonly EmailConfiguration _emailcofig;
        public readonly IBlobStorageService _blobStorageService;

        public string accessToken = "";
        

        public IActionResult Index(string userId, string invoiceNo)
        {
            PaymentModel model = new PaymentModel();
            model.CustNo = userId;
            model.invoiceNo = invoiceNo;
            return View(model);
        }

        public PaymentController(AzureStorageConfiguration azureConfig, EmailConfiguration emailconfig, IBlobStorageService blobStorageService)
        {
            _azureConfig = azureConfig;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _emailcofig = emailconfig;
            _blobStorageService = blobStorageService;   
        }

        [HttpPost]
        public async Task<IActionResult> Index(PaymentModel model)
        {
            if (model.Paymentfile == null || model.Paymentfile.Length == 0)
            {
                ViewBag.msg = "file not selected";
                ViewBag.success = false;
                return View();
            }


            string path = uploadFiles(model.Paymentfile, model.CustNo, "Payment", model.invoiceNo);
            path = path + @"\" + model.Paymentfile.FileName;
            var data1 = JObject.FromObject(new
            {
                PartitionKey = "Payment",
                RowKey = model.CustNo,
                path = path
            });

            var response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicPayment", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            ViewBag.msg = "Payment Receipt uploaded successfully";
            ViewBag.success = true;
            return View();
        }

        public string uploadFiles(IFormFile tempFile, string path, string name,string invoiceNo)
        {
            string filepath = "";

            if (tempFile != null)
            {
                #region Read File Content
                string fileName = Path.GetFileName(tempFile.FileName);
                string TempFilename = name + "_" + tempFile.FileName;
                byte[] fileData;
                using (var target = new MemoryStream())
                {
                    tempFile.CopyTo(target);
                    fileData = target.ToArray();
                }

                
                string mimeType = tempFile.ContentType;


                filepath = _blobStorageService.UploadFileToBlob(TempFilename, fileData, mimeType, path);
                string extension = System.IO.Path.GetExtension(TempFilename);
                SendEmailNotification(TempFilename, fileData, path, extension, invoiceNo);
                sendNotificationToBC(invoiceNo, path, TempFilename);
                #endregion
            }
            return filepath;
        }

        //public void SendEmailNotification(string strFileName, byte[] fileData, string CustNo,string mimeType)
        //{
        //    strFileName = "<p>Dear Valuable Contractor, customer " + CustNo + " successfully updated the POP in portal please review  <br/><br/>Thank you,<br/>CIC Team</p>";
        //    Email emailobj = new Email(_emailcofig);
        //    emailobj.SendPaymentAsync("POP", strFileName, fileData, mimeType);
        //}
        public void SendEmailNotification(string strFileName, byte[] fileData, string CustNo, string mimeType, string invoiceNo)
        {
            var invoiceNoTrimmed = GetInvoiceNumberWithoutCustNo(CustNo, invoiceNo);
            strFileName = "<p>Dear Finance team, Please review attachment of POP that has been uploaded by " + CustNo + " for Invoice No. " + invoiceNoTrimmed + "<br/><br/>Thank you.</p>";
            Email emailobj = new Email(_emailcofig);
            emailobj.SendPaymentAsync("POP", strFileName, fileData, mimeType);
        }

        private string GetInvoiceNumberWithoutCustNo(string custNo, string invoiceNo)
        {
            if (invoiceNo.Contains(custNo, StringComparison.InvariantCultureIgnoreCase))
            {
                return invoiceNo.Replace($"{custNo}-", string.Empty, StringComparison.InvariantCultureIgnoreCase).Replace("-SalesInvoice.pdf", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }
            return invoiceNo;
        }

        public void sendNotificationToBC(string varInvoiceNo, string CustNo, string TempFilename)
        {
            int index1 = varInvoiceNo.IndexOf('-')+1;            
            string strInvoiceNo = varInvoiceNo.Substring(index1);
            string strInvoiceNo1 = strInvoiceNo.Substring(0,strInvoiceNo.IndexOf('-'));

            GetAccessToken();

            var data1 = JObject.FromObject(new
            {
                invoiceNo = strInvoiceNo1,
                customer = CustNo,
                paymentFile = TempFilename
            });

            var json = JsonConvert.SerializeObject(data1);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync(@_azureConfig.BCURL + "/cicPaymentInvoices", data).Result;
            }
        }

        public string GetAccessToken()
        {
            //Get new token from Azure for BC
            string url = _azureConfig.TokenURL;

            //ConfigurationSettings.AzureAccessToken
            Uri uri = new Uri(_azureConfig.Authority.Replace("{AadTenantId}", _azureConfig.AadTenantId));
            Dictionary<string, string> requestBody = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials" },
                    {"client_id" , _azureConfig.ClientId },
                    {"client_secret", _azureConfig.ClientSecret },
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
