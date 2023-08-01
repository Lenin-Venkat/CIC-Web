using CICLatest.Helper;
using CICLatest.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    public class ViewForm3Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly UserManager<UserModel> _userManager;
        public static string accessToken;

        public ViewForm3Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult ViewForm3(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            Form3Model model = new Form3Model();

            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform3", rowkey, out jsonData);

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
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.Fax = (string)myJObject["value"][i]["Fax"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Phyaddress = (string)myJObject["value"][i]["Phyaddress"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.NameType = (string)myJObject["value"][i]["NameType"];
                model.SurName = (string)myJObject["value"][i]["SurName"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.CategoryId = (int)myJObject["value"][i]["CategoryId"];
                model.Subcatogory = (int)myJObject["value"][i]["Subcatogory"];
                model.SubcatogoryId = (string)myJObject["value"][i]["SubcatogoryId"];
                model.NameofApplicant = (string)myJObject["value"][i]["NameofApplicant"];
                model.CountryOfOrigin = (string)myJObject["value"][i]["CountryOfOrigin"];
                model.ContactDetails = (string)myJObject["value"][i]["ContactDetails"];
                model.CICRegistrationNo = (string)myJObject["value"][i]["CICRegistrationNo"];
                model.Shareholding = (int)myJObject["value"][i]["Shareholding"];
                model.NameofContractor = (string)myJObject["value"][i]["NameofContractor"];
                model.CountryyofOrigin = (string)myJObject["value"][i]["CountryyofOrigin"];
                model.CICRegistrationNoConsultant = (string)myJObject["value"][i]["CICRegistrationNoConsultant"];
                model.DescriptionOfWork = (string)myJObject["value"][i]["DescriptionOfWork"];
                model.ContractValueOfWork = (decimal)myJObject["value"][i]["ContractValueOfWork"];
                model.customerCheck = (int)myJObject["value"][i]["customerCheck"];
                model.BidReferenceNo = (string)myJObject["value"][i]["BidReferenceNo"];
                model.ProjectTitle = (string)myJObject["value"][i]["ProjectTitle"];
                model.DateofAward = (DateTime)myJObject["value"][i]["DateofAward"];
                model.CommencementDate = (DateTime)myJObject["value"][i]["CommencementDate"];
                model.CompletionDate = (DateTime)myJObject["value"][i]["CompletionDate"];
                model.DescriptionofProject = (string)myJObject["value"][i]["DescriptionofProject"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitleDesignation = (string)myJObject["value"][i]["WitnessedTitleDesignation"];
                model.ClientName = (string)myJObject["value"][i]["ClientName"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.ContractValue = (decimal)myJObject["value"][i]["ContractValue"];
                model.TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.JVGrade = (string)myJObject["value"][i]["JVGrade"];
                // model.OldRegNo = (string)myJObject["value"][i]["OldRegNo"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];

            }

            string catname = "";
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context, _userManager);
            if (model.Subcatogory != 0)
            {
                ViewBag.SubCategory = vf4.GetSubCategorybyName(model.Subcatogory);
            }
            if (model.CategoryId != 0)
            {
                ViewBag.Category = vf4.GetCategorybyName(model.CategoryId);
                catname = vf4.GetCategorybyName(model.CategoryId);
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<ParticularsofJointVentureParties> d = new List<ParticularsofJointVentureParties>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new ParticularsofJointVentureParties
                {
                    NameofApplicant = (string)myJObject1["value"][i]["NameofApplicant"],
                    CountryOfOrigin = (string)myJObject1["value"][i]["CountryOfOrigin"],
                    ContactDetails = (string)myJObject1["value"][i]["ContactDetails"],
                    CICRegistrationNo = (string)myJObject1["value"][i]["CICRegistrationNo"],
                    Shareholding = (int)myJObject1["value"][i]["Shareholding"]
                });
            }
            model.JointVenturePartiesModel = d;

            string jsonForm1Data;
            AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonForm1Data);

            JObject businessesObject = JObject.Parse(jsonForm1Data);

            var businesses = businessesObject["value"].ToList();
            var selectedBusinesses = model.JointVenturePartiesModel.Select(x => x.NameofApplicant);

            var matchedBusinesses = businesses.Where(x => selectedBusinesses.Contains((string)x["BusinessName"])).ToList();

            List<string> selectedCompanyGrades = new List<string>();

            for (int i = 0; i < selectedBusinesses.Count(); i++)
            {
                var latestRecord = (from rec in businessesObject["value"]
                                    where (string)rec["BusinessName"] == selectedBusinesses.ToList()[i]
                                    orderby (DateTime)rec["Timestamp"] descending
                                    select rec).FirstOrDefault();

                if (latestRecord != null)
                {
                    selectedCompanyGrades.Add((string)latestRecord["Grade"]);
                }
            }

            List<string> cicForm3Grades = GetJointVentureGrade(model.CategoryId);

            foreach (string grade in cicForm3Grades)
            {
                if (selectedCompanyGrades.Contains(grade))
                {
                    ViewBag.SelectedGrade = grade;
                    break;
                }
            }

            //Technical staff
            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3TechnicalandAdministrativeStaff", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<TechnicalAdministrativeStaff> t = new List<TechnicalAdministrativeStaff>();
            for (int i = 0; i < cntJson2; i++)
            {

                t.Add(new TechnicalAdministrativeStaff
                {
                    Category = (string)myJObject2["value"][i]["Category"],
                    Number = (int)myJObject2["value"][i]["Number"],
                    YearsofExperience = (int)myJObject2["value"][i]["YearsofExperience"],

                });
            }
            model.TechnicalAdministrativeStaffModel = t;

            //Project Staff
            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3ProjectStaff", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<ProjectStaff> p = new List<ProjectStaff>();
            for (int i = 0; i < cntJson3; i++)
            {

                p.Add(new ProjectStaff
                {
                    StaffName = (string)myJObject3["value"][i]["StaffName"],
                    StaffPosition = (string)myJObject3["value"][i]["StaffPosition"],
                    StaffQualification = (string)myJObject3["value"][i]["StaffQualification"],
                    StaffExperience = (int)myJObject3["value"][i]["StaffExperience"],
                    StaffNationality = (string)myJObject3["value"][i]["StaffNationality"],
                    IdNO = (string)myJObject3["value"][i]["IdNO"],
                    StaffActivity = (string)myJObject3["value"][i]["StaffActivity"]
                });
            }
            model.projectStaffModel = p;

            //Labour force
            string jsonData4;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3LabourForce", rowkey, out jsonData4);
            JObject myJObject4 = JObject.Parse(jsonData4);
            int cntJson4 = myJObject4["value"].Count();
            List<LabourForce> l = new List<LabourForce>();
            for (int i = 0; i < cntJson4; i++)
            {

                l.Add(new LabourForce
                {
                    Gender = (string)myJObject4["value"][i]["Gender"],
                    Swazi1 = (int)myJObject4["value"][i]["Swazi1"],
                    Foreign1 = (int)myJObject4["value"][i]["Foreign1"],
                    Swazi2 = (int)myJObject4["value"][i]["Swazi2"],
                    Foreign2 = (int)myJObject4["value"][i]["Foreign2"],
                    Total = (int)myJObject4["value"][i]["Total"]

                });
            }
            model.LabourForceModel = l;


            //
            string jsonData5;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", rowkey, out jsonData5);
            JObject myJObject5 = JObject.Parse(jsonData5);
            int cntJson5 = myJObject5["value"].Count();
            List<SubContractors> s = new List<SubContractors>();
            for (int i = 0; i < cntJson5; i++)
            {

                s.Add(new SubContractors
                {
                    NameofContractor = (string)myJObject5["value"][i]["NameofContractor"],
                    CountryyofOrigin = (string)myJObject5["value"][i]["CountryyofOrigin"],
                    CICRegistrationNo = (string)myJObject5["value"][i]["CICRegistrationNo"],
                    DescriptionOfWork = (string)myJObject5["value"][i]["DescriptionOfWork"],
                    ContractValueOfWork = (decimal)myJObject5["value"][i]["ContractValueOfWork"]
                });
            }
            model.SubContractorModel = s;


            List<FileList> AllFileList = new List<FileList>();
            BlobStorageService b = new BlobStorageService();
            AllFileList = b.GetBlobList(model.ImagePath);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {

                    switch (AllFileList[j].FileKey)
                    {
                        case "BusineesParticularsfile1": model.BusineesParticularsfile1Name = AllFileList[j].FileValue; break;
                        case "BusineesParticularsfile2": model.BusineesParticularsfile2Name = AllFileList[j].FileValue; break;
                        case "Signature": model.SignatureName = AllFileList[j].FileValue; break;
                        case "Signature1": model.Signature2Name = AllFileList[j].FileValue; break;
                    }
                }
            }


            bool SFlag = ShareValidation(model.JointVenturePartiesModel);
            string gradetype = "";
            if (model.TypeofJoointVenture == "foreign" || SFlag == true)
            {
                if (catname.Contains("General Building") == true)
                {
                    gradetype = "JvBF";
                }
                else if (catname.Contains("General Civil") == true)
                {
                    gradetype = "JvCF";
                }
                else if (catname.Contains("General Electrical") == true)
                {
                    gradetype = "JvEF";
                }
                else if (catname.Contains("General Mechanical") == true)
                {
                    gradetype = "JvMF";
                }
                else if (catname.Contains("Mechanical Specialist") == true)
                {
                    gradetype = "JvMSF";
                }
                else if (catname.Contains("Civils Specialist") == true)
                {
                    gradetype = "JvCSF";
                }
                else if (catname.Contains("Electrical Specialist") == true)
                {
                    gradetype = "JvESF";
                }
                else if (catname.Contains("Buildings Specialist") == true)
                {
                    gradetype = "JvBSF";
                }
            }
            else
            {
                gradetype = "Swazi";
            }
            var fees = calculateFees(model.FormName, gradetype);

            model.AdminFee = fees.AdminFees;
            model.RenewalFee = fees.RenewalFees;
            model.RegistrationFee = fees.RegistrationFees;

            memoryCache.Set("Form3Data", model);
            if (model.Reviewer != "Clerk")
            {
                ViewBag.grade = "ReadOnly";
            }
            else
            {
                ViewBag.grade = "";
            }
            return View(model);
        }

        public CICFees calculateFees(string formname, string grade)
        {
            //int fees = 0;

            var feelist = (from item in _context.cicFees
                           where item.FormName == formname & item.Grade == grade
                           select item).FirstOrDefault();

            return feelist;

        }

        public bool ShareValidation(List<ParticularsofJointVentureParties> p)
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
        public IActionResult ViewForm3(Form3Model m)
        {
            Form3Model model = new Form3Model();
            bool isExist = memoryCache.TryGetValue("Form3Data", out model);
            string g = Request.Form["JVGrade"];
            if (g == "")
            {
                ModelState.AddModelError("JVGrade", "Please enter Grade");
                m = model;
                model.JVGrade = "";
                if (model.Reviewer != "Clerk")
                {
                    ViewBag.grade = "ReadOnly";
                }
                else
                {
                    ViewBag.grade = "";
                }
                return View(m);
            }

            if (isExist)
            {
                string comment = Request.Form["comment"];
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        model.FormStatus = "Approve";
                        model.JVGrade = g;
                        //ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        //string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        ////viewForm1.sendNotification(model.BusinessEmail, "Request for invoice", body);
                        //viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        //viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);

                        //body = "<p>Hi " + model.BusinessEmail + ",<br/>Form " + model.RowKey + " is reviewed and you are applicable for grade " + g + "<br/>Thank you,<br/>CIC Team</p>";
                        //viewForm1.sendNotification(model.CreatedBy, "CIC registration has been approved", body);
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

                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved. :</br>Comment:" + comment + "</br></br> Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        //viewForm1.sendNotification(model.BusinessEmail, "Request for invoice", body);
                        viewForm1.sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        viewForm1.sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);

                        body = "<p>Hi " + model.BusinessEmail + ",<br/>Form " + model.RowKey + " is reviewed and you are applicable for grade " + g + "<br/>Thank you,<br/>CIC Team</p>";
                        viewForm1.sendNotification(model.CreatedBy, "CIC registration has been approved", body);


                        ViewForm1Controller viewForm2 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
                        string invoiceNo;
                        //AK
                        string id = viewForm2.CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceNo, model.PartitionKey, model.FormName);
                        model.InvoiceNo = invoiceNo;
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
                        string jsonRegistrationDetails;
                        AzureTablesData.GetEntity(StorageName, StorageKey, "cicform3", model.RowKey, out jsonRegistrationDetails);
                        JObject myJObject = JObject.Parse(jsonRegistrationDetails);
                        int cntJson = myJObject["value"].Count();

                        accessToken = view1Form.GetAccessToken();
                        for (int i = 0; i < cntJson; i++)
                        {
                            UpdateRegistrationDetails(myJObject, i, invoiceNo, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), model.Phyaddress);
                        }

                        break;
                }
                model.JointVenturePartiesModel = null;
                model.TechnicalAdministrativeStaffModel = null;
                model.projectStaffModel = null;
                model.LabourForceModel = null;
                model.SubContractorModel = null;
                model.comment = comment;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform3", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsond = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form3Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {

            Form3Model model = new Form3Model();
            bool isExist = memoryCache.TryGetValue("Form3Data", out model);

            if (isExist)
            {
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
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Registration Analyst comment - " + comment;
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Rejected";
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;
                        break;
                }
                model.JointVenturePartiesModel = null;
                model.TechnicalAdministrativeStaffModel = null;
                model.projectStaffModel = null;
                model.LabourForceModel = null;
                model.SubContractorModel = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform3", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }


            memoryCache.Remove("Form3Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult Preview(CICForm3Model model)  //model plan
        {
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context, _userManager);
            if (model.businessModel.selectedsubcategory != 0)
            {
                ViewBag.SubCategory = vf4.GetSubCategorybyName(model.businessModel.selectedsubcategory);
            }
            if (model.businessModel.SelectedCategoryValue != 0)
            {
                ViewBag.Category = vf4.GetCategorybyName(model.businessModel.SelectedCategoryValue);
            }
            return PartialView("Form3Preview", model);
        }

        private List<string> GetJointVentureGrade(int categoryId)
        {
            var gradesList = new List<string>();

            switch (categoryId)
            {
                case 1:
                    gradesList = new List<string>() { "B1", "B2", "B3", "B4", "B5", "B6" };
                    break;
                case 2:
                    gradesList = new List<string>() { "C1", "C2", "C3", "C4", "C5", "C6" };
                    break;
                case 7:
                    gradesList = new List<string>() { "E1", "E2", "E3", "E4", "E5", "E6" };
                    break;
                case 8:
                    gradesList = new List<string>() { "M1", "M2", "M3", "M4", "M5", "M6" };
                    break;
                default:
                    break;
            }

            return gradesList;
        }

        public string UpdateRegistrationDetails(JObject myJObject, int i, string invoiceNo, decimal registratinFee, decimal adminFee, decimal reFee, string postalAddress)
        {
            string custno = (string)myJObject["value"][i]["CustNo"];
            DateTime createdDate = DateTime.Now;
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
                    businessName = (string)myJObject["value"][i]["BusinessName"],
                    certificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    registration = registratinFee,
                    renewal = reFee,
                    adminFee = adminFee,
                    penalty = 0,
                    credit = 0,
                    owing = 0,
                    total = 0,
                    dateofPay = "2022-07-19",
                    typeofPay = "EFT",
                    bank = (string)myJObject["value"][i]["BankName"],
                    category = (string)myJObject["value"][i]["Category"],
                    monthofReg = createdDate.ToString("MMMM"),
                    grade = (string)myJObject["value"][i]["JVGrade"],
                    postalAddress = postalAddress
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

                    }
                }
                return custno;
            }
            catch
            { return ""; }

        }
    }
}
