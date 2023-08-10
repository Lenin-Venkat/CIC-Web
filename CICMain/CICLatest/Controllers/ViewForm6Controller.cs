using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm6Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;

        public ViewForm6Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult ViewForm6(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm6 model = new SaveModelForm6();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6", rowkey, out jsonData);

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
                model.Categories = (string)myJObject["value"][i]["Categories"];
                model.AssociationName = (string)myJObject["value"][i]["AssociationName"];
                model.AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Surname = (string)myJObject["value"][i]["Surname"];
                model.IDNo = (string)myJObject["value"][i]["IDNo"];
                model.Nationality = (string)myJObject["value"][i]["Nationality"];
                model.HomeArea = (string)myJObject["value"][i]["HomeArea"];
                model.Chief = (string)myJObject["value"][i]["Chief"];
                model.Indvuna = (string)myJObject["value"][i]["Indvuna"];
                model.TemporalResidencePermitNo = (string)myJObject["value"][i]["TemporalResidencePermitNo"];
                model.WorkPermitNo = (string)myJObject["value"][i]["WorkPermitNo"];
                model.CellphoneNo = (string)myJObject["value"][i]["CellphoneNo"];
                model.Emailaddress = (string)myJObject["value"][i]["Emailaddress"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
               // model.FaxNo = (string)myJObject["value"][i]["Email"];
                model.ResidentialAddress = (string)myJObject["value"][i]["ResidentialAddress"];
                model.NextofKin = (string)myJObject["value"][i]["NextofKin"];
                model.Relationship = (string)myJObject["value"][i]["Relationship"];
                model.ContactNo = (string)myJObject["value"][i]["ContactNo"];
                model.NatureofTradeExpertise = (string)myJObject["value"][i]["NatureofTradeExpertise"];model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.path = (string)myJObject["value"][i]["ImagePath"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedSignatureTitleDesignation = (string)myJObject["value"][i]["WitnessedSignatureTitleDesignation"];
                model.Namee = (string)myJObject["value"][i]["Namee"];
                model.TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"];
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

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6Education", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<EducationBackGroundetails> d = new List<EducationBackGroundetails>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new EducationBackGroundetails
                {
                    EducationInstitution = (string)myJObject1["value"][i]["EducationInstitution"],
                    HighestEducationLevel = (string)myJObject1["value"][i]["HighestEducationLevel"],
                    QualificationsAttained = (string)myJObject1["value"][i]["QualificationsAttained"]
                });
            }
            model.educationBackground = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6ReferenceDetails", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<BackGroundetails> a = new List<BackGroundetails>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new BackGroundetails
                {
                    NamesandSurname = (string)myJObject2["value"][i]["NamesandSurname"],
                    TypeofWorkPerformed = (string)myJObject2["value"][i]["TypeofWorkPerformed"],
                    TelephoneNo = (string)myJObject2["value"][i]["TelephoneNo"]
                });
            }
            model.backGroundetails = a;
            List<FileList> AllFileList = new List<FileList>();
            BlobStorageService b = new BlobStorageService();
            AllFileList = b.GetBlobList(model.path);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "Filee": model.FileeName = AllFileList[j].FileValue; break;

                        case "NatureofTradeUpload": model.NatureofTradeUploadName = AllFileList[j].FileValue; break;

                        case "fileupload1": model.fileupload1Name = AllFileList[j].FileValue; break;
                        case "fileupload2": model.fileupload2Name = AllFileList[j].FileValue; break;
                        case "fileupload3": model.fileupload3Name = AllFileList[j].FileValue; break;
                        case "Signature": model.SignatureName = AllFileList[j].FileValue; break;
                        case "fileupload6": model.fileupload6Name = AllFileList[j].FileValue; break;
                        
                    }
                }
            }
            memoryCache.Set("Form6Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm6()
        {
            //string jsond = "";
            SaveModelForm6 model = new SaveModelForm6();
            bool isExist = memoryCache.TryGetValue("Form6Data", out model);
            

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform6", model.PartitionKey, model.RowKey, jsond);
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

                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string invoiceno, id;
                        var fees = calculateFees(model.FormName);

                        model.AdminFee = 0;
                        model.RenewalFee = 0;
                        model.RegistrationFee = fees.RegistrationFees;
                        //AK
                        id = viewForm2.CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceno, model.PartitionKey,model.FormName);
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


                        ViewForm1Controller view1Form = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string jsonProjectData;
                        AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6", model.RowKey, out jsonProjectData);
                        JObject myJObject = JObject.Parse(jsonProjectData);
                        int cntJson = myJObject["value"].Count();

                        for (int i = 0; i < cntJson; i++)
                        {
                         model.RegistrationID = view1Form.UpdateRegistrationDetails(myJObject, i, invoiceno, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee),model.ResidentialAddress, model.Name + " " + model.Surname, Convert.ToDecimal(penalty));
                        }

                        break;
                }
                model.comment = comment;
                model.educationBackground = null;
                model.backGroundetails = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform6", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form6Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            //var jsond = "";
            //string reviewer = "";
            SaveModelForm6 model = new SaveModelForm6();
            bool isExist = memoryCache.TryGetValue("Form6Data", out model);

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform6", model.PartitionKey, model.RowKey, jsond);

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
                model.educationBackground = null;
                model.backGroundetails = null;
                //model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform6", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            

            memoryCache.Remove("Form6Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public CICFees calculateFees(string formname)
        {
            var feelist = (from item in _context.cicFees
                           where item.FormName == formname 
                           select item).FirstOrDefault();

            return feelist;

        }

        [HttpPost]
        public void DownloadFile()
        {
            SaveModelForm6 model = new SaveModelForm6();           
            BlobStorageService objBlobService = new BlobStorageService();

            bool isExist = memoryCache.TryGetValue("Form6Data", out model);
            if (isExist)
            {
                objBlobService.DownloadBlob(model.path);
            }


        }
        //adding for preview
        public IActionResult PreviewForm6(Cicf6Model model)
        {

            return PartialView("Form6Preview", model);
        }
    }
}
