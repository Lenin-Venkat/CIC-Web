using CICLatest.Helper;
using CICLatest.MappingConfigurations;
using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
//using Microsoft.Bot.Configuration;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    [Authorize]
    public class Form3Controller : Controller
    {
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");
        CICForm3Model form3Model = new CICForm3Model();
        static string StorageName = "";
        static string StorageKey = "";
        BusinessDetailsModel3 businessModel = new BusinessDetailsModel3();
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        int cnt = 1;
        private readonly IMemoryCache memoryCache;
        private readonly UserManager<UserModel> _userManager;
        static string filepath = "NA";
        CustomValidations cv = new CustomValidations();
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public Form3Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
            , UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            _context = context;
            _azureConfig = azureConfig;
            this.memoryCache = memoryCache;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _userManager = userManager;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }
        public CICForm3Model loadData(CICForm3Model m)
        {


            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category
                            where Category.FormType == "F1" ||
                            Category.FormType == "F2"
                            select Category).ToList();


            List<categoryType> categoryTypeList = new List<categoryType>();

            for (int i = 0; i < categorylist.Count; i++)
            {


                categoryTypeList.Add(new categoryType { CategoryID = categorylist[i].CategoryID, CategoryName = categorylist[i].CategoryName });


            }

            businessModel.Category = categoryTypeList;
            businessModel.SelectedCategoryValue = m.businessModel == null ? 0 : m.businessModel.SelectedCategoryValue;

            m.businessModel = businessModel;
            return m;

        }

        public CICForm3Model loadData1(CICForm3Model m, int CategoryID = 0)
        {

            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            subCategorylist = (from SubCategoryType in _context.SubCategory
                               where SubCategoryType.CategoryID == CategoryID
                               select SubCategoryType).ToList();



            List<subCategory> sub = new List<subCategory>();

            for (int i = 0; i < subCategorylist.Count; i++)
            {
                sub.Add(new subCategory { SubCategoryID = subCategorylist[i].SubCategoryID, SubCategoryName = subCategorylist[i].SubCategoryName });
            }
            businessModel.subCategoryModel = sub;


            //string jsonDataBusinesses;
            //AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonDataBusinesses);

            //JObject businessesObject = JObject.Parse(jsonDataBusinesses);

            //var businesses = businessesObject["value"]
            //    .Where(x => !string.IsNullOrEmpty((string)x["BusinessName"]) && !string.IsNullOrEmpty((string)x["CertificateNo"]))
            //    .GroupBy(x => x["BusinessName"])
            //    .Select(x => x.OrderByDescending(y => y["Timestamp"]).FirstOrDefault())
            //    .OrderBy(x => ((string)x["BusinessName"]).ToLower())
            //    .ToList();

            //List<Business> businessesList = new List<Business>();

            //businessesList.Add(new Business() { BusinessNameValue = " ", BusinessNameText = "Select" });
            //for (int i = 0; i < businesses.Count(); i++)
            //{
            //    businessesList.Add(new Business()
            //    {
            //        BusinessNameValue = (string)businesses[i]["BusinessName"]
            //        ,
            //        BusinessNameText = (string)businesses[i]["BusinessName"]
            //    });
            //}
            //businessModel.Businesses = businessesList;
            //m.businessModel = businessModel;

            //memoryCache.Set("ListOfBusinesses", businessesList);

            if (businessModel.SelectedCategoryValue > 0)
                businessModel.Businesses = GetBusinessesByCategory(businessModel.SelectedCategoryValue);
            else
                businessModel.Businesses = new List<Business>();

            m.businessModel = businessModel;

            return m;
        }

        public List<Business> GetBusinessesByCategory(int CategoryID)
        {

            string jsonDataBusinesses;

            var selectedCategory = _context.Category.FirstOrDefault(x => x.CategoryID == CategoryID)?.CategoryName;

            AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonDataBusinesses);

            JObject businessesObject = JObject.Parse(jsonDataBusinesses);

            var businesses = businessesObject["value"]
                .Where(x => !string.IsNullOrEmpty((string)x["BusinessName"]) && !string.IsNullOrEmpty((string)x["CertificateNo"])
                    && ((string)x["Category"]).ToLower() == selectedCategory.ToLower())
                .GroupBy(x => x["BusinessName"])
                .Select(x => x.OrderByDescending(y => y["Timestamp"]).FirstOrDefault())
                .OrderBy(x => ((string)x["BusinessName"]).ToLower())
                .ToList();

            List<Business> businessesList = new List<Business>();

            businessesList.Add(new Business() { BusinessNameValue = " ", BusinessNameText = "Select" });
            for (int i = 0; i < businesses.Count(); i++)
            {
                businessesList.Add(new Business()
                {
                    BusinessNameValue = (string)businesses[i]["BusinessName"]
                    ,
                    BusinessNameText = (string)businesses[i]["BusinessName"]
                });
            }
            businessModel.Businesses = businessesList;

            return businessesList;
        }

        public JsonResult GetBusinesses(int CategoryID)
        {
            //memoryCache.Set("ListOfBusinesses", businessesList);

            //memoryCache.TryGetValue("ListOfBusinesses", out businesses);

            var businessesList = GetBusinessesByCategory(CategoryID);

            var jsonString = JsonConvert.SerializeObject(businessesList);

            return Json(jsonString);
        }

        public JsonResult GetSubCategory(int CategoryID)
        {
            businessModel.SelectedCategoryValue = CategoryID;

            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            //------- Getting data from database using EntityframeworkCore -------
            subCategorylist = (from SubCategoryType in _context.SubCategory
                               where SubCategoryType.CategoryID == CategoryID
                               select SubCategoryType).ToList();

            //------- Inserting select items in list -------
            subCategorylist.Insert(0, new SubCategoryType { SubCategoryID = 0, SubCategoryName = "Select" });
            List<subCategory> sub = new List<subCategory>();

            for (int i = 0; i < subCategorylist.Count; i++)
            {
                sub.Add(new subCategory { SubCategoryID = subCategorylist[i].SubCategoryID, SubCategoryName = subCategorylist[i].SubCategoryName });
            }

            businessModel.subCategoryModel = sub;
            return Json(businessModel.subCategoryModel);
        }

        public ActionResult CicForm3()
        {
            CICForm3Model form3EditModel = new CICForm3Model();

            bool isExist = memoryCache.TryGetValue("Form3", out form3EditModel);
            ViewBag.typebusiness = "active";

            if (isExist)
            {
                form3EditModel.JointVenturePartiesModel?.ForEach(x => x.BusinessName = x.NameofApplicant);
                form3Model = form3EditModel;
            }
            else
            {
                form3Model.JVGridCnt = cnt;
                form3Model.CategoryGridCnt = cnt;
                form3Model.ProjectGrid = cnt;
                form3Model.forthGrid = cnt;
                form3Model.SubContractorGridCnt = cnt;

                form3Model.JointVenturePartiesModel = new List<ParticularsofJointVentureParties>();
                form3Model.JointVenturePartiesModel.Add(new ParticularsofJointVentureParties { BusinessName = "", NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0 });

                form3Model.TechnicalAdministrativeStaffModel = new List<TechnicalAdministrativeStaff>();
                form3Model.TechnicalAdministrativeStaffModel.Add(new TechnicalAdministrativeStaff { Category = "", Number = 0, FormRegistrationNo = 0, YearsofExperience = 0, PartitionKey = "-", RowKey = "-" });

                form3Model.projectStaffModel = new List<ProjectStaff>();
                form3Model.projectStaffModel.Add(new ProjectStaff { StaffName = "", StaffPosition = "", StaffQualification = "", StaffExperience = 0, StaffNationality = "", IdNO = "", StaffActivity = "" });

                form3Model.LabourForceModel = new List<LabourForce>();
                form3Model.LabourForceModel.Add(new LabourForce { Gender = "Male", Swazi1 = 0, Foreign1 = 0, Swazi2 = 0, Foreign2 = 0, Total = 0, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                form3Model.LabourForceModel.Add(new LabourForce { Gender = "Female", Swazi1 = 0, Foreign1 = 0, Swazi2 = 0, Foreign2 = 0, Total = 0, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                form3Model.LabourForceModel.Add(new LabourForce { Gender = "TOTAL", Swazi1 = 0, Foreign1 = 0, Swazi2 = 0, Foreign2 = 0, Total = 0, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });


                form3Model.SubContractorModel = new List<SubContractors>();
                form3Model.SubContractorModel.Add(new SubContractors { NameofContractor = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0 });
                form3Model = loadData(form3Model);
                form3Model = loadData1(form3Model);

            }


            return View(form3Model);
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

        public void setGetFileEdit(CICForm3Model p)
        {
            if (p.doc3.BusineesParticularsfile1 != null)
            {
                memoryCache.Set("BusineesParticularsfile1", p.doc3.BusineesParticularsfile1);
                p.doc3.BusineesParticularsfile1Name = p.doc3.BusineesParticularsfile1.FileName;
                memoryCache.Set("BusineesParticularsfile1", p.doc3.BusineesParticularsfile1.FileName);
            }
            else
            {
                p.doc3.BusineesParticularsfile1Name = SetandGetFileEdit("BusineesParticularsfile1");
            }


            if (p.doc3.BusineesParticularsfile2 != null)
            {
                memoryCache.Set("BusineesParticularsfile2", p.doc3.BusineesParticularsfile2);
                p.doc3.BusineesParticularsfile2Name = p.doc3.BusineesParticularsfile2.FileName;
                memoryCache.Set("BusineesParticularsfile2", p.doc3.BusineesParticularsfile2.FileName);
            }
            else
            {
                p.doc3.BusineesParticularsfile2Name = SetandGetFileEdit("BusineesParticularsfile2");
            }

            if (p.doc3.Signature != null)
            {
                memoryCache.Set("Signature", p.doc3.Signature);
                p.doc3.SignatureName = p.doc3.Signature.FileName;
                memoryCache.Set("Signature", p.doc3.Signature.FileName);
            }
            else
            {
                p.doc3.SignatureName = SetandGetFileEdit("Signature");
            }

            if (p.declaration3.Signature != null)
            {
                memoryCache.Set("Signature1", p.declaration3.Signature);
                p.declaration3.SignatureName = p.declaration3.Signature.FileName;
                memoryCache.Set("Signature1", p.declaration3.Signature.FileName);
            }
            else
            {
                p.declaration3.SignatureName = SetandGetFileEdit("Signature1");
            }
        }

        public string SetandGetFileEdit(string key)
        {
            string tempFilename;
            bool isExist = memoryCache.TryGetValue(key, out tempFilename);
            if (isExist && tempFilename != null)
            {
                return tempFilename;
            }


            return "";
        }

        public void SetandGetFileAdd(CICForm3Model p)
        {
            IFormFile fsign;
            //string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("BusineesParticularsfile2", out fsign);
            if (!isExist)
            {
                if (p.doc3.BusineesParticularsfile2 != null)
                {
                    memoryCache.Set("BusineesParticularsfile2", p.doc3.BusineesParticularsfile2);
                    p.doc3.BusineesParticularsfile2Name = p.doc3.BusineesParticularsfile2.FileName;
                }
            }
            else
            {
                if (p.doc3.BusineesParticularsfile2 != null)
                {
                    memoryCache.Set("BusineesParticularsfile2", p.doc3.BusineesParticularsfile2);
                }
                else
                {
                    p.doc3.BusineesParticularsfile2 = fsign;
                }
                p.doc3.BusineesParticularsfile2Name = p.doc3.BusineesParticularsfile2.FileName;
            }


            isExist = memoryCache.TryGetValue("BusineesParticularsfile1", out fsign);
            if (!isExist)
            {
                if (p.doc3.BusineesParticularsfile1 != null)
                {
                    memoryCache.Set("BusineesParticularsfile1", p.doc3.BusineesParticularsfile1);
                    p.doc3.BusineesParticularsfile1Name = p.doc3.BusineesParticularsfile1.FileName;
                }
            }
            else
            {
                if (p.doc3.BusineesParticularsfile1 != null)
                {
                    memoryCache.Set("BusineesParticularsfile1", p.doc3.BusineesParticularsfile1);
                }
                else
                {
                    p.doc3.BusineesParticularsfile1 = fsign;
                }
                p.doc3.BusineesParticularsfile1Name = p.doc3.BusineesParticularsfile1.FileName;

            }

            isExist = memoryCache.TryGetValue("Signature", out fsign);
            if (!isExist)
            {
                if (p.doc3.Signature != null)
                {
                    memoryCache.Set("Signature", p.doc3.Signature);
                    p.doc3.SignatureName = p.doc3.Signature.FileName;
                }
            }
            else
            {
                if (p.doc3.Signature != null)
                {
                    memoryCache.Set("Signature", p.doc3.Signature);
                }
                else
                {
                    p.doc3.Signature = fsign;
                }
                p.doc3.SignatureName = p.doc3.Signature.FileName;

            }

            isExist = memoryCache.TryGetValue("Signature1", out fsign);
            if (!isExist)
            {
                if (p.declaration3.Signature != null)
                {
                    memoryCache.Set("Signature1", p.declaration3.Signature);
                    p.declaration3.SignatureName = p.declaration3.Signature.FileName;
                }
            }
            else
            {
                if (p.declaration3.Signature != null)
                {
                    memoryCache.Set("Signature1", p.declaration3.Signature);
                }
                else
                {
                    p.declaration3.Signature = fsign;
                }
                p.declaration3.SignatureName = p.declaration3.Signature.FileName;

            }
        }

        public void fileDefault(CICForm3Model p)
        {
            p.doc3.BusineesParticularsfile1Name = (p.doc3.BusineesParticularsfile1Name != "") ? p.doc3.BusineesParticularsfile1Name : "-";
            p.doc3.BusineesParticularsfile2Name = (p.doc3.BusineesParticularsfile2Name != "") ? p.doc3.BusineesParticularsfile2Name : "-";
            p.doc3.BusineesParticularsfile1Name = (p.doc3.BusineesParticularsfile1Name != null) ? p.doc3.BusineesParticularsfile1Name : "-";
            p.doc3.BusineesParticularsfile2Name = (p.doc3.BusineesParticularsfile2Name != null) ? p.doc3.BusineesParticularsfile2Name : "-";
            p.doc3.SignatureName = (p.doc3.SignatureName != "") ? p.doc3.SignatureName : "-";
            p.doc3.SignatureName = (p.doc3.SignatureName != null) ? p.doc3.SignatureName : "-";
            p.declaration3.SignatureName = (p.declaration3.SignatureName != null) ? p.declaration3.SignatureName : "-";
            p.declaration3.SignatureName = (p.declaration3.SignatureName != "") ? p.declaration3.SignatureName : "-";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm3(CICForm3Model p, string name, string next, string pre)
        {
            if (p.formval == "Edit")
            {
                setGetFileEdit(p);
            }
            else
            {
                SetandGetFileAdd(p);
            }


            if (p.JointVenturePartiesModel != null)
            {
                p.JVGridCnt = p.JointVenturePartiesModel.Count() + 1;
                for (int i = 0; i < p.JointVenturePartiesModel.Count; i++)
                {
                    p.JointVenturePartiesModel[i].PartitionKey = "-";
                    p.JointVenturePartiesModel[i].RowKey = "-";
                }

            }
            else
            {
                p.JVGridCnt = cnt;
                p.JointVenturePartiesModel = new List<ParticularsofJointVentureParties>();
                p.JointVenturePartiesModel.Add(new ParticularsofJointVentureParties { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0, PartitionKey = "-", RowKey = "-" });
            }

            if (p.TechnicalAdministrativeStaffModel != null)
            {
                p.CategoryGridCnt = p.TechnicalAdministrativeStaffModel.Count() + 1;
                for (int i = 0; i < p.TechnicalAdministrativeStaffModel.Count; i++)
                {
                    p.TechnicalAdministrativeStaffModel[i].PartitionKey = "-";
                    p.TechnicalAdministrativeStaffModel[i].RowKey = "-";
                }

            }
            else
            {
                p.CategoryGridCnt = cnt;
                p.TechnicalAdministrativeStaffModel = new List<TechnicalAdministrativeStaff>();
                p.TechnicalAdministrativeStaffModel.Add(new TechnicalAdministrativeStaff { Category = "", Number = 0, FormRegistrationNo = 0, YearsofExperience = 0, PartitionKey = "-", RowKey = "-" });
            }

            if (p.projectStaffModel != null)
            {
                p.ProjectGrid = p.projectStaffModel.Count() + 1;

                for (int i = 0; i < p.projectStaffModel.Count; i++)
                {
                    p.projectStaffModel[i].PartitionKey = "-";
                    p.projectStaffModel[i].RowKey = "-";
                }

            }
            else
            {
                p.ProjectGrid = cnt;
                p.projectStaffModel = new List<ProjectStaff>();
                p.projectStaffModel.Add(new ProjectStaff { StaffName = "", StaffPosition = "", StaffQualification = "", StaffExperience = 0, StaffNationality = "", IdNO = "", StaffActivity = "", PartitionKey = "-", RowKey = "-" });

            }

            if (p.SubContractorModel != null)
            {
                p.SubContractorGridCnt = p.SubContractorModel.Count() + 1;

                for (int i = 0; i < p.SubContractorModel.Count; i++)
                {
                    p.SubContractorModel[i].PartitionKey = "-";
                    p.SubContractorModel[i].RowKey = "-";
                }

            }
            else
            {
                p.SubContractorGridCnt = cnt;
                p.SubContractorModel = new List<SubContractors>();
                p.SubContractorModel.Add(new SubContractors { NameofContractor = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0, PartitionKey = "-", RowKey = "-" });
            }
            setDefault(p);
            fileDefault(p);

            bool checkshares = ShareValidation(p.JointVenturePartiesModel);

            if (checkshares)
            {
                p.App.TypeofJoointVenture = "foreign";
                ViewBag.BType = "checked";
            }

            //if (p.FormRegistrationNo == 0)
            //{
            //    p.FormRegistrationNo = getRegNo(p);

            //}

            p.FormRegistrationNo = GenericHelper.GetRegNo(p.FormRegistrationNo, p.formval, _azureConfig); //AK

            switch (name)
            {

                case "typebusiness":
                    bool AppFlag = cv.IsAnyNullOrEmpty(p.App);

                    if (AppFlag == true)
                    {
                        ViewBag.typebusiness = "active";
                    }
                    else if (AppFlag == false && next != null)
                    {
                        ViewBag.joint = "active";
                    }

                    break;

                case "joint":
                    bool BFlag = JointVentureModelValidations(p);

                    if (BFlag == true)
                    {
                        ViewBag.joint = "active";
                    }
                    else if (BFlag == false && next != null)
                    {
                        ViewBag.technical = "active";
                    }
                    else if (BFlag == false && pre != null)
                    {
                        ViewBag.typebusiness = "active";
                    }
                    break;


                case "technical":
                    bool FFlag = false;

                    FFlag = TechnicalModelValidation(p);

                    if (FFlag == true)
                    {
                        ViewBag.technical = "active";
                    }
                    else if (FFlag == false && next != null)
                    {
                        ViewBag.project = "active";
                    }
                    else if (FFlag == false && pre != null)
                    {
                        ViewBag.joint = "active";
                    }
                    break;

                case "project":

                    bool WFlag = cv.IsAnyNullOrEmpty(p.projectDetailsModel);

                    if (p.projectDetailsModel.CompletionDate == default(DateTime) || p.projectDetailsModel.DateofAward == default(DateTime) || p.projectDetailsModel.CommencementDate == default(DateTime))
                    {
                        WFlag = true;

                    }
                    if (WFlag == true)
                    {
                        ViewBag.project = "active";
                    }
                    else if (WFlag == false && next != null)
                    {
                        ViewBag.docUpload = "active";
                    }
                    else if (WFlag == false && pre != null)
                    {
                        ViewBag.technical = "active";
                    }
                    break;

                case "docUpload":

                    bool docFlag = DocModelValidation(p);

                    if (docFlag == true)
                    {
                        ViewBag.docUpload = "active";
                    }

                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.project = "active";
                    }

                    break;

                case "draft":

                    p.Reviewer = "";
                    p.FormStatus = "Draft";
                    UploadBlob(p, p.FormRegistrationNo);
                    string result3 = Savedata(p);
                    return RedirectToAction("CicformReview", "Form3", new { result = result3, text = "Draft" });

                case "final":
                    setDefault(p);
                    fileDefault(p);
                    UploadBlob(p, p.FormRegistrationNo);
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    if (A == true)
                    {
                        ViewBag.typebusiness = "active";
                        break;
                    }

                    bool B = JointVentureModelValidations(p);
                    if (B == true)
                    {
                        ViewBag.joint = "active";
                        break;
                    }

                    bool F = false;

                    F = TechnicalModelValidation(p);
                    if (F == true)
                    {
                        ViewBag.technical = "active";
                        break;
                    }

                    bool W = cv.IsAnyNullOrEmpty(p.projectDetailsModel);
                    if (W == true)
                    {
                        ViewBag.project = "active";
                        break;
                    }

                    bool D = DocModelValidation(p);
                    if (D == true)
                    {
                        ViewBag.docUpload = "active";
                        break;
                    }
                    else
                    {
                        p.Reviewer = "Clerk";
                        p.FormStatus = "Submit";
                        string result4 = Savedata(p);
                        return RedirectToAction("CicformReview", "Form3", new { result = result4, text = "Submit" });
                    }
            }

            int selectedsub = p.businessModel.selectedsubcategory;
            int categoryId = p.businessModel.SelectedCategoryValue;
            loadData(p);
            loadData1(p, categoryId);

            if (p.businessModel.subCategoryModel.Count != 0 && selectedsub == 0)
            {
                ViewBag.joint = "active";
                ModelState.AddModelError("businessModel.selectedsubcategory", "Please select sub category");
            }
            else
            {
                ModelState.Remove("businessModel.selectedsubcategory");
                p.businessModel.selectedsubcategory = selectedsub;
            }

            if (p.businessModel.subCategoryModel.Count != 0)
            {
                ViewBag.Subcat = true;
            }
            else
            {
                ViewBag.Subcat = false;
            }

            return View(p);
        }

        void setDefault(CICForm3Model p)
        {
            if (p.formval != "Edit")
            {
                p.ImagePath = "-";
                p.CreatedDate = "-";
            }

            if (p.App.FaxNo == null)
            {
                p.App.FaxNo = "-";
            }
            if (p.App.Fax == null)
            {
                p.App.Fax = "-";
            }

        }

        public void removeDatafromSession()
        {
            memoryCache.Remove("Form3");
            memoryCache.Remove("BusineesParticularsfile1");
            memoryCache.Remove("BusineesParticularsfile2");
            memoryCache.Remove("Signature1");
            memoryCache.Remove("Signature");
        }

        public bool DocModelValidation(CICForm3Model p)
        {
            bool AppFlag = false;
            if (p.doc3.Name == null)
            {
                AppFlag = true;
                ModelState.AddModelError("doc3.Name", "Please enter Name");
            }
            if (p.formval != "Edit")
            {
                if (p.doc3.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.Signature", "Please upload signature");
                }
            }
            else
            {
                if (p.doc3.Signature == null && (p.doc3.SignatureName == null || p.doc3.SignatureName == "-"))
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.Signature", "Please upload signature");
                }
            }

            if (p.formval != "Edit")
            {
                if (p.doc3.BusineesParticularsfile1 == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.BusineesParticularsfile1", "Please upload signature");
                }
            }
            else
            {
                if (p.doc3.BusineesParticularsfile1 == null && (p.doc3.BusineesParticularsfile1Name == null || p.doc3.BusineesParticularsfile1Name == "-"))
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.BusineesParticularsfile1", "Please upload signature");
                }
            }

            if (p.formval != "Edit")
            {
                if (p.doc3.BusineesParticularsfile2 == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.BusineesParticularsfile2", "Please upload signature");
                }
            }
            else
            {
                if (p.doc3.BusineesParticularsfile2 == null && (p.doc3.BusineesParticularsfile2Name == null || p.doc3.BusineesParticularsfile2Name == "-"))
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc3.BusineesParticularsfile2", "Please upload signature");
                }
            }


            if (p.doc3.TitleDesignation == null)
            {
                AppFlag = true;
                ModelState.AddModelError("doc3.TitleDesignation", "Please enter title");
            }

            if (p.doc3.TermsAndConditions == false)
            {
                AppFlag = true;
                ModelState.AddModelError("doc3.TermsAndConditions", "Please accept Terms and conditions");
            }

            return AppFlag;
        }

        public bool JointVentureModelValidations(CICForm3Model p)
        {
            bool DFlag = false, DFlag1 = false;
            int tempPercentage = 0;

            p.JointVenturePartiesModel.ForEach(x => x.NameofApplicant = x.BusinessName);

            if (p.businessModel.SelectedCategoryValue == 0)
            {
                DFlag = true;
                ModelState.AddModelError("businessModel.SelectedCategoryValue", "Please select Category");
            }

            //if (p.businessModel.selectedsubcategory == 0)
            //{
            //    DFlag = true;
            //    ModelState.AddModelError("businessModel.selectedsubcategory", "Please select sub category");
            //}

            for (int i = 0; i < p.JointVenturePartiesModel.Count; i++)
            {
                //DFlag1 = cv.IsAnyNullOrEmpty(p.JointVenturePartiesModel[i]);

                if (ModelState["JointVenturePartiesModel[" + i + "].Shareholding"] != null)
                {
                    if (ModelState["JointVenturePartiesModel[" + i + "].Shareholding"].Errors.Any())
                    {
                        DFlag1 = true;
                        ModelState.AddModelError("JointVenturePartiesModel[" + i + "].Shareholding", "Please use values between 1 to 100 for % Shares!");
                    }
                }

                if (p.JointVenturePartiesModel[i].CountryOfOrigin == "Select Country")
                {
                    DFlag1 = true;
                    ModelState.AddModelError("JointVenturePartiesModel[" + i + "].CountryOfOrigin", "Please Select Country Of Origin!");
                }

                //tempPercentage = tempPercentage + p.JointVenturePartiesModel[i].Shareholding;
                //if (tempPercentage > 100)
                //{
                //    DFlag1 = true;
                //    ModelState.AddModelError("JointVenturePartiesModel[" + i + "].Shareholding", "Shares can not be greater than 100%");
                //}

                if (DFlag1 == true)
                    break;
            }

            if (p.JointVenturePartiesModel != null)
            {
                if (p.JointVenturePartiesModel.Count > 0 && p.JointVenturePartiesModel.Count < 2)
                {
                    DFlag1 = true;
                    ModelState.AddModelError("JoinVenturesCountError", "Please add more than 1 Contractor");
                }
                if (p.JointVenturePartiesModel.Sum(x => x.Shareholding) != 100)
                {
                    DFlag1 = true;
                    ModelState.AddModelError("JointVenturesShareholdingError", "Please enter total Shareholding 100%");
                }
            }

            if (DFlag1)
            {
                DFlag = true;
            }
            return DFlag;
        }

        bool validateIdNo(ProjectStaff p)
        {
            if (p.IdNO != null)
            {
                if (p.StaffNationality == "Swazi")
                {
                    if (p.IdNO.Length == 13)
                    {
                        string tempstr = p.IdNO.Substring(0, 6);

                        bool isValid = regex.IsMatch(tempstr);

                        string str = p.IdNO.Substring(7, 1);

                        if (!isValid || str != "1")
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TechnicalModelValidation(CICForm3Model p)
        {
            bool FFlag = false;
            bool aFlag = false, pFlag = false;
            for (int i = 0; i < p.TechnicalAdministrativeStaffModel.Count; i++)
            {
                if (p.TechnicalAdministrativeStaffModel[i].Category != "Select")
                {
                    if (p.TechnicalAdministrativeStaffModel[i].Number == 0)
                    {
                        FFlag = true;
                        ModelState.AddModelError("TechnicalAdministrativeStaffModel[" + i + "].Number", "Please enter Number");
                    }
                    else
                    {
                        ModelState.Remove("TechnicalAdministrativeStaffModel[" + i + "].Number");
                    }
                    if (p.TechnicalAdministrativeStaffModel[i].YearsofExperience == 0)
                    {
                        FFlag = true;
                        ModelState.AddModelError("TechnicalAdministrativeStaffModel[" + i + "].YearsofExperience", "Please enter Years of Experience");
                    }
                    else
                    {
                        ModelState.Remove("TechnicalAdministrativeStaffModel[" + i + "].YearsofExperience");
                    }
                }
            }

            for (int i = 0; i < p.SubContractorModel.Count; i++)
            {
                if (p.SubContractorModel[i].NameofContractor != null)
                {
                    if (p.SubContractorModel[i].CountryyofOrigin == null)
                    {
                        aFlag = true;
                        ModelState.AddModelError("SubContractorModel[" + i + "].CountryyofOrigin", "Please enter Country of Origin");
                    }
                    else
                    {
                        ModelState.Remove("SubContractorModel[" + i + "].CountryyofOrigin");
                    }

                    if (p.SubContractorModel[i].CICRegistrationNo == null)
                    {
                        aFlag = true;
                        ModelState.AddModelError("SubContractorModel[" + i + "].CICRegistrationNo", "Please enter CIC Registration No");
                    }
                    else
                    {
                        ModelState.Remove("SubContractorModel[" + i + "].CICRegistrationNo");
                    }

                    if (p.SubContractorModel[i].DescriptionOfWork == null)
                    {
                        aFlag = true;
                        ModelState.AddModelError("SubContractorModel[" + i + "].DescriptionOfWork", "Please enter Description Of Work");
                    }
                    else
                    {
                        ModelState.Remove("SubContractorModel[" + i + "].DescriptionOfWork");
                    }

                    if (p.SubContractorModel[i].ContractValueOfWork == null)
                    {
                        aFlag = true;
                        ModelState.AddModelError("SubContractorModel[" + i + "].ContractValueOfWork", "Please enter Contract Value Of Work");
                    }
                    else
                    {
                        ModelState.Remove("SubContractorModel[" + i + "].ContractValueOfWork");
                    }
                }
            }

            for (int i = 0; i < p.projectStaffModel.Count; i++)
            {
                if (p.projectStaffModel[i].StaffName != null)
                {
                    if (p.projectStaffModel[i].StaffPosition == null)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].StaffPosition", "Please enter Position");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].StaffPosition");
                    }

                    if (p.projectStaffModel[i].StaffQualification == null)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].StaffQualification", "Please enter Qualification");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].StaffQualification");
                    }

                    if (p.projectStaffModel[i].StaffNationality == "Swazi")
                    {
                        if (validateIdNo(p.projectStaffModel[i]))
                        {
                            pFlag = true;
                            ModelState.AddModelError("projectStaffModel[" + i + "].IdNO", "Invalid Id number!");
                            ViewBag.IdError = "Invalid Id number!";
                        }
                        else
                        {
                            ModelState.Remove("projectStaffModel[" + i + "].IdNO");
                        }
                    }

                    if (p.projectStaffModel[i].StaffExperience == 0)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].StaffExperience", "Please enter Experience");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].StaffExperience");
                    }

                    if (p.projectStaffModel[i].StaffNationality == null)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].StaffNationality", "Please enter Nationality");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].StaffNationality");
                    }

                    if (p.projectStaffModel[i].IdNO == null)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].IdNO", "Please enter Id NO");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].IdNO");
                    }

                    if (p.projectStaffModel[i].StaffActivity == null)
                    {
                        pFlag = true;
                        ModelState.AddModelError("projectStaffModel[" + i + "].StaffActivity", "Please enter Activity");
                    }
                    else
                    {
                        ModelState.Remove("projectStaffModel[" + i + "].StaffActivity");
                    }
                }
            }

            if (pFlag)
            {
                FFlag = true;
            }
            if (aFlag)
            {
                FFlag = true;
            }

            return FFlag;
        }

        public IActionResult Getdata(string apptype)
        {
            CICForm3Model model = new CICForm3Model();
            string jsonData;
            int tempcategory = 0, TempSubCategory = 0;
            AzureTablesData.GetEntitybyLoginId(StorageName, StorageKey, "cicform3", User.Identity.Name, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            var latestRecord = (from rec in myJObject["value"]
                                orderby (int)rec["FormRegistrationNo"] descending
                                select rec).FirstOrDefault();

            if (latestRecord != null)
            {
                model.RowKey = (string)latestRecord["RowKey"];

                ApplicationTypeModel3 App = new ApplicationTypeModel3
                {
                    AppType = apptype,
                    BusinessEmail = (string)latestRecord["BusinessEmail"],
                    BusinessTelephone = (string)latestRecord["BusinessTelephone"],
                    Designation = (string)latestRecord["Designation"],
                    Email = (string)latestRecord["Email"],
                    Fax = (string)latestRecord["Fax"],
                    FaxNo = (string)latestRecord["FaxNo"],
                    FirstName = (string)latestRecord["FirstName"],
                    MobileNo = (string)latestRecord["MobileNo"],
                    NameOFJoinVenture = (string)latestRecord["NameOFJoinVenture"],
                    NameType = (string)latestRecord["NameType"],
                    Phyaddress = (string)latestRecord["Phyaddress"],
                    SurName = (string)latestRecord["SurName"],
                    Telephone = (string)latestRecord["Telephone"],
                    TypeofJoointVenture = (string)latestRecord["TypeofJoointVenture"]
                };

                model.App = App;
                tempcategory = (int)latestRecord["CategoryId"];
                TempSubCategory = (int)latestRecord["Subcatogory"];
                model.CustNo = (string)latestRecord["CustNo"];

                BusinessDetailsModel3 business = new BusinessDetailsModel3()
                {
                    SelectedCategoryValue = (int)latestRecord["CategoryId"],
                    selectedsubcategory = (int)latestRecord["Subcatogory"],
                };

                model.businessModel = business;

                Doc3 docs = new Doc3
                {
                    Name = (string)latestRecord["Name"],
                    TitleDesignation = (string)latestRecord["TitleDesignation"]
                };

                model.doc3 = docs;

                Declaration3 d1 = new Declaration3
                {
                    Name = (string)latestRecord["WitnessedName"],
                    TitleDesignation = (string)latestRecord["WitnessedTitleDesignation"]
                };
                model.declaration3 = d1;

                ProjectDetails projects = new ProjectDetails
                {
                    BidReferenceNo = (string)latestRecord["BidReferenceNo"],
                    ProjectTitle = (string)latestRecord["ProjectTitle"],
                    DateofAward = (DateTime)latestRecord["DateofAward"],
                    CommencementDate = (DateTime)latestRecord["CommencementDate"],
                    CompletionDate = (DateTime)latestRecord["CompletionDate"],
                    DescriptionofProject = (string)latestRecord["DescriptionofProject"],
                    ClientName = (string)latestRecord["ClientName"],
                    ContractValue = (decimal)latestRecord["ContractValue"]
                };
                model.projectDetailsModel = projects;

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", model.RowKey, out jsonData1);
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

                if (model.JointVenturePartiesModel.Count == 0)
                {
                    model.JVGridCnt = 1;
                    model.JointVenturePartiesModel = new List<ParticularsofJointVentureParties>();
                    model.JointVenturePartiesModel.Add(new ParticularsofJointVentureParties { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0, PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.JVGridCnt = model.JointVenturePartiesModel.Count;
                }

                //Technical staff
                string jsonData2;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3TechnicalandAdministrativeStaff", model.RowKey, out jsonData2);
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
                if (model.TechnicalAdministrativeStaffModel.Count == 0)
                {
                    model.CategoryGridCnt = cnt;
                    model.TechnicalAdministrativeStaffModel = new List<TechnicalAdministrativeStaff>();
                    model.TechnicalAdministrativeStaffModel.Add(new TechnicalAdministrativeStaff { Category = "", Number = 0, FormRegistrationNo = 0, YearsofExperience = 0, PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.JVGridCnt = model.TechnicalAdministrativeStaffModel.Count;

                }


                //Project Staff
                string jsonData3;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3ProjectStaff", model.RowKey, out jsonData3);
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
                if (model.projectStaffModel.Count == 0)
                {
                    model.ProjectGrid = cnt;
                    model.projectStaffModel = new List<ProjectStaff>();
                    model.projectStaffModel.Add(new ProjectStaff { StaffName = "", StaffPosition = "", StaffQualification = "", StaffExperience = 0, StaffNationality = "", IdNO = "", StaffActivity = "", PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.ProjectGrid = model.projectStaffModel.Count;

                }

                //Labour force
                string jsonData4;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm3LabourForce", model.RowKey, out jsonData4);
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

                //
                string jsonData5;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", model.RowKey, out jsonData5);
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
                if (model.SubContractorModel.Count == 0)
                {
                    model.SubContractorGridCnt = cnt;
                    model.SubContractorModel = new List<SubContractors>();
                    model.SubContractorModel.Add(new SubContractors { NameofContractor = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0, PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.SubContractorGridCnt = model.SubContractorModel.Count;

                }


                model.LabourForceModel = l;
                model.forthGrid = model.LabourForceModel.Count;

                loadData(model);
                loadData1(model, tempcategory);

                model.businessModel.SelectedCategoryValue = tempcategory;
                model.businessModel.selectedsubcategory = TempSubCategory;

                for (int k = 0; k < model.businessModel.Category.Count; k++)
                {
                    if (model.businessModel.Category[k].CategoryID == tempcategory)
                    {
                        model.businessModel.Category[k].Selected = true;
                    }
                }

                memoryCache.Set("Form3", model);
            }


            return RedirectToAction("CicForm3", "Form3");
        }
        public IActionResult IndexFromDashboard(string rowkey)
        {
            CICForm3Model model = new CICForm3Model();
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            int tempcategory = 0, TempSubCategory = 0;
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
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.FormStatus = (string)myJObject["value"][i]["Telephone"];
                model.Reviewer = (string)myJObject["value"][i]["Telephone"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];

                string key, value;
                AllFileList = _blobStorageService.GetBlobList(model.ImagePath);

                string signature = null, BusinessFile2 = null, BusinessFile1 = null, signature1 = null;
                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;
                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);

                        switch (key)
                        {
                            case "BusineesParticularsfile1": BusinessFile1 = AllFileList[j].FileValue; break;
                            case "BusineesParticularsfile2": BusinessFile2 = AllFileList[j].FileValue; break;
                            case "Signature": signature = AllFileList[j].FileValue; break;
                            case "Signature1": signature1 = AllFileList[j].FileValue; break;
                        }
                    }
                }

                ApplicationTypeModel3 App = new ApplicationTypeModel3
                {
                    AppType = (string)myJObject["value"][i]["AppType"],
                    BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"],
                    BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"],
                    Designation = (string)myJObject["value"][i]["Designation"],
                    Email = (string)myJObject["value"][i]["Email"],
                    Fax = (string)myJObject["value"][i]["Fax"],
                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                    FirstName = (string)myJObject["value"][i]["FirstName"],
                    MobileNo = (string)myJObject["value"][i]["MobileNo"],
                    NameOFJoinVenture = (string)myJObject["value"][i]["NameOFJoinVenture"],
                    NameType = (string)myJObject["value"][i]["NameType"],
                    Phyaddress = (string)myJObject["value"][i]["Phyaddress"],
                    SurName = (string)myJObject["value"][i]["SurName"],
                    Telephone = (string)myJObject["value"][i]["Telephone"],
                    TypeofJoointVenture = (string)myJObject["value"][i]["TypeofJoointVenture"]
                };

                model.App = App;
                tempcategory = (int)myJObject["value"][i]["CategoryId"];
                TempSubCategory = (int)myJObject["value"][i]["Subcatogory"];
                BusinessDetailsModel3 business = new BusinessDetailsModel3()
                {
                    SelectedCategoryValue = (int)myJObject["value"][i]["CategoryId"],
                    selectedsubcategory = (int)myJObject["value"][i]["Subcatogory"],
                };

                model.businessModel = business;

                Doc3 docs = new Doc3
                {
                    Name = (string)myJObject["value"][i]["Name"],
                    //Signature = (string)myJObject["value"][i]["Signature"],
                    TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"],
                    SignatureName = signature,
                    BusineesParticularsfile1Name = BusinessFile1,
                    BusineesParticularsfile2Name = BusinessFile2

                };

                model.doc3 = docs;

                Declaration3 d1 = new Declaration3
                {
                    Name = (string)myJObject["value"][i]["WitnessedName"],
                    TitleDesignation = (string)myJObject["value"][i]["WitnessedTitleDesignation"],
                    SignatureName = signature1
                };
                model.declaration3 = d1;

                ProjectDetails projects = new ProjectDetails
                {
                    BidReferenceNo = (string)myJObject["value"][i]["BidReferenceNo"],
                    ProjectTitle = (string)myJObject["value"][i]["ProjectTitle"],
                    DateofAward = (DateTime)myJObject["value"][i]["DateofAward"],
                    CommencementDate = (DateTime)myJObject["value"][i]["CommencementDate"],
                    CompletionDate = (DateTime)myJObject["value"][i]["CompletionDate"],
                    DescriptionofProject = (string)myJObject["value"][i]["DescriptionofProject"],
                    ClientName = (string)myJObject["value"][i]["ClientName"],
                    ContractValue = (decimal)myJObject["value"][i]["ContractValue"]
                };
                model.projectDetailsModel = projects;

                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];

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

            if (model.JointVenturePartiesModel.Count == 0)
            {
                model.JVGridCnt = 1;
                model.JointVenturePartiesModel = new List<ParticularsofJointVentureParties>();
                model.JointVenturePartiesModel.Add(new ParticularsofJointVentureParties { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0, PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.JVGridCnt = model.JointVenturePartiesModel.Count;
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
            if (model.TechnicalAdministrativeStaffModel.Count == 0)
            {
                model.CategoryGridCnt = cnt;
                model.TechnicalAdministrativeStaffModel = new List<TechnicalAdministrativeStaff>();
                model.TechnicalAdministrativeStaffModel.Add(new TechnicalAdministrativeStaff { Category = "", Number = 0, FormRegistrationNo = 0, YearsofExperience = 0, PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.JVGridCnt = model.TechnicalAdministrativeStaffModel.Count;

            }


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
            if (model.projectStaffModel.Count == 0)
            {
                model.ProjectGrid = cnt;
                model.projectStaffModel = new List<ProjectStaff>();
                model.projectStaffModel.Add(new ProjectStaff { StaffName = "", StaffPosition = "", StaffQualification = "", StaffExperience = 0, StaffNationality = "", IdNO = "", StaffActivity = "", PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.ProjectGrid = model.projectStaffModel.Count;

            }

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
            if (model.SubContractorModel.Count == 0)
            {
                model.SubContractorGridCnt = cnt;
                model.SubContractorModel = new List<SubContractors>();
                model.SubContractorModel.Add(new SubContractors { NameofContractor = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0, PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.SubContractorGridCnt = model.SubContractorModel.Count;

            }


            model.LabourForceModel = l;
            model.forthGrid = model.LabourForceModel.Count;

            loadData(model);
            loadData1(model, tempcategory);

            model.businessModel.SelectedCategoryValue = tempcategory;
            model.businessModel.selectedsubcategory = TempSubCategory;

            for (int k = 0; k < model.businessModel.Category.Count; k++)
            {
                if (model.businessModel.Category[k].CategoryID == tempcategory)
                {
                    model.businessModel.Category[k].Selected = true;
                }
            }
            model.formval = "Edit";

            memoryCache.Set("Form3", model);
            return RedirectToAction("CicForm3", "Form3");
        }

        public void UploadBlob(CICForm3Model p, int tempMax)
        {
            if (p.ImagePath == "NA" || p.ImagePath == "-")
            {
                //p.ImagePath = "PRN" + tempMax;
                p.ImagePath = "Form" + tempMax;
            }

            uploadFiles1(p.doc3.BusineesParticularsfile1, p.ImagePath, "BusineesParticularsfile1");
            uploadFiles1(p.doc3.BusineesParticularsfile2, p.ImagePath, "BusineesParticularsfile2");
            uploadFiles1(p.doc3.Signature, p.ImagePath, "Signature");
            uploadFiles1(p.declaration3.Signature, p.ImagePath, "Signature1");

        }

        public int getRegNo(CICForm3Model p)
        {
            int tempMax = 0;

            if (p.formval == "Edit")
            {
                tempMax = p.FormRegistrationNo;
            }
            else
            {
                string jsonData;

                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform", out jsonData);//Get data

                JObject myJObject = JObject.Parse(jsonData);
                int cntJson = myJObject["value"].Count();
                int tempRegNo;

                if (cntJson != 0)
                {
                    tempMax = (int)myJObject["value"][0]["ProjectRegistrationNo"];
                }


                for (int i = 0; i < cntJson; i++)
                {
                    tempRegNo = (int)myJObject["value"][i]["ProjectRegistrationNo"];

                    if (tempRegNo > tempMax)
                    {
                        tempMax = tempRegNo;
                    }
                }
                tempMax++;

                //Adding new registration no 
                AddNewRegistrationNo addNew = new AddNewRegistrationNo();
                addNew.PartitionKey = tempMax.ToString();
                // addNew.RowKey = "PRN" + tempMax.ToString();
                addNew.RowKey = "Form" + tempMax.ToString(); //AK
                addNew.ProjectRegistrationNo = tempMax.ToString();
                var response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));
            }

            return tempMax;

        }

        public async Task<string> uploadFiles1(IFormFile tempFile, string path, string name)
        {
            if (tempFile != null)
            {
                #region Read File Content
                string fileName = Path.GetFileName(tempFile.FileName);
                string TempFilename = name + "_" + tempFile.FileName;
                byte[] fileData;


                using (var memoryStream = new MemoryStream())
                {
                    await tempFile.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                string mimeType = tempFile.ContentType;

                filepath = _blobStorageService.UploadFileToBlob(TempFilename, fileData, mimeType, path);
                #endregion
            }
            else
            {
                if (!path.Contains("https"))
                {
                    var imgpath = _appSettingsReader.Read("ImagePath");
                    filepath = imgpath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + path;
                }
                else
                {
                    filepath = path;
                }
            }
            return filepath;
        }



        public string Savedata(CICForm3Model p3)
        {
            string response = "";
            Form3Model ModelForm3 = new Form3Model();
            Form3Mapper k = new Form3Mapper();
            string TableName = "CicForm3()";

            
            int tempMax = p3.FormRegistrationNo;
            // model.App.ImagePath = "PRN" + tempMax; //AK
            AddNewRegistrationNo addNew = new AddNewRegistrationNo();
            addNew.PartitionKey = tempMax.ToString();
            // addNew.RowKey = "PRN" + tempMax.ToString(); //AK
            addNew.RowKey = "Form" + tempMax.ToString();
            addNew.ProjectRegistrationNo = tempMax.ToString();
            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));

            p3.CustNo = HttpContext.Session.GetString("CustNo");
            p3.CreatedBy = User.Identity.Name;
            memoryCache.Set("emailto", User.Identity.Name);
            if (filepath != "NA")
            {
                p3.ImagePath = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    var imgpath = _appSettingsReader.Read("ImagePath");
                    p3.ImagePath = imgpath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }
            k.mapData(p3, ModelForm3, tempMax);

            string firmNo = ModelForm3.RowKey;
            if (p3.formval == "Edit")
            {
                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform3", JsonConvert.SerializeObject(ModelForm3, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), ModelForm3.PartitionKey, ModelForm3.RowKey);
            }
            else
            {
                ModelForm3.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(ModelForm3, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }

            if (response == "Created" || response == "NoContent")
            {
                p3.JointVenturePartiesModel.ForEach(x => x.NameofApplicant = x.BusinessName);

                //Saving data for first section
                ParticularsofJointVentureParties JointVenturePartiesModel = new ParticularsofJointVentureParties();
                for (int i = 0; i < p3.JointVenturePartiesModel.Count; i++)
                {
                    k.mapJointDetails(JointVenturePartiesModel, p3.JointVenturePartiesModel[i], tempMax);

                    bool DFlag = cv.IsAnyNullOrEmpty(p3.JointVenturePartiesModel[i]);
                    if (DFlag == false)
                    {
                        string Data;
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm5ProjectDetails", JointVenturePartiesModel.PartitionKey, JointVenturePartiesModel.RowKey, out Data);

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm5ProjectDetails", JsonConvert.SerializeObject(JointVenturePartiesModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), JointVenturePartiesModel.PartitionKey, JointVenturePartiesModel.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5ProjectDetails", JsonConvert.SerializeObject(JointVenturePartiesModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }

                //technical staff
                TechnicalAdministrativeStaff tech = new TechnicalAdministrativeStaff();
                for (int i = 0; i < p3.TechnicalAdministrativeStaffModel.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(p3.TechnicalAdministrativeStaffModel[i]);
                    if (DFlag == false)
                    {
                        string Data;
                        k.mapTechStaffDetails(tech, p3.TechnicalAdministrativeStaffModel[i], tempMax);
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm3TechnicalandAdministrativeStaff", tech.PartitionKey, tech.RowKey, out Data);

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm3TechnicalandAdministrativeStaff", JsonConvert.SerializeObject(tech, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), tech.PartitionKey, tech.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm3TechnicalandAdministrativeStaff", JsonConvert.SerializeObject(tech, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }

                //Project staff
                ProjectStaff p = new ProjectStaff();
                for (int i = 0; i < p3.projectStaffModel.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(p3.projectStaffModel[i]);
                    if (DFlag == false)
                    {
                        string Data;
                        k.mapProjectStaffDetails(p, p3.projectStaffModel[i], tempMax);
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm3ProjectStaff", p.PartitionKey, p.RowKey, out Data);

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm3ProjectStaff", JsonConvert.SerializeObject(p, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), p.PartitionKey, p.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm3ProjectStaff", JsonConvert.SerializeObject(p, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }

                //Labour                
                LabourForce l = new LabourForce();
                if (p3.LabourForceModel != null)
                {
                    for (int i = 0; i < p3.LabourForceModel.Count; i++)
                    {
                        p3.LabourForceModel[i].PartitionKey = "-";
                        p3.LabourForceModel[i].RowKey = "-";

                        bool DFlag = cv.IsAnyNullOrEmpty(p3.LabourForceModel[i]);
                        if (DFlag == false)
                        {
                            string Data;
                            k.mapLabourDetails(l, p3.LabourForceModel[i], tempMax);
                            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm3LabourForce", l.PartitionKey, l.RowKey, out Data);

                            JObject myJObject1 = JObject.Parse(Data);
                            int cntJson1 = myJObject1["value"].Count();

                            if (cntJson1 != 0)
                            {
                                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm3LabourForce", JsonConvert.SerializeObject(l, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), l.PartitionKey, l.RowKey);
                            }
                            else
                            {
                                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm3LabourForce", JsonConvert.SerializeObject(l, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }



                    }
                }
                //Subcontract
                SubContractors s = new SubContractors();
                for (int i = 0; i < p3.SubContractorModel.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(p3.SubContractorModel[i]);
                    if (DFlag == false)
                    {
                        string Data;
                        k.mapSubContractDetails(s, p3.SubContractorModel[i], tempMax);
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm5SubconsultantDetails", s.PartitionKey, s.RowKey, out Data);

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", JsonConvert.SerializeObject(s, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), s.PartitionKey, s.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", JsonConvert.SerializeObject(s, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }
            }
            return firmNo;

        }

        public void uploadFiles(IFormFile tempFile, string path)
        {
            #region Read File Content
            string fileName = Path.GetFileName(tempFile.FileName);
            byte[] fileData;
            using (var target = new MemoryStream())
            {
                tempFile.CopyTo(target);
                fileData = target.ToArray();
            }

            string mimeType = tempFile.ContentType;

            filepath = _blobStorageService.UploadFileToBlob(tempFile.FileName, fileData, mimeType, path);
            #endregion
        }

        public IActionResult CicformReview(string result, string text)
        {
            ViewBag.Result = result;
            ViewBag.sts = text;
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            string body = "", subject = "", emailto = "";
            ViewBag.yr = yr;

            var domain = _appSettingsReader.Read("Domain");
            if (text == "Draft")
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='"+ domain +"'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been saved as draft";
            }
            else
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been successfully submitted. To view your application status, please log in <a href='" + domain + "'>CIC Portal</a> and view your dashboard. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been successfully submitted";
            }
            memoryCache.TryGetValue("emailto", out emailto);
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
        }

        public JsonResult GetBusinessDetails(string BusinessName)
        {
            if (!string.IsNullOrEmpty(BusinessName) && BusinessName != "Select")
            {
                string jsonCicForm1Data;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonCicForm1Data);

                JObject cicForm1Object = JObject.Parse(jsonCicForm1Data);

                string jsonShareDividendsData;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1ShareDividends", out jsonShareDividendsData);

                JObject sharedDividendsObject = JObject.Parse(jsonShareDividendsData);

                var subContractors = (from f1 in cicForm1Object["value"].Where(x => ((string)x["BusinessName"]) == BusinessName
                                        && !string.IsNullOrEmpty((string)x["CertificateNo"]))
                                      join sd in sharedDividendsObject["value"] on f1["RowKey"] equals sd["RowKey"]
                                      orderby f1["Timestamp"] descending, sd["Timestamp"] descending
                                      select new
                                      {
                                          CountryofOrigin = (string)sd["Nationnality"],
                                          ContactDetails = (string)f1["BusinessRepresentativeCellNo"],
                                          CICRegistrationNo = (string)f1["CertificateNo"]
                                      }).FirstOrDefault();

                var jsonString = JsonConvert.SerializeObject(subContractors);

                return Json(jsonString);
            }
            else
            {
                var jsonString = JsonConvert.SerializeObject(new
                {
                    CountryofOrigin = "",
                    ScopeofWork = "",
                    ContactDetails = "",
                    CICRegistrationNo = ""
                });

                return Json(jsonString);
            }
        }

        public JsonResult GetAllBusinesses()
        {
            var businesses = new List<Business>();

            memoryCache.TryGetValue("ListOfBusinesses", out businesses);

            var jsonString = JsonConvert.SerializeObject(businesses);

            return Json(jsonString);
        }
    }
}
