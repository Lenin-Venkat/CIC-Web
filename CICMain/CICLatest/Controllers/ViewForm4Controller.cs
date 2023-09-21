using CICLatest.Contracts;
using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm4Controller : Controller
    {
        private readonly ApplicationContext _context;
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly UserManager<UserModel> _userManager;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;


        public ViewForm4Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context
            , UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            this.memoryCache = memoryCache;
            _context = context;
            _azureConfig = azureConfig;
            _userManager = userManager;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }

        public IActionResult ViewForm4(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            Form4Model model = new Form4Model();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform4", rowkey, out jsonData);

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
                model.AssociationName = (string)myJObject["value"][i]["AssociationName"];
                model.AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"];
                model.BusinessName = (string)myJObject["value"][i]["BusinessName"];
                model.TradingStyle = (string)myJObject["value"][i]["TradingStyle"];
                model.BusinessType = (string)myJObject["value"][i]["BusinessType"];
                model.CompanyRegistrationDate = (DateTime)myJObject["value"][i]["CompanyRegistrationDate"];
                model.CompanyRegistrationPlace = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.CompanyRegistrationNumber = (string)myJObject["value"][i]["CompanyRegistrationNumber"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.subCategoryName = (int)myJObject["value"][i]["subCategoryName"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                //add level of registration
               model.subCategoryName = (int)myJObject["value"][i]["subCategoryName"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"];
                model.Title = (string)myJObject["value"][i]["Title"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
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

            if(model.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            if (model.subCategoryName != 0)
            {
                ViewBag.SubCategory = GetSubCategorybyName(model.subCategoryName);
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<DirectorshipShareDividends4> d = new List<DirectorshipShareDividends4>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DirectorshipShareDividends4
                {
                    DirectorName = (string)myJObject1["value"][i]["DirectorName"],
                    IdNO = (string)myJObject1["value"][i]["IdNO"],
                    Nationnality = (string)myJObject1["value"][i]["Nationnality"],
                    CellphoneNo = (string)myJObject1["value"][i]["CellphoneNo"],
                    Country = (string)myJObject1["value"][i]["Country"],
                    Qualifications = (string)myJObject1["value"][i]["Qualifications"],
                    SharePercent = (int)myJObject1["value"][i]["SharePercent"]

                });
            }
            model.Sharelist = d;
            List<FileList> AllFileList = new List<FileList>();
            AllFileList = _blobStorageService.GetBlobList(model.path);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "Filesignature": model.signature = AllFileList[j].FileValue; break;

                        case "Businesssignature": model.BusinessRepresentativeSign = AllFileList[j].FileValue; break;                        
                        case "BusinessFile1": model.BusinessFile1Name = AllFileList[j].FileValue; break;
                        case "BusinessFile2": model.BusinessFile2Name = AllFileList[j].FileValue; break;
                        case "BusinessFile3": model.BusinessFile3Name = AllFileList[j].FileValue; break;
                        case "BusinessFile4": model.BusinessFile4Name = AllFileList[j].FileValue; break;
                        case "BusinessFile5": model.BusinessFile5Name = AllFileList[j].FileValue; break;
                        case "BusinessFile6": model.BusinessFile6Name = AllFileList[j].FileValue; break;
                        case "BusinessFile7": model.BusinessFile7Name = AllFileList[j].FileValue; break;
                        case "ShareholdersFile1": model.ShareholdersFile1Name = AllFileList[j].FileValue; break;
                        case "Signature1": model.sign1Name = AllFileList[j].FileValue; break;
                        case "Signature2": model.sign2Name = AllFileList[j].FileValue; break;
                        case "TaxLaw": model.taxLawName = AllFileList[j].FileValue; break;
                        case "Evidence": model.EvidenceName = AllFileList[j].FileValue; break;
                        case "Compliance": model.ComplienceName = AllFileList[j].FileValue; break;
                    }
                }
            }
            memoryCache.Set("Form4Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm4()
        {
            //string jsond = "";
            Form4Model model = new Form4Model();
            bool isExist = memoryCache.TryGetValue("Form4Data", out model);
            
            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform4", model.PartitionKey, model.RowKey, jsond);
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

                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        string t = "", invoiceno, id;
                        if(model.BusinessType == "ForeignCompany")
                        {
                            t = "Foreign";
                        }
                        else
                        {
                            t = "Local";
                        }

                        var fees = calculateFees(model.FormName, t);

                        model.AdminFee = fees.AdminFees;
                        model.RenewalFee = fees.RenewalFees;

                        if (model.AppType != "Renewal")
                        {
                            model.RegistrationFee = fees.RegistrationFees;
                        }
                        //AK
                        id = viewForm2.CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceno,model.PartitionKey,  model.FormName);
                        model.InvoiceNo = invoiceno;
                        //viewForm2.CreateInvoiceLineItemERP(id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee));

                        string jsonData;
                        int penalty;

                        AzureTablesData.GetAllEntity(StorageName, StorageKey, "GracePeriodDetails", out jsonData);//Get data
                        JObject gracePeriodObject = JObject.Parse(jsonData);

                        DateTime allowedGracePeriod = (DateTime)gracePeriodObject["value"][0]["allowedDate"];

                        if (allowedGracePeriod < DateTime.Now && model.AppType == "Renewal")
                        {
                            penalty = (model.RenewalFee * 10) / 100;

                        }
                        else
                        {
                            penalty = 0;
                        }

                        viewForm2.CreateInvoiceLineItemERP(id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), Convert.ToDecimal(penalty));


                        ViewForm1Controller view1Form = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        string jsonProjectData;
                        AzureTablesData.GetEntity(StorageName, StorageKey, "cicform4", model.RowKey, out jsonProjectData);
                        JObject myJObject = JObject.Parse(jsonProjectData);
                        int cntJson = myJObject["value"].Count();

                        for (int i = 0; i < cntJson; i++)
                        {
                            string subCategoryName = GetSubCategorybyName(model.subCategoryName);
                            model.RegistrationID = view1Form.UpdateRegistrationDetails(myJObject, i, invoiceno, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), model.PostalAddress,model.TradingStyle, Convert.ToDecimal(penalty),model.AppType, subCategoryName);
                        }
                        break;
                }
                model.Sharelist = null;
                model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform4", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form4Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            //var jsond = "";
            //string reviewer = "";
            Form4Model model = new Form4Model();
            bool isExist = memoryCache.TryGetValue("Form4Data", out model);

            if (isExist)
            {
               // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform4", model.PartitionKey, model.RowKey, jsond);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.FormStatus = "Rejected";
                        model.comment = "Clerk comment - " + comment;
                        var domain = _appSettingsReader.Read("Domain");
                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='"+ domain +"'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
                        view1Controller.sendNotification(model.CreatedBy, "Your Form is Rejected", body);

                        CICCommonService commService = new CICCommonService(_userManager);
                        body = "Hi " + RepresentativeName + ", Your form is rejected due to reason:" + comment + ".To access CIC portal you can login at: " + domain + " Thank you, CIC Team";
                        commService.sendSMS(model.CreatedBy, body);

                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Clerk";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Registration Officer comment - " + comment;
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Compliance Officer";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Registration Analyst comment - " + comment;
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        break;
                }
                model.Sharelist = null;
                //model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform4", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            
            memoryCache.Remove("Form4Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        

        [HttpPost]
        public void DownloadFile()
        {
            Form1Model model = new Form1Model();
            bool isExist = memoryCache.TryGetValue("Form4Data", out model);
            if (isExist)
            {
                _blobStorageService.DownloadBlob(model.path);
            }


        }

        [HttpPost]
        public IActionResult Preview(CICForm4Model model)  //model plan
        {
            if(model.businessModel.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            if(model.businessModel.selectedsubcategory !=0)
            {
                ViewBag.SubCategory = GetSubCategorybyName(model.businessModel.selectedsubcategory);
            }
            
            return PartialView("Form4Preview", model);
        }

        public string GetSubCategorybyName(int subcategoryId)
        {            
            string subName = "";
            subName = (from SubCategoryType in _context.SubCategory
                       where SubCategoryType.SubCategoryID == subcategoryId
                       select SubCategoryType.SubCategoryName).FirstOrDefault();

            return subName;
        }

        public string GetCategorybyName(int categoryId)
        {
            string Name = "";
            Name = (from categoryType in _context.Category
                       where categoryType.CategoryID == categoryId
                       select categoryType.CategoryName).FirstOrDefault();

            return Name;
        }

        public CICFees calculateFees(string formname, string grade)
        {
            //int fees = 0;

            var feelist = (from item in _context.cicFees
                           where item.FormName == formname & item.Grade == grade
                           select item).FirstOrDefault();

            return feelist;

        }

    }
}
