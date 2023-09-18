using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Models;
using CICLatest.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm5Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private IHostingEnvironment _env;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;
        public static string accessToken;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;


        public ViewForm5Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context
            ,IHostingEnvironment env, UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            _context = context;
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _userManager = userManager;
            _env = env;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }
        public IActionResult ViewForm5(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm5 model = new SaveModelForm5();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5", rowkey, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.AppType = (string)myJObject["value"][i]["AppType"];               
                model.NameOFJoinVenture = (string)myJObject["value"][i]["NameOFJoinVenture"];
                model.TypeofJoointVenture = (string)myJObject["value"][i]["TypeofJoointVenture"];
                model.BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"];
                model.BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Phyaddress = (string)myJObject["value"][i]["Phyaddress"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.SurName = (string)myJObject["value"][i]["SurName"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.Fax = (string)myJObject["value"][i]["Fax"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.SubcatogoryId = (string)myJObject["value"][i]["SubcatogoryId"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.DateofRegistration = (DateTime)myJObject["value"][i]["DateofRegistration"];
                model.TaxIdentityNo = (string)myJObject["value"][i]["TaxIdentityNo"];
                //model.SubcatogoryId = (string)myJObject["value"][i]["Subcatogory"];                
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];               
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];               
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                if (myJObject["value"][i]["AdminFee"] != null)
                {
                    model.AdminFee = (int)myJObject["value"][i]["AdminFee"];
                }
                if (myJObject["value"][i]["RegistrationFee"] != null)
                {
                    model.RegistrationFee = (int)myJObject["value"][i]["RegistrationFee"];
                }
                if (myJObject["value"][i]["RenewalFee"] != null)
                {
                    model.RenewalFee = (int)myJObject["value"][i]["RenewalFee"];
                }
            }
           
            if (model.SubcatogoryId != "0")
            {
                ViewBag.SubCategory = GetSubCategorybyName(Convert.ToInt32(model.SubcatogoryId));
            }
            

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<DetailOfProjects> d = new List<DetailOfProjects>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DetailOfProjects
                {
                    CICRegistrationNo = (string)myJObject1["value"][i]["CICRegistrationNo"],
                    ContactDetails = (string)myJObject1["value"][i]["ContactDetails"],
                   // AttchedDoc = (string)myJObject1["value"][i]["AttchedDoc"],
                    CountryOfOrigin = (string)myJObject1["value"][i]["CountryOfOrigin"],
                    NameofApplicant = (string)myJObject1["value"][i]["NameofApplicant"],
                    //ShareFile = 
                    Shareholding = (int)myJObject1["value"][i]["Shareholding"]

                });
            }
            model.detailOfProjects = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5SubconsultantDetails", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<SubConsultantDetail> a = new List<SubConsultantDetail>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new SubConsultantDetail
                {
                    CICRegistrationNo = (string)myJObject2["value"][i]["CICRegistrationNo"],
                    ContractValueOfWork = (decimal)myJObject2["value"][i]["ContractValueOfWork"],
                    CountryyofOrigin = (string)myJObject2["value"][i]["CountryyofOrigin"],
                    DescriptionOfWork = (string)myJObject2["value"][i]["DescriptionOfWork"],
                    NameofConsultant = (string)myJObject2["value"][i]["NameofConsultant"]

                });
            }
            model.subConsultantDetail = a;
            List<FileList> AllFileList = new List<FileList>();
            AllFileList = _blobStorageService.GetBlobList(model.ImagePath);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "Signature": model.SignatureName = AllFileList[j].FileValue; break;

                        case "BusineesParticularsfile1": model.BusineesParticularsfile1Name = AllFileList[j].FileValue; break;

                        case "BusineesParticularsfile2": model.BusineesParticularsfile2Name = AllFileList[j].FileValue; break;
                        case "Signature1": model.Signature2Name = AllFileList[j].FileValue; break;
                        
                    }
                }
            }
            memoryCache.Set("Form5Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm5()
        {
           //string jsond = "";
            SaveModelForm5 model = new SaveModelForm5();
            bool isExist = memoryCache.TryGetValue("Form5Data", out model);           

            if (isExist)
            {
                // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform5", model.PartitionKey, model.RowKey, jsond);
                string comment = Request.Form["comment"];
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        model.FormStatus = "Approve";
                        //ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager);
                        //string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        //viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Compliance Analyst";
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Ops Manager";
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Ops Manager";
                        model.FormStatus = "Completed";
                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);

                        bool SFlag = ShareValidation(model.detailOfProjects);

                        string invoice, id;
                        CICFees fees = null;

                        if (model.TypeofJoointVenture == "Foreign/Foreign" || SFlag)
                        {
                            fees = calculateFees(model.FormName, "Foreign");
                            
                        }
                        else
                        {
                            fees = calculateFees(model.FormName, "Local");
                        }

                        model.AdminFee = fees.AdminFees;

                        if (model.CountryOfOrigin != "Swazi")
                        {
                            model.RenewalFee = fees.RenewalFees;
                        }
                        model.RegistrationFee = fees.RegistrationFees;

                        ///AK
                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        id = viewForm2.CreateInvoiceERP(model.CustNo, model.RowKey, out invoice, model.PartitionKey,model.FormName);
                        model.InvoiceNo = invoice;
                        // viewForm2.CreateInvoiceLineItemERP(id, model.RegistrationFee, model.AdminFee, model.RenewalFee);

                        string jsonData;
                        int penalty;

                        AzureTablesData.GetAllEntity(StorageName, StorageKey, "GracePeriodDetails", out jsonData);//Get data
                        JObject gracePeriodObject = JObject.Parse(jsonData);

                        DateTime allowedGracePeriod = (DateTime)gracePeriodObject["value"][0]["allowedDate"];

                        if (allowedGracePeriod < DateTime.Now && model.AppType == "Renewal")
                        {
                            penalty = (model.AdminFee * 10) / 100;

                        }
                        else
                        {
                            penalty = 0;
                        }

                        viewForm2.CreateInvoiceLineItemERP(id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), Convert.ToDecimal(penalty));


                        ViewForm1Controller view1Form = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        string jsonProjectData;
                        AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5", model.RowKey, out jsonProjectData);
                        JObject myJObject = JObject.Parse(jsonProjectData);
                        int cntJson = myJObject["value"].Count();

                        accessToken = view1Form.GetAccessToken();
                        for (int i = 0; i < cntJson; i++)
                        {
                            model.RegistrationID = UpdateRegistrationDetails(myJObject, i, invoice, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee),model.Phyaddress, Convert.ToDecimal(penalty),model.AppType);
                        }

                        break;
                }
                model.detailOfProjects = null;
                model.comment = comment;
                model.subConsultantDetail = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform5", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form5Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public CICFees calculateFees(string formname, string grade)
        {
            //int fees = 0;

            var feelist = (from item in _context.cicFees
                           where item.FormName == formname & item.Grade == grade
                           select item).FirstOrDefault();

            return feelist;

        }
        public bool ShareValidation(List<DetailOfProjects> p)
        {
            int ForeignShare = 0, SwaziShare = 0;

            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].CountryOfOrigin != "Swazi")
                {
                    ForeignShare = p[i].Shareholding;
                }

                if (p[i].CountryOfOrigin == "Swazi")
                {
                    SwaziShare = SwaziShare + p[i].Shareholding;
                }
            }

            if (SwaziShare != 0 && ForeignShare != 0 && SwaziShare < 60)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            //var jsond = "";
            //string reviewer = "";
            SaveModelForm5 model = new SaveModelForm5();
            bool isExist = memoryCache.TryGetValue("Form5Data", out model);

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform5", model.PartitionKey, model.RowKey, jsond);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.FormStatus = "Rejected";
                        model.comment = "Clerk comment - " + comment;
                        var domain = _appSettingsReader.Read("Domain");
                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='" + domain + "'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        view1Controller.sendNotification(model.CreatedBy, "Your Form is Rejected", body);

                        CICCommonService commService = new CICCommonService(_userManager);
                        body = "Hi " + RepresentativeName + ", Your form is rejected due to reason:" + comment + ".To access CIC portal you can login at: "+ domain +" Thank you, CIC Team";
                        commService.sendSMS(model.CreatedBy, body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Clerk";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Registration Officer comment - " + comment;
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Compliance Officer";
                        model.comment = model.comment + Environment.NewLine + "Registration Analyst comment - " + comment;
                        model.FormStatus = "Rejected";
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Compliance Analyst";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        model.FormStatus = "Rejected";
                        break;
                }
                model.detailOfProjects = null;
                model.subConsultantDetail = null;
                //model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform5", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            

            memoryCache.Remove("Form5Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

       
        [HttpPost]
        public string DownloadFile()
        {
            SaveModelForm5 model = new SaveModelForm5();
            
            bool isExist = memoryCache.TryGetValue("Form5Data", out model);
            if (isExist)
            {
                var webRoot = _env.WebRootPath;
                var fileP = System.IO.Path.Combine(webRoot, @"CIC\");
                _blobStorageService.DownloadBlob1(model.ImagePath, fileP);

                return fileP;
            }
            return "";
        }
        //adding new for preview
        public IActionResult PreviewForm5(Cicf5Model model)
        {
            if (model.selectedsubcategory != 0)
            {
                ViewBag.SubCategory = GetSubCategorybyName(model.selectedsubcategory);
            }
            return PartialView("Form5Preview", model);
        }
        public string GetSubCategorybyName(int subcategoryId)
        {
            string subName = "";
            subName = (from SubCategoryType in _context.SubCategory
                       where SubCategoryType.SubCategoryID == subcategoryId
                       select SubCategoryType.SubCategoryName).FirstOrDefault();

            return subName;
        }

        public string UpdateRegistrationDetails(JObject myJObject, int i, string invoiceNo, decimal registratinFee, decimal adminFee, decimal reFee,string postalAddress,decimal penaltyFee,string typeofApplication)
        {
            string custno = (string)myJObject["value"][i]["CustNo"];
            DateTime createdDate = DateTime.Now;
            string regID = "";
            if (myJObject["value"][i]["CreatedDate"] != null)
            {
                createdDate = (DateTime)myJObject["value"][i]["CreatedDate"];
            }
            try
            {
                var data1 = JObject.FromObject(new
                {
                    customerNo = (string)myJObject["value"][i]["CustNo"],
                    externalDocumentNo = (string)myJObject["value"][i]["RowKey"],
                    invoiceNo = invoiceNo,
                    tradeName = (string)myJObject["value"][i]["NameOFJoinVenture"],
                    businessName = (string)myJObject["value"][i]["BusinessName"],
                    certificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    registration = registratinFee,
                    renewal = reFee,
                    adminFee = adminFee,
                    penalty = penaltyFee,
                    credit = 0,
                    owing = 0,
                    total = 0,
                    dateofPay = "2022-07-19",
                    typeofPay = "EFT",
                    bank = (string)myJObject["value"][i]["BankName"],
                    category = (string)myJObject["value"][i]["Category"],
                    monthofReg = createdDate.ToString("MMMM"),
                    typeofApplication = typeofApplication
                });
                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = _azureConfig.BCURL + "/customersContract1";
                    HttpResponseMessage response = httpClient.PostAsync(@u, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string str = response.Content.ReadAsStringAsync().Result;
                        JObject myProjectJObject = JObject.Parse(str);
                        regID = (string)myProjectJObject["id"];
                    }
                }
                //updating Blob Postal Address
                using (var httpClient = new HttpClient())
                {
                    string BCUrl2 = _azureConfig.BCURL + "/customersContract1(" + regID + ")/PostalAddress";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, postalAddress, "text/plain", accessToken));
                    t.Wait();
                }
                return regID;
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
            HttpContent c = new StringContent(json, null, appType);

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
    }
}
