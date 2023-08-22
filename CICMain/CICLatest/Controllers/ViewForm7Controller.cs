using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm7Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;

        public ViewForm7Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult ViewForm7(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveForm7Model model = new SaveForm7Model();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform7", rowkey, out jsonData);

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
                //model.re = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.WorkDisciplineType = (string)myJObject["value"][i]["WorkDisciplineType"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                model.StateDetailsOfMaterials = (string)myJObject["value"][i]["StateDetailsOfMaterials"];
                model.StateOfAttainent = (string)myJObject["value"][i]["StateOfAttainent"];
                model.AnnualTurnoverYear1 = (decimal)myJObject["value"][i]["AnnualTurnoverYear1"];
                model.AnnualTurnoverYear2 = (decimal)myJObject["value"][i]["AnnualTurnoverYear2"];
                model.AnnualTurnoverYear3 = (decimal)myJObject["value"][i]["AnnualTurnoverYear3"];                
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
                model.ReceiptNo = (string)myJObject["value"][i]["ReceiptNo"];
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


            if (model.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<DirectorshipShareDividendsSection> d = new List<DirectorshipShareDividendsSection>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DirectorshipShareDividendsSection
                {
                    DirectorName = (string)myJObject1["value"][i]["DirectorName"],
                    IdNO = (string)myJObject1["value"][i]["IdNO"],
                    Nationnality = (string)myJObject1["value"][i]["Nationnality"],
                    CellphoneNo = (string)myJObject1["value"][i]["CellphoneNo"],
                    Country = (string)myJObject1["value"][i]["Country"],
                    //ShareFile = 
                    SharePercent = (int)myJObject1["value"][i]["SharePercent"]

                });
            }
            model.Sharelist = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7ApplicationBankDetails", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<CompanyBank> a = new List<CompanyBank>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new CompanyBank
                {
                    AccountHoulderName = (string)myJObject2["value"][i]["AccountHoulderName"],
                    AccountNo = (int)myJObject2["value"][i]["AccountNo"],
                    AccountTYpe = (string)myJObject2["value"][i]["AccountTYpe"],
                    BankName = (string)myJObject2["value"][i]["BankName"],
                    BranchCode = (string)myJObject2["value"][i]["BranchCode"],
                    BranchName = (string)myJObject2["value"][i]["BranchName"],
                    TelephoneNo = (string)myJObject2["value"][i]["TelephoneNo"]

                });
            }
            model.companyBank = a;

            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform7ListOfPreviousProject", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<ListOfPreviousClient> w = new List<ListOfPreviousClient>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new ListOfPreviousClient
                {
                    ContractValue = (decimal)myJObject3["value"][i]["ContractValue"],
                    DateOrPeriod = (DateTime)myJObject3["value"][i]["DateOrPeriod"],
                    NameofClient = (string)myJObject3["value"][i]["NameofClient"],
                    ServiceProvided = (string)myJObject3["value"][i]["ServiceProvided"]                    

                });
            }
            model.listOfPrevousClent = w;
            List<FileList> AllFileList = new List<FileList>();
            BlobStorageService b = new BlobStorageService();
            AllFileList = b.GetBlobList(model.path);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "Signature": model.SignatureName = AllFileList[j].FileValue; break;

                        case "Signature2": model.Signature2Name = AllFileList[j].FileValue; break;

                        case "AssociationCertificateAttachment": model.AssociationCertificateAttachmentName = AllFileList[j].FileValue; break;
                        case "Signature3": model.Signature3Name = AllFileList[j].FileValue; break;
                        case "companyRegistration": model.companyRegistrationName = AllFileList[j].FileValue; break;
                        case "shraeCertificate": model.shraeCertificateName = AllFileList[j].FileValue; break;
                        case "TradingLicense": model.TradingLicenseName = AllFileList[j].FileValue; break;
                        case "formj": model.formjName = AllFileList[j].FileValue; break;
                        case "FormC": model.FormCName = AllFileList[j].FileValue; break;
                        case "RegistrationCertificate": model.RegistrationCertificateName = AllFileList[j].FileValue; break;
                        case "CertifiedCompanyRegistration": model.CertifiedCompanyRegistrationName = AllFileList[j].FileValue; break;
                        case "CertifiedIdentityDocuments": model.CertifiedIdentityDocumentsName = AllFileList[j].FileValue; break;
                        case "RenewBusinessScopeOfwork": model.RenewBusinessScopeOfworkName = AllFileList[j].FileValue; break;
                        case "RenewCertificate": model.RenewCertificateName = AllFileList[j].FileValue; break;
                        case "RenewFormJ": model.RenewFormJName = AllFileList[j].FileValue; break;
                        case "ReNewFormC": model.ReNewFormCName = AllFileList[j].FileValue; break;
                        case "RenewFormBMCA": model.RenewFormBMCAName = AllFileList[j].FileValue; break;
                        case "RenewFinancialRequirment": model.RenewFinancialRequirmentName = AllFileList[j].FileValue; break;
                        case "RenewIdentification": model.RenewIdentificationName = AllFileList[j].FileValue; break;                        
                    }
                }
            }
            memoryCache.Set("Form7Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm7()
        {
           // string jsond = "";
            SaveForm7Model model = new SaveForm7Model();
            bool isExist = memoryCache.TryGetValue("Form7Data", out model);            

            if (isExist)
            {
                // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform7", model.PartitionKey, model.RowKey, jsond);
                string comment = Request.Form["comment"];
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        //model.PartitionKey = "CO";
                        model.FormStatus = "Approve";
                        //ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager);
                        //string body = "<p>Hi Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        //viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Compliance Analyst";
                        //model.PartitionKey = "CA";
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Ops Manager";
                        //model.PartitionKey = "OM";
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Ops Manager";
                        model.FormStatus = "Completed";

                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string body = "<p>Hi Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);

                        CICFees fees=null;
                        string id, invoiceno;
                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);

                        if (model.BusinessType == "ForeignCompany")
                        {
                             fees = calculateFees(model.FormName, model.AnnualTurnoverYear1,model.AnnualTurnoverYear2,model.AnnualTurnoverYear3, "Foreign");
                        }
                        else
                        {
                            fees = calculateFees(model.FormName, model.AnnualTurnoverYear1, model.AnnualTurnoverYear2, model.AnnualTurnoverYear3, "Local");
                        }

                        model.AdminFee = fees.AdminFees;
                        model.RenewalFee = fees.RenewalFees;
                        if (model.AppType != "Renewal")
                        {
                            model.RegistrationFee = fees.RegistrationFees;
                        }

                       // AK
                        id = viewForm2.CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceno, model.PartitionKey,model.FormName);
                        model.InvoiceNo = invoiceno;
                        // viewForm2.CreateInvoiceLineItemERP(id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee));

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


                        ViewForm1Controller view1Form = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string jsonProjectData;
                        AzureTablesData.GetEntity(StorageName, StorageKey, "cicform7", model.RowKey, out jsonProjectData);
                        JObject myJObject = JObject.Parse(jsonProjectData);
                        int cntJson = myJObject["value"].Count();

                        for (int i = 0; i < cntJson; i++)
                        {
                          model.RegistrationID = view1Form.UpdateRegistrationDetails(myJObject, i, invoiceno, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee),model.PostalAddress,model.TradingStyle, Convert.ToDecimal(penalty), model.AppType);
                        }

                        break;
                }
                model.comment = comment;
                model.Sharelist = null;
                model.companyBank = null;
                model.listOfPrevousClent = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform7", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form7Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public CICFees calculateFees(string formname, decimal AnnualTurnoverYear1, decimal AnnualTurnoverYear2, decimal AnnualTurnoverYear3, string btype)
        {
            string grade = "";

            decimal BestAnnualTurnover = 0;
            string cat = "";
            BestAnnualTurnover = ((AnnualTurnoverYear1 > AnnualTurnoverYear2 && AnnualTurnoverYear1 > AnnualTurnoverYear3) ? AnnualTurnoverYear1 : (AnnualTurnoverYear2 > AnnualTurnoverYear3) ? AnnualTurnoverYear2 : AnnualTurnoverYear3);

            if (1 <= BestAnnualTurnover && BestAnnualTurnover <= 500000)
            {
                cat = "7" +btype;
            }
            else if (500001 <= BestAnnualTurnover && BestAnnualTurnover <= 1000000)
            {
                cat = "6" + btype;
            }
            else if (1000001 <= BestAnnualTurnover && BestAnnualTurnover <= 2500000)
            {
                cat = "5" + btype;
            }
            else if (2500001 <= BestAnnualTurnover && BestAnnualTurnover <= 5000000)
            {
                cat = "4" + btype;
            }
            else if (5000001 <= BestAnnualTurnover && BestAnnualTurnover <= 7500000)
            {
                cat = "3" + btype;
            }
            else if (7500001 <= BestAnnualTurnover && BestAnnualTurnover <= 10000000)
            {
                cat = "2" + btype;
            }
            else if (10000001 <= BestAnnualTurnover && BestAnnualTurnover <= long.MaxValue)
            {
                cat = "1" + btype;
            }

            var feelist = (from item in _context.cicFees
                           where item.FormName == formname & item.Grade == cat
                           select item).FirstOrDefault();

            return feelist;

        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            //var jsond = "";
            SaveForm7Model model = new SaveForm7Model();
            bool isExist = memoryCache.TryGetValue("Form7Data", out model);

            if (isExist)
            {
               // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform7", model.PartitionKey, model.RowKey, jsond);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.comment = "Clerk comment - " + comment;
                        model.FormStatus = "Rejected";

                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='https://constructioncouncil.azurewebsites.net/'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        view1Controller.sendNotification(model.CreatedBy, "Your Form is Rejected", body);

                        CICCommonService commService = new CICCommonService(_userManager);
                        body = "Hi " + RepresentativeName + ", Your form is rejected due to reason:" + comment + ".To access CIC portal you can login at: https://constructioncouncil.azurewebsites.net/ Thank you, CIC Team";
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

                    case "Ops Manager":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        break;
                }
                //model.Reviewer = "Contractor";
                //model.PartitionKey = "Contractor";
                //model.comment = comment;
                model.Sharelist = null;
                model.companyBank = null;
                model.listOfPrevousClent = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform7", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            

            memoryCache.Remove("Form7Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public void DownloadFile()
        {
            SaveForm7Model model = new SaveForm7Model();
            BlobStorageService objBlobService = new BlobStorageService();

            bool isExist = memoryCache.TryGetValue("Form7Data", out model);
            if (isExist)
            {
                objBlobService.DownloadBlob(model.path);
            }


            

        }
        //function for Preview
        public IActionResult Preview(Cicf7Model model)
        {

            if (model.businessModel.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }


            return PartialView("Form7Preview", model);
        }
    }
}
