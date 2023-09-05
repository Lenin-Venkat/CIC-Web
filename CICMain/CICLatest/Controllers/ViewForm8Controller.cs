using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CICLatest.Contracts;
using CICLatest.Helper;
using CICLatest.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm8Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly BCConfiguration _bcConfig;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public ViewForm8Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context
            , UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            _context = context;
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _userManager = userManager;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }
        public IActionResult ViewForm8(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm8 model = new SaveModelForm8();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform8", rowkey, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            for (int i = 0; i < cntJson; i++)
            {
                //adding
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.Oraganization = (string)myJObject["value"][i]["Oraganization"];
                // model.CategoryId = (int)myJObject["value"][i]["CategoryId"];
                model.CategoryId = (string)myJObject["value"][i]["CategoryId"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.CertificateNo = (string)myJObject["value"][i]["CertificateNo"];
                model.TypeGender = (string)myJObject["value"][i]["TypeGender"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.Surname = (string)myJObject["value"][i]["Surname"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.EmailAdress = (string)myJObject["value"][i]["EmailAdress"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.EmailAddress = (string)myJObject["value"][i]["EmailAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];

                model.Form1PostalAddress = (string)myJObject["value"][i]["Form1PostalAddress"]; //AK

                model.Name = (string)myJObject["value"][i]["Name"];
                model.IDNO = (string)myJObject["value"][i]["IDNO"];
                model.PassportNo = (string)myJObject["value"][i]["PassportNo"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.TelephoneWorkdiscipline = (string)myJObject["value"][i]["TelephoneWorkdiscipline"];
                model.FaxNoWorkdiscipline = (string)myJObject["value"][i]["FaxNoWorkdiscipline"];
                model.EmailAddress = (string)myJObject["value"][i]["EmailAddress"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.AuthrisedFirstName = (string)myJObject["value"][i]["AuthrisedFirstName"];
                model.AuthorisedSurname = (string)myJObject["value"][i]["AuthorisedSurname"];
                model.DesignationWorkdiscipline = (string)myJObject["value"][i]["DesignationWorkdiscipline"];
                model.AuthorisedMobile = (string)myJObject["value"][i]["AuthorisedMobile"];
                model.AuthorisedFaxNo = (string)myJObject["value"][i]["AuthorisedFaxNo"];
                model.AuthorisedEmail = (string)myJObject["value"][i]["AuthorisedEmail"];
                model.OwnerCategoryId = (string)myJObject["value"][i]["OwnerCategoryId"];//adding new for owner category
                model.DateofAward = (DateTime)myJObject["value"][i]["DateofAward"];

                // model.WorkDisciplinefor = (int)myJObject["value"][i]["WorkDisciplinefor"];
                model.WorkDisciplinefor = (string)myJObject["value"][i]["WorkDisciplinefor"];
                model.ProjectType = (string)myJObject["value"][i]["ProjectType"];
                model.BidReference = (string)myJObject["value"][i]["BidReference"];
                model.ProjectTite = (string)myJObject["value"][i]["ProjectTite"];
                model.ProjectorFunder = (string)myJObject["value"][i]["ProjectorFunder"];
                model.ProjectLocation = (string)myJObject["value"][i]["ProjectLocation"];
                model.TownInkhundla = (string)myJObject["value"][i]["TownInkhundla"];
                model.Region = (string)myJObject["value"][i]["Region"];
                model.GPSCo = (string)myJObject["value"][i]["GPSCo"];
                model.BriefDescriptionofProject = (string)myJObject["value"][i]["BriefDescriptionofProject"];
                model.ProposedCommencmentDate = (DateTime)myJObject["value"][i]["ProposedCommencmentDate"];
                model.ProposedCompleteDate = (DateTime)myJObject["value"][i]["ProposedCompleteDate"];
                model.RevisedDate = (DateTime)myJObject["value"][i]["RevisedDate"];
                model.ContractVAlue = (decimal)myJObject["value"][i]["ContractVAlue"];
                model.LevyPaybale = (decimal)myJObject["value"][i]["LevyPaybale"];
                model.PoPostalAddress = (string)myJObject["value"][i]["PoPostalAddress"];

                model.TotalProjectCost = (decimal)myJObject["value"][i]["TotalProjectCost"];
                model.TotalProjectCostIncludingLevy = (decimal)myJObject["value"][i]["TotalProjectCostIncludingLevy"];
                model.LevyPaymentOptions = (string)myJObject["value"][i]["LevyPaymentOptions"];
                model.TimeFrameoption = (string)myJObject["value"][i]["TimeFrameoption"];
                model.RevisedDate = (DateTime)myJObject["value"][i]["RevisedDate"];
                model.AuthorisedTelePhone = (string)myJObject["value"][i]["AuthorisedTelePhone"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessName = (string)myJObject["value"][i]["WitnessName"];
                model.WitnessName1 = (string)myJObject["value"][i]["WitnessName1"];
                model.WitnessTitleDesignation = (string)myJObject["value"][i]["WitnessTitleDesignation"];
                model.WitnessTitleDesignation1 = (string)myJObject["value"][i]["WitnessTitleDesignation1"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.Subcategory = (int)myJObject["value"][i]["Subcategory"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.CompName = (string)myJObject["value"][i]["CompName"];
                model.Place = (string)myJObject["value"][i]["Place"];
                model.Position = (string)myJObject["value"][i]["Position"];
                model.Day = (int)myJObject["value"][i]["Day"];
                model.Month = (int)myJObject["value"][i]["Month"];
                model.Year = (int)myJObject["value"][i]["Year"];
                model.RepresentativeName = (string)myJObject["value"][i]["RepresentativeName"];
                model.WitnessName = (string)myJObject["value"][i]["WitnessName"];
                model.WitnessName1 = (string)myJObject["value"][i]["WitnessName1"];
                model.WitnessTitleDesignation = (string)myJObject["value"][i]["WitnessTitleDesignation"];
                model.WitnessTitleDesignation1 = (string)myJObject["value"][i]["WitnessTitleDesignation1"];

                model.PartialInvoiceCount = (int)myJObject["value"][i]["PartialInvoiceCount"];
                model.CreateClearenceCertificate = (int)myJObject["value"][i]["CreateClearenceCertificate"];
                model.projectCertificateCreated = (int)myJObject["value"][i]["projectCertificateCreated"];
                model.NoOfPartialCertificateCreated = (int)myJObject["value"][i]["NoOfPartialCertificateCreated"];

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
            if (model.OwnerCategoryId == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            if (model.Subcategory != 0)
            {
                ViewBag.SubCategory = GetSubCategorybyName(model.Subcategory);
            }

            if (model.ProjectType == "NewProject")
            {
                ViewBag.projectDate = true;
            }
            else
            {
                ViewBag.projectDate = false;
            }
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8DetailsOfAllConsultamcyFirms", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<Tab3FirstSection> d = new List<Tab3FirstSection>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new Tab3FirstSection
                {
                    CicRegistrationNo = (string)myJObject1["value"][i]["CicRegistrationNo"],
                    Contact = (string)myJObject1["value"][i]["Contact"],
                    CountryofOrigin = (string)myJObject1["value"][i]["CountryofOrigin"],
                    FormRegistrationNo = (int)myJObject1["value"][i]["FormRegistrationNo"],
                    NameofConsultancyFirm = (string)myJObject1["value"][i]["NameofConsultancyFirm"],
                    //ShareFile = 
                    Profession = (string)myJObject1["value"][i]["Profession"]

                });
            }
            model.Tab3MainSection = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<Tab3SecSection> a = new List<Tab3SecSection>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new Tab3SecSection
                {
                    ContactDetails = (string)myJObject2["value"][i]["ContactDetails"],
                    CountryofOrigin = (string)myJObject2["value"][i]["CountryofOrigin"],
                    FormRegistrationNo = (int)myJObject2["value"][i]["FormRegistrationNo"],
                    NameofSubContractors = (string)myJObject2["value"][i]["NameofSubContractors"],
                    RegistrationNo = (string)myJObject2["value"][i]["RegistrationNo"],
                    ScopeofWork = (string)myJObject2["value"][i]["ScopeofWork"],
                });
            }
            model.tab3SecSection = a;

            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8AllSupplier", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<Tab3ThirdSection> w = new List<Tab3ThirdSection>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new Tab3ThirdSection
                {
                    ScopeofWork = (string)myJObject3["value"][i]["ScopeofWork"],
                    ContactDetails = (string)myJObject3["value"][i]["ContactDetails"],
                    CountryofOrigin = (string)myJObject3["value"][i]["CountryofOrigin"],
                    FormRegistrationNo = (int)myJObject3["value"][i]["FormRegistrationNo"],
                    RegistrationNo = (string)myJObject3["value"][i]["RegistrationNo"],
                    Supplier = (string)myJObject3["value"][i]["Supplier"]

                });
            }
            model.tab3ThirdSection = w;
            List<FileList> AllFileList = new List<FileList>();
            AllFileList = _blobStorageService.GetBlobList(model.path);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "Signature1": model.Signature1 = AllFileList[j].FileValue; break;

                        case "WitnessSignature": model.WitnessSignature = AllFileList[j].FileValue; break;

                        case "WitnessSignature1": model.WitnessSignature1 = AllFileList[j].FileValue; break;
                        case "SummaryPage": model.SummaryPage = AllFileList[j].FileValue; break;
                        case "LetterOfContract": model.LetterOfContract = AllFileList[j].FileValue; break;
                        case "Letterindicating": model.Letterindicating = AllFileList[j].FileValue; break;
                        case "Signedletterupload": model.Signedletterupload = AllFileList[j].FileValue; break;
                    }
                }
            }
            memoryCache.Set("Form8Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm8()
        {
            // string jsond = "";
            SaveModelForm8 model = new SaveModelForm8();
            bool isExist = memoryCache.TryGetValue("Form8Data", out model);

            if (isExist)
            {
                string comment = Request.Form["comment"];
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform8", model.PartitionKey, model.RowKey, jsond);
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        //model.PartitionKey = "CO";
                        model.FormStatus = "Approve";
                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Compliance Analyst";
                        //model.PartitionKey = "CA";
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "CIC Compliance";
                        //model.PartitionKey = "OM";
                        break;
                    case "CIC Compliance":
                        model.Reviewer = "CEO";
                        model.FormStatus = "Completed";
                        string id;
                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);


                        body = "<p>Hello Team, " + model.CustNo + " have been successfully registered as a contractor for the financial year. Please release Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                        viewForm2.sendNotification("cic@jenny.co.sz", "Release Certificate", body);

                        string invoiceNo;

                        //AK - 24052023
                        //post release updates: invoice generation and update Project Details
                        Form8Helpers form8Helpers = new Form8Helpers();
                        form8Helpers.PostReleaseUpdates(model, StorageName, StorageKey);                      

                        break;
                }
                model.comment = comment;
                model.Tab3MainSection = null;
                model.tab3SecSection = null;
                model.tab3ThirdSection = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform8", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form8Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }


        public string CreateInvoiceERP(string cust, string AppNo, out string invoiceNo, string partitionKey, string FormName)
        {
            string istr = "";
            invoiceNo = "";

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
                    var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/cicSalesInvoices";
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

        public void CreateInvoiceLineItemERP(string istr, decimal TotalProjectCost)
        {

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data2 = JObject.FromObject(new
                    {
                        lineType = "Account",
                        lineObjectNumber = "1000/006",
                        description = "Construction Levy",
                        unitPrice = TotalProjectCost,
                        quantity = 1,
                        discountAmount = 0,
                        discountPercent = 0
                    });
                    var json2 = JsonConvert.SerializeObject(data2);
                    var data3 = new StringContent(json2, Encoding.UTF8, "application/json");
                    string uitemline = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/v2.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/cicSalesInvoices(" + istr + ")/cicSalesInvoiceLines";
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
        public string UpdateProjectDetails(JObject myJObject, int i, string rowkey, string invoiceNo)
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
                    completionDate = ((DateTime)myJObject["value"][i]["ProposedCompleteDate"]).ToString("yyyy-MM-dd")
                });

                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract";
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
                    string BCUrl2 = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract(" + istr + ")/pocPhysicalAddress";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PhysicalAddress"], "text/plain"));
                    t.Wait();
                }

                //updating Blob Postal Address
                using (var httpClient = new HttpClient())
                {
                    string BCUrl2 = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract(" + istr + ")/pocPostalAddress";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PostalAddress"], "text/plain"));
                    t.Wait();
                }


                return custno;
            }
            catch
            { return ""; }

        }

        static async Task<HttpResponseMessage> PatchData(Uri u, string json, string appType)
        {
            HttpClient client1 = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
            client1.DefaultRequestHeaders.Clear();
            client1.DefaultRequestHeaders.Add("If-Match", "*");
            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
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


        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            //var jsond = "";
            SaveModelForm8 model = new SaveModelForm8();
            bool isExist = memoryCache.TryGetValue("Form8Data", out model);

            if (isExist)
            {
                // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform8", model.PartitionKey, model.RowKey, jsond);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.comment = "Clerk comment - " + comment;
                        model.FormStatus = "Rejected";
                        var domain = _appSettingsReader.Read("Domain");
                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='"+ domain +"'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        view1Controller.sendNotification(model.CreatedBy, "Your Form is Rejected", body);

                        CICCommonService commService = new CICCommonService(_userManager);
                        body = "Hi " + RepresentativeName + ", Your form is rejected due to reason:" + comment + ".To access CIC portal you can login at: "+ domain +" Thank you, CIC Team";
                        commService.sendSMS(model.CreatedBy, body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Clerk";
                        model.comment = model.comment + Environment.NewLine + "Registration Officer comment - " + comment;
                        model.FormStatus = "Rejected";
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Compliance Officer";
                        model.comment = model.comment + Environment.NewLine + "Registration Analyst comment - " + comment;
                        model.FormStatus = "Rejected";
                        break;
                    case "CIC Compliance":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "CIC Compliance comment - " + comment;
                        break;
                    case "Ops Manager":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        break;
                }
                //model.Reviewer = "Contractor";
                //model.PartitionKey = "Contractor";
                //model.comment = comment;
                model.Tab3MainSection = null;
                model.tab3SecSection = null;
                model.tab3ThirdSection = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform8", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }



            memoryCache.Remove("Form8Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public void DownloadFile()
        {
            SaveModelForm8 model = new SaveModelForm8();

            bool isExist = memoryCache.TryGetValue("Form8Data", out model);
            if (isExist)
            {
                _blobStorageService.DownloadBlob(model.path);
            }




        }
        //method for preview for form8
        //method for preview for form8
        public IActionResult PreviewForm(MainViewModel model)
        {

            ////Getting Category for first tab

            //List<Category> categorylistt = new List<Category>();

            ////------- Getting data from database using EntityframeworkCore -------
            //if (model.SelectedCategory != "0")
            //{
            //    categorylistt = (from category in _context.Category where category.FormType == "F2" && category.CategoryID == Convert.ToInt32(model.SelectedCategory) select category).ToList();

            //    foreach (Category categorylist in categorylistt)
            //    {
            //        model.SelectedCategory = categorylist.CategoryName;
            //    }
            //}
            //else
            //{
            //    model.SelectedCategory = "";

            //}
            //if (model.OwnerCategory != null)
            //{
            //    //Getting  owner category 
            //    List<Category> ownercategorylist = new List<Category>();

            //    ownercategorylist = (from category in _context.Category where category.FormType == "F8" && category.CategoryID == Convert.ToInt32(model.OwnerCategory) select category).ToList();
            //    foreach (Category categorylist in ownercategorylist)
            //    {
            //        model.OwnerCategory = categorylist.CategoryName;
            //    }
            //}

            //if (model.WorkDiscipline != null)
            //{
            //    //Getting  Workdisipline category 
            //    List<Category> Workdisiplinecategorylist2 = new List<Category>();

            //    Workdisiplinecategorylist2 = (from category in _context.Category where (category.FormType == "F1" || category.FormType == "F2") && category.CategoryID == Convert.ToInt32(model.WorkDiscipline) select category).ToList();
            //    foreach (Category categorylist in Workdisiplinecategorylist2)
            //    {

            //        model.WorkDiscipline = categorylist.CategoryName;


            //    }
            //}

            if (model.selectedsubcategory != 0)
            {
                ViewBag.SubCategory = GetSubCategorybyName(model.selectedsubcategory);
            }
            return PartialView("Form8Preview", model);
        }
        public string GetSubCategorybyName(int subcategoryId)
        {
            string subName = "";
            subName = (from SubCategoryType in _context.SubCategory
                       where SubCategoryType.SubCategoryID == subcategoryId
                       select SubCategoryType.SubCategoryName).FirstOrDefault();

            return subName;
        }
    }
}
