using CICLatest.Helper;
using CICLatest.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    [Authorize]
    public class ReviewerDashboardController : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        static string filepdfpath = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int BuildingCnt = 0, CivilCnt = 0, MechanicalCnt = 0, ElectricalCnt = 0, ArchitectureCnt = 0, QuantityCnt = 0, AlliedCnt=0;
        static string BGrade = "", CGrade = "", EGrade = "", MGrade = "";
        public ReviewerDashboardController(IMemoryCache memoryCache, EmailConfiguration emailconfig, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
        }

        public IActionResult ReviewerDashboard()
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            List<ReviewerModel> reviewerList = new List<ReviewerModel>();
            string role = HttpContext.Session.GetString("UserRole");

            if (role == "CEO")
            {
                getReviewerData(reviewerList, "cicform1()");
                getReviewerData(reviewerList, "CicForm3()");
                getReviewerData(reviewerList, "cicform4()");
                getReviewerData(reviewerList, "cicform5()");
                getReviewerData(reviewerList, "cicform6()");
                getReviewerData(reviewerList, "cicform7()");
                getReviewerData(reviewerList, "cicform8()");
            }
            else
            {
                getReviewerData(reviewerList, "cicform1()");
                getReviewerData(reviewerList, "CicForm3()");
                getReviewerData(reviewerList, "cicform4()");
                getReviewerData(reviewerList, "cicform5()");
                getReviewerData(reviewerList, "cicform6()");
                getReviewerData(reviewerList, "cicform7()");
                getReviewerData(reviewerList, "cicform8()");
                getReviewerData(reviewerList, "cicform9()");
            }
            string sts ="";
            bool isexist = memoryCache.TryGetValue("Cer", out sts);
            if(isexist)
            {
                ViewBag.sts = sts;
                memoryCache.Remove("Cer");
            }
            return View(reviewerList);
        }

        List<ReviewerModel> getReviewerData(List<ReviewerModel>  reviewerList, string tableName, string id = null, string Val = null)
        {
            string jsonData, jsonlockdata;
            if (id == null || Val == null)
            {
                AzureTablesData.GetAllEntity(StorageName, StorageKey, tableName, out jsonData);
            }
            else
            {
                if(id== "AppType")
                {
                    string newal = Val.ToLower();

                    switch(newal)
                    {
                        case "renewal":
                        case "renew":
                            Val = "Renewal";break;

                        case "additionalwork": 
                        case "additional": 
                            Val = "AdditionalWork"; break;

                        case "newapplication":
                        case "new":
                            Val = "NewApplication"; break;
                    }
                }
                AzureTablesData.GetEntitybyFilterDashboard(StorageName, StorageKey, tableName, id, Val, out jsonData);
            }
            string role = HttpContext.Session.GetString("UserRole");

            AzureTablesData.GetAllEntity(StorageName, StorageKey, "ApplicationLock", out jsonlockdata);
            JObject myJObject1 = JObject.Parse(jsonlockdata);
            int cnt = myJObject1["value"].Count();
            List<string> idlist = new List<string>();

            for (int j = 0; j < cnt; j++)
            {
                if ((string)myJObject1["value"][j]["AssignedTo"] != User.Identity.Name)
                {
                    idlist.Add((string)myJObject1["value"][j]["RowKey"]);
                }
            }

            JObject myJObject = JObject.Parse(jsonData);
            //DateTime tempd = Convert.ToDateTime();
            //var latestRecord = from p in myJObject["value"]
            //                    orderby (DateTime)p["Timestamp"] descending
            //                    select p;

            var latestRecord = from pp in myJObject["value"].Where(i => !idlist.Contains(i["RowKey"].Value<string>())).OrderByDescending(i => Convert.ToDateTime(i["CreatedDate"]))
                               select pp;

            int cntJson = myJObject["value"].Count();
            int newcnt = latestRecord.Count();
            bool stsFlag = false;

            switch (role)
            {
                case "NormalUser": ViewBag.Role = "My"; break;
                case "Clerk": ViewBag.Role = "Clerk"; ViewBag.dispalyId = ""; break;
                case "Compliance Officer": ViewBag.Role = "Registration Officer";ViewBag.dispalyId = "display: none;"; break;
                case "Compliance Analyst": ViewBag.Role = "Registration Analyst"; ViewBag.dispalyId = "display: none;"; break;
                case "Ops Manager": ViewBag.Role = "Ops Manager"; ViewBag.dispalyId = "display: none;"; break;
                case "CIC Compliance": ViewBag.Role = "CIC Compliance"; ViewBag.dispalyId = "display: none;"; break;
                case "CEO": ViewBag.Role = "CEO"; ViewBag.dispalyId = "display: none;"; break;

            }
           
            if (role == "NormalUser")
            {
                string tempPath = "", certificateNO = "", newpath = "";

                foreach (var item in latestRecord)
                {
                    stsFlag = false;
                    string partitionkey = (string)item["PartitionKey"];
                    string formName = (string)item["FormName"];
                    string rowkey = (string)item["RowKey"];
                    string sts = (string)item["FormStatus"];
                    DateTime FDate = (DateTime)item["Timestamp"];
                    string formattedDate = (string)item["CreatedDate"];// FDate.ToString("yyyy-MM-dd");
                    string comm = (string)item["comment"];
                    string t = (string)item["AppType"];

                    if ((string)item["FormStatus"] == "Finished" && (string)item["CreatedBy"] == User.Identity.Name)
                    {
                        newpath = "";
                        tempPath = (string)item["path"];
                        if (tempPath == null)
                        {
                            tempPath = (string)item["ImagePath"];
                        }
                        tempPath = tempPath.Replace("https:\\", "");
                        certificateNO = (string)item["CertificateNo"];
                        if (certificateNO.Contains(','))
                        {
                            String[] strList = certificateNO.Split(",");
                            int len = strList.Length;
                            for (int j = 0; j < len; j++)
                            {
                                if (newpath == "")
                                {
                                    newpath = tempPath + @"\Files\Certificate_" + strList[j] + ".pdf";
                                }
                                else
                                {
                                    newpath = newpath + ";" + tempPath + @"\Files\Certificate_" + strList[j] + ".pdf";
                                }

                            }
                        }
                        else
                        {
                            newpath = tempPath + @"\Files\Certificate_" + certificateNO + ".pdf";
                        }


                    }

                    if ((string)item["FormStatus"] == "Draft" || (string)item["FormStatus"] == "Rejected" || (string)item["FormStatus"] == "Submit" || (string)item["FormStatus"] == "Completed" || (string)item["FormStatus"] == "Finished")
                    {
                        stsFlag = true;
                    }
                    if (sts == "Submit")
                    {
                        comm = "Under Review";
                        sts = "Submitted";
                    }

                    if (stsFlag && (string)item["CreatedBy"] == User.Identity.Name)
                    {
                        reviewerList.Add(new ReviewerModel { FormDesc = getFormName(formName), FormDate = formattedDate, Status = sts, PartitionKey = partitionkey, RowKey = rowkey, FormName = formName, comment = comm, pdfFile = newpath, apptype= t});
                        stsFlag = false;
                    }

                }                
            }
            else if(role == "CEO")
            {
                foreach (var item in latestRecord)
                {
                    stsFlag = false;
                    string partitionkey = (string)item["PartitionKey"];
                    string formName = (string)item["FormName"];
                    string rowkey = (string)item["RowKey"];
                    string sts = (string)item["FormStatus"];
                    DateTime FDate = (DateTime)item["Timestamp"];
                    string formattedDate = (string)item["CreatedDate"];// FDate.ToString("yyyy-MM-dd");
                    string comm = (string)item["comment"];
                    string t = (string)item["AppType"];

                    if ((string)item["FormStatus"] == "Completed")
                    {
                        stsFlag = true;
                    }

                    if (stsFlag && (string)item["Reviewer"] == role)
                    {
                        reviewerList.Add(new ReviewerModel { FormDesc = getFormName(formName), FormDate = formattedDate, Status = sts, PartitionKey = partitionkey, RowKey = rowkey, FormName = formName, comment = comm ,apptype=t});
                        stsFlag = false;
                    }
                }                   
            }
            else
            {
                foreach(var item in latestRecord)
                {
                    stsFlag = false;
                    string partitionkey = (string)item["PartitionKey"];
                    string formName = (string)item["FormName"];
                    string rowkey = (string)item["RowKey"];
                    DateTime FDate = (DateTime)item["Timestamp"];
                    string formattedDate = (string)item["CreatedDate"];//FDate.ToString("yyyy-MM-dd");
                    string sts = (string)item["FormStatus"];
                    string comm = (string)item["comment"];
                    string t = (string)item["AppType"];
                    if (sts == "Submit" || sts == "Approve" || sts == "Rejected")
                    {
                        stsFlag = true;
                    }
                    if (sts == "Submit")
                    {
                        sts = "Submitted";
                    }
                    if (sts == "Approve")
                    {
                        sts = "Approved";
                    }
                    if (stsFlag && (string)item["Reviewer"] == role)
                    {
                        reviewerList.Add(new ReviewerModel { FormDesc = getFormName(formName), FormDate = formattedDate, Status = sts, PartitionKey = partitionkey, RowKey = rowkey, FormName = formName, comment = comm, apptype=t });
                    }
                }               
            }
            return reviewerList;

        }

        string getFormName(string name)
        {
            string FormName = "";
            switch(name)
            {
                case "Form1": FormName = "CONSTRUCTION FIRMS REGISTRATION FORM - CICF 1";
                    break;

                case "Form2": FormName = "SPECIALIST WORKS CONTRACTORS REGISTRATION FORM -CICF 2";
                    break;

                case "Form3":
                    FormName = "JOINT VENTURE CONSTRUCTION FIRMS AND SPECIALIST WORKS CONTRACTORS REGISTRATION FORM -CICF 3";
                    break;

                case "Form4":
                    FormName = "CONSULTANCY PRACTICES REGISTRATION FORM – CICF 4";
                    break;

                case "Form5":
                    FormName = "JOINT VENTURE CONSULTANCY PRACTICES REGISTRATION FROM – CICF 5";
                    break;

                case "Form6":
                    FormName = "INDIVIDUAL ARTISANS REGISTRATION FORM – CICF 6";
                    break;

                case "Form7":
                    FormName = "MANUFACTURERS AND SUPPLIERS REGISTRATION FORM – CICF 7";
                    break;

                case "Form8":
                    FormName = "CONSTRUCTION PROJECTS AND LEVY ASSESSMENT REGISTRATION FORM – CICF 8";
                    break;

                case "Form9":
                    FormName = "REGISTRATION OF CONSTRUCTION PROJECTS – CICF 9";
                    break;
            }

            return FormName;
        }

        //[HttpPost]
        public IActionResult ViewForm(string PartitionKey, string row, string form)
        {
            string role = HttpContext.Session.GetString("UserRole");

            if (role == "NormalUser")
            {
                switch (form)
                {
                    case "Form1":
                    case "Form2":
                        return RedirectToAction("IndexFromDashboard", "Form1", new { rowkey = row });

                    case "Form3": return RedirectToAction("IndexFromDashboard", "Form3", new { rowkey = row });

                    case "Form4": 
                        return RedirectToAction("IndexFromDashboard", "Form4", new { rowkey = row });

                    case "Form5": 
                        return RedirectToAction("IndexFromDashboard", "Cicform5", new { rowkey = row });

                    case "Form6":
                       
                        return RedirectToAction("IndexFromDashboard", "Cicform6", new { rowkey = row });


                    case "Form7":
                        return RedirectToAction("IndexFromDashboard", "Cicform7", new { rowkey = row });

                    case "Form8":
                        return RedirectToAction("IndexFromDashboard", "Form8", new { rowkey = row });

                    //case "Form9": break;
                    case "Form9":
                        return RedirectToAction("IndexFromDashboard", "Form9", new { rowkey = row });


                }

            }

            Form1Model model = new Form1Model();

            ApplicationLockModel lockdata = new ApplicationLockModel();
            lockdata.FormName = form;
            lockdata.RowKey = row;
            lockdata.PartitionKey = PartitionKey;
            lockdata.AssignedTo = User.Identity.Name;

            var response = AzureTablesData.InsertEntity(StorageName, StorageKey, "ApplicationLock", JsonConvert.SerializeObject(lockdata, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            switch (form)
            {
                case "Form1":
                case "Form2":
                    return RedirectToAction("ViewForm", "ViewForm1", new { rowkey = row });

                case "Form3": return RedirectToAction("ViewForm3", "ViewForm3", new { rowkey = row });

                case "Form4": return RedirectToAction("ViewForm4", "ViewForm4", new { rowkey = row });

                case "Form5": return RedirectToAction("ViewForm5", "ViewForm5", new { rowkey = row });

                case "Form6": return RedirectToAction("ViewForm6", "ViewForm6", new { rowkey = row });

                case "Form7": return RedirectToAction("ViewForm7", "ViewForm7", new { rowkey = row });

                case "Form8": return RedirectToAction("ViewForm8", "ViewForm8", new { rowkey = row });

                case "Form9": return RedirectToAction("ViewForm9", "ViewForm9", new { rowkey = row });

            }
       
            return View(model);
        }


        public IActionResult GenerateCertificate(string rowkey, string form, string PartitionKey)
        {
            ApplicationLockModel lockdata = new ApplicationLockModel();
            lockdata.FormName = form;
            lockdata.RowKey = rowkey;
            lockdata.PartitionKey = PartitionKey;
            lockdata.AssignedTo = User.Identity.Name;

            var response = AzureTablesData.InsertEntity(StorageName, StorageKey, "ApplicationLock", JsonConvert.SerializeObject(lockdata, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            switch (form)
            {
                case "Form1": return RedirectToAction("Index", "CertificateForm1", new { rowkey = rowkey }); 
                case "Form2":
                    return RedirectToAction("Index", "CertificateForm2", new { rowkey = rowkey });
                case "Form3": return RedirectToAction("Index", "CertificateForm3", new { rowkey = rowkey });
                case "Form4": return RedirectToAction("Index", "CertificateForm4", new { rowkey = rowkey });
                case "Form5": return RedirectToAction("Index", "CertificateForm5", new { rowkey = rowkey });
                case "Form6": return RedirectToAction("Index", "CertificateForm6", new { rowkey = rowkey });
                case "Form7": return RedirectToAction("Index", "CertificateForm7", new { rowkey = rowkey });
                case "Form8": return RedirectToAction("Index", "CertificateForm8", new { rowkey = rowkey });

            }

            

            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");            
        }



        

        public FileResult DownloadCertificate(string PartitionKey, string row, string form)
        {
            string tempPath = "", certificateNO = "";
            string jsonData;
            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1", PartitionKey, row, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            for (int i = 0; i < cntJson; i++)
            {
                tempPath = (string)myJObject["value"][i]["path"];
                certificateNO = (string)myJObject["value"][i]["CertificateNo"];
            }

            tempPath = tempPath + @"\Files\Certificate_" + certificateNO;
            
            byte[] bytes = System.IO.File.ReadAllBytes(tempPath);

            
            return File(bytes, "application/octet-stream", "Certificate");
        }


        public IActionResult SearchFilter(string id, string Val)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            List<ReviewerModel> reviewerList = new List<ReviewerModel>();
            string role = HttpContext.Session.GetString("UserRole");

            if (role == "CEO")
            {
                getReviewerData(reviewerList, "cicform1()", id, Val);
                getReviewerData(reviewerList, "CicForm3()", id, Val);
                getReviewerData(reviewerList, "cicform4()", id, Val);
                getReviewerData(reviewerList, "cicform5()", id, Val);
                getReviewerData(reviewerList, "cicform6()", id, Val);
                getReviewerData(reviewerList, "cicform7()", id, Val);
                getReviewerData(reviewerList, "cicform8()", id, Val);
            }
            else
            {
                getReviewerData(reviewerList, "cicform1()", id, Val);
                getReviewerData(reviewerList, "CicForm3()", id, Val);
                getReviewerData(reviewerList, "cicform4()", id, Val);
                getReviewerData(reviewerList, "cicform5()", id, Val);
                getReviewerData(reviewerList, "cicform6()", id, Val);
                getReviewerData(reviewerList, "cicform7()", id, Val);
                getReviewerData(reviewerList, "cicform8()", id, Val);
                getReviewerData(reviewerList, "cicform9()", id, Val);
            }
            string sts = "";
            bool isexist = memoryCache.TryGetValue("Cer", out sts);
            if (isexist)
            {
                ViewBag.sts = sts;
                memoryCache.Remove("Cer");
            }
            return View("ReviewerDashboard", reviewerList);
        }


    }
}
