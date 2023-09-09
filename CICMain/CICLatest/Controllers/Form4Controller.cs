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
    public class Form4Controller : Controller
    {
        public string accessToken = "";
        private readonly ApplicationContext _context;
        CICForm4Model form4Model = new CICForm4Model();
        BusinessDetailsModel4 businessModel = new BusinessDetailsModel4();
        static string StorageName = "";      
        static string StorageKey = "";
        int cnt = 1;
        static string path = "";
        private readonly UserManager<UserModel> _userManager;
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");
        static string filepath = "NA";
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly IMemoryCache memoryCache;
        CustomValidations cv = new CustomValidations();
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public Form4Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
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

        public IActionResult CicForm4()
        {
            CICForm4Model form4EditModel = new CICForm4Model();

            bool isExist = memoryCache.TryGetValue("Form4", out form4EditModel);

            ViewBag.type = "active";
            form4Model.c = cnt;

            if (isExist)
            {
                form4Model = form4EditModel;
                
                List<AssociationList> AList = new List<AssociationList>();

                memoryCache.TryGetValue("listAssociation", out AList);
                ViewBag.ListofAssociation = AList;
            }

            if (!isExist)
            {
                form4Model.Sharelist = new List<DirectorshipShareDividends4>();
                form4Model.Sharelist.Add(new DirectorshipShareDividends4 { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", Qualifications = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });

                form4Model = loadData(form4Model);
                form4Model = loadData1(form4Model);
            }

            return View(form4Model);
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

        public void setGetFileEdit(CICForm4Model p)
        {
            if (p.App.Filesignature != null)
            {
                memoryCache.Set("Filesignature", p.App.Filesignature);
                p.App.signaturefilename = p.App.Filesignature.FileName;
                memoryCache.Set("Filesignature", p.App.Filesignature.FileName);
            }
            else
            {
                p.App.signaturefilename = SetandGetFileEdit("Filesignature");
            }

            if (p.businessModel.Businesssignature != null)
            {
                memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;
                memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
            }
            else
            {
                p.businessModel.BusinessFileSignatureName = SetandGetFileEdit("Businesssignature");
            }            

            if (p.docs.BusinessFile1 != null)
            {
                memoryCache.Set("BusinessFile1", p.docs.BusinessFile1);
                p.docs.BusinessFile1Name = p.docs.BusinessFile1.FileName;
                memoryCache.Set("BusinessFile1", p.docs.BusinessFile1Name);
            }
            else
            {
                p.docs.BusinessFile1Name = SetandGetFileEdit("BusinessFile1");
            }

            if (p.docs.BusinessFile2 != null)
            {
                memoryCache.Set("BusinessFile2", p.docs.BusinessFile2);
                p.docs.BusinessFile2Name = p.docs.BusinessFile2.FileName;
                memoryCache.Set("BusinessFile2", p.docs.BusinessFile2Name);
            }
            else
            {
                p.docs.BusinessFile2Name = SetandGetFileEdit("BusinessFile2");
            }

            if (p.docs.BusinessFile3 != null)
            {
                memoryCache.Set("BusinessFile3", p.docs.BusinessFile3);
                p.docs.BusinessFile3Name = p.docs.BusinessFile3.FileName;
                memoryCache.Set("BusinessFile3", p.docs.BusinessFile3Name);
            }
            else
            {
                p.docs.BusinessFile3Name = SetandGetFileEdit("BusinessFile3");
            }

            if (p.docs.BusinessFile4 != null)
            {
                memoryCache.Set("BusinessFile4", p.docs.BusinessFile4);
                p.docs.BusinessFile4Name = p.docs.BusinessFile4.FileName;
                memoryCache.Set("BusinessFile4", p.docs.BusinessFile4Name);
            }
            else
            {
                p.docs.BusinessFile4Name = SetandGetFileEdit("BusinessFile4");
            }

            if (p.docs.BusinessFile5 != null)
            {
                memoryCache.Set("BusinessFile5", p.docs.BusinessFile5);
                p.docs.BusinessFile5Name = p.docs.BusinessFile5.FileName;
                memoryCache.Set("BusinessFile5", p.docs.BusinessFile5Name);
            }
            else
            {
                p.docs.BusinessFile5Name = SetandGetFileEdit("BusinessFile5");
            }

            if (p.docs.BusinessFile6 != null)
            {
                memoryCache.Set("BusinessFile6", p.docs.BusinessFile6);
                p.docs.BusinessFile6Name = p.docs.BusinessFile6.FileName;
                memoryCache.Set("BusinessFile6", p.docs.BusinessFile6Name);
            }
            else
            {
                p.docs.BusinessFile6Name = SetandGetFileEdit("BusinessFile6");
            }

            if (p.docs.BusinessFile7 != null)
            {
                memoryCache.Set("BusinessFile7", p.docs.BusinessFile7);
                p.docs.BusinessFile7Name = p.docs.BusinessFile7.FileName;
                memoryCache.Set("BusinessFile7", p.docs.BusinessFile7Name);
            }
            else
            {
                p.docs.BusinessFile7Name = SetandGetFileEdit("BusinessFile7");
            }

            if (p.docs.ShareholdersFile1 != null)
            {
                memoryCache.Set("ShareholdersFile1", p.docs.ShareholdersFile1);
                p.docs.ShareholdersFile1Name = p.docs.ShareholdersFile1.FileName;
                memoryCache.Set("ShareholdersFile1", p.docs.ShareholdersFile1Name);
            }
            else
            {
                p.docs.ShareholdersFile1Name = SetandGetFileEdit("ShareholdersFile1");
            }

            if (p.docs.Signature1 != null)
            {
                memoryCache.Set("Signature1", p.docs.Signature1);
                p.docs.Signature1Name = p.docs.Signature1.FileName;
                memoryCache.Set("Signature1", p.docs.Signature1Name);
            }
            else
            {
                p.docs.Signature1Name = SetandGetFileEdit("Signature1");
            }

            if (p.docs.Signature2 != null)
            {
                memoryCache.Set("Signature2", p.docs.Signature2);
                p.docs.Signature2Name = p.docs.Signature2.FileName;
                memoryCache.Set("Signature2", p.docs.Signature2Name);
            }
            else
            {
                p.docs.Signature2Name = SetandGetFileEdit("Signature2");
            }

            if (p.docs.TaxLaw != null)
            {
                memoryCache.Set("TaxLaw", p.docs.TaxLaw);
                p.docs.TaxLawName = p.docs.TaxLaw.FileName;
                memoryCache.Set("TaxLaw", p.docs.TaxLawName);
            }
            else
            {
                p.docs.TaxLawName = SetandGetFileEdit("TaxLaw");
            }


            if (p.docs.Evidence != null)
            {
                memoryCache.Set("Evidence", p.docs.Evidence);
                p.docs.EvidenceName = p.docs.Evidence.FileName;
                memoryCache.Set("Evidence", p.docs.EvidenceName);
            }
            else
            {
                p.docs.EvidenceName = SetandGetFileEdit("Evidence");
            }


            if (p.docs.Compliance != null)
            {
                memoryCache.Set("Compliance", p.docs.Compliance);
                p.docs.ComplianceName = p.docs.Compliance.FileName;
                memoryCache.Set("Compliance", p.docs.ComplianceName);
            }
            else
            {
                p.docs.ComplianceName = SetandGetFileEdit("Compliance");
            }
        }

        public CICForm4Model SetandGetFileAdd(CICForm4Model p)
        {
            IFormFile fsign;
            //string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("Filesignature", out fsign);


            if (!isExist)
            {
                if (p.App.Filesignature != null)
                {
                    memoryCache.Set("Filesignature", p.App.Filesignature);
                    p.App.signaturefilename = p.App.Filesignature.FileName;
                }
            }
            else
            {
                if (p.App.Filesignature != null)
                {
                    memoryCache.Set("Filesignature", p.App.Filesignature);
                }
                else
                {
                    p.App.Filesignature = fsign;
                }
                p.App.signaturefilename = p.App.Filesignature.FileName;

            }


            isExist = memoryCache.TryGetValue("Businesssignature", out fsign);
            if (!isExist)
            {
                if (p.businessModel.Businesssignature != null)
                {
                    memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                    p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;
                }
            }
            else
            {
                if (p.businessModel.Businesssignature != null)
                {
                    memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                }
                else
                {
                    p.businessModel.Businesssignature = fsign;
                }
                p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;

            }

            
            //////document
            isExist = memoryCache.TryGetValue("BusinessFile1", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile1 != null)
                {
                    memoryCache.Set("BusinessFile1", p.docs.BusinessFile1);
                    p.docs.BusinessFile1Name = p.docs.BusinessFile1.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile1 != null)
                {
                    memoryCache.Set("BusinessFile1", p.docs.BusinessFile1);
                }
                else
                {
                    p.docs.BusinessFile1 = fsign;
                }
                p.docs.BusinessFile1Name = p.docs.BusinessFile1.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile2", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile2 != null)
                {
                    memoryCache.Set("BusinessFile2", p.docs.BusinessFile2);
                    p.docs.BusinessFile2Name = p.docs.BusinessFile2.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile2 != null)
                {
                    memoryCache.Set("BusinessFile2", p.docs.BusinessFile2);
                }
                else
                {
                    p.docs.BusinessFile2 = fsign;
                }
                p.docs.BusinessFile2Name = p.docs.BusinessFile2.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile3", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile3 != null)
                {
                    memoryCache.Set("BusinessFile3", p.docs.BusinessFile3);
                    p.docs.BusinessFile3Name = p.docs.BusinessFile3.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile3 != null)
                {
                    memoryCache.Set("BusinessFile3", p.docs.BusinessFile3);
                }
                else
                {
                    p.docs.BusinessFile3 = fsign;
                }
                p.docs.BusinessFile3Name = p.docs.BusinessFile3.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile4", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile4 != null)
                {
                    memoryCache.Set("BusinessFile4", p.docs.BusinessFile4);
                    p.docs.BusinessFile4Name = p.docs.BusinessFile4.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile4 != null)
                {
                    memoryCache.Set("BusinessFile4", p.docs.BusinessFile4);
                }
                else
                {
                    p.docs.BusinessFile4 = fsign;
                }
                p.docs.BusinessFile4Name = p.docs.BusinessFile4.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile5", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile5 != null)
                {
                    memoryCache.Set("BusinessFile5", p.docs.BusinessFile5);
                    p.docs.BusinessFile5Name = p.docs.BusinessFile5.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile5 != null)
                {
                    memoryCache.Set("BusinessFile5", p.docs.BusinessFile5);
                }
                else
                {
                    p.docs.BusinessFile5 = fsign;
                }
                p.docs.BusinessFile5Name = p.docs.BusinessFile5.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile6", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile6 != null)
                {
                    memoryCache.Set("BusinessFile6", p.docs.BusinessFile6);
                    p.docs.BusinessFile6Name = p.docs.BusinessFile6.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile6 != null)
                {
                    memoryCache.Set("BusinessFile6", p.docs.BusinessFile6);
                }
                else
                {
                    p.docs.BusinessFile6 = fsign;
                }
                p.docs.BusinessFile6Name = p.docs.BusinessFile6.FileName;

            }

            isExist = memoryCache.TryGetValue("BusinessFile7", out fsign);
            if (!isExist)
            {
                if (p.docs.BusinessFile7 != null)
                {
                    memoryCache.Set("BusinessFile7", p.docs.BusinessFile7);
                    p.docs.BusinessFile7Name = p.docs.BusinessFile7.FileName;
                }
            }
            else
            {
                if (p.docs.BusinessFile7 != null)
                {
                    memoryCache.Set("BusinessFile7", p.docs.BusinessFile7);
                }
                else
                {
                    p.docs.BusinessFile7 = fsign;
                }
                p.docs.BusinessFile7Name = p.docs.BusinessFile7.FileName;

            }

            isExist = memoryCache.TryGetValue("ShareholdersFile1", out fsign);
            if (!isExist)
            {
                if (p.docs.ShareholdersFile1 != null)
                {
                    memoryCache.Set("ShareholdersFile1", p.docs.ShareholdersFile1);
                    p.docs.ShareholdersFile1Name = p.docs.ShareholdersFile1.FileName;
                }
            }
            else
            {
                if (p.docs.ShareholdersFile1 != null)
                {
                    memoryCache.Set("ShareholdersFile1", p.docs.ShareholdersFile1);
                }
                else
                {
                    p.docs.ShareholdersFile1 = fsign;
                }
                p.docs.ShareholdersFile1Name = p.docs.ShareholdersFile1.FileName;

            }

            isExist = memoryCache.TryGetValue("Signature1", out fsign);
            if (!isExist)
            {
                if (p.docs.Signature1 != null)
                {
                    memoryCache.Set("Signature1", p.docs.Signature1);
                    p.docs.Signature1Name = p.docs.Signature1.FileName;
                }
            }
            else
            {
                if (p.docs.Signature1 != null)
                {
                    memoryCache.Set("Signature1", p.docs.Signature1);
                }
                else
                {
                    p.docs.Signature1 = fsign;
                }
                p.docs.Signature1Name = p.docs.Signature1.FileName;

            }

            isExist = memoryCache.TryGetValue("Signature2", out fsign);
            if (!isExist)
            {
                if (p.docs.Signature2 != null)
                {
                    memoryCache.Set("Signature2", p.docs.Signature2);
                    p.docs.Signature2Name = p.docs.Signature2.FileName;
                }
            }
            else
            {
                if (p.docs.Signature2 != null)
                {
                    memoryCache.Set("Signature2", p.docs.Signature2);
                }
                else
                {
                    p.docs.Signature2 = fsign;
                }
                p.docs.Signature2Name = p.docs.Signature2.FileName;

            }

            isExist = memoryCache.TryGetValue("TaxLaw", out fsign);
            if (!isExist)
            {
                if (p.docs.TaxLaw != null)
                {
                    memoryCache.Set("TaxLaw", p.docs.TaxLaw);
                    p.docs.TaxLawName = p.docs.TaxLaw.FileName;
                }
            }
            else
            {
                if (p.docs.TaxLaw != null)
                {
                    memoryCache.Set("TaxLaw", p.docs.TaxLaw);
                }
                else
                {
                    p.docs.TaxLaw = fsign;
                }
                p.docs.TaxLawName = p.docs.TaxLaw.FileName;

            }

            isExist = memoryCache.TryGetValue("Evidence", out fsign);
            if (!isExist)
            {
                if (p.docs.Evidence != null)
                {
                    memoryCache.Set("Evidence", p.docs.Evidence);
                    p.docs.EvidenceName = p.docs.Evidence.FileName;
                }
            }
            else
            {
                if (p.docs.Evidence != null)
                {
                    memoryCache.Set("Evidence", p.docs.Evidence);
                }
                else
                {
                    p.docs.Evidence = fsign;
                }
                p.docs.EvidenceName = p.docs.Evidence.FileName;

            }

            isExist = memoryCache.TryGetValue("Compliance", out fsign);
            if (!isExist)
            {
                if (p.docs.Compliance != null)
                {
                    memoryCache.Set("Compliance", p.docs.Compliance);
                    p.docs.ComplianceName = p.docs.Compliance.FileName;
                }
            }
            else
            {
                if (p.docs.Compliance != null)
                {
                    memoryCache.Set("Compliance", p.docs.Compliance);
                }
                else
                {
                    p.docs.Compliance = fsign;
                }
                p.docs.ComplianceName = p.docs.Compliance.FileName;

            }


            return p;
        }

        
        public int GetSubCategoryIdbyName(string subcategoryName)
        {
            //int id = Convert.ToInt32(subcategoryId);
            int subId = 0;
            subId = (from SubCategoryType in _context.SubCategory
                       where SubCategoryType.SubCategoryName == subcategoryName
                       select SubCategoryType.SubCategoryID).FirstOrDefault();

            return subId;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm4(CICForm4Model p, string name, string next, string pre)
        {
            
            string FormStatus = "";
            //p.businessModel.Other = "Other";
            bool isValid;
            p.FormName = "Form4";

           
            if (p.formval == "Edit")
            {
                setGetFileEdit(p);
            }
            else
            {
                SetandGetFileAdd(p);
            }

            if (p.Sharelist != null)
            {
                p.c = p.Sharelist.Count() + 1;

                for (int i = 0; i < p.Sharelist.Count; i++)
                {                    
                    p.Sharelist[i].PartitionKey = "-";
                    p.Sharelist[i].RowKey = "-";

                }
            }
            else
            {
                p.c = cnt;
                p.Sharelist = new List<DirectorshipShareDividends4>();
                p.Sharelist.Add(new DirectorshipShareDividends4 { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", Qualifications = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
            }           

            setDefault(p);
            fileDefault(p);
            bool checkshares = ShareValidation(p.Sharelist);

            if (checkshares)
            {
                p.businessModel.BusinessType = "ForeignCompany";
                ViewBag.BType = "checked";
            }

            p.FormRegistrationNo = GenericHelper.GetRegNo(p.FormRegistrationNo, p.formval, _azureConfig);

            switch (name)
            {
                case "type":

                    bool AppFlag = cv.IsAnyNullOrEmpty(p.App);
                    bool AppFlag1 = AppModelValidations(p);
                    if (AppFlag == true || AppFlag1 == true)
                    {
                        ViewBag.type = "active";
                    }
                    else if (AppFlag == false && next != null)
                    {
                        ViewBag.business = "active";
                    }
                    UploadBlob(p, p.FormRegistrationNo);
                    break;

                case "business":

                    bool BFlag = cv.IsAnyNullOrEmpty(p.businessModel);

                    bool DFlag = false;
                    DFlag = BusinessModelvalidations(p);

                    
                    if (BFlag == true || DFlag == true)
                    {
                        ViewBag.business = "active";
                    }
                    else if (BFlag == false && next != null)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (BFlag == false && pre != null)
                    {
                        ViewBag.type = "active";
                    }
                    UploadBlob(p, p.FormRegistrationNo);
                    break;


                case "doc":
                    bool docFlag = cv.IsAnyNullOrEmpty(p.docs);
                    bool docFlag1 = DocModelValidation(p);
                    
                    if (docFlag == true || docFlag1== true)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.business = "active";
                    }
                    UploadBlob(p, p.FormRegistrationNo);
                    break;

                case "draft":
                    p.FormStatus = "Draft";
                    UploadBlob(p, p.FormRegistrationNo);
                    string result = Savedata(p);
                    removeDatafromSession();
                    return RedirectToAction("CicformReview", "Form4", new { result = result, text = "Draft" });
                //break;

                case "final":
                    ModelState.Remove("p.App.ImagePath");
                    ModelState.Remove("p.businessModel.FaxNo");
                    ModelState.Remove("p.businessModel.BusinessRepresentativeFax");
                    ModelState.Remove("p.FormStatus");
                    ModelState.Remove("p.err");
                    ModelState.Remove("p.PartitionKey");
                    ModelState.Remove("p.RowKey");
                    ModelState.Remove("p.Grade");
                    setDefault(p);
                    fileDefault(p);
                    UploadBlob(p, p.FormRegistrationNo);
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    bool A1 = AppModelValidations(p);
                    if (A == true || A1 == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }
                    

                    bool B = cv.IsAnyNullOrEmpty(p.businessModel);
                    bool B1 = BusinessModelvalidations(p);
                    if (B == true || B1== true)
                    {
                        ViewBag.business = "active";
                        break;
                    }

                    bool D = DocModelValidation(p);
                    if (D == true)
                    {
                        ViewBag.doc = "active";
                        break;
                    }
                  
                    ModelState.Clear();

                    if (ModelState.IsValid)
                    {
                        p.FormStatus = "Submit";
                        p.Reviewer = "Clerk";                       
                        string result1 = Savedata(p);
                        removeDatafromSession();
                        return RedirectToAction("CicformReview", "Form4", new { result = result1, text = "Final" });
                    }
                    else
                    {
                        ViewBag.doc = "active";
                    }
                    break;
            }

            
            string category = p.businessModel.SelectedCategoryValue;
            int categoryId = GetCategoryId(category);

            loadData(p);
            
            loadData1(p, categoryId);

            
            if (p.formval == "Edit")
            {
                if (p.businessModel.Businesssignature != null)
                {
                    memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                    p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;
                    memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                }
                else
                {
                    p.businessModel.BusinessFileSignatureName = SetandGetFileEdit("Businesssignature");
                }
            }
            else
            {
                IFormFile fsign;
                bool isExist = memoryCache.TryGetValue("Businesssignature", out fsign);
                if (!isExist)
                {
                    if (p.businessModel.Businesssignature != null)
                    {
                        memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                        p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;
                    }
                }
                else
                {
                    if (p.businessModel.Businesssignature != null)
                    {
                        memoryCache.Set("Businesssignature", p.businessModel.Businesssignature);
                    }
                    else
                    {
                        p.businessModel.Businesssignature = fsign;
                    }
                    p.businessModel.BusinessFileSignatureName = p.businessModel.Businesssignature.FileName;

                }
            }
            if (p.App.Filesignature != null)
            {
                ModelState.Remove("App.Filesignature");
            }
            if (p.businessModel.Businesssignature != null)
            {
                ModelState.Remove("businessModel.Businesssignature");
            }

             checkshares = ShareValidation(p.Sharelist);

            if (checkshares)
            {
                p.businessModel.BusinessType = "ForeignCompany";
                ViewBag.BType = "checked";
            }


            return View(p);


        }

        public int GetCategoryId(string CategoryName)
        {
            //List<Category> categorylist = new List<Category>();
            int categoryId = (from Category in _context.Category
                            where Category.CategoryName == CategoryName
                            select Category.CategoryID).FirstOrDefault();

            
            return categoryId;
        }

        public void fileDefault(CICForm4Model p)
        {
            p.App.signaturefilename = (p.App.signaturefilename != "") ? p.App.signaturefilename : "-";
            p.businessModel.BusinessFileSignatureName = (p.businessModel.BusinessFileSignatureName != "") ? p.businessModel.BusinessFileSignatureName : "-";            
            p.docs.BusinessFile1Name = (p.docs.BusinessFile1Name != "") ? p.docs.BusinessFile1Name : "-";
            p.docs.BusinessFile2Name = (p.docs.BusinessFile2Name != "") ? p.docs.BusinessFile2Name : "-";
            p.docs.BusinessFile3Name = (p.docs.BusinessFile3Name != "") ? p.docs.BusinessFile3Name : "-";
            p.docs.BusinessFile4Name = (p.docs.BusinessFile4Name != "") ? p.docs.BusinessFile4Name : "-";
            p.docs.BusinessFile5Name = (p.docs.BusinessFile5Name != "") ? p.docs.BusinessFile5Name : "-";
            p.docs.BusinessFile6Name = (p.docs.BusinessFile6Name != "" ) ? p.docs.BusinessFile6Name : "-";
            p.docs.BusinessFile7Name = (p.docs.BusinessFile7Name != "" ) ? p.docs.BusinessFile7Name : "-";
            p.docs.ShareholdersFile1Name = (p.docs.ShareholdersFile1Name != "" ) ? p.docs.ShareholdersFile1Name : "-";            
            p.docs.Signature1Name = (p.docs.Signature1Name != "" ) ? p.docs.Signature1Name : "-";
            p.docs.Signature2Name = (p.docs.Signature2Name != "" ) ? p.docs.Signature2Name : "-";
            p.docs.TaxLawName = (p.docs.TaxLawName != "" ) ? p.docs.TaxLawName : "-";
            p.docs.EvidenceName = (p.docs.EvidenceName != "" ) ? p.docs.EvidenceName : "-";
            p.docs.ComplianceName = (p.docs.ComplianceName != "" ) ? p.docs.ComplianceName : "-";
            p.App.signaturefilename = (p.App.signaturefilename != null) ? p.App.signaturefilename : "-";
            p.businessModel.BusinessFileSignatureName = (p.businessModel.BusinessFileSignatureName != null) ? p.businessModel.BusinessFileSignatureName : "-";
            p.docs.BusinessFile1Name = (p.docs.BusinessFile1Name != null) ? p.docs.BusinessFile1Name : "-";
            p.docs.BusinessFile2Name = (p.docs.BusinessFile2Name != null) ? p.docs.BusinessFile2Name : "-";
            p.docs.BusinessFile3Name = (p.docs.BusinessFile3Name != null) ? p.docs.BusinessFile3Name : "-";
            p.docs.BusinessFile4Name = (p.docs.BusinessFile4Name != null) ? p.docs.BusinessFile4Name : "-";
            p.docs.BusinessFile5Name = (p.docs.BusinessFile5Name != null) ? p.docs.BusinessFile5Name : "-";
            p.docs.BusinessFile6Name = (p.docs.BusinessFile6Name != null) ? p.docs.BusinessFile6Name : "-";
            p.docs.BusinessFile7Name = (p.docs.BusinessFile7Name != null) ? p.docs.BusinessFile7Name : "-";
            p.docs.ShareholdersFile1Name = (p.docs.ShareholdersFile1Name != null) ? p.docs.ShareholdersFile1Name : "-";
            p.docs.Signature1Name = (p.docs.Signature1Name != null) ? p.docs.Signature1Name : "-";
            p.docs.Signature2Name = (p.docs.Signature2Name != null) ? p.docs.Signature2Name : "-";
            p.docs.TaxLawName = (p.docs.TaxLawName != null) ? p.docs.TaxLawName : "-";
            p.docs.EvidenceName = (p.docs.EvidenceName != null) ? p.docs.EvidenceName : "-";
            p.docs.ComplianceName = (p.docs.ComplianceName != null) ? p.docs.ComplianceName : "-";
        }

        public IActionResult IndexFromDashboard(string rowkey)
        {
            ViewBag.type = "active";
            CICForm4Model model = new CICForm4Model();
            string c = "";            
            List<FileList> AllFileList = new List<FileList>();
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
                path = (string)myJObject["value"][i]["path"];
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
                string key, value;
                AllFileList = _blobStorageService.GetBlobList(path);
                string AppFile = null, Businesssignature = null, BusinessFile1 = null, BusinessFile2 = null, BusinessFile7 = null,
                    BusinessFile3 = null, BusinessFile4 = null, BusinessFile5 = null, BusinessFile6 = null, ShareholdersFile1 = null, Signature1 = null, Signature2 = null, TaxLaw = null,
                     Evidence = null, Compliance = null;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;
                        // AppFile = AllFileList[j].FileValue;
                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);


                        switch (key)
                        {
                            case "Filesignature": AppFile = AllFileList[j].FileValue; break;

                            case "Businesssignature": Businesssignature = AllFileList[j].FileValue; break;
                            
                            case "BusinessFile1": BusinessFile1 = AllFileList[j].FileValue; break;
                            case "BusinessFile2": BusinessFile2 = AllFileList[j].FileValue; break;
                            case "BusinessFile3": BusinessFile3 = AllFileList[j].FileValue; break;
                            case "BusinessFile4": BusinessFile4 = AllFileList[j].FileValue; break;
                            case "BusinessFile5": BusinessFile5 = AllFileList[j].FileValue; break;
                            case "BusinessFile6": BusinessFile6 = AllFileList[j].FileValue; break;
                            case "BusinessFile7": BusinessFile7 = AllFileList[j].FileValue; break;
                            case "ShareholdersFile1": ShareholdersFile1 = AllFileList[j].FileValue; break;                                                      
                            case "Signature1": Signature1 = AllFileList[j].FileValue; break;
                            case "Signature2": Signature2 = AllFileList[j].FileValue; break;
                            case "TaxLaw": TaxLaw = AllFileList[j].FileValue; break;
                            case "Evidence": Evidence = AllFileList[j].FileValue; break;
                            case "Compliance": Compliance = AllFileList[j].FileValue; break;
                        }
                    }
                }

                ApplicationTypeModel4 App = new ApplicationTypeModel4
                {
                    AppType = (string)myJObject["value"][i]["AppType"],
                    AssociationName = (string)myJObject["value"][i]["AssociationName"],
                    AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"],
                    signaturefilename = AppFile,
                    ImagePath = path
                };

                model.App = App;

                c = (string)myJObject["value"][i]["Category"];
                businessModel = new BusinessDetailsModel4
                {
                    BusinessName = (string)myJObject["value"][i]["BusinessName"],
                    TradingStyle = (string)myJObject["value"][i]["TradingStyle"],
                    BusinessType = (string)myJObject["value"][i]["BusinessType"],
                    CompanyRegistrationDate = (DateTime)myJObject["value"][i]["CompanyRegistrationDate"],
                    CompanyRegistrationPlace = (string)myJObject["value"][i]["CompanyRegistrationPlace"],
                    CompanyRegistrationNumber = (string)myJObject["value"][i]["CompanyRegistrationNumber"],
                    PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"],
                    CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"],
                    PostalAddress = (string)myJObject["value"][i]["PostalAddress"],
                    TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"],
                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                    Email = (string)myJObject["value"][i]["Email"],
                    BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"],
                    Other = (string)myJObject["value"][i]["Other"],
                    BusinessFileSignatureName = Businesssignature,
                    SelectedCategoryValue = (string)myJObject["value"][i]["Category"],
                    selectedsubcategory = (int)myJObject["value"][i]["subCategoryName"]
                };
                model.businessModel = businessModel;


                Documents4 doc = new Documents4
                {
                    BusinessFile1Name = BusinessFile1,
                    BusinessFile2Name = BusinessFile2,
                    BusinessFile3Name = BusinessFile3,
                    BusinessFile4Name = BusinessFile4,
                    BusinessFile5Name = BusinessFile5,
                    BusinessFile6Name = BusinessFile6,
                    BusinessFile7Name = BusinessFile7,
                    ComplianceName = Compliance,
                    EvidenceName = Evidence,                    
                    ShareholdersFile1Name = ShareholdersFile1,
                    Signature1Name = Signature1,
                    Signature2Name = Signature2,                    
                    TaxLawName = TaxLaw,
                    Name = (string)myJObject["value"][i]["Name"],
                    WitnessedName = (string)myJObject["value"][i]["WitnessedName"],
                    WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"],
                    Title = (string)myJObject["value"][i]["Title"]
                };
                model.docs = doc;
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
               // model.Grade = (string)myJObject["value"][i]["Grade"];
               model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];                
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
            if (model.Sharelist.Count == 0)
            {
                model.c = 1;
                model.Sharelist = new List<DirectorshipShareDividends4>();
                model.Sharelist.Add(new DirectorshipShareDividends4 { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", Qualifications = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.c = model.Sharelist.Count;
            }
            
            model = loadData(model);
            int categoryId = GetCategoryId(model.businessModel.SelectedCategoryValue);
            model = loadData1(model, categoryId);
            
            model.formval = "Edit";
            for (int k = 0; k < model.businessModel.Category.Count; k++)
            {
                if (model.businessModel.Category[k].CategoryName == c)
                {
                    model.businessModel.Category[k].Selected = true;
                }
            }

            List<AssociationList> AList = new List<AssociationList>();
            AList = ViewBag.ListofAssociation;
            memoryCache.Set("listAssociation", AList);
            memoryCache.Set("Form4", model);
            return RedirectToAction("CicForm4", "Form4");
        }

        public IActionResult GetData(string apptype)
        {
            ViewBag.type = "active";
            CICForm4Model model = new CICForm4Model();
            string c = "";
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntitybyLoginId(StorageName, StorageKey, "cicform4", User.Identity.Name, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            var latestRecord = (from rec in myJObject["value"]
                                orderby (int)rec["FormRegistrationNo"] descending
                                select rec).FirstOrDefault();

            if (latestRecord != null)
            {
                model.RowKey = (string)latestRecord["RowKey"];
                if (latestRecord["AdminFee"] != null)
                {
                    model.AdminFee = (int)latestRecord["AdminFee"];
                }
                if (latestRecord["RegistrationFee"] != null)
                {
                    model.RegistrationFee = (int)latestRecord["RegistrationFee"];
                }
                if (latestRecord["RenewalFee"] != null)
                {
                    model.RenewalFee = (int)latestRecord["RenewalFee"];
                }

                ApplicationTypeModel4 App = new ApplicationTypeModel4
                {
                    AppType = apptype,
                    AssociationName = (string)latestRecord["AssociationName"],
                    AuthorisedOfficerName = (string)latestRecord["AuthorisedOfficerName"]
                };

                model.App = App;

                c = (string)latestRecord["Category"];
                model.CustNo = (string)latestRecord["CustNo"];
                businessModel = new BusinessDetailsModel4
                {
                    BusinessName = (string)latestRecord["BusinessName"],
                    TradingStyle = (string)latestRecord["TradingStyle"],
                    BusinessType = (string)latestRecord["BusinessType"],
                    CompanyRegistrationDate = (DateTime)latestRecord["CompanyRegistrationDate"],
                    CompanyRegistrationPlace = (string)latestRecord["CompanyRegistrationPlace"],
                    CompanyRegistrationNumber = (string)latestRecord["CompanyRegistrationNumber"],
                    PhysicalAddress = (string)latestRecord["PhysicalAddress"],
                    CompanyHOPhysicalAddress = (string)latestRecord["CompanyHOPhysicalAddress"],
                    PostalAddress = (string)latestRecord["PostalAddress"],
                    TelephoneNumber = (string)latestRecord["TelephoneNumber"],
                    FaxNo = (string)latestRecord["FaxNo"],
                    Email = (string)latestRecord["Email"],
                    BusinessRepresentativeName = (string)latestRecord["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)latestRecord["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)latestRecord["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)latestRecord["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)latestRecord["BusinessRepresentativeEmail"],
                    Other = (string)latestRecord["Other"],
                    SelectedCategoryValue = (string)latestRecord["Category"],
                    selectedsubcategory = (int)latestRecord["subCategoryName"]
                };
                model.businessModel = businessModel;

                Documents4 doc = new Documents4
                {

                    Name = (string)latestRecord["Name"],
                    WitnessedName = (string)latestRecord["WitnessedName"],
                    WitnessedTitle = (string)latestRecord["WitnessedTitle"],
                    Title = (string)latestRecord["Title"]
                };
                model.docs = doc;
                model.FormName = (string)latestRecord["FormName"];

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", model.RowKey, out jsonData1);
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
                if (model.Sharelist.Count == 0)
                {
                    model.c = 1;
                    model.Sharelist = new List<DirectorshipShareDividends4>();
                    model.Sharelist.Add(new DirectorshipShareDividends4 { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", Qualifications = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.c = model.Sharelist.Count;
                }

                model = loadData(model);
                int categoryId = GetCategoryId(model.businessModel.SelectedCategoryValue);
                model = loadData1(model, categoryId);

                for (int k = 0; k < model.businessModel.Category.Count; k++)
                {
                    if (model.businessModel.Category[k].CategoryName == c)
                    {
                        model.businessModel.Category[k].Selected = true;
                    }
                }

                List<AssociationList> AList = new List<AssociationList>();
                AList = ViewBag.ListofAssociation;
                memoryCache.Set("listAssociation", AList);
                memoryCache.Set("Form4", model);
                
            }

            return RedirectToAction("CicForm4", "Form4");
        }

        void setDefault(CICForm4Model p)
        {
            //
            if (p.formval != "Edit")
            {
                p.App.ImagePath = "-";
                p.CreatedDate = "-";
            }

            if (p.businessModel.FaxNo == null)
            {
                p.businessModel.FaxNo = "-";
            }
            if (p.businessModel.BusinessRepresentativeFax == null)
            {
                p.businessModel.BusinessRepresentativeFax = "-";
            }
            if (p.businessModel.Other == null)
            {
                p.businessModel.Other = "-";
            }
        }

        bool validateIdNo(DirectorshipShareDividends4 p)
        {
            if (p.IdNO != null)
            {
                if (p.Nationnality == "Swazi")
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

        public void removeDatafromSession()
        {
            memoryCache.Remove("Form4");
            memoryCache.Remove("Filesignature");
            memoryCache.Remove("Businesssignature");            
            memoryCache.Remove("BusinessFile1");
            memoryCache.Remove("BusinessFile2");
            memoryCache.Remove("BusinessFile3");
            memoryCache.Remove("BusinessFile4");
            memoryCache.Remove("BusinessFile5");
            memoryCache.Remove("BusinessFile6");
            memoryCache.Remove("BusinessFile7");
            memoryCache.Remove("ShareholdersFile1");           
            memoryCache.Remove("Signature1");
            memoryCache.Remove("Signature2");
            memoryCache.Remove("TaxLaw");
            memoryCache.Remove("Evidence");
            memoryCache.Remove("Compliance");
        }
        public bool AppModelValidations(CICForm4Model p)
        {
            bool AppFlag = false;
            if (p.formval != "Edit")
            {
                if (p.App.Filesignature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.Filesignature", "Please upload file");
                }
            }
            else
            {
                if (p.App.signaturefilename == null && p.App.Filesignature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.Filesignature", "Please upload file");
                }
            }
            if (p.App.AssociationName == null)
            {
                AppFlag = true;
                ModelState.AddModelError("App.AssociationName", "Please select Association Name");
            }

            return AppFlag;
        }

        public bool BusinessModelvalidations(CICForm4Model p)
        {
            int tempPercentage = 0;
            bool DFlag = false;

            if (p.formval != "Edit")
            {
                if (p.businessModel.Businesssignature == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("businessModel.Businesssignature", "Please upload file");
                }
            }
            else
            {
                if (p.businessModel.BusinessFileSignatureName == null && p.businessModel.Businesssignature == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("businessModel.Businesssignature", "Please upload file");
                }
            }
            if (p.businessModel.SelectedCategoryValue == null)
            {
                DFlag = true;
                ModelState.AddModelError("businessModel.Category", "Please select category");
                
            }
            

            if (p.businessModel.selectedsubcategory == 0)
            {
                DFlag = true;
                ModelState.AddModelError("businessModel.selectedsubcategory", "Please select sub category");
            }
            bool DFlag1 = false;

            for (int i = 0; i < p.Sharelist.Count; i++)
            {
                DFlag1 = cv.IsAnyNullOrEmpty(p.Sharelist[i]);

                if (p.Sharelist[i].Nationnality == "Select Nationality")
                {
                    DFlag1 = true;
                    ModelState.AddModelError("Sharelist[" + i + "].Nationnality", "Please Select Nationality!");
                }

                if (p.Sharelist[i].Nationnality == "Swazi")
                {
                    if (validateIdNo(p.Sharelist[i]))
                    {
                        DFlag1 = true;
                        ModelState.AddModelError("Sharelist[" + i + "].IdNO", "Invalid Id number!");
                    }
                }

                if (ModelState["Sharelist[" + i + "].IdNO"] != null)
                {
                    if (ModelState["Sharelist[" + i + "].IdNO"].Errors.Any())
                    {
                        DFlag1 = true;
                    }
                }

                if (ModelState["Sharelist[" + i + "].SharePercent"] != null)
                {
                    if (ModelState["Sharelist[" + i + "].SharePercent"].Errors.Any())
                    {
                        DFlag1 = true;
                        ModelState.AddModelError("Sharelist[" + i + "].SharePercent", "Please use values between 1 to 100 for % Shares!");
                    }
                }
                tempPercentage = tempPercentage + p.Sharelist[i].SharePercent;
                if (tempPercentage > 100)
                {
                    DFlag1 = true;
                    ModelState.AddModelError("Sharelist[" + i + "].SharePercent", "Shares can not be greater than 100%");
                }
            }
            if(DFlag1)
            {
                DFlag = true;
            }
            return DFlag;
        }

        public bool DocModelValidation(CICForm4Model p)
        {
            bool AppFlag = false;
            if (p.docs.Name == null)
            {
                AppFlag = true;
                ModelState.AddModelError("docs.Name", "Please enter Name");
            }
            if (p.formval != "Edit")
            {
                if (p.docs.Signature1 == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("docs.Signature1", "Please upload signature");
                }
            }
            else
            {
                if (p.docs.Signature1 == null && p.docs.Signature1Name == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.Signature1", "Please upload file");
                }
            }
            if (p.docs.Title == null)
            {
                AppFlag = true;
                ModelState.AddModelError("docs.Title", "Please enter title");
            }

            if (p.docs.TermsAndConditions == false)
            {
                AppFlag = true;
                ModelState.AddModelError("docs.TermsAndConditions", "Please accept Terms and conditions");
            }

            return AppFlag;
        }
        public void UploadBlob(CICForm4Model model, int tempMax)
        {
            if (model.App.ImagePath == "NA" || model.App.ImagePath == "-")
            {
               // model.App.ImagePath = "PRN" + tempMax; //AK
                model.App.ImagePath = "Form" + tempMax;
            }


            uploadFiles1(model.App.Filesignature, model.App.ImagePath, "Filesignature");
            uploadFiles1(model.businessModel.Businesssignature, model.App.ImagePath, "Businesssignature");
            uploadFiles1(model.docs.BusinessFile1, model.App.ImagePath, "BusinessFile1");
            uploadFiles1(model.docs.BusinessFile2, model.App.ImagePath, "BusinessFile2");
            uploadFiles1(model.docs.BusinessFile3, model.App.ImagePath, "BusinessFile3");
            uploadFiles1(model.docs.BusinessFile4, model.App.ImagePath, "BusinessFile4");
            uploadFiles1(model.docs.BusinessFile5, model.App.ImagePath, "BusinessFile5");
            uploadFiles1(model.docs.BusinessFile6, model.App.ImagePath, "BusinessFile6");
            uploadFiles1(model.docs.BusinessFile7, model.App.ImagePath, "BusinessFile7");
            uploadFiles1(model.docs.ShareholdersFile1, model.App.ImagePath, "ShareholdersFile1");
            uploadFiles1(model.docs.Signature1, model.App.ImagePath, "Signature1");
            uploadFiles1(model.docs.Signature2, model.App.ImagePath, "Signature2");
            uploadFiles1(model.docs.TaxLaw, model.App.ImagePath, "TaxLaw");
            uploadFiles1(model.docs.Evidence, model.App.ImagePath, "Evidence");
            uploadFiles1(model.docs.Compliance, model.App.ImagePath, "Compliance");

            //for (int i = 0; i < model.Sharelist.Count; i++)
            //{
            //    string tempn = "ShareFile" + i.ToString();
            //    uploadFiles1(model.Sharelist[i].ShareFile, model.App.ImagePath, tempn);
            //}
        }

        public int getRegNo(CICForm4Model p)
        {
            int tempMax = 0;

            if (p.formval == "Edit")
            {
                tempMax = p.FormRegistrationNo;
            }
            else
            {
                var res = AzureTablesData.GetAllEntityWithContinuationToken(StorageName, StorageKey, "cicform");
                var firstPageObj = JObject.Parse(res.Data);

                var jTokens = new List<JToken> ();
                var nextPartitionKey = res.NextPartitionKey;
                var nextRowKey = res.NextRowKey;
                while (nextPartitionKey != null && nextRowKey != null)
                {
                    var response = AzureTablesData.GetEntitybyNextRowPartition(_azureConfig.StorageAccount, _azureConfig.StorageKey1, "cicform", res.NextPartitionKey, res.NextRowKey);

                    var nextPageObj = JObject.Parse(response.Data);
                    jTokens = firstPageObj["value"].Concat(nextPageObj["value"]).ToList();

                    nextPartitionKey = response.NextPartitionKey;
                    nextRowKey = response.NextRowKey;
                }
                
                //var finalObj = new JObject();

                int cntJson = jTokens.Count();
                int tempRegNo;

                if (cntJson != 0)
                {
                    tempMax = (int)jTokens[0]["ProjectRegistrationNo"];
                }


                for (int i = 0; i < cntJson; i++)
                {
                    tempRegNo = (int)jTokens[i]["ProjectRegistrationNo"];

                    if (tempRegNo > tempMax)
                    {
                        tempMax = tempRegNo;
                    }
                }
                tempMax++;
            }

            return tempMax;

        }

        public bool ShareValidation(List<DirectorshipShareDividends4> p)
        {
            int ForeignShare = 0, SwaziShare = 0;

            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].Nationnality != "Swazi")
                {
                    ForeignShare = p[i].SharePercent;
                }

                if (p[i].Nationnality == "Swazi")
                {
                    SwaziShare = SwaziShare+ p[i].SharePercent;
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



        public CICForm4Model loadData(CICForm4Model m)
        {

            List<tblAssociation> associationlist = new List<tblAssociation>();
            List<AssociationList> AList = new List<AssociationList>();

            associationlist = (from association in _context.tblAssociation
                               where association.formType == "Form4"
                               select association).ToList();

            AList.Add(new AssociationList { AssociationName = " ", AssociationName1 = "Select Association Name" });
            for (int i = 0; i < associationlist.Count; i++)
            {
                AList.Add(new AssociationList { AssociationName = associationlist[i].AssociationName, AssociationName1 = associationlist[i].AssociationName });
            }

            ViewBag.ListofAssociation = AList;

            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category
                            where Category.FormType == "F4"
                            select Category).ToList();


            List<categoryType> categoryTypeList = new List<categoryType>();

            for (int i = 0; i < categorylist.Count; i++)
            {
                categoryTypeList.Add(new categoryType { CategoryID = categorylist[i].CategoryID, CategoryName = categorylist[i].CategoryName });
            }

            businessModel.Category = categoryTypeList;

            
            m.businessModel = businessModel;
            return m;
        }

        public CICForm4Model loadData1(CICForm4Model m, int CategoryID = 0)
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
            m.businessModel = businessModel;
            return m;
        }

        public JsonResult GetSubCategory(int CategoryID)
        {
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

        public string Savedata(CICForm4Model model)
        {

            string TableName = "cicform4()";
            string response = "";
            Form4Model newModel = new Form4Model();

            Form4Wrapper k = new Form4Wrapper();
            int regNumber = model.FormRegistrationNo;
            // model.App.ImagePath = "PRN" + tempMax; //AK
            model.App.ImagePath = "Form" + regNumber;
            AddNewRegistrationNo addNew = new AddNewRegistrationNo();
            addNew.PartitionKey = regNumber.ToString();
            // addNew.RowKey = "PRN" + tempMax.ToString(); //AK
            addNew.RowKey = "Form" + regNumber.ToString();
            addNew.ProjectRegistrationNo = regNumber.ToString();
            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));
  
            if (filepath != "NA")
            {
                model.App.ImagePath = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    var imgPath = _appSettingsReader.Read("ImagePath");
                    model.App.ImagePath = imgPath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }
            model.CustNo = HttpContext.Session.GetString("CustNo");
            model.CreatedBy = User.Identity.Name;
            memoryCache.Set("emailto", User.Identity.Name);
            k.mapData(model, newModel, regNumber);
            string firmNo = newModel.RowKey;
            if (model.formval == "Edit")
            {
                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform4", JsonConvert.SerializeObject(newModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
            }
            else
            {
                newModel.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(newModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }

                
            if (response == "Created" || response == "NoContent")
            {
                TableName = "cicform1ShareDividends()";
                DirectorshipShareDividends4 d = new DirectorshipShareDividends4();
                for (int i = 0; i < model.Sharelist.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(model.Sharelist[i]);

                    if (DFlag == false)
                    {
                        string Data;
                        k.mapShareDetails(d, model.Sharelist[i], regNumber);
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1ShareDividends", d.PartitionKey, d.RowKey, out Data);

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1ShareDividends", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), d.PartitionKey, d.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                            //uploadFiles(model.Sharelist[i].ShareFile, model.App.ImagePath);
                            //string tempn = "ShareFile" + i.ToString();
                            //uploadFiles1(model.Sharelist[i].ShareFile, model.App.ImagePath, tempn);
                        }
                    }         
                    
                }
 
            }
            
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform4", firmNo, out jsonData1);//Get data
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            for (int i = 0; i < cntJson2; i++)
                AddCustinERP(myJObject2, i);
            return firmNo; ;
        }

        public void uploadFiles(IFormFile tempFile, string path)
        {
            if (tempFile != null)
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
                    filepath = _appSettingsReader.Read("ImagePath") + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + path;
                }
                else
                {
                    filepath = path;
                }
            }
            return filepath;
        }

        public IActionResult CicformReview(string result, string text)
        {
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            string body = "", subject = "", emailto = "";
            ViewBag.Result = result;
            ViewBag.yr = yr;
            ViewBag.sts = text;
            var domain = _appSettingsReader.Read("Domain");
            if (text == "Draft")
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='" + domain + "'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been saved as draft";
            }
            else
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been successfully submitted. To view your application status, please log in <a href='"+ domain +"'>CIC Portal</a> and view your dashboard. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been successfully submitted";
            }
            memoryCache.TryGetValue("emailto", out emailto);
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager, _appSettingsReader, _blobStorageService);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
        }

        public string AddCustinERP(JObject myJObject, int i)
        {
            string custno = (string)myJObject["value"][i]["RegistrationID"];
            try
            {
                var data1 = JObject.FromObject(new
                {
                    businessName = (string)myJObject["value"][i]["BusinessName"],
                    certificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    monthofReg = (string)myJObject["value"][i]["CreatedDate"],
                    registration = (decimal)myJObject["value"][i]["RegistrationFee"],
                    renewal = (decimal)myJObject["value"][i]["RenewalFee"],
                    adminFee = (decimal)myJObject["value"][i]["AdminFee"],
                    penalty = 0,
                    levy = 0,
                    credit = 0,
                    owing = 0,
                    total = 0,
                    dateofPay = "2022-07-19",
                    typeofPay = "Bank",
                    bank = "World"                   
                });
                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    GetAccessToken();
                    string BCUrl2 = _azureConfig.BCURL + "/customersContract1(" + custno + ")";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, json, accessToken));
                    t.Wait();

                }
                return custno;
            }
            catch
            { return ""; }

        }

        static async Task<HttpResponseMessage> PatchData(Uri u, string json,string accessToken)
        {
            
            HttpClient client1 = new HttpClient();
            client1.DefaultRequestHeaders.Clear();
            client1.DefaultRequestHeaders.Add("If-Match", "*");
            client1.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent c = new StringContent(json, Encoding.UTF8, "application/json");

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
    }
}
