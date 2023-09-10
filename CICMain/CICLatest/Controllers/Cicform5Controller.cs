using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{

    public class Cicform5Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        Cicf5Model form5Model = new Cicf5Model();
        Businessdetails5 businessModel = new Businessdetails5();
        private readonly ApplicationContext _context;
        CustomValidations cv = new CustomValidations();
        int cnt = 1;
        static string filepath = "NA";
        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        private readonly IMemoryCache memoryCache;
        private readonly UserManager<UserModel> _userManager;
        public readonly IAppSettingsReader _appSettingsReader;
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;

        public Cicform5Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
            , UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            _context = context;
            _azureConfig = azureConfig;
            this.memoryCache = memoryCache;
            _userManager = userManager;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }
        public Cicf5Model loadData(Cicf5Model m, int CategoryID = 0)
        {
           

            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category
                            where Category.FormType == "F4"
                            select Category).ToList();


            List<categoryType> categoryTypeList = new List<categoryType>();

            for (int i = 0; i < categorylist.Count; i++)
            {


                    categoryTypeList.Add(new categoryType { CategoryID = categorylist[i].CategoryID, CategoryName = categorylist[i].CategoryName});

                
            }

            // businessModel.Category = categoryTypeList;
            m.Category = categoryTypeList;
        
            m.businessdetails5 = businessModel;

            //for subcatogory 
            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            subCategorylist = (from SubCategoryType in _context.SubCategory
                               where SubCategoryType.CategoryID == CategoryID
                               select SubCategoryType).ToList();


            List<subCategory> sub = new List<subCategory>();

            for (int i = 0; i < subCategorylist.Count; i++)
            {
                sub.Add(new subCategory { SubCategoryID = subCategorylist[i].SubCategoryID, SubCategoryName = subCategorylist[i].SubCategoryName });
            }
            m.subCategoryModel = sub;
            // if()
            ViewBag.ListSubCategory = sub;



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
            form5Model.subCategory= subCategorylist; ;
            //businessModel.subCategory = subCategorylist;
            //form1Model.businessModel = businessModel;
            return Json(new SelectList(subCategorylist, "SubCategoryID", "SubCategoryName"));
        }
        public ActionResult CicForm5()
        {
            Cicf5Model form5EditModel = new Cicf5Model();
            bool isExist = memoryCache.TryGetValue("Form5", out form5EditModel);

            ViewBag.type = "active";
            form5Model.firsGrid = cnt;
            form5Model.secondGrid = cnt;
            if (isExist)
            {
                form5Model = form5EditModel;

               
            }

            if (!isExist)
            {
                form5Model.detailOfProjects = new List<DetailOfProjects>();
                form5Model.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0});
                form5Model.subConsultantDetail = new List<SubConsultantDetail>();
                form5Model.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0 });
                form5Model = loadData(form5Model);
            }

            //form5Model.firsGrid = cnt;
            //form5Model.secondGrid = cnt;

            //form5Model.detailOfProjects = new List<DetailOfProjects>();
            //form5Model.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "" , CICRegistrationNo ="", Shareholding =0, AttchedDoc =null});
            //form5Model.subConsultantDetail = new List<SubConsultantDetail>();
            //form5Model.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "" , DescriptionOfWork="", ContractValueOfWork="" });

            //ViewBag.type = "active";

            //form5Model = loadData(form5Model);

            return View(form5Model);
        }
        public void fileDefault(Cicf5Model p)
        {
            p.BusineesParticularsfile1 = (p.BusineesParticularsfile1 != "") ? p.BusineesParticularsfile1 : "-";
            p.BusineesParticularsfile2 = (p.BusineesParticularsfile2 != "") ? p.BusineesParticularsfile2 : "-";
            p.Signature = (p.Signature != "") ? p.Signature : "-";
            p.Signature1 = (p.Signature1 != "") ? p.Signature1 : "-";

            //for null

            p.BusineesParticularsfile1= (p.BusineesParticularsfile1 != null) ? p.BusineesParticularsfile1 : "-";
            p.BusineesParticularsfile2 = (p.BusineesParticularsfile2 != null) ? p.BusineesParticularsfile2 : "-";
            p.Signature = (p.Signature != null) ? p.Signature : "-";
            p.Signature1 = (p.Signature1 != null) ? p.Signature1 : "-";


        }
        public void setGetFileEdit(Cicf5Model p)
        {
            if (p.doc5.BusineesParticularsfile1 != null)
            {
                memoryCache.Set("BusineesParticularsfile1", p.doc5.BusineesParticularsfile1);
                p.BusineesParticularsfile1 = p.doc5.BusineesParticularsfile1.FileName;
                memoryCache.Set("BusineesParticularsfile1", p.doc5.BusineesParticularsfile1.FileName);
            }
            else
            {
                p.BusineesParticularsfile1 = SetandGetFileEdit("BusineesParticularsfile1");
            }
            if (p.doc5.BusineesParticularsfile2 != null)
            {
                memoryCache.Set("BusineesParticularsfile2", p.doc5.BusineesParticularsfile2);
                p.BusineesParticularsfile2 = p.doc5.BusineesParticularsfile2.FileName;
                memoryCache.Set("BusineesParticularsfile2", p.doc5.BusineesParticularsfile2.FileName);
            }
            else
            {
                p.BusineesParticularsfile2 = SetandGetFileEdit("BusineesParticularsfile2");
            }
            if (p.doc5.Signature != null)
            {
                memoryCache.Set("Signature", p.doc5.Signature);
                p.Signature = p.doc5.Signature.FileName;
                memoryCache.Set("Signature", p.doc5.Signature.FileName);
            }
            else
            {
                p.Signature = SetandGetFileEdit("Signature");
            }

            if (p.declaration5.Signature != null)
            {
                memoryCache.Set("Signature1", p.declaration5.Signature);
                p.Signature1 = p.declaration5.Signature.FileName;
                memoryCache.Set("Signature1", p.declaration5.Signature.FileName);
            }
            else
            {
                p.Signature1 = SetandGetFileEdit("Signature1");
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
        public Cicf5Model SetandGetFileAdd(Cicf5Model p)
        {
            IFormFile fsign;
            //string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("Filesignature", out fsign);


            if (!isExist)
            {
                if (p.doc5.BusineesParticularsfile1 != null)
                {
                    memoryCache.Set("BusineesParticularsfile1", p.doc5.BusineesParticularsfile1);
                    p.BusineesParticularsfile1 = p.doc5.BusineesParticularsfile1.FileName;
                }
            }
            else
            {
                if (p.doc5.BusineesParticularsfile1 != null)
                {
                    memoryCache.Set("BusineesParticularsfile1", p.doc5.BusineesParticularsfile1);
                }
                else
                {
                    p.doc5.BusineesParticularsfile1 = fsign;
                }
                p.BusineesParticularsfile1 = p.doc5.BusineesParticularsfile1.FileName;

            }
            isExist = memoryCache.TryGetValue("BusineesParticularsfile2", out fsign);


            if (!isExist)
            {
                if (p.doc5.BusineesParticularsfile2 != null)
                {
                    memoryCache.Set("BusineesParticularsfile2", p.doc5.BusineesParticularsfile2);
                    p.BusineesParticularsfile2 = p.doc5.BusineesParticularsfile2.FileName;
                }
            }
            else
            {
                if (p.doc5.BusineesParticularsfile2 != null)
                {
                    memoryCache.Set("BusineesParticularsfile2", p.doc5.BusineesParticularsfile2);
                }
                else
                {
                    p.doc5.BusineesParticularsfile2 = fsign;
                }
                p.BusineesParticularsfile2 = p.doc5.BusineesParticularsfile2.FileName;

            }

            isExist = memoryCache.TryGetValue("Signature", out fsign);

            if (!isExist)
            {
                if (p.doc5.Signature != null)
                {
                    memoryCache.Set("Signature", p.doc5.Signature);
                    p.Signature = p.doc5.Signature.FileName;
                }
            }
            else
            {
                if (p.doc5.Signature != null)
                {
                    memoryCache.Set("Signature", p.doc5.Signature);
                }
                else
                {
                    p.doc5.Signature = fsign;
                }
                p.Signature = p.doc5.Signature.FileName;

            }
            isExist = memoryCache.TryGetValue("Signature1", out fsign);

            if (!isExist)
            {
                if (p.declaration5.Signature != null)
                {
                    memoryCache.Set("Signature1", p.declaration5.Signature);
                    p.Signature1 = p.declaration5.Signature.FileName;
                }
            }
            else
            {
                if (p.declaration5.Signature != null)
                {
                    memoryCache.Set("Signature1", p.declaration5.Signature);
                }
                else
                {
                    p.declaration5.Signature = fsign;
                }
                p.Signature1 = p.declaration5.Signature.FileName;

            }


            return p;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm5(Cicf5Model p, string name, string next, string pre)
        {

            if (p.formval != "Edit")
            {
                p.ImagePath = "-";
            }

            if (p.formval == "Edit")
            {
                setGetFileEdit(p);
            }
            else
            {
                SetandGetFileAdd(p);
            }

           

            if (p.businessdetails5.FaxNo == null)
            {
                p.businessdetails5.FaxNo = "-";
            }
            if (p.businessdetails5.Fax == null)
            {
                p.businessdetails5.Fax = "-";
            }

            if (p.formval == "Edit")
            {
                if (p.detailOfProjects != null)
                {
                    p.firsGrid = p.detailOfProjects.Count() + 1;
                   

                }
                //else
                //{
                //    p.firsGrid = cnt;
                //    p.detailOfProjects = new List<DetailOfProjects>();
                //    p.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0, PartitionKey = "-", RowKey = "-" });
                //}

                if (p.subConsultantDetail != null)
                {
                    p.secondGrid = p.subConsultantDetail.Count() + 1;

                }
                //else
                //{
                //    p.secondGrid = cnt;
                //    p.subConsultantDetail = new List<SubConsultantDetail>();
                //    p.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = "", PartitionKey = "-", RowKey = "-" });
                //}

            }
            else
            {
                if (p.detailOfProjects != null)
                {
                    p.firsGrid = p.detailOfProjects.Count() + 1;
                    for (int i = 0; i < p.detailOfProjects.Count; i++)
                    {
                        p.detailOfProjects[i].PartitionKey = "-";
                        p.detailOfProjects[i].RowKey = "-";
                    }

                }
                else
                {
                    p.firsGrid = cnt;
                    p.detailOfProjects = new List<DetailOfProjects>();
                    p.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0, PartitionKey = "-", RowKey = "-" });
                }

                if (p.subConsultantDetail != null)
                {
                    p.secondGrid = p.subConsultantDetail.Count() + 1;

                    for (int i = 0; i < p.subConsultantDetail.Count; i++)
                    {
                        p.subConsultantDetail[i].PartitionKey = "-";
                        p.subConsultantDetail[i].RowKey = "-";
                    }

                }
                else
                {
                    p.secondGrid = cnt;
                    p.subConsultantDetail = new List<SubConsultantDetail>();
                    p.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0, PartitionKey = "-", RowKey = "-" });
                }


            }
            //Grid section
            
            fileDefault(p);
            //p.FormRegistrationNo = getRegNo(p);

            p.FormRegistrationNo = GenericHelper.GetRegNo(p.FormRegistrationNo, p.formval, _azureConfig); //AK

            CustomValidations cv = new CustomValidations();
            switch (name)
            {
                case "type":
                    bool AppFlag =cv.IsAnyNullOrEmpty(p.App);
                    if (AppFlag == true)
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


                    bool DFlag = cv.IsAnyNullOrEmpty(p.businessdetails5);
                    bool DFlag1 = BusinessModelvalidations(p);
                   
                        

                    if (DFlag == true || DFlag1==true)
                    {
                        ViewBag.business = "active";
                    }
                    else if (DFlag == false && next != null)
                    {
                        ViewBag.fin = "active";
                    }
                    else if (DFlag == false && pre != null)
                    {
                        ViewBag.type = "active";
                    }
                    UploadBlob(p, p.FormRegistrationNo);
                    break;


                case "finance":

                    //bool FFlag = cv.IsAnyNullOrEmpty(p.subConsultantDetail);
                    //bool aFlag = cv.IsAnyNullOrEmpty(p.detailOfProjects);

                    bool E = checkTab3(p, next, pre);
                    if (E == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    else if(E==false && next!=null)
                    {

                        ViewBag.work = "active";
                    }
                   else  if (E == false && pre != null)
                    {

                        ViewBag.business = "active";
                    }

                    //if ((FFlag == false && aFlag == false) && next != null)
                    //{
                    //    ViewBag.work = "active";
                    //}

                    //if ((FFlag == false && aFlag == false) && pre != null)
                    //{
                    //    ViewBag.business = "active";
                    //}
                    UploadBlob(p, p.FormRegistrationNo);

                    break;


                case "work":
                  
                    bool WFlag = cv.IsAnyNullOrEmpty(p.doc5);

                    //if (WFlag == true)
                    //{
                    //    ViewBag.work = "active";
                    //}

                    //else if (WFlag == false && pre != null)
                    //{
                    //    ViewBag.fin = "active";
                    //}
                    bool docFlag1 = DocModelValidation(p);

                    if (WFlag == true || docFlag1 == true)
                    {
                        ViewBag.work = "active";
                    }
                    else if (WFlag == false && pre != null)
                    {
                        ViewBag.fin = "active";
                    }




                    UploadBlob(p, p.FormRegistrationNo);

                    break;

                case "draft":

                    p.Reviewer = "";
                    p.FormStatus = "Draft";
                    UploadBlob(p, p.FormRegistrationNo);
                    string result3 = Savedata(p);
                    removeDatafromSession();


                    return RedirectToAction("Form5Result", "Cicform5", new { result = result3, text = "Draft" });
                case "final":
                    fileDefault(p);
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    if (A == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }

                    //bool B = cv.IsAnyNullOrEmpty(p.businessdetails5);
                    //if (B == true)
                    //{
                    //    ViewBag.business = "active";
                    //    break;
                    //}

                    bool B = cv.IsAnyNullOrEmpty(p.businessdetails5);
                    bool B1 = BusinessModelvalidations(p);
                    if (B == true || B1 == true)
                    {
                        ViewBag.business = "active";
                        break;
                    }


                    //  bool C = cv.IsAnyNullOrEmpty(p.doc5);

                    bool fin = checkTab3(p, next, pre);
                    if (fin == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }

                    //adding final condition
                    bool D = DocModelValidation(p);
                    if (D == true)
                    {
                        ViewBag.work = "active";
                        break;
                    }

                    ModelState.Clear();

                    if (ModelState.IsValid)
                    {
                        p.Reviewer = "Clerk";
                        p.FormStatus = "Submit";
                        UploadBlob(p, p.FormRegistrationNo);
                        string result4 = Savedata(p);
                        removeDatafromSession();

                        return RedirectToAction("Form5Result", "Cicform5", new { result = result4, text = "Final" });
                    }
                    else
                    {
                        ViewBag.work = "active";
                    }
                    break;

                    //if (C == true)
                    //{
                    //    ViewBag.work = "active";
                    //    break;
                    //}

                    //else
                    //{

                    //    p.Reviewer = "Clerk";
                    //    p.FormStatus = "Submit";
                    //    UploadBlob(p, p.FormRegistrationNo);
                    //    string result4 = Savedata(p);
                    //    removeDatafromSession();

                    //    return RedirectToAction("Form5Result", "Cicform5", new { result = result4, text = "Final" });
                    //}


            }
            // loadData(p);
            int categoryId = 0;
            if (p.WorkDiscipline != null)
            {
                categoryId = GetCategoryId(p.WorkDiscipline);
            }

            loadData(p, categoryId);


            return View(p);
        }
        public bool DocModelValidation(Cicf5Model p)
        {
            bool AppFlag = false;
            if (p.doc5.Name == null)
            {
                AppFlag = true;
                ModelState.AddModelError("doc5.Name", "Please enter Name");
            }
            if (p.formval != "Edit")
            {
                if (p.doc5.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc5.Signature", "Please upload signature");
                }
            }
            else
            {
                if (p.doc5.Signature == null && p.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("doc5.Signature", "Please upload file");
                }
            }
            if (p.doc5.TitleDesignation == null)
            {
                AppFlag = true;
                ModelState.AddModelError("doc5.TitleDesignation ", "Please enter title");
            }

            if (p.doc5.TermsAndConditions == false)
            {
                AppFlag = true;
                ModelState.AddModelError("doc5.TermsAndConditions", "Please accept Terms and conditions");
            }

            return AppFlag;
        }
        public bool checkTab3(Cicf5Model p, string next, string pre)
        {
          
            bool aFlag = false;
            bool bFlag = false;

           
            


            if (p.detailOfProjects != null)
            {
                if (p.detailOfProjects.Count > 0)
                {
                    for (int i = 0; i < p.detailOfProjects.Count; i++)
                    {

                        if (p.detailOfProjects.Count >= 1)
                        {
                            if (p.detailOfProjects[i].NameofApplicant == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.detailOfProjects[i].CountryOfOrigin == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.detailOfProjects[i].ContactDetails == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.detailOfProjects[i].CICRegistrationNo == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.detailOfProjects[i].Shareholding == 0)
                            {
                                aFlag = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (p.subConsultantDetail != null)
            {
                if (p.subConsultantDetail.Count > 0)
                {
                    for (int i = 0; i < p.subConsultantDetail.Count; i++)
                    {

                        if (p.subConsultantDetail.Count >= 1)
                        {
                            if (p.subConsultantDetail[i].NameofConsultant == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.subConsultantDetail[i].CountryyofOrigin == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.subConsultantDetail[i].CICRegistrationNo == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.subConsultantDetail[i].DescriptionOfWork == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.subConsultantDetail[i].ContractValueOfWork == null)
                            {
                                bFlag = true;
                                break;
                            }
                        }
                    }
                }
            }

            if ( (aFlag == false && bFlag == false) && next != null)
            {
               ViewBag.work = "active";

                return false;

            }
            if (( aFlag == false && bFlag == false) && pre != null)
            {

                ViewBag.business = "active";
                return false;
            }
            if (aFlag == false && bFlag == false)
            {

                ViewBag.work = "active";
                return false;
            }
            else
            {

                ViewBag.fin = "active";

                return true;
            }

        }


        public void removeDatafromSession()
        {
            memoryCache.Remove("Form5");
            memoryCache.Remove("BusineesParticularsfile1");
            memoryCache.Remove("BusineesParticularsfile2");
            memoryCache.Remove("Signature1");
            memoryCache.Remove("Signature");

           



        }
        public int GetCategoryId(string CategoryName)
              {
            //List<Category> categorylist = new List<Category>();
            int categoryId = (from Category in _context.Category
                              where (Category.CategoryName == CategoryName  && Category.FormType== "F4")
                              select Category.CategoryID).FirstOrDefault();


            return categoryId;
           }


        public void UploadBlob(Cicf5Model p1, int tempMax)
        {
            if (p1.ImagePath == "NA" || p1.ImagePath == "-")
            {
               // p1.ImagePath = "PRN" + tempMax; //AK
                p1.ImagePath = "Form" + tempMax;

            }

            //addding new upload 

            if (p1.doc5.BusineesParticularsfile2 != null)
            {
                uploadFiles1(p1.doc5.BusineesParticularsfile2, p1.ImagePath, "BusineesParticularsfile2");

            }
            if (p1.declaration5.Signature != null)
            {
                uploadFiles1(p1.declaration5.Signature, p1.ImagePath, "Signature1");
            }
            if (p1.doc5.Signature != null)
            {
                uploadFiles1(p1.doc5.Signature, p1.ImagePath, "Signature");
            }
            if (p1.doc5.BusineesParticularsfile1 != null)
            {
                uploadFiles1(p1.doc5.BusineesParticularsfile1, p1.ImagePath, "BusineesParticularsfile1");
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
            return filepath;
        }
        public int getRegNo(Cicf5Model p)
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
            }

            return tempMax;

        }
        public IActionResult Form5Result(string result, string text)
        {
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            string body = "", subject = "", emailto = "";
            ViewBag.Result = result;
            ViewBag.sts = text;
            ViewBag.yr = yr;
            var domain = _appSettingsReader.Read("Domain");
            if (text == "Draft")
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year "+ yr +" CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='"+ domain + "'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been saved as draft";
            }
            else
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been successfully submitted. To view your application status, please log in <a href='" + domain + "'>CIC Portal</a> and view your dashboard. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been successfully submitted";
            }
            memoryCache.TryGetValue("emailto", out emailto);
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager, _appSettingsReader, _blobStorageService);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
        }
        public IActionResult CicformReview()
        {
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            ViewBag.yr = yr;
            return View();
        }

        public string Savedata(Cicf5Model p1)
        {
            string response = "";
            SaveModelForm5 saveModelForm5 = new SaveModelForm5();
            string TableName = "CicForm5()";
            //For edit 
            string FormRegNo = "";
            int tempMax;
            if (p1.formval == "Edit")
            {

                tempMax = p1.FormRegistrationNo;
                saveModelForm5.PartitionKey = p1.PartitionKey;
                saveModelForm5.RowKey = p1.RowKey;
                saveModelForm5.FormStatus = p1.FormStatus;
                FormRegNo = p1.RowKey;
            }
            else
            {
                //string jsonData;
                //AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform", out jsonData);//Get data

                //JObject myJObject = JObject.Parse(jsonData);
                //int cntJson = myJObject["value"].Count();
                //int tempRegNo;

                //tempMax = (int)myJObject["value"][0]["ProjectRegistrationNo"]; ;
                //for (int i = 0; i < cntJson; i++)
                //{

                //    tempRegNo = (int)myJObject["value"][i]["ProjectRegistrationNo"];
                //    if (tempRegNo > tempMax)
                //    {
                //        tempMax = tempRegNo;
                //    }
                //}
                tempMax = GenericHelper.GetRegNo(p1.FormRegistrationNo, p1.formval, _azureConfig);

                //Adding new rEgistration no 
                AddNewRegistrationNo addNew = new AddNewRegistrationNo();

                addNew.PartitionKey = tempMax.ToString();
               // addNew.RowKey = "PRN" + tempMax.ToString(); //AK
                addNew.RowKey = "Form" + tempMax.ToString();
                addNew.ProjectRegistrationNo = tempMax.ToString();

                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));
                p1.ImagePath = "Form" + tempMax; //AK
               
                

                saveModelForm5.FormRegistrationNo = tempMax;
                if(p1.businessdetails5.NameOFJoinVenture == null)
                {
                    saveModelForm5.PartitionKey = "-";
                }
                else
                {
                    saveModelForm5.PartitionKey = p1.businessdetails5.NameOFJoinVenture;
                }
                
                saveModelForm5.RowKey = "Form" + tempMax.ToString(); //AK

                FormRegNo = "Form" + tempMax.ToString(); ;

            }

            saveModelForm5.FormRegistrationNo = tempMax;
            saveModelForm5.FormName = "Form5";

            saveModelForm5.AppType = p1.App.AppType;
            //
            saveModelForm5.NameOFJoinVenture=p1.businessdetails5.NameOFJoinVenture;
            saveModelForm5.TypeofJoointVenture = p1.businessdetails5.TypeofJoointVenture;
            saveModelForm5.Telephone = p1.businessdetails5.Telephone;
            saveModelForm5.Fax = p1.businessdetails5.Fax;
            saveModelForm5.Email = p1.businessdetails5.Email;
            saveModelForm5.Phyaddress = p1.businessdetails5.Phyaddress;
            saveModelForm5.FirstName = p1.businessdetails5.FirstName;
            saveModelForm5.NameType = p1.businessdetails5.NameType;
            saveModelForm5.SurName = p1.businessdetails5.SurName;
            saveModelForm5.Designation = p1.businessdetails5.Designation;
            saveModelForm5.BusinessTelephone = p1.businessdetails5.BusinessTelephone;
            saveModelForm5.FaxNo = p1.businessdetails5.FaxNo;
            saveModelForm5.MobileNo = p1.businessdetails5.MobileNo;
            saveModelForm5.BusinessEmail = p1.businessdetails5.BusinessEmail;
            saveModelForm5.DateofRegistration = p1.businessdetails5.DateofRegistration;
            saveModelForm5.TaxIdentityNo = p1.businessdetails5.TaxIdentityNo;
            saveModelForm5.FormStatus = p1.FormStatus;
            saveModelForm5.Reviewer = p1.Reviewer.Trim();
            saveModelForm5.CreatedBy = User.Identity.Name;
            saveModelForm5.Category = p1.WorkDiscipline;
            //saveModelForm5.CategoryId = p1.businessdetails5.CategoryId;
            saveModelForm5.CustNo = HttpContext.Session.GetString("CustNo");
            memoryCache.Set("emailto", User.Identity.Name);
            saveModelForm5.SubcatogoryId = p1.selectedsubcategory.ToString(); 
            if (p1.doc5.TermsAndConditions==true)
            {
                saveModelForm5.customerCheck = 0;
            }
           else
            {
                saveModelForm5.customerCheck = 1;
            }
            saveModelForm5.Namee = p1.doc5.Name;

            p1.ImagePath = "Form" + tempMax;
            //addding new upload 
            //if (p1.doc5.Signature!=null)
            //{
            //    uploadFiles(p1.doc5.Signature, p1.ImagePath, "Signature");
            //}

            saveModelForm5.TitleDesignation = p1.doc5.TitleDesignation;
            //saveModelForm5.TitleDesignation = p1.doc5.TitleDesignation;

            //if(p1.doc5.BusineesParticularsfile1!=null)
            //{
            //    uploadFiles(p1.doc5.BusineesParticularsfile1, p1.ImagePath, "BusineesParticularsfile1");
            //}
            //if(p1.doc5.BusineesParticularsfile2 != null)
            //{
            //    uploadFiles(p1.doc5.BusineesParticularsfile2, p1.ImagePath, "BusineesParticularsfile2");

            //}
            saveModelForm5.WitnessedName = p1.declaration5.Name;
            //if (p1.declaration5.Signature!=null)
            //{
            //    uploadFiles(p1.declaration5.Signature, p1.ImagePath, "Signature1");
            //}
           
            saveModelForm5.WitnessedTitleDesignation = p1.declaration5.TitleDesignation;
           
            //adding for path
            if (filepath != "NA")
            {
                saveModelForm5.ImagePath = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                   
                    saveModelForm5.ImagePath = _appSettingsReader.Read("ImagePath") + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }
            if (p1.formval == "Edit")
            {

             
                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm5", JsonConvert.SerializeObject(saveModelForm5, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), saveModelForm5.PartitionKey, saveModelForm5.RowKey);
            }

            else
            {
                saveModelForm5.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5", JsonConvert.SerializeObject(saveModelForm5, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            }
          
            if (response == "Created" || response == "NoContent")
            {
                DetailOfProjects detailOfProjects = new DetailOfProjects();
                for (int i = 0; i < p1.detailOfProjects.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(p1.detailOfProjects[i]);
                    DFlag = false;
                    if (DFlag == false)
                    {
                        string Data;
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm5ProjectDetails", p1.detailOfProjects[i].PartitionKey, p1.detailOfProjects[i].RowKey, out Data);
                      

                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();


                        if (cntJson1 != 0)
                        {
                            // AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm5ProjectDetails", p1.detailOfProjects[i].PartitionKey, p1.detailOfProjects[i].RowKey, Data);
                            detailOfProjects.PartitionKey = p1.detailOfProjects[i].PartitionKey;
                            detailOfProjects.RowKey = p1.detailOfProjects[i].RowKey;

                            detailOfProjects.NameofApplicant = p1.detailOfProjects[i].NameofApplicant;
                            detailOfProjects.CountryOfOrigin = p1.detailOfProjects[i].CountryOfOrigin;
                            detailOfProjects.ContactDetails = p1.detailOfProjects[i].ContactDetails;
                            detailOfProjects.CICRegistrationNo = p1.detailOfProjects[i].CICRegistrationNo;
                            detailOfProjects.Shareholding = p1.detailOfProjects[i].Shareholding;
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm5ProjectDetails", JsonConvert.SerializeObject(detailOfProjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), detailOfProjects.PartitionKey, detailOfProjects.RowKey);

                        }
                        else
                        {
                            detailOfProjects.PartitionKey = p1.detailOfProjects[i].NameofApplicant;
                            detailOfProjects.RowKey = "Form" + tempMax.ToString(); //AK

                            detailOfProjects.NameofApplicant = p1.detailOfProjects[i].NameofApplicant;
                            detailOfProjects.CountryOfOrigin = p1.detailOfProjects[i].CountryOfOrigin;
                            detailOfProjects.ContactDetails = p1.detailOfProjects[i].ContactDetails;
                            detailOfProjects.CICRegistrationNo = p1.detailOfProjects[i].CICRegistrationNo;
                            detailOfProjects.Shareholding = p1.detailOfProjects[i].Shareholding;
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5ProjectDetails", JsonConvert.SerializeObject(detailOfProjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                        }


                    }

                }
                //for second grid 
                SubConsultantDetail subConsultantDetail = new SubConsultantDetail();
                for (int i = 0; i < p1.subConsultantDetail.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(p1.subConsultantDetail[i]);
                    DFlag = false;
                    if (DFlag == false)
                    {
                        string Data;
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm5SubconsultantDetails", p1.subConsultantDetail[i].PartitionKey, p1.subConsultantDetail[i].RowKey, out Data);


                        JObject myJObject1 = JObject.Parse(Data);
                        int cntJson1 = myJObject1["value"].Count();

                        if (cntJson1 != 0)
                        {

                            subConsultantDetail.PartitionKey = p1.subConsultantDetail[i].PartitionKey;
                            subConsultantDetail.RowKey = p1.subConsultantDetail[i].RowKey;

                            subConsultantDetail.NameofConsultant = p1.subConsultantDetail[i].NameofConsultant;
                            subConsultantDetail.CountryyofOrigin = p1.subConsultantDetail[i].CountryyofOrigin;
                            subConsultantDetail.CICRegistrationNo = p1.subConsultantDetail[i].CICRegistrationNo;
                            subConsultantDetail.DescriptionOfWork = p1.subConsultantDetail[i].DescriptionOfWork;
                            subConsultantDetail.ContractValueOfWork = p1.subConsultantDetail[i].ContractValueOfWork;
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", JsonConvert.SerializeObject(subConsultantDetail, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), subConsultantDetail.PartitionKey, subConsultantDetail.RowKey);
                            //AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", p1.subConsultantDetail[i].PartitionKey, p1.subConsultantDetail[i].RowKey, Data);

                        }
                        else
                        {

                            subConsultantDetail.PartitionKey = p1.subConsultantDetail[i].NameofConsultant;
                            subConsultantDetail.RowKey = "Form" + tempMax.ToString(); //AK

                            subConsultantDetail.NameofConsultant = p1.subConsultantDetail[i].NameofConsultant;
                            subConsultantDetail.CountryyofOrigin = p1.subConsultantDetail[i].CountryyofOrigin;
                            subConsultantDetail.CICRegistrationNo = p1.subConsultantDetail[i].CICRegistrationNo;
                            subConsultantDetail.DescriptionOfWork = p1.subConsultantDetail[i].DescriptionOfWork;
                            subConsultantDetail.ContractValueOfWork = p1.subConsultantDetail[i].ContractValueOfWork;
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", JsonConvert.SerializeObject(subConsultantDetail, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                        }

                    }
                    }

                }
            //add new comment
            //if (response == "Created")
            //{
            //    //Saving data for first section
            //    DetailOfProjects detailOfProjects = new DetailOfProjects();
            //    for (int i = 0; i < p1.detailOfProjects.Count; i++)
            //    {
            //        detailOfProjects.PartitionKey = p1.detailOfProjects[i].NameofApplicant;
            //        detailOfProjects.RowKey = "PRN"+tempMax.ToString();
            //        detailOfProjects.NameofApplicant = p1.detailOfProjects[i].NameofApplicant;
            //        detailOfProjects.CountryOfOrigin = p1.detailOfProjects[i].CountryOfOrigin;
            //        detailOfProjects.ContactDetails = p1.detailOfProjects[i].ContactDetails;
            //        detailOfProjects.CICRegistrationNo = p1.detailOfProjects[i].CICRegistrationNo;
            //        detailOfProjects.Shareholding = p1.detailOfProjects[i].Shareholding;
            //        if (p1.detailOfProjects[i].AttchedDoc != null)
            //        {
            //            //detailOfProjects.AttchedDoc = p1.detailOfProjects[i].AttchedDoc;
            //            uploadFiles(p1.detailOfProjects[i].AttchedDoc, p1.ImagePath, "AttchedDoc");
            //        }

            //        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5ProjectDetails", JsonConvert.SerializeObject(detailOfProjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            //    }


            //    SubConsultantDetail subConsultantDetail = new SubConsultantDetail();
            //    for (int i = 0; i < p1.subConsultantDetail.Count; i++)
            //    {
            //        subConsultantDetail.PartitionKey = p1.subConsultantDetail[i].NameofConsultant;
            //        subConsultantDetail.RowKey = "PRN"+tempMax.ToString();

            //        subConsultantDetail.NameofConsultant = p1.subConsultantDetail[i].NameofConsultant;
            //        subConsultantDetail.CountryyofOrigin = p1.subConsultantDetail[i].CountryyofOrigin;
            //        subConsultantDetail.CICRegistrationNo = p1.subConsultantDetail[i].CICRegistrationNo;
            //        subConsultantDetail.DescriptionOfWork = p1.subConsultantDetail[i].DescriptionOfWork;
            //        subConsultantDetail.ContractValueOfWork = p1.subConsultantDetail[i].ContractValueOfWork;
            //        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm5SubconsultantDetails", JsonConvert.SerializeObject(subConsultantDetail, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            //    }
            //}

            // return response;
            
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5", FormRegNo, out jsonData1);//Get data
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
            accessToken = viewForm1.GetAccessToken();
            for (int i = 0; i < cntJson2; i++)
                updateContractDetails(myJObject2, i);

            return FormRegNo;

        }
        public bool BusinessModelvalidations(Cicf5Model p)
        {
            bool DFlag = false;

            
            if (p.WorkDiscipline == null)
            {
                DFlag = true;
                ViewBag.WorkDiscipline = "Please select work discipline";
                ModelState.AddModelError("p.WorkDiscipline", "Please select work discipline");

            }
           
            if (p.selectedsubcategory == 0)
            {
                DFlag = true;
                ModelState.AddModelError("p.selectedsubcategory", "Please select sub category");
            }
            
            return DFlag;
        }
        public void uploadFiles(IFormFile tempFile, string path, string name)
        {
            #region Read File Content
            string fileName = Path.GetFileName(tempFile.FileName);
            string TempFilename = name + "_" + tempFile.FileName;
            byte[] fileData;
            using (var target = new MemoryStream())
            {
                tempFile.CopyTo(target);
                fileData = target.ToArray();
            }

            string mimeType = tempFile.ContentType;

            // path = objBlobService.UploadFileToBlob(tempFile.FileName, fileData, mimeType);


            filepath = _blobStorageService.UploadFileToBlob(TempFilename, fileData, mimeType, path);
            #endregion
        }
        public JsonResult GetSubCategory1(int CategoryID)
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

            form5Model.subCategoryModel = sub;
            return Json(form5Model.subCategoryModel);
        }
        public IActionResult IndexFromDashboard(string rowkey)
        {
            ViewBag.type = "active";
            Cicf5Model model = new Cicf5Model();

            string c = "";
          
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5", rowkey, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            //set master table values into model 
            for (int i = 0; i < cntJson; i++)
            {

                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                //adding for files 
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                 model.RowKey = fName;
                path = (string)myJObject["value"][i]["ImagePath"];

                string key, value;
                AllFileList = _blobStorageService.GetBlobList(path);
                string BusineesParticularsfile1 = null, BusineesParticularsfile2 = null,Signature=null, Signature1= null ;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;

                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);

                        switch (key)
                        {
                          
                            case "BusineesParticularsfile1": BusineesParticularsfile1 = AllFileList[j].FileValue; break;
                         
                            case "BusineesParticularsfile2": BusineesParticularsfile2 = AllFileList[j].FileValue; break;
                            case "Signature": Signature = AllFileList[j].FileValue; break;
                            case "Signature1": Signature1 = AllFileList[j].FileValue; break;

                        }
                    }
                }


                ApplicationType5 App = new ApplicationType5
                {


                    AppType = (string)myJObject["value"][i]["AppType"]
                   
                };
                model.App = App;
                //Businessdetails5 businessdetails5 = new Businessdetails5
                businessModel = new Businessdetails5
                   {


                    NameOFJoinVenture= (string)myJObject["value"][i]["NameOFJoinVenture"],
                   TypeofJoointVenture = (string)myJObject["value"][i]["TypeofJoointVenture"],
                   BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"],
                   BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"],
                   Email = (string)myJObject["value"][i]["Email"],
                   Phyaddress = (string)myJObject["value"][i]["Phyaddress"],
                   FirstName = (string)myJObject["value"][i]["FirstName"],
                   SurName = (string)myJObject["value"][i]["SurName"],
                   Designation = (string)myJObject["value"][i]["Designation"],
                  Telephone = (string)myJObject["value"][i]["Telephone"],
                    Fax = (string)myJObject["value"][i]["Fax"],
                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                 
                  MobileNo = (string)myJObject["value"][i]["MobileNo"],
                  DateofRegistration = (DateTime)myJObject["value"][i]["DateofRegistration"],
                  TaxIdentityNo = (string)myJObject["value"][i]["TaxIdentityNo"],
               
                 };

                Doc5 doc = new Doc5
                {
                    Name = (string)myJObject["value"][i]["Namee"],
                    TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"]
                };
                Declaration5 dec = new Declaration5
                {
                    Name = (string)myJObject["value"][i]["WitnessedName"],
                    TitleDesignation = (string)myJObject["value"][i]["WitnessedTitleDesignation"]
                };
                model.doc5 = doc;
                model.declaration5 = dec;
                model.businessdetails5 = businessModel;
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.RowKey = (string)myJObject["value"][i]["RowKey"];
                model.PartitionKey = (string)myJObject["value"][i]["PartitionKey"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
               
                model.WorkDiscipline = (string)myJObject["value"][i]["Category"];
                model.selectedsubcategory = (int)myJObject["value"][i]["SubcatogoryId"];

                model.BusineesParticularsfile1 = BusineesParticularsfile1;
                model.BusineesParticularsfile2 = BusineesParticularsfile2;
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.Signature = Signature;
                model.Signature1 = Signature1;

            }

            string jsonData1; 
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<DetailOfProjects> d = new List<DetailOfProjects>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DetailOfProjects
                {

                   
                 
                    CountryOfOrigin = (string)myJObject1["value"][i]["CountryOfOrigin"],
                    NameofApplicant = (string)myJObject1["value"][i]["NameofApplicant"],
                    CICRegistrationNo = (string)myJObject1["value"][i]["CICRegistrationNo"],
                    ContactDetails = (string)myJObject1["value"][i]["ContactDetails"],

                    Shareholding = (int)myJObject1["value"][i]["Shareholding"],
                    PartitionKey = (string)myJObject1["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject1["value"][i]["RowKey"]



                });
            }
            model.detailOfProjects = d;
            if (model.detailOfProjects.Count == 0)
            {
                model.firsGrid = 1;
                model.detailOfProjects = new List<DetailOfProjects>();
                model.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0});
               

            }
            else
            {
                model.firsGrid = model.detailOfProjects.Count;
            }

            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5SubconsultantDetails", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<SubConsultantDetail> a = new List<SubConsultantDetail>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new SubConsultantDetail
                {
                    CICRegistrationNo = (string)myJObject2["value"][i]["CICRegistrationNo"],
                    ContractValueOfWork = (decimal)myJObject2["value"][i]["ContractValueOfWork"],
                    CountryyofOrigin = (string)myJObject2["value"][i]["CountryyofOrigin"],
                    DescriptionOfWork = (string)myJObject2["value"][i]["DescriptionOfWork"],
                    NameofConsultant = (string)myJObject2["value"][i]["NameofConsultant"],
                    PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject2["value"][i]["RowKey"]


                });
            }
             model.subConsultantDetail = a;
            if (model.subConsultantDetail.Count == 0)
            {
                model.secondGrid = 1;
                model.subConsultantDetail = new List<SubConsultantDetail>();
                model.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0 });
            }
            else
            {
                model.secondGrid = model.subConsultantDetail.Count;
            }
            int categoryId = GetCategoryId(model.WorkDiscipline);
            
            loadData(model, categoryId);
            for (int k = 0; k < model.Category.Count; k++)
            {
                if (model.Category[k].CategoryName == c)
                {
                    model.Category[k].CategoryName = c;
                }
            }

            model.formval = "Edit";
            memoryCache.Set("Form5", model);

            return RedirectToAction("Cicform5", "Cicform5");


        }

        public IActionResult GetData(string apptype)
        {
            ViewBag.type = "active";
            Cicf5Model model = new Cicf5Model();

            string c = "";

            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntitybyLoginId(StorageName, StorageKey, "CicForm5", User.Identity.Name, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            var latestRecord = (from rec in myJObject["value"]
                                orderby (int)rec["FormRegistrationNo"] descending
                                select rec).FirstOrDefault();

            if(latestRecord !=null)
            {
                model.RowKey = (string)latestRecord["RowKey"];
                model.CustNo = (string)latestRecord["CustNo"];
                ApplicationType5 App = new ApplicationType5
                {
                    AppType = apptype

                };
                model.App = App;

                businessModel = new Businessdetails5
                {
                    NameOFJoinVenture = (string)latestRecord["NameOFJoinVenture"],
                    TypeofJoointVenture = (string)latestRecord["TypeofJoointVenture"],
                    BusinessTelephone = (string)latestRecord["BusinessTelephone"],
                    BusinessEmail = (string)latestRecord["BusinessEmail"],
                    Email = (string)latestRecord["Email"],
                    Phyaddress = (string)latestRecord["Phyaddress"],
                    FirstName = (string)latestRecord["FirstName"],
                    SurName = (string)latestRecord["SurName"],
                    Designation = (string)latestRecord["Designation"],
                    Telephone = (string)latestRecord["Telephone"],
                    Fax = (string)latestRecord["Fax"],
                    FaxNo = (string)latestRecord["FaxNo"],
                    MobileNo = (string)latestRecord["MobileNo"],
                    DateofRegistration = (DateTime)latestRecord["DateofRegistration"],
                    TaxIdentityNo = (string)latestRecord["TaxIdentityNo"],
                };

                Doc5 doc = new Doc5
                {
                    Name = (string)latestRecord["Namee"],
                    TitleDesignation = (string)latestRecord["TitleDesignation"]
                };
                Declaration5 dec = new Declaration5
                {
                    Name = (string)latestRecord["WitnessedName"],
                    TitleDesignation = (string)latestRecord["WitnessedTitleDesignation"]
                };
                model.doc5 = doc;
                model.declaration5 = dec;
                model.businessdetails5 = businessModel;
                model.WorkDiscipline = (string)latestRecord["Category"];
                model.selectedsubcategory = (int)latestRecord["SubcatogoryId"];

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", model.RowKey, out jsonData1);
                JObject myJObject1 = JObject.Parse(jsonData1);
                int cntJson1 = myJObject1["value"].Count();
                List<DetailOfProjects> d = new List<DetailOfProjects>();
                for (int i = 0; i < cntJson1; i++)
                {
                    d.Add(new DetailOfProjects
                    {
                        CountryOfOrigin = (string)myJObject1["value"][i]["CountryOfOrigin"],
                        NameofApplicant = (string)myJObject1["value"][i]["NameofApplicant"],
                        CICRegistrationNo = (string)myJObject1["value"][i]["CICRegistrationNo"],
                        ContactDetails = (string)myJObject1["value"][i]["ContactDetails"],
                        Shareholding = (int)myJObject1["value"][i]["Shareholding"],
                        PartitionKey = "-",
                        RowKey = "-"
                    });
                }
                model.detailOfProjects = d;
                if (model.detailOfProjects.Count == 0)
                {
                    model.firsGrid = 1;
                    model.detailOfProjects = new List<DetailOfProjects>();
                    model.detailOfProjects.Add(new DetailOfProjects { NameofApplicant = "", CountryOfOrigin = "", ContactDetails = "", CICRegistrationNo = "", Shareholding = 0 });
                }
                else
                {
                    model.firsGrid = model.detailOfProjects.Count;
                }

                string jsonData2;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform5SubconsultantDetails", model.RowKey, out jsonData2);
                JObject myJObject2 = JObject.Parse(jsonData2);
                int cntJson2 = myJObject2["value"].Count();
                List<SubConsultantDetail> a = new List<SubConsultantDetail>();
                for (int i = 0; i < cntJson2; i++)
                {

                    a.Add(new SubConsultantDetail
                    {
                        CICRegistrationNo = (string)myJObject2["value"][i]["CICRegistrationNo"],
                        ContractValueOfWork = (decimal)myJObject2["value"][i]["ContractValueOfWork"],
                        CountryyofOrigin = (string)myJObject2["value"][i]["CountryyofOrigin"],
                        DescriptionOfWork = (string)myJObject2["value"][i]["DescriptionOfWork"],
                        NameofConsultant = (string)myJObject2["value"][i]["NameofConsultant"],
                        PartitionKey = "-",
                        RowKey = "-"
                    });
                }
                model.subConsultantDetail = a;
                if (model.subConsultantDetail.Count == 0)
                {
                    model.secondGrid = 1;
                    model.subConsultantDetail = new List<SubConsultantDetail>();
                    model.subConsultantDetail.Add(new SubConsultantDetail { NameofConsultant = "", CountryyofOrigin = "", CICRegistrationNo = "", DescriptionOfWork = "", ContractValueOfWork = 0 });
                }
                else
                {
                    model.secondGrid = model.subConsultantDetail.Count;
                }
                int categoryId = GetCategoryId(model.WorkDiscipline);

                loadData(model, categoryId);
                for (int k = 0; k < model.Category.Count; k++)
                {
                    if (model.Category[k].CategoryName == c)
                    {
                        model.Category[k].CategoryName = c;
                    }
                }

                memoryCache.Set("Form5", model);
            }

            
            return RedirectToAction("Cicform5", "Cicform5");
        }

        public string updateContractDetails(JObject myJObject, int i)
        {
            string custno = (string)myJObject["value"][i]["RegistrationID"];
            try
            {
                var data1 = JObject.FromObject(new
                {
                    businessName = "",
                    certificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    monthofReg = (string)myJObject["value"][i]["CreatedDate"],
                    registration = (decimal)myJObject["value"][i]["RegistrationFee"],
                    renewal = (decimal)myJObject["value"][i]["RenewalFee"],
                    adminFee = (decimal)myJObject["value"][i]["AdminFee"]
                    
                });
                //penalty = 0,
                //    levy = 0,
                //    credit = 0,
                //    owing = 0,
                //    total = 0,
                //    dateofPay = "",
                //    typeofPay = "",
                //    bank = ""
                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    string BCUrl2 = _azureConfig.BCURL + "/customersContract1(" + custno + ")";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, json));
                    t.Wait();

                }
                return custno;
            }
            catch
            { return ""; }

        }

        static async Task<HttpResponseMessage> PatchData(Uri u, string json)
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

    }

}


