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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm9Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;

        public ViewForm9Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult ViewForm9(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm9 model = new SaveModelForm9();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform9", rowkey, out jsonData);

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
                model.CompanyName = (string)myJObject["value"][i]["CompanyName"];
                model.InstitutionFocalPerson = (string)myJObject["value"][i]["InstitutionFocalPerson"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];               
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.RepresentativeName = (string)myJObject["value"][i]["RepresentativeName"];
                model.CompName = (string)myJObject["value"][i]["CompName"];
                model.Position = (string)myJObject["value"][i]["Position"];
                model.Place = (string)myJObject["value"][i]["Place"];
                model.Day = (int)myJObject["value"][i]["Day"];
                model.Month = (int)myJObject["value"][i]["Month"];
                model.Year = (int)myJObject["value"][i]["Year"];
                model.path = (string)myJObject["value"][i]["path"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9BuildingWorkForProject", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<BuildingWorkForProject> d = new List<BuildingWorkForProject>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new BuildingWorkForProject
                {
                    EstProjectCost = (int)myJObject1["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject1["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject1["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject1["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject1["value"][i]["ProposedCompletionDate"]
                    
                });
            }
            model.buildingWorkForProject = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9CivilsWorksProjects", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<CivilsWorksProjects> a = new List<CivilsWorksProjects>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new CivilsWorksProjects
                {
                    EstProjectCost = (int)myJObject2["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject2["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject2["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject2["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject2["value"][i]["ProposedCompletionDate"]

                });
            }
            model.civilsWorksProjects = a;

            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9MechanicalWorksProjects", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<MechanicalWorksProjects> w = new List<MechanicalWorksProjects>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new MechanicalWorksProjects
                {
                    EstProjectCost = (int)myJObject3["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject3["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject3["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject3["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject3["value"][i]["ProposedCompletionDate"]

                });
            }
            model.mechanicalWorksProjects = w;
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

                        case "PurchaseordersFile": model.PurchaseordersFileName = AllFileList[j].FileValue; break;

                        case "InvoicesFile": model.InvoicesFileName = AllFileList[j].FileValue; break;
                        case "SummarybillofquantitiesFile": model.SummarybillofquantitiesFileName = AllFileList[j].FileValue; break;                       
                    }
                }
            }
            memoryCache.Set("Form9Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm9()
        {
            //string jsond = "";
            SaveModelForm9 model = new SaveModelForm9();
            bool isExist = memoryCache.TryGetValue("Form9Data", out model);
            //string reviewer = "";

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform9", model.PartitionKey, model.RowKey, jsond);
                string comment = Request.Form["comment"];
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        //model.PartitionKey = "CO";
                        model.FormStatus = "Approve";
                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager);
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
                        model.Reviewer = "Ops Manager";
                        //model.PartitionKey = "OM";
                        break;

                    case "Ops Manager":
                        model.Reviewer = "CEO";
                        model.FormStatus = "Completed";

                        //model.PartitionKey = "CEO";

                       //AK
                        break;
                }
                model.buildingWorkForProject = null;
                model.civilsWorksProjects = null;
                model.mechanicalWorksProjects = null;
                model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform9", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form9Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
           // var jsond = "";
            //string reviewer = "";
            SaveModelForm9 model = new SaveModelForm9();
            bool isExist = memoryCache.TryGetValue("Form9Data", out model);

            if (isExist)
            {
               // var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform9", model.PartitionKey, model.RowKey, jsond);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.FormStatus = "Rejected";
                        model.comment = "Clerk comment - " + comment;

                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='https://constructioncouncil.azurewebsites.net/'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        view1Controller.sendNotification(model.CreatedBy, "Your Form is Rejected", body);

                        CICCommonService commService = new CICCommonService(_userManager);
                        body = "Hi " + RepresentativeName + ", Your form is rejected due to reason:" + comment + ".To access CIC portal you can login at: https://constructioncouncil.azurewebsites.net/ Thank you, CIC Team";
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
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        break;
                }
                
                //model.comment = comment;
                model.buildingWorkForProject = null;
                model.civilsWorksProjects = null;
                model.mechanicalWorksProjects = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform9", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            memoryCache.Remove("Form9Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        
        [HttpPost]
        public void DownloadFile()
        {
            SaveModelForm9 model = new SaveModelForm9();
            BlobStorageService objBlobService = new BlobStorageService();

            bool isExist = memoryCache.TryGetValue("Form9Data", out model);
            if (isExist)
            {
                objBlobService.DownloadBlob(model.path);
            }


        }
        //adding Preview
        public IActionResult Preview(Form9ViewModel model)
        {
           

            return PartialView("Form9Preview", model);
        }

    }
}
