using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure;
using Newtonsoft.Json;

namespace CICLatest.Controllers
{
    public class AdminController : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        static string filepdfpath = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly EmailConfiguration _emailcofig;
        private readonly ApplicationContext _context;

        public AdminController(IMemoryCache memoryCache, AzureStorageConfiguration azureConfig, ApplicationContext context)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _context = context;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }
        public IActionResult Index(string id)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            List<ReviewerModel> reviewerList = new List<ReviewerModel>();
            string role = HttpContext.Session.GetString("UserRole");
            memoryCache.Set("Reviewer", id);
            getReviewerData(reviewerList, "cicform1()");
            getReviewerData(reviewerList, "CicForm3()");
            getReviewerData(reviewerList, "cicform4()");
            getReviewerData(reviewerList, "cicform5()");
            getReviewerData(reviewerList, "cicform6()");
            getReviewerData(reviewerList, "cicform7()");
            getReviewerData(reviewerList, "cicform8()");
            getReviewerData(reviewerList, "cicform9()");

            memoryCache.Set("rlist", reviewerList);
            return View(reviewerList);
        }

        List<ReviewerModel> getReviewerData(List<ReviewerModel> reviewerList, string tableName, string id = null, string val = null)
        {
            string jsonData, jsonlockdata;
            string queuename;
            bool isexist = memoryCache.TryGetValue("Reviewer", out queuename);

            AzureTablesData.GetEntitybyFilterDashboard(StorageName, StorageKey, tableName, "Reviewer", queuename, out jsonData);
            string role = HttpContext.Session.GetString("UserRole");

            JObject myJObject = JObject.Parse(jsonData);

            AzureTablesData.GetAllEntity(StorageName, StorageKey, "ApplicationLock", out jsonlockdata);
            JObject myJObject1 = JObject.Parse(jsonlockdata);
            int cnt = myJObject1["value"].Count();
            List<string> idlist = new List<string>();
            Dictionary<string, string> mydictionary = new Dictionary<string, string>();
            Dictionary<string, string> mydictionaryfordate = new Dictionary<string, string>();

            for (int j = 0; j < cnt; j++)
            {
                DateTime d = (DateTime)myJObject1["value"][j]["Timestamp"];

                idlist.Add((string)myJObject1["value"][j]["RowKey"]);
                mydictionary.Add((string)myJObject1["value"][j]["RowKey"], (string)myJObject1["value"][j]["AssignedTo"]);
                mydictionaryfordate.Add((string)myJObject1["value"][j]["RowKey"], d.ToString("yyyy-MM-dd"));
            }

            var latestRecord = from pp in myJObject["value"].Where(i => idlist.Contains(i["RowKey"].Value<string>())) //.OrderByDescending(i => i["Timestamp"])
                               select pp;

            int cntJson = myJObject["value"].Count();
            int newcnt = latestRecord.Count();
            bool stsFlag = false;



            foreach (var item in latestRecord)
            {
                stsFlag = false;
                string partitionkey = (string)item["PartitionKey"];
                string formName = (string)item["FormName"];
                string rowkey = (string)item["RowKey"];
                DateTime FDate = (DateTime)item["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                string sts = (string)item["FormStatus"];
                string q = (string)item["Reviewer"];
                string value, date;
                bool hasValue = mydictionary.TryGetValue(rowkey, out value);
                bool hasValuefordate = mydictionaryfordate.TryGetValue(rowkey, out date);

                switch (q)
                {
                    case "Clerk": q = "Clerk"; break;
                    case "Compliance Officer": q = "Registration Officer"; break;
                    case "Compliance Analyst": q = "Registration Analyst"; break;
                    case "Ops Manager": q = "Ops Manager"; break;
                    case "CIC Compliance": q = "CIC Compliance";break;
                    case "CEO": q = "CEO"; break;

                }
                if (id == null)
                {
                    reviewerList.Add(new ReviewerModel { FormDesc = getFormName(formName), AppNo = rowkey, FormDate = date, Status = q, PartitionKey = partitionkey, RowKey = rowkey, FormName = formName, comment = value });
                }
                else
                {
                    if (id == "AssignedTo" && val == value)
                    {
                        reviewerList.Add(new ReviewerModel { FormDesc = getFormName(formName), AppNo = rowkey, FormDate = date, Status = q, PartitionKey = partitionkey, RowKey = rowkey, FormName = formName, comment = value });
                    }
                }
            }

            return reviewerList;

        }

        string getFormName(string name)
        {
            string FormName = "";
            switch (name)
            {
                case "Form1":
                    FormName = "CONSTRUCTION FIRMS REGISTRATION FORM - CICF 1";
                    break;

                case "Form2":
                    FormName = "SPECIALIST WORKS CONTRACTORS REGISTRATION FORM -CICF 2";
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

        public IActionResult RemoveLock(string rowkey, string partitioKey)
        {
            string jsond = "";
            string queuename;
            bool isexist = memoryCache.TryGetValue("Reviewer", out queuename);
            var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", partitioKey, rowkey, jsond);
            return RedirectToAction("Index", "Admin", new { id = queuename });
        }

        public IActionResult Reports()
        {
            List<Category> categorylist = new List<Category>();
            //categorylist.Add(new Category { CategoryID = 0, CategoryName = "Select", FormType = "" });
            ReportModel m = new ReportModel();
            m.CategoryModel = categorylist;
            return View(m);
        }

        public IActionResult OperationalReports()
        {
            List<Category> categorylist = new List<Category>();
            OperationalReports reportModel = new OperationalReports();
            //m.CategoryModel = categorylist;
            reportModel.SelectedReportType = "Projects";
            reportModel.ProjectsData = GetProjectData("", "");
            return View(reportModel);
        }

        public IActionResult OperationReportWithSearchData(string reportType, string searchType, string searchValue)
        {
            OperationalReports reportModel = new OperationalReports();
            reportModel.SelectedReportType = reportType;
            reportModel.SearchValue = searchValue; ;
            reportModel.SelectedSearchType = searchType;
            if (reportType == "Projects")
            {
                reportModel.ProjectsData = GetProjectData(searchType, searchValue);
            }
            else if (reportType == "Contractors")
            {
                reportModel.ContractorsData = GetContractorData(reportModel, searchType, searchValue);
            }
            return View("OperationalReports", reportModel);
        }
        private List<Project> GetProjectData(string searchType, string searchValue)
        {
            List<Project> projectlist = new List<Project>();
            string jsonData;
            if (searchType != "" && searchType != "-1" && searchValue != "")
            {
                AzureTablesData.GetEntitybyFilterDashboard(StorageName, StorageKey, "CicForm8()", searchType, searchValue, out jsonData);

            }
            else
            {
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "CicForm8()", out jsonData);

            }
            if (JObject.Parse(jsonData)["value"].Count() > 0)
            {
                foreach (var item in JObject.Parse(jsonData)["value"])
                {
                    Project eachRow = new Project();
                    eachRow.CreatedDate = (string)item["CreatedDate"];
                    eachRow.DateofAward = (string)item["DateofAward"];
                    eachRow.Proposedcommencedate = (string)item["ProposedCommencmentDate"];
                    eachRow.Proposedcompleteddate = (string)item["ProposedCompleteDate"];
                    eachRow.Name = (string)item["Name"];
                    eachRow.OwnerCategoryId = (string)item["OwnerCategoryId"];
                    eachRow.Organization = (string)item["Oraganization"];
                    eachRow.Grade = (string)item["Grade"];
                    eachRow.MobileNo = (string)item["MobileNo"];
                    eachRow.Telephone = (string)item["Telephone"];
                    eachRow.FirstNameSurnamefields = (string)item["FirstName"] + " " + (string)item["Surname"];
                    eachRow.BriefDescriptionofProject = (string)item["BriefDescriptionofProject"];
                    eachRow.ContractVAlue = (string)item["ContractVAlue"];
                    eachRow.LevyPaybale = (string)item["LevyPaybale"];
                    eachRow.TotalProjectCost = (string)item["TotalProjectCost"];

                    if ((string)item["FormStatus"] == "Finished" || (string)item["FormStatus"] == "Completed")
                    {
                        projectlist.Add(eachRow);
                    }
                }
            }
            return projectlist;
        }
        private List<Contractor> GetContractorData(OperationalReports reportModel, string searchType, string searchValue)
        {
            List<Contractor> contractorList = new List<Contractor>();
            string searchTypeName = string.Empty;

            foreach (ContractorFormAlias item in reportModel.ContractorFormAlias)
            {
                //fetching actual Search Column

                if (searchType != "" && searchType != "-1" && searchValue != "")
                {
                    searchTypeName = (string)item.ActualColumnNameOf.GetType().GetProperty(searchType).GetValue(item.ActualColumnNameOf, null);
                }

                GetContractorsData(reportModel, contractorList, item.FormName, searchTypeName, searchValue);
            }

            return contractorList;
        }
        private void GetContractorsData(OperationalReports reportModel, List<Contractor> contractorListstring, string tableName, string searchType, string searchValue)
        {
            string jsonData;
            //foreach (string eachSearchType in searchType.Split('|'))
            //{

            if (searchType != "" && searchType != "-1" && searchValue != "")
            {
                AzureTablesData.GetEntitybyFilterDashboard(StorageName, StorageKey, tableName, searchType, searchValue, out jsonData);
            }
            else
            {
                AzureTablesData.GetAllEntity(StorageName, StorageKey, tableName, out jsonData);
            }

            if (JObject.Parse(jsonData)["value"].Count() > 0)
            {
                foreach (var item in JObject.Parse(jsonData)["value"])
                {
                    Contractor contractor = new Contractor();
                    contractor.RegNo = (string)item["CertificateNo"];
                    contractor.Category = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.Category];
                    contractor.WorkDiscipline = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.WorkDiscipline];
                    contractor.Grade = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.Grade];
                    contractor.ContractorName = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.ContractorName];
                    contractor.TelephoneNo = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.TelephoneNo];
                    contractor.MobileNo = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.MobileNo];
                    contractor.Email = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.Email];
                    contractor.PostalPhysicalAddress = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.PostalPhysicalAddress];
                    contractor.RegDate = (string)item[reportModel.ContractorFormAlias.Find(x => x.FormName == tableName).ActualColumnNameOf.RegDate];

                    if ((string)item["FormStatus"] == "Finished" || (string)item["FormStatus"] == "Completed")
                    {
                        contractorListstring.Add(contractor);
                    }
                }
            }
            //}
        }
        [HttpPost]
        public IActionResult ExportReports(ReportModel model)
        {
            string id = "", Val = "", fileName = "";
            List<ReviewerListModel> reviewerList = new List<ReviewerListModel>();
            OperationalReports reportModel = new OperationalReports();
            if (ModelState.IsValid)
            {
                //if (model.AppTypeValue == null || model.CatValue == "Select category" )
                //{
                //    ModelState.AddModelError(nameof(model.err), "Please enter data for export");
                //    return View("Reports", model);
                //}

                if (model.filterColumnName == "AppType")
                {
                    if (model.AppTypeValue == "Project")
                    {
                        getReviewerData(reviewerList, "cicform8()", "", "");
                        getReviewerData(reviewerList, "cicform9()", "", "");
                    }
                    else
                    {
                        getReviewerData(reviewerList, "cicform1()", "AppType", model.AppTypeValue);
                        getReviewerData(reviewerList, "CicForm3()", "AppType", model.AppTypeValue);
                        getReviewerData(reviewerList, "cicform4()", "AppType", model.AppTypeValue);
                        getReviewerData(reviewerList, "cicform5()", "AppType", model.AppTypeValue);
                        getReviewerData(reviewerList, "cicform6()", "AppType", model.AppTypeValue);
                        getReviewerData(reviewerList, "cicform7()", "AppType", model.AppTypeValue);
                    }
                    fileName = "CICDatabyApplicationType.xlsx";

                }

                int i = 0;
                if (model.filterColumnName == "form")
                {
                    fileName = "CICDatabyCategory.xlsx";

                    getReviewerData(reviewerList, model.Formname, "Category", model.CatValue);
                    bool result = int.TryParse(model.CatValue, out i);
                    if (result)
                    {
                        getReviewerData(reviewerList, model.Formname, "CategoryId", GetCategorybyName(i));
                    }

                    getReviewerData(reviewerList, model.Formname, "Category", model.CatValue);
                    getReviewerData(reviewerList, model.Formname, "Category", model.CatValue);
                    getReviewerData(reviewerList, model.Formname, "WorkDisciplineType", model.CatValue);
                    getReviewerData(reviewerList, model.Formname, "CategoryId", model.CatValue);
                }

                if (model.filterColumnName == "operationreport")
                {
                    if (model.OperationalReportType == "Projects")
                    {
                        reportModel.ProjectsData = GetProjectData("", "");
                    }
                    else if (model.OperationalReportType == "Contractors")
                    {
                        reportModel.ContractorsData = GetContractorData(reportModel, "", "");
                    }
                    fileName = "CICDatabyApplicationType.xlsx";

                }

                if (model.filterColumnName != "operationreport")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("CIC Data");
                        var currentRow = 1;
                        worksheet.Cell(currentRow, 1).Value = "Form Name";
                        worksheet.Cell(currentRow, 2).Value = "Aplication Number";
                        worksheet.Cell(currentRow, 3).Value = "Status";
                        worksheet.Cell(currentRow, 4).Value = "Submitted Date";
                        worksheet.Cell(currentRow, 5).Value = "Appliation Type";


                        foreach (var user in reviewerList)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = user.FormDesc;
                            worksheet.Cell(currentRow, 2).Value = user.RowKey;
                            worksheet.Cell(currentRow, 3).Value = user.Status;
                            worksheet.Cell(currentRow, 4).Value = user.FormDate;
                            worksheet.Cell(currentRow, 5).Value = user.apptype;
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();

                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                else
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("CIC Data");
                        var currentRow = 1;
                        var collumn = 1;
                        if (model.OperationalReportType == "Projects")
                        {
                            foreach (string item in reportModel.ProjectColumns1)
                            {
                                worksheet.Cell(currentRow, collumn).Value = item;
                                collumn++;
                            }
                            foreach (var user in reportModel.ProjectsData)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = user.CreatedDate;
                                worksheet.Cell(currentRow, 2).Value = user.DateofAward;
                                worksheet.Cell(currentRow, 3).Value = user.Proposedcommencedate;
                                worksheet.Cell(currentRow, 4).Value = user.Proposedcompleteddate;
                                worksheet.Cell(currentRow, 5).Value = user.Name;
                                worksheet.Cell(currentRow, 6).Value = user.OwnerCategoryId;
                                worksheet.Cell(currentRow, 7).Value = user.Organization;
                                worksheet.Cell(currentRow, 8).Value = user.Grade;
                                worksheet.Cell(currentRow, 9).Value = user.MobileNo;
                                worksheet.Cell(currentRow, 10).Value = user.Telephone;
                                worksheet.Cell(currentRow, 11).Value = user.FirstNameSurnamefields;
                                worksheet.Cell(currentRow, 12).Value = user.BriefDescriptionofProject;
                                worksheet.Cell(currentRow, 13).Value = user.ContractVAlue;
                                worksheet.Cell(currentRow, 14).Value = user.LevyPaybale;
                                worksheet.Cell(currentRow, 15).Value = user.TotalProjectCost;
                            }
                        }
                        else
                        {
                            foreach (string item in reportModel.ContractorColumns)
                            {
                                worksheet.Cell(currentRow, collumn).Value = item;
                                collumn++;
                            }
                            foreach (var user in reportModel.ContractorsData)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = user.RegNo;
                                worksheet.Cell(currentRow, 2).Value = user.Category;
                                worksheet.Cell(currentRow, 3).Value = user.WorkDiscipline;
                                worksheet.Cell(currentRow, 4).Value = user.Grade;
                                worksheet.Cell(currentRow, 5).Value = user.ContractorName;
                                worksheet.Cell(currentRow, 6).Value = user.TelephoneNo;
                                worksheet.Cell(currentRow, 7).Value = user.MobileNo;
                                worksheet.Cell(currentRow, 8).Value = user.Email;
                                worksheet.Cell(currentRow, 9).Value = user.PostalPhysicalAddress;
                                worksheet.Cell(currentRow, 10).Value = user.RegDate;
                            }
                        }                        

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();

                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
            }

            List<Category> categorylist = new List<Category>();
            //categorylist.Add(new Category { CategoryID = 0, CategoryName = "Select", FormType = "" });
            model.CategoryModel = categorylist;
            return View("Reports", model);
        }



        List<ReviewerListModel> getReviewerData(List<ReviewerListModel> reviewerList, string tableName, string id = null, string Val = null)
        {
            string newal = Val.ToLower();
            string jsonData;

            if (id == "")
            {
                AzureTablesData.GetAllEntity(StorageName, StorageKey, tableName, out jsonData);
            }
            else
            {
                AzureTablesData.GetEntitybyFilterDashboard(StorageName, StorageKey, tableName, id, Val, out jsonData);
            }


            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string formName = (string)myJObject["value"][i]["FormName"];
                string rowkey = (string)myJObject["value"][i]["RowKey"];
                string sts = (string)myJObject["value"][i]["FormStatus"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = (string)myJObject["value"][i]["CreatedDate"];// FDate.ToString("yyyy-MM-dd");
                string comm = (string)myJObject["value"][i]["comment"];
                string t = (string)myJObject["value"][i]["AppType"];
                reviewerList.Add(new ReviewerListModel { FormDesc = getFormName(formName), FormDate = formattedDate, Status = sts, RowKey = rowkey, apptype = t });
            }
            return reviewerList;

        }

        List<ReviewerListModel> getReviewerDatabyDate(List<ReviewerListModel> reviewerList, string tableName, string id, DateTime Val1, DateTime Val2)
        {

            string jsonData;

            AzureTablesData.GetEntitybyDate(StorageName, StorageKey, tableName, id, Val1, Val2, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string formName = (string)myJObject["value"][i]["FormName"];
                string rowkey = (string)myJObject["value"][i]["RowKey"];
                string sts = (string)myJObject["value"][i]["FormStatus"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = (string)myJObject["value"][i]["CreatedDate"];// FDate.ToString("yyyy-MM-dd");
                string comm = (string)myJObject["value"][i]["comment"];
                string t = (string)myJObject["value"][i]["AppType"];
                reviewerList.Add(new ReviewerListModel { FormDesc = getFormName(formName), FormDate = formattedDate, Status = sts, RowKey = rowkey, apptype = t });
            }
            return reviewerList;

        }

        public IActionResult SearchFilter(string id, string Val)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            List<ReviewerModel> reviewerList = new List<ReviewerModel>();
            string role = HttpContext.Session.GetString("UserRole");


            bool isexistlist = memoryCache.TryGetValue("rlist", out reviewerList);

            if (isexistlist)
            {
                reviewerList = filterdata(reviewerList, id, Val);
            }


            return View("Index", reviewerList);
        }

        List<ReviewerModel> filterdata(List<ReviewerModel> reviewerList, string id = null, string val = null)
        {
            if (id == "AssignedTo")
            {
                reviewerList = reviewerList.Where(x => x.comment == val).ToList();
            }
            else if (id == "RowKey")
            {
                reviewerList = reviewerList.Where(x => x.AppNo == val).ToList();
            }
            else if (id == "Timestamp")
            {
                reviewerList = reviewerList.Where(x => x.FormDate == val).ToList();
            }

            return reviewerList;

        }

        public JsonResult GetCategory(string formname)
        {
            string cattype = "";

            switch (formname)
            {
                case "cicform1": cattype = "F1"; break;
                case "cicform2": cattype = "F2"; break;
                case "cicform3": cattype = "F1"; break;
                case "cicform4": cattype = "F4"; break;
                case "cicform5": cattype = "F4"; break;
                    //case "cicform6": cattype = "F6"; break;
            }
            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category
                            where Category.FormType == cattype
                            select Category).ToList();

            if (formname == "cicform3")
            {
                List<Category> categorylist1 = new List<Category>();
                categorylist1 = (from Category in _context.Category
                                 where Category.FormType == "F2"
                                 select Category).ToList();

                for (int i = 0; i < categorylist1.Count; i++)
                {
                    categorylist.Add(new Category { CategoryID = categorylist1[i].CategoryID, CategoryName = categorylist1[i].CategoryName });
                }


            }
            categorylist.Add(new Category { CategoryID = 0, CategoryName = "Select Category" });
            return Json(categorylist);
        }

        public string GetCategorybyName(int categoryId)
        {
            string Name = "";
            Name = (from categoryType in _context.Category
                    where categoryType.CategoryID == categoryId
                    select categoryType.CategoryName).FirstOrDefault();

            return Name;
        }

        public IActionResult GracePeriod()
        {
            string jsonData;
            GracePeriodModel reportModel = new GracePeriodModel();

            AzureTablesData.GetAllEntity(StorageName, StorageKey, "GracePeriodDetails", out jsonData);//Get data
            JObject gracePeriodObject = JObject.Parse(jsonData);
            int cntJson = gracePeriodObject["value"].Count();
            if(cntJson > 0)
            {
                reportModel.allowedDate= (DateTime)gracePeriodObject["value"][0]["allowedDate"];
            }
            return View(reportModel);
        }

        public IActionResult SetGracePeriod(GracePeriodModel model)
        {
            ViewBag.StatusMessage = string.Empty;

            if (ModelState.IsValid)
            {
                string jsonData;

                AzureTablesData.GetAllEntity(StorageName, StorageKey, "GracePeriodDetails", out jsonData);//Get data
                JObject gracePeriodObject = JObject.Parse(jsonData);

                int cntJson = gracePeriodObject["value"].Count();

                model.Timestamp = DateTime.Now;
                model.CreatedBy = "Admin";
               // model.allowedDate = DateTime.Now.AddDays((double)model.NumberOfDays);

                if (cntJson > 0)
                {
                    ViewBag.StatusMessage = "Record Updated Successfully";

                    model.PartitionKey = (string)gracePeriodObject["value"][0]["PartitionKey"]; ;
                    model.RowKey = (string)gracePeriodObject["value"][0]["RowKey"]; ;
                    

                    AzureTablesData.UpdateEntity(StorageName, StorageKey, "GracePeriodDetails", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                }
                else
                {
                    ViewBag.StatusMessage = "Record Inserted Successfully";

                    model.PartitionKey = "1";
                    model.RowKey = "PRN" + "1";
                    AzureTablesData.InsertEntity(StorageName, StorageKey, "GracePeriodDetails", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }

            return View("GracePeriod", model);
        }
    }
}
