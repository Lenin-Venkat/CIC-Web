using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using CICLatest.Helper;
using CICLatest.MappingConfigurations;
using CICLatest.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace CICLatest.Controllers
{
    [Authorize]
    public class ViewForm1Controller : Controller
    {
        static string StorageName = "";//"cicdatastorage";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly ApplicationContext _context;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly UserManager<UserModel> _userManager;
        public static string accessToken;

        public ViewForm1Controller(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult ViewForm(string rowkey)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            Form1Model model = new Form1Model();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1", rowkey, out jsonData);

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
                DateTime CDate =(DateTime)(myJObject["value"][i]["CompanyRegistrationDate"]);
                string CDatestr = CDate.ToShortDateString();
                model.cdate = CDatestr;
                model.CompanyRegistrationDate = (DateTime)(myJObject["value"][i]["CompanyRegistrationDate"]);
                model.CompanyRegistrationPlace = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.CompanyRegistrationNumber = (string)myJObject["value"][i]["CompanyRegistrationNumber"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.PresentGrade = (string)myJObject["value"][i]["PresentGrade"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                model.AnnualTurnoverYear1 = (decimal)myJObject["value"][i]["AnnualTurnoverYear1"];
                model.AnnualTurnoverYear2 = (decimal)myJObject["value"][i]["AnnualTurnoverYear2"];
                model.AnnualTurnoverYear3 = (decimal)myJObject["value"][i]["AnnualTurnoverYear3"];
                model.FinancialValue = (decimal)myJObject["value"][i]["FinancialValue"];
                model.FinancialInstitutionName = (string)myJObject["value"][i]["FinancialInstitutionName"];
                model.AvailableCapital = (decimal)myJObject["value"][i]["AvailableCapital"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"];
                model.Title = (string)myJObject["value"][i]["Title"];
                model.FirmRegistrationNo = (int)myJObject["value"][i]["FirmRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.ElectricalSubCategory = (int)myJObject["value"][i]["ElectricalSubCategory"];
                model.CivilSubCategory = (int)myJObject["value"][i]["CivilSubCategory"];
                model.BuildingSubCategory = (int)myJObject["value"][i]["BuildingSubCategory"];
                model.MechanicalSubCategory = (int)myJObject["value"][i]["MechanicalSubCategory"];
                //model.OldRegNo = (string)myJObject["value"][i]["OldRegNo"];
                model.ScoreStr = (string)myJObject["value"][i]["ScoreStr"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.newGradecomment = (string)myJObject["value"][i]["newGradecomment"];
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

            string subcategory = "";
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context,_userManager);
            if (model.FormName == "Form2")
            {
                if (subcategory == "")
                {
                    if (model.BuildingSubCategory != 0)
                    {
                        subcategory = vf4.GetSubCategorybyName(model.BuildingSubCategory);
                    }
                                        
                    if (model.CivilSubCategory != 0)
                    {
                        if (subcategory == "")
                        {
                            subcategory = vf4.GetSubCategorybyName(model.CivilSubCategory);
                        }
                        else
                        {
                            subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.CivilSubCategory);
                        }
                        
                    }

                    if (model.ElectricalSubCategory != 0)
                    {
                        if (subcategory == "")
                        {
                            subcategory = vf4.GetSubCategorybyName(model.ElectricalSubCategory);
                        }
                        else
                        {
                            subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.ElectricalSubCategory);
                        }

                    }

                    if (model.MechanicalSubCategory != 0)
                    {
                        if (subcategory == "")
                        {
                            subcategory = vf4.GetSubCategorybyName(model.MechanicalSubCategory);
                        }
                        else
                        {
                            subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.MechanicalSubCategory);
                        }
                        
                    }
                }


                ViewBag.subCategory = subcategory;

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
            List<DirectorshipShareDividends> d = new List<DirectorshipShareDividends>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DirectorshipShareDividends
                {
                    DirectorName = (string)myJObject1["value"][i]["DirectorName"],
                    IdNO = (string)myJObject1["value"][i]["IdNO"],
                    Nationnality = (string)myJObject1["value"][i]["Nationnality"],
                    CellphoneNo = (string)myJObject1["value"][i]["CellphoneNo"],
                    Country = (string)myJObject1["value"][i]["Country"],
                    //ShareFile = 
                    SharePercent= (int)myJObject1["value"][i]["SharePercent"]
                   
                });
            }
            model.Sharelist = d;

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1BankDetails", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<ApplicantBank> a = new List<ApplicantBank>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new ApplicantBank
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
            model.applicantBank = a;

            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1WorkDetails", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<WorksCapability> w = new List<WorksCapability>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new WorksCapability
                {
                    CompletionDate = (DateTime)myJObject3["value"][i]["CompletionDate"],
                    ContractSum = (decimal)myJObject3["value"][i]["ContractSum"],
                    Location = (string)myJObject3["value"][i]["Location"],
                    ProjectName = (string)myJObject3["value"][i]["ProjectName"],
                    RegistationNo = (string)myJObject3["value"][i]["RegistationNo"],
                    TelephoneNo = (string)myJObject3["value"][i]["TelephoneNo"],
                    TypeofInvolvement = (string)myJObject3["value"][i]["TypeofInvolvement"]
                });
            }
            model.worksCapability = w;
            if(model.AppType == "NewApplication")
            {
                ViewBag.grade = false;
            }
            else
            {
                ViewBag.grade = true;
            }

            if (model.FormName == "Form1")
            {
                ViewBag.name = "CONSTRUCTION FIRMS REGISTRATION FORM - CICF 1";

            }
            else
            {
                ViewBag.name = "SPECIALIST WORKS CONTRACTORS REGISTRATION FORM - CICF 2";
            }
            if (model.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            

            String[] gList = model.Grade.Split(",");
            String[] sList = model.ScoreStr.Split(",");
            List<string> gsStr = new List<string>();

            for(int i = 0; i< gList.Length;i++)
            {
                gsStr.Add(gList[i] + " Score -" + sList[i] );
            }
            ViewBag.gradeSCore = gsStr;

            List<FileList> AllFileList = new List<FileList>();
            BlobStorageService b = new BlobStorageService();
            AllFileList = b.GetBlobList(model.path);

            if (AllFileList != null)
            {
                for (int j = 0; j < AllFileList.Count; j++)
                {
                   
                    switch (AllFileList[j].FileKey)
                    {
                        case "Filesignature": model.signature = AllFileList[j].FileValue; break;

                        case "Businesssignature": model.BusinessRepresentativeSign = AllFileList[j].FileValue; break;

                        case "StatmentFile": model.statementsign = AllFileList[j].FileValue; break;
                        case "BusinessFile1": model.BusinessFile1Name = AllFileList[j].FileValue; break;
                        case "BusinessFile2": model.BusinessFile2Name = AllFileList[j].FileValue; break;
                        case "BusinessFile3": model.BusinessFile3Name = AllFileList[j].FileValue; break;
                        case "BusinessFile4": model.BusinessFile4Name = AllFileList[j].FileValue; break;
                        case "BusinessFile5": model.BusinessFile5Name = AllFileList[j].FileValue; break;
                        case "BusinessFile6": model.BusinessFile6Name = AllFileList[j].FileValue; break;
                        case "ShareholdersFile1": model.ShareholdersFile1Name = AllFileList[j].FileValue; break;
                        case "ShareholdersFile2": model.ShareholdersFile2Name = AllFileList[j].FileValue; break;
                        case "ShareholdersFile3": model.ShareholdersFile3Name = AllFileList[j].FileValue; break;
                        case "FinancialFile1": model.FinancialFile1Name = AllFileList[j].FileValue; break;
                        case "FinancialFile2": model.FinancialFile2Name = AllFileList[j].FileValue; break;
                        case "FinancialFile3": model.FinancialFile3Name = AllFileList[j].FileValue; break;
                        case "TrackRecordFile1": model.TrackRecordFile1Name = AllFileList[j].FileValue; break;
                        case "TrackRecordFile2": model.TrackRecordFile2Name = AllFileList[j].FileValue; break;
                        case "TrackRecordFile3": model.TrackRecordFile3Name = AllFileList[j].FileValue; break;
                        case "JointVentureFile1": model.JointVentureFile1Name = AllFileList[j].FileValue; break;
                        case "JointVentureFile2": model.JointVentureFile2Name = AllFileList[j].FileValue; break;
                        case "JointVentureFile3": model.JointVentureFile3Name = AllFileList[j].FileValue; break;
                        case "JointVentureFile4": model.JointVentureFile4Name = AllFileList[j].FileValue; break;
                        case "Signature1": model.sign1Name = AllFileList[j].FileValue; break;
                        case "Signature2": model.sign2Name = AllFileList[j].FileValue; break;
                        case "TaxLaw": model.taxLawName = AllFileList[j].FileValue; break;
                        case "Evidence": model.EvidenceName = AllFileList[j].FileValue; break;
                        case "Compliance": model.ComplienceName = AllFileList[j].FileValue; break;
                    }
                }
            }

            memoryCache.Set("Form1Data", model);
            return View(model);
        }

        [HttpPost]
        public IActionResult ViewForm()
        {
            //string jsond ="";
            Form1Model model = new Form1Model();
            bool isExist = memoryCache.TryGetValue("Form1Data", out model);            

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform1", model.PartitionKey, model.RowKey, jsond);
                string comment = Request.Form["comment"];
                //string upgradeGrade = Request.Form["gradeUpgrade"];
                int count = Request.Form["gradeUpgrade"].Count();
                int gradeNo, scoreno;
                String[] gList = model.Grade.Split(",");
                String[] sList = model.ScoreStr.Split(",");
                string newgrade = "";
                bool gradeflag = false;

                foreach (var item in Request.Form["gradeUpgrade"])
                {
                    //for grade
                    if(model.FormName == "Form1")
                    {
                        if(item.Substring(1, 1) != "1")
                        {
                            gradeNo = Convert.ToInt32(item.Substring(1, 1)) - 1;
                            scoreno = Convert.ToInt32(item.Substring(10)) + 1;
                            newgrade = item.Substring(0, 1) + gradeNo.ToString();
                            gradeflag = true;
                        }
                        else
                        {
                            gradeNo = Convert.ToInt32(item.Substring(1, 1));
                            scoreno = Convert.ToInt32(item.Substring(10));
                            newgrade = item.Substring(0, 1) + gradeNo.ToString();                            
                        }
                        
                    }
                    else
                    {
                        if(item.Substring(2, 1) != "1")
                        {
                            gradeNo = Convert.ToInt32(item.Substring(2, 1)) - 1;
                            scoreno = Convert.ToInt32(item.Substring(11)) + 1;
                            newgrade = item.Substring(0, 2) + gradeNo.ToString();
                            gradeflag = true;
                        }
                        else
                        {
                            gradeNo = Convert.ToInt32(item.Substring(2, 1));
                            scoreno = Convert.ToInt32(item.Substring(11));
                            newgrade = item.Substring(0, 2) + gradeNo.ToString();
                        }
                    }


                    if (model.FormName == "Form1")
                    {
                        for (int i = 0; i < gList.Length; i++)
                        {
                            if (item.Substring(0, 2) == gList[i])
                            {
                                gList[i] = newgrade;
                                sList[i] = scoreno.ToString();
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < gList.Length; i++)
                        {
                            if (item.Substring(0, 3) == gList[i])
                            {
                                gList[i] = newgrade;
                                sList[i] = scoreno.ToString();
                            }
                        }
                    }

                    


                }
                if (count > 0 && gradeflag == true)
                {
                    model.newGradecomment = "Note: Clerk upgraded the grade for this application. Old grade was " + model.Grade + "(score-" + model.ScoreStr + ")";
                }
                string combinedStringforGrade = string.Join(",", gList);
                string combinedStringforscore = string.Join(",", sList);

                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Compliance Officer";
                        model.FormStatus = "Approve";
                        //string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved.</br>Comment:" + comment + "</br>Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        //sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        //sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        //sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);
                        break;

                    case "Compliance Officer":
                        model.Reviewer = "Compliance Analyst";
                        model.FormStatus = "Approve";
                        break;

                    case "Compliance Analyst":
                        model.Reviewer = "Ops Manager";
                        model.FormStatus = "Approve";
                        break;

                    case "Ops Manager":
                        model.Reviewer = "Ops Manager";
                        // model.Reviewer = "CEO"; //AK changed "Ops Manager" to CEO
                        model.FormStatus = "Completed";

                        string body = "<p>Hello Team,<br/><br/>Form: " + model.RowKey + " is approved.</br>Comment:" + comment + "</br>Requesting you to create invoice for this customer <br/><br/>Thank you,<br/>CIC Team</p>";
                        sendNotification("makhosazane@cic.co.sz", "Request for invoice", body);
                        sendNotification("sikhumbuzo@cic.co.sz", "Request for invoice", body);
                        sendNotification("mduduzi@cic.co.sz", "Request for invoice", body);

                        CICFees fees = null;
                        string invoiceNo;

                        string i_id = CreateInvoiceERP(model.CustNo, model.RowKey, out invoiceNo, model.PartitionKey, model.FormName);

                        if (model.Grade.Contains(','))
                        {
                            String[] GradeList = model.Grade.Split(",");
                            int len1 = GradeList.Length;

                            for (int k = 0; k < len1; k++)
                            {
                                fees = calculateFees(model.FormName, GradeList[k]);
                                //if (model.AppType == "NewApplication")
                                //{
                                    model.AdminFee = model.AdminFee + fees.AdminFees;
                                    model.RenewalFee = model.RenewalFee + fees.RenewalFees;
                                    model.RegistrationFee = model.RegistrationFee + fees.RegistrationFees;
                                //}
                                //else
                                //{
                                //    model.AdminFee = model.AdminFee + fees.AdminFees;
                                //    model.RenewalFee = model.RenewalFee + fees.RenewalFees;
                                //    model.RegistrationFee = 0;
                                //}
                            }
                        }
                        else
                        {
                            fees = calculateFees(model.FormName, model.Grade);

                            if (model.AppType == "NewApplication")
                            {
                                model.AdminFee = fees.AdminFees;
                                model.RenewalFee = fees.RenewalFees;
                                model.RegistrationFee = fees.RegistrationFees;
                            }
                            else
                            {
                                model.AdminFee = fees.AdminFees;
                                model.RenewalFee = fees.RenewalFees;
                                model.RegistrationFee = 0;
                            }

                            //CreateInvoiceLineItemERP(i_id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee));
                        }

                        string jsonData;
                        int penalty;

                        AzureTablesData.GetAllEntity(StorageName, StorageKey, "GracePeriodDetails", out jsonData);//Get data
                        JObject gracePeriodObject = JObject.Parse(jsonData);

                        DateTime allowedGracePeriod = (DateTime)gracePeriodObject["value"][0]["allowedDate"];

                        if (allowedGracePeriod < DateTime.Now && model.AppType == "Renewal") {
                            penalty = (model.RenewalFee * 10) / 100;

                        } else
                        {
                            penalty = 0;
                        }

                        CreateInvoiceLineItemERP(i_id, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), Convert.ToDecimal(penalty));


                        model.InvoiceNo = invoiceNo;
                        string jsonProjectData;
                        if (invoiceNo != "")
                        {
                            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1", model.RowKey, out jsonProjectData);

                            JObject myJObject = JObject.Parse(jsonProjectData);
                            int cntJson = myJObject["value"].Count();

                            for (int i = 0; i < cntJson; i++)
                            {
                                model.RegistrationID = UpdateRegistrationDetails(myJObject, i, model.InvoiceNo, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), model.PostalAddress, model.TradingStyle, Convert.ToDecimal(penalty),model.AppType);
                            }
                        }

                        break;
                       
                }
                
                model.comment = comment;
                model.Sharelist = null;
                model.applicantBank = null;
                model.worksCapability = null;
                model.Grade = combinedStringforGrade;
                model.ScoreStr = combinedStringforscore;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);

                //string jsonProjectData;
                //if (model.InvoiceNo != "")
                //{
                //    AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1", model.RowKey, out jsonProjectData);

                //    JObject myJObject = JObject.Parse(jsonProjectData);
                //    int cntJson = myJObject["value"].Count();

                //    for (int i = 0; i < cntJson; i++)
                //    {
                //       model.RegistrationID = UpdateRegistrationDetails(myJObject, i, model.InvoiceNo, Convert.ToDecimal(model.RegistrationFee), Convert.ToDecimal(model.AdminFee), Convert.ToDecimal(model.RenewalFee), model.PostalAddress,model.TradingStyle, Convert.ToDecimal(penalty));
                //    }
                //}
                //var responseR = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);

                string jsond = "";
               
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsond);
            }
            memoryCache.Remove("Form1Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        [HttpPost]
        public IActionResult RejectForm(string value, string emailId, string RepresentativeName, string comment)
        {
            var jsond = "";
            Form1Model model = new Form1Model();
            bool isExist = memoryCache.TryGetValue("Form1Data", out model);
            if(comment == null)
            {               
                return View(model);
            }

            if (isExist)
            {
                //var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform1", model.PartitionKey, model.RowKey, jsond);
                
                switch (model.Reviewer)
                {
                    case "Clerk":
                        model.Reviewer = "Contractor";
                        model.FormStatus = "Rejected";
                        model.comment = "Clerk comment - " + comment;

                        string body = "<p>Hi " + RepresentativeName + ",<br/><br/>Your form is rejected due to following reason:</br>" + comment + "</br></br>To access CIC portal you can login at: <a href='https://constructioncouncil.azurewebsites.net/'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        sendNotification(model.CreatedBy, "Your Form is Rejected", body);

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
                        model.comment = model.comment + Environment.NewLine + "Ops Manager comment - " + comment;// model.comment + Environment.NewLine + comment;
                        break;
                } 
               
                
                Form1Mapper k = new Form1Mapper();
                model.Sharelist = null;
                model.applicantBank = null;
                model.worksCapability = null;
                //k.mapData(model,model,model.FirmRegistrationNo);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),model.PartitionKey,model.RowKey);
                string jsondata = "";
                var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", model.PartitionKey, model.RowKey, jsondata);
            }

            memoryCache.Remove("Form1Data");
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public void sendNotification(string to, string subject, string body)
        {
            try
            {
                var c = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("emailTo", to),
                new KeyValuePair<string, string>("emailSubject", subject),
                new KeyValuePair<string, string>("emailBody", body),

            });

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = httpClient.PostAsync("https://ciccommunicationapi.azurewebsites.net/api/Email?emailTo=" + to + "&emailSubject= " + subject + "&emailBody=" + body, c).Result;
                    response.EnsureSuccessStatusCode();

                }
            }
            catch
            { }

        }

        [HttpPost]
        public void DownloadFile()
        {
            Form1Model model = new Form1Model();
            BlobStorageService objBlobService = new BlobStorageService();
            
            bool isExist = memoryCache.TryGetValue("Form1Data", out model);
            if(isExist)
            {
                objBlobService.DownloadBlob(model.path);                
            }

            
        }

        public FileResult GetReport()
        {
            string ReportURL = "{Your File Path}";
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, "application/pdf");
        }

        [HttpPost]
        public IActionResult Preview(CICForm1Model model)  
        {
            string tempCategory = null;

            if (model.App.AppType == "NewApplication")
            {
                ViewBag.grade = false;
            }
            else
            {
                ViewBag.grade = true;
            }

            if (model.businessModel.BusinessType == "Other")
            {
                ViewBag.other = true;
            }
            else
            {
                ViewBag.other = false;
            }

            for (int i = 0; i < model.businessModel.Category.Count; i++)
            {
                if (model.businessModel.Category[i].Selected)
                {
                    if (tempCategory == null)
                    {
                        tempCategory = model.businessModel.Category[i].CategoryName;
                    }
                    else
                    {
                        tempCategory = tempCategory + "," + model.businessModel.Category[i].CategoryName;
                    }
                }
            }
            if (tempCategory != null)
            {
                ViewBag.category = tempCategory;
            }
            else
            {
                ViewBag.category = "";
            }
            string subcategory = "";
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context,_userManager);

            if (model.FormName == "Form2")
            {
                if (subcategory == "")
                {
                    if (model.businessModel.selectedElectSubcategory != 0)
                    {
                        subcategory = vf4.GetSubCategorybyName(model.businessModel.selectedElectSubcategory);
                    }
                    if (model.businessModel.selectedBuildingSubcategory != 0)
                    {
                        subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.businessModel.selectedBuildingSubcategory);
                    }
                    if (model.businessModel.selectedCivilSubcategory != 0)
                    {
                        subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.businessModel.selectedCivilSubcategory);
                    }
                    if (model.businessModel.selectedMechSubcategory != 0)
                    {
                        subcategory = subcategory + "," + vf4.GetSubCategorybyName(model.businessModel.selectedMechSubcategory);
                    }
                }


                ViewBag.subCategory = subcategory;

            }
            return PartialView("Form1Preview", model);
        }


        public CICFees calculateFees(string formname, string grade)
        {
            var feelist = (from item in _context.cicFees
                     where item.FormName == formname & item.Grade == grade
                     select item).FirstOrDefault();
            return feelist;
        }

        public string CreateInvoiceERP(string cust, string AppNo, out string invoiceNo,string partitionKey,string FormName)
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
                    levyInvoice = false,
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
                    string u = _azureConfig.BCURL + "/cicSalesInvoices";
                    HttpResponseMessage response = httpClient.PostAsync(@u,data).Result;
                    
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
            catch(Exception e)
            { string s = e.Message; }

            return "";
        }

        public void CreateInvoiceLineItemERP(string istr, decimal registratinFee, decimal adminFee, decimal reFee, decimal penalty)
        {
            GetAccessToken();
            try
            {                
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                       
                    if (registratinFee != 0)
                    {
                        var data2 = JObject.FromObject(new
                        {
                            lineType = "G/L Account",
                            lineObjectNumber = "1000/004",
                            description = "Registration Fees",
                            unitPrice = registratinFee,
                            quantity = 1,
                            discountAmount = 0,
                            discountPercent = 0
                        });
                        var json2 = JsonConvert.SerializeObject(data2);
                        var data3 = new StringContent(json2, Encoding.UTF8, "application/json");
                        string uitemline = _azureConfig.BCURL + "/cicSalesInvoices(" + istr + ")/cicSalesInvoiceLines";
                        HttpResponseMessage response1 = httpClient.PostAsync(@uitemline, data3).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            string str = response1.Content.ReadAsStringAsync().Result;
                        }
                    }

                    if (adminFee != 0)
                    {
                        var data2 = JObject.FromObject(new
                        {
                            lineType = "G/L Account",
                            lineObjectNumber = "1000/005",
                            description = "Administration Fees",
                            unitPrice = adminFee,
                            quantity = 1,
                            discountAmount = 0,
                            discountPercent = 0,
                        });
                        var json2 = JsonConvert.SerializeObject(data2);
                        var data3 = new StringContent(json2, Encoding.UTF8, "application/json");
                        string uitemline = _azureConfig.BCURL + "/cicSalesInvoices(" + istr + ")/cicSalesInvoiceLines";
                        HttpResponseMessage response1 = httpClient.PostAsync(@uitemline, data3).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            string str = response1.Content.ReadAsStringAsync().Result;
                        }
                    }

                    if (reFee != 0)
                    {
                        var data2 = JObject.FromObject(new
                        {
                            lineType = "G/L Account",
                            lineObjectNumber = "1000/003",
                            description = "Annual Subscription",
                            unitPrice = reFee,
                            quantity = 1,
                            discountAmount = 0,
                            discountPercent = 0
                        });
                        var json2 = JsonConvert.SerializeObject(data2);
                        var data3 = new StringContent(json2, Encoding.UTF8, "application/json");
                    
                        string uitemline = _azureConfig.BCURL + "/cicSalesInvoices(" + istr + ")/cicSalesInvoiceLines";
                        HttpResponseMessage response1 = httpClient.PostAsync(@uitemline, data3).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            string str = response1.Content.ReadAsStringAsync().Result;
                        }
                    }

                    if (penalty != 0)
                    {
                        var data2 = JObject.FromObject(new
                        {
                            lineType = "G/L Account",
                            lineObjectNumber = "1000/007",
                            description = "Penalty",
                            unitPrice = penalty,
                            quantity = 1,
                            discountAmount = 0,
                            discountPercent = 0
                        });
                        var json2 = JsonConvert.SerializeObject(data2);
                        var data3 = new StringContent(json2, Encoding.UTF8, "application/json");

                        string uitemline = _azureConfig.BCURL + "/cicSalesInvoices(" + istr + ")/cicSalesInvoiceLines";
                        HttpResponseMessage response1 = httpClient.PostAsync(@uitemline, data3).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            string str = response1.Content.ReadAsStringAsync().Result;
                        }
                    }

                }
            }
            catch (Exception e)
            { string s = e.Message; }
           
        }


        public string UpdateRegistrationDetails(JObject myJObject, int i,string invoiceNo, decimal registratinFee, decimal adminFee, decimal reFee, string postalAddress, string tradeStyle,decimal penaltyFee, string typeofApplication)
        {
            GetAccessToken();
            string custno = (string)myJObject["value"][i]["CustNo"];
            string regID = "";
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
                    tradeName = tradeStyle,
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
                    grade = (string)myJObject["value"][i]["Grade"],
                    typeofApplication = typeofApplication
                }) ;
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
    }
}
