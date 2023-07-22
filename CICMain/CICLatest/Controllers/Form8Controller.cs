using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
using CICLatest.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Net.Http.Headers;

namespace CICLatest.Controllers
{
    //[Authorize]
    public class Form8Controller : Controller
    {

        static string filepath = "NA";
        int cnt = 1;
        private readonly UserManager<UserModel> _userManager;
        MainViewModel main = new MainViewModel();

        static string StorageName = "";
        static string StorageKey = "";
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");

        private readonly ApplicationContext _context;
        //Adding variable for edit 

        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        BlobStorageService b = new BlobStorageService();
        private readonly IMemoryCache memoryCache;
        CustomValidations cv = new CustomValidations();

        public Form8Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache, UserManager<UserModel> userManager)
        {
            _context = context;
            _azureConfig = azureConfig;
            this.memoryCache = memoryCache;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _userManager = userManager;
        }

        // [HttpGet]
        public IActionResult CicForm8()
        {

            //MainViewModel obj = new MainViewModel();

            MainViewModel form1EditModel = new MainViewModel();
            bool isExist = memoryCache.TryGetValue("Form8", out form1EditModel);
            ViewBag.type = "active";

            main.Tab3Sec = cnt;
            main.Tab3Third = cnt;


            if (isExist)
            {
                List<Category> AList = new List<Category>();

                memoryCache.TryGetValue("ListofCategory", out AList);
                ViewBag.ListofCategory = AList;
                // string selectedcategory = "";
                //memoryCache.TryGetValue("SelectedCategory",  out selectedcategory);
                main = form1EditModel;
                //main.SelectedCategory = selectedcategory;

                //if (main.Tab3MainSection == null)
                //{
                //    main.Tab3MainSection = new List<Tab3FirstSection>();
                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Project Manager", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Architect", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (civil/structural)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (electrical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (mechanical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Quantity Surveyor", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                //    main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Other (specify)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });


                //}


            }
            if (!isExist)
            {
                main.Tab3MainSection = new List<Tab3FirstSection>();
                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Project Manager", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Architect", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (civil/structural)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (electrical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (mechanical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Quantity Surveyor", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Other (specify)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });



                main.tab3SecSection = new List<Tab3SecSection>();
                main.tab3SecSection.Add(new Tab3SecSection { NameofSubContractors = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                main.tab3ThirdSection = new List<Tab3ThirdSection>();
                main.tab3ThirdSection.Add(new Tab3ThirdSection { Supplier = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                LoadData(main);

            }

            return View(main); //--- Sending MainViewModel to CicForm5 view
        }

        public JsonResult GetSubCategory(int CategoryID)
        {
            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            //------- Getting data from database using EntityframeworkCore -------
            subCategorylist = (from subcategory in _context.SubCategory where subcategory.CategoryID == CategoryID select subcategory).ToList();

            //------- Inserting select items in list -------
            subCategorylist.Insert(0, new SubCategoryType { SubCategoryID = 0, SubCategoryName = "Select" });

            return Json(new SelectList(subCategorylist, "SubCategoryID", "SubCategoryName"));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm8(MainViewModel MainModel, string name, string next, string pre)
        {

            ModelState.Remove("tab2.FaxNo");
            ModelState.Remove("selectedsubcategory");


            if (MainModel.formval == "Edit")
            {

                setGetFileEdit(MainModel);

                if (MainModel.Tab3MainSection == null)
                {
                    MainModel.Tab3MainSection = new List<Tab3FirstSection>();
                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Project Manager", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Architect", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (civil/structural)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (electrical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Engineer (mechanical)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Quantity Surveyor", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });

                    MainModel.Tab3MainSection.Add(new Tab3FirstSection { Profession = "Other (specify)", CountryofOrigin = "", NameofConsultancyFirm = "", Contact = "", CicRegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });


                }
                if (MainModel.tab3SecSection == null)
                {
                    MainModel.Tab3Sec = cnt;
                    MainModel.tab3SecSection = new List<Tab3SecSection>();
                    MainModel.tab3SecSection.Add(new Tab3SecSection { NameofSubContractors = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });
                }

                if (MainModel.tab3ThirdSection == null)
                {
                    MainModel.Tab3Third = cnt;
                    MainModel.tab3ThirdSection = new List<Tab3ThirdSection>();
                    MainModel.tab3ThirdSection.Add(new Tab3ThirdSection { Supplier = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });
                }
            }
            else
            {
                removeDatafromSession();
                if (MainModel.Tab3MainSection != null)
                {
                    for (int i = 0; i < MainModel.Tab3MainSection.Count; i++)
                    {
                        MainModel.Tab3MainSection[i].PartitionKey = "-";
                        MainModel.Tab3MainSection[i].RowKey = "-";
                        MainModel.Tab3MainSection[i].FormRegistrationNo = 0;
                    }
                }
                //SetandGetFileAdd(p);

                if (MainModel.tab3SecSection != null)
                {
                    MainModel.Tab3Sec = MainModel.tab3SecSection.Count() + 1;
                    for (int i = 0; i < MainModel.tab3SecSection.Count; i++)
                    {
                        MainModel.tab3SecSection[i].PartitionKey = "-";
                        MainModel.tab3SecSection[i].RowKey = "-";
                        MainModel.tab3SecSection[i].FormRegistrationNo = 0;
                    }

                }
                else
                {
                    MainModel.Tab3Sec = cnt;
                    main.tab3SecSection = new List<Tab3SecSection>();
                    main.tab3SecSection.Add(new Tab3SecSection { NameofSubContractors = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });
                }
                if (MainModel.tab3ThirdSection != null)
                {
                    MainModel.Tab3Third = MainModel.tab3ThirdSection.Count() + 1;
                    for (int i = 0; i < MainModel.tab3ThirdSection.Count; i++)
                    {
                        MainModel.tab3ThirdSection[i].PartitionKey = "-";
                        MainModel.tab3ThirdSection[i].RowKey = "-";
                        MainModel.tab3ThirdSection[i].FormRegistrationNo = 0;
                    }

                }
                else
                {
                    MainModel.Tab3Third = cnt;
                    MainModel.tab3ThirdSection = new List<Tab3ThirdSection>();
                    MainModel.tab3ThirdSection.Add(new Tab3ThirdSection { Supplier = "", CountryofOrigin = "", ScopeofWork = "", ContactDetails = "", RegistrationNo = "", PartitionKey = "-", RowKey = "-", FormRegistrationNo = 0 });
                }
            }



            //Validations on ID 
            if (MainModel.tab2.IDNO != null)
            {
                if (MainModel.tab2.IDNO != "-")
                {
                    if (validateIdNo(MainModel.tab2.IDNO))
                    {
                        ModelState.AddModelError(nameof(MainModel.err), "Invalid Id number!");
                    }
                }
            }

            if (MainModel.SelectedCategory == "Select")
            {
                ViewBag.SelectedCategory = "Please select Category";
                //ViewBag.type = "active";
                //return;
            }

            if (MainModel.OwnerCategory == null)
                ViewBag.OwnerCategory = "Please select Owner Category";
            if (MainModel.WorkDiscipline == null)
                ViewBag.WorkDiscipline = "Please select work discipline";

            //if(MainModel.WorkDiscipline!=null)
            //{
            //    if (MainModel.WorkDiscipline != "General Buildings" || MainModel.WorkDiscipline != "General Civils" || MainModel.WorkDiscipline != "General Electrical" || MainModel.WorkDiscipline != "General Mechanical")
            //    {
            //        if (MainModel.selectedsubcategory == 0)
            //            ViewBag.selectedsubcategory = "Please select Category for  specialists categories";
            //    }

            //}
            //else
            //{
            //    ViewBag.selectedsubcategory = "";
            //}


            //Validation Part

            if (MainModel.tabb1.FaxNo == null)
            {
                MainModel.tabb1.FaxNo = "-";
            }
            if (MainModel.tab2.AuthorisedFaxNo == null)
            {
                MainModel.tab2.AuthorisedFaxNo = "-";
            }
            if (MainModel.tab2.FaxNo == null)
            {
                MainModel.tab2.FaxNo = "-";
            }
            //adding new 

            if (MainModel.tab2.IDNO == null)
            {
                MainModel.tab2.IDNO = "-";
            }
            if (MainModel.tab2.PassportNo == null)
            {
                MainModel.tab2.PassportNo = "-";
            }

            //
            if (MainModel.tab5.WitnessName1 == null)
            {
                MainModel.tab5.WitnessName1 = "-";
            }

            if (MainModel.tab5.WitnessTitleDesignation1 == null)
            {
                MainModel.tab5.WitnessTitleDesignation1 = "-";
            }

            if (MainModel.tab2.Other == null)
            {
                MainModel.tab2.Other = "-";
            }

            switch (name)
            {
                case "type":
                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");

                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");

                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");

                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");

                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");

                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");

                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");

                        }

                    }
                    bool AppFlag = cv.IsAnyNullOrEmpty(MainModel.tabb1);
                    if (MainModel.SelectedCategory == "Select")
                    {
                        ViewBag.SelectedCategory = "Please select Category";
                        AppFlag = true;
                    }

                    if (AppFlag == true)
                    {
                        ViewBag.type = "active";
                    }
                    else if (AppFlag == false && next != null)
                    {
                        ViewBag.business = "active";
                    }

                    break;

                case "business":
                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");

                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");

                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");

                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");

                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");

                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");

                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");

                        }

                    }
                    bool BFlag = cv.IsAnyNullOrEmpty(MainModel.tab2);

                    if (MainModel.OwnerCategory == null)
                    {
                        ViewBag.OwnerCategory = "Please select Owner Category";
                        BFlag = true;
                    }
                    if (MainModel.WorkDiscipline == null)
                    {
                        ViewBag.WorkDiscipline = "Please select work discipline";
                        BFlag = true;
                    }
                    if (BFlag == true)
                    {
                        ViewBag.business = "active";
                    }
                    else if (BFlag == false && next != null)
                    {
                        ViewBag.fin = "active";
                    }
                    else if (BFlag == false && pre != null)
                    {
                        ViewBag.type = "active";
                    }
                    break;

                case "finance":

                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");

                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");

                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");

                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");

                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");

                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");

                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");

                        }

                    }
                    bool Ftab = checkTab3(MainModel, next, pre);

                    if (Ftab == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    else if (Ftab == false && next != null)
                    {

                        ViewBag.work = "active";
                    }
                    else if (Ftab == false && pre != null)
                    {

                        ViewBag.business = "active";
                    }

                    break;

                case "work":

                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");

                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");

                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");

                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");

                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");

                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");

                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");

                        }

                    }

                    bool WFlag = cv.IsAnyNullOrEmpty(MainModel.tab4);
                    if (WFlag == true)
                    {
                        ViewBag.work = "active";
                    }
                    else if (WFlag == false && next != null)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (WFlag == false && pre != null)
                    {
                        ViewBag.fin = "active";
                    }
                    break;

                case "doc":


                    bool docFlag = cv.IsAnyNullOrEmpty(MainModel.tab5);

                    //edit case 
                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");
                            docFlag = false;
                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");
                            docFlag = false; ;
                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");
                            docFlag = false;
                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");
                            docFlag = false;
                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");
                            docFlag = false;
                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");
                            docFlag = false;
                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");
                            docFlag = false;
                        }
                    }
                    //
                    if (docFlag == true)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.work = "active";
                    }

                    break;

                case "draft":
                    MainModel.Reviewer = "";
                    //if()
                    //{

                    //}
                    MainModel.FormStatus = "Draft";
                    ModelState.Remove("p.filepath");
                    string result = Savedata(MainModel);

                    return RedirectToAction("Form8Result", "Form8", new { result = result, text = "Draft" });

                case "final":

                    //if (MainModel.formval == "Edit")
                    //{
                    //    MainModel.FormStatus = MainModel.FormStatus;
                    //    MainModel.Reviewer = "Clerk";
                    //    string result1 = Savedata(MainModel);
                    //    removeDatafromSession();
                    //    //  return RedirectToAction("CicformReview", "Form8");
                    //    return RedirectToAction("Form8Result", "Form8", new { result = result1, text = "Final" });
                    //}
                    //else
                    //{


                    ModelState.Remove("MainModel.tabb1.FaxNo");

                    bool A = cv.IsAnyNullOrEmpty(MainModel.tabb1);
                    if (A == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }

                    bool B = cv.IsAnyNullOrEmpty(MainModel.tab2);
                    if (B == true)
                    {
                        ViewBag.business = "active";
                        break;
                    }


                    bool C = cv.IsAnyNullOrEmpty(MainModel.tab4);
                    if (C == true)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    bool E = checkTab3(MainModel, next, pre);
                    if (E == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }

                    bool D = cv.IsAnyNullOrEmpty(MainModel.tab5);
                    if (MainModel.formval == "Edit")
                    {
                        if (MainModel.SummaryPage != null)
                        {

                            ModelState.Remove("tab5.SummaryPage");
                            D = false;
                        }
                        if (MainModel.LetterOfContract != null)
                        {

                            ModelState.Remove("tab5.LetterOfContract");
                            D = false; ;
                        }
                        if (MainModel.Letterindicating != null)
                        {

                            ModelState.Remove("tab5.Letterindicating");
                            D = false;
                        }
                        if (MainModel.Signedletterupload != null)
                        {

                            ModelState.Remove("tab5.Signedletterupload");
                            D = false;
                        }
                        if (MainModel.Signature1 != null)
                        {

                            ModelState.Remove("tab5.Signature1");
                            D = false;
                        }
                        if (MainModel.WitnessSignature != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature");
                            D = false;
                        }
                        if (MainModel.WitnessSignature1 != null)
                        {

                            ModelState.Remove("tab5.WitnessSignature1");
                            D = false;
                        }

                    }


                    if (D == true) //adding
                    {
                        ViewBag.doc = "active";
                        break;
                    }
                    ModelState.Clear();



                    if (ModelState.IsValid)
                    {

                        MainModel.FormStatus = "Submit";
                        MainModel.Reviewer = "Clerk";
                        string result1 = Savedata(MainModel);
                        removeDatafromSession();
                        //  return RedirectToAction("CicformReview", "Form8");
                        return RedirectToAction("Form8Result", "Form8", new { result = result1, text = "Final" });


                    }
                    else
                    {
                        ViewBag.doc = "active";
                    }
                    //}

                    break;

            }
            // int sub = MainModel.selectedsubcategory;
            int categoryId = 0;
            if (MainModel.WorkDiscipline != null)
            {
                categoryId = GetCategoryId(MainModel.WorkDiscipline);
            }

            LoadData(MainModel, categoryId);

            // MainModel = loadData1(MainModel);


            return View(MainModel);
        }
        public IActionResult Form8Result(string result, string text)
        {
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            string body = "", subject = "", emailto = "";
            ViewBag.Result = result;
            ViewBag.sts = text;
            ViewBag.yr = yr;
            if (text == "Draft")
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='https://constructioncouncil.azurewebsites.net/'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been saved as draft";
            }
            else
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been successfully submitted. To view your application status, please log in <a href='https://constructioncouncil.azurewebsites.net/'>CIC Portal</a> and view your dashboard. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been successfully submitted";
            }
            memoryCache.TryGetValue("emailto", out emailto);
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
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
        public void setGetFileEdit(MainViewModel p)
        {
            if (p.tab5.SummaryPage != null)
            {
                memoryCache.Set("SummaryPage", p.tab5.SummaryPage);
                p.SummaryPage = p.tab5.Signature.FileName;
                memoryCache.Set("SummaryPage", p.tab5.SummaryPage.FileName);
            }
            else
            {
                p.SummaryPage = SetandGetFileEdit("SummaryPage");
            }
            //
            if (p.tab5.LetterOfContract != null)
            {
                memoryCache.Set("LetterOfContract", p.tab5.LetterOfContract);
                p.LetterOfContract = p.tab5.LetterOfContract.FileName;
                memoryCache.Set("LetterOfContract", p.tab5.LetterOfContract.FileName);
            }
            else
            {
                p.LetterOfContract = SetandGetFileEdit("LetterOfContract");
            }
            //lll
            if (p.tab5.Letterindicating != null)
            {
                memoryCache.Set("Letterindicating", p.tab5.Letterindicating);
                p.Letterindicating = p.tab5.Letterindicating.FileName;
                memoryCache.Set("Letterindicating", p.tab5.Letterindicating.FileName);
            }
            else
            {
                p.Letterindicating = SetandGetFileEdit("Letterindicating");
            }
            //
            if (p.tab5.Signedletterupload != null)
            {
                memoryCache.Set("Signedletterupload", p.tab5.Signedletterupload);
                p.Signedletterupload = p.tab5.Signedletterupload.FileName;
                memoryCache.Set("Signedletterupload", p.tab5.Signedletterupload.FileName);
            }
            else
            {
                p.Signedletterupload = SetandGetFileEdit("Signedletterupload");
            }

            if (p.tab5.Signature != null)
            {
                memoryCache.Set("Signature1", p.tab5.Signature);
                p.Signature1 = p.tab5.Signature.FileName;
                memoryCache.Set("Signature1", p.tab5.Signature.FileName);
            }
            else
            {
                p.Signature1 = SetandGetFileEdit("Signature1");
            }
            //

            if (p.tab5.WitnessSignature != null)
            {
                memoryCache.Set("WitnessSignature", p.tab5.WitnessSignature);
                p.WitnessSignature = p.tab5.WitnessSignature.FileName;
                memoryCache.Set("WitnessSignature", p.tab5.WitnessSignature.FileName);
            }
            else
            {
                p.WitnessSignature = SetandGetFileEdit("WitnessSignature");
            }
            //

            if (p.tab5.WitnessSignature1 != null)
            {
                memoryCache.Set("WitnessSignature1", p.tab5.WitnessSignature1);
                p.WitnessSignature1 = p.tab5.WitnessSignature1.FileName;
                memoryCache.Set("WitnessSignature1", p.tab5.WitnessSignature.FileName);
            }
            else
            {
                p.WitnessSignature1 = SetandGetFileEdit("WitnessSignature1");
            }
        }

        private void LoadData(MainViewModel model, int CategoryID = 0)
        {
            string loggedInUserEmail = _userManager.GetUserAsync(User).Result.Email;

            List<Category> categorylistt = new List<Category>();

            //------- Getting data from database using EntityframeworkCore -------
            categorylistt = (from category in _context.Category where category.FormType == "F1" || category.FormType == "F2" select category).ToList();

            //------- Inserting select items in list -------
            categorylistt.Insert(0, new Category { CategoryID = 0, CategoryName = "Select" });

            //------- Assigning categorylist to ViewBag.ListofCategory -------
            ViewBag.ListofCategory = categorylistt;





            //--------------- Getting Selected Value
            string SelectedValue = model.SelectedCategory;

            List<Category> categorylist = new List<Category>();

            //------- Getting data from database using EntityframeworkCore -------

            //categorylist = (from category in _context.Category select category).ToList();
            categorylist = (from category in _context.Category where category.FormType == "F8" select category).ToList();

            //----- Maintaining state of RadioButtonList (Rebinding ListCategory)
            model.ListCategory = categorylist;

            //added code 
            //------- Getting data from database using EntityframeworkCore -------
            List<Category> categorylist2 = new List<Category>();

            categorylist2 = (from category in _context.Category where category.FormType == "F1" || category.FormType == "F2" select category).ToList();

            //-------- Creating Instance of MainView Model and assigning value to ListCategory

            model.ListCategory1 = categorylist2;
            model.SelectedCategory = string.Empty;

            //for sub

            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            subCategorylist = (from SubCategoryType in _context.SubCategory
                               where SubCategoryType.CategoryID == CategoryID
                               select SubCategoryType).ToList();


            List<subCategory> sub = new List<subCategory>();

            for (int i = 0; i < subCategorylist.Count; i++)
            {
                sub.Add(new subCategory { SubCategoryID = subCategorylist[i].SubCategoryID, SubCategoryName = subCategorylist[i].SubCategoryName });
            }
            model.subCategoryModel = sub;
            // if()
            ViewBag.ListSubCategory = sub;

            // Suppliers
            string jsonDataSuppliers;
            AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform7", out jsonDataSuppliers);

            JObject suppliersObject = JObject.Parse(jsonDataSuppliers);

            var suppliers = suppliersObject["value"]
                .Where(x => ((string)x["FormStatus"]).ToLower() == "finished" && (string)x["CertificateNo"] != null && (string)x["CertificateNo"] != "")
                .GroupBy(x => x["BusinessName"])
                .Select(x => x.OrderByDescending(y => y["Timestamp"]).FirstOrDefault())
                .OrderBy(x => x["BusinessName"])
                .ToList();

            List<SuppliersList> suppliersList = new List<SuppliersList>();

            for (int i = 0; i < suppliers.Count(); i++)
            {
                suppliersList.Add(new SuppliersList()
                {
                    SupplierValue = (string)suppliers[i]["BusinessName"]
                    ,
                    SupplierText = (string)suppliers[i]["BusinessName"]
                });
            }

            memoryCache.Set("ListOfSuppliers", suppliersList);

            ViewBag.ListOfSuppliers = suppliersList;
            model.ListOfSuppliers = suppliersList;


            //subcontractor
            string jsonDataSubcontractor;
            AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonDataSubcontractor);

            JObject suppliersObject1 = JObject.Parse(jsonDataSubcontractor);

            var suppliers1 = suppliersObject1["value"]
                .Where(x => ((string)x["FormStatus"]).ToLower() == "finished" && (string)x["CertificateNo"] != null && (string)x["CertificateNo"] != "")
                .GroupBy(x => x["BusinessName"])
                .Select(x => x.OrderByDescending(y => y["Timestamp"]).FirstOrDefault())
                .OrderBy(x => x["BusinessName"])
                .ToList();

            List<SubContractorsList> subContractorsList = new List<SubContractorsList>();

            for (int i = 0; i < suppliers1.Count(); i++)
            {
                subContractorsList.Add(new SubContractorsList()
                {
                    SubContractorValue = (string)suppliers1[i]["BusinessName"]
                    ,
                    SubContractorText = (string)suppliers1[i]["BusinessName"]
                });
            }

            memoryCache.Set("ListOfSubContractors", subContractorsList);

            ViewBag.ListOfSubContractors = subContractorsList;
            model.ListOfSubContractors = subContractorsList;

            // Certificates
            string jsonDataCertificates;
            AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonDataCertificates);

            JObject certificatesObject = JObject.Parse(jsonDataCertificates);

            var certificates = certificatesObject["value"]
                .Where(x => ((string)x["CreatedBy"]).ToLower() == loggedInUserEmail.ToLower() && x["CertificateNo"] != null)
                .GroupBy(x => x["CertificateNo"])
                .Select(x => x.OrderBy(y => y["CertificateNo"]).FirstOrDefault())
                .ToList();

            List<CertificatesList> certificatesList = new List<CertificatesList>();

            for (int i = 0; i < certificates.Count(); i++)
            {
                certificatesList.Add(new CertificatesList()
                {
                    CertificateNoValue = (string)certificates[i]["CertificateNo"]
                    ,
                    CertificateNoText = (string)certificates[i]["CertificateNo"]
                });
            }

            ViewBag.ListOfCertificates = certificatesList;
            model.ListOfCertificates = certificatesList;
        }

        public JsonResult GetAllSubContractors()
        {
            var subContractors = new List<SubContractorsList>();

            memoryCache.TryGetValue("ListOfSubContractors", out subContractors);

            var jsonString = JsonConvert.SerializeObject(subContractors);

            return Json(jsonString);
        }

        public JsonResult GetSubContractorDetails(string SubContractor)
        {
            if (!string.IsNullOrEmpty(SubContractor) && SubContractor != "Select")
            {
                string jsonCicForm1Data;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonCicForm1Data);

                JObject cicForm1Object = JObject.Parse(jsonCicForm1Data);

                string jsonShareDividendsData;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1ShareDividends", out jsonShareDividendsData);

                JObject sharedDividendsObject = JObject.Parse(jsonShareDividendsData);

                var subContractors = (from f1 in cicForm1Object["value"].Where(x => (string)x["BusinessName"] == SubContractor)
                                      join sd in sharedDividendsObject["value"] on f1["RowKey"] equals sd["RowKey"]
                                      orderby f1["Timestamp"] descending, sd["Timestamp"] descending
                                      select new
                                      {
                                          CountryofOrigin = (string)sd["Nationnality"],
                                          ScopeofWork = (string)f1["Category"],
                                          ContactDetails = (string)f1["BusinessRepresentativeCellNo"],
                                          RegistrationNo = (string)f1["CertificateNo"]
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
                    RegistrationNo = ""
                });

                return Json(jsonString);
            }

        }

        public JsonResult GetAllSuppliers()
        {
            var suppliers = new List<SuppliersList>();

            memoryCache.TryGetValue("ListOfSuppliers", out suppliers);

            var jsonString = JsonConvert.SerializeObject(suppliers);

            return Json(jsonString);
        }

        public JsonResult GetSupplierDetails(string Supplier)
        {
            if (!string.IsNullOrEmpty(Supplier) && Supplier != "Select")
            {
                string jsonCicForm1Data;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "CicForm7", out jsonCicForm1Data); //cicform1

                JObject cicForm1Object = JObject.Parse(jsonCicForm1Data);

                string jsonShareDividendsData;
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform7ShareDividends", out jsonShareDividendsData); //cicform1ShareDividends

                JObject sharedDividendsObject = JObject.Parse(jsonShareDividendsData);

                var suppliers = (from f1 in cicForm1Object["value"].Where(x => (string)x["BusinessName"] == Supplier)
                                 orderby f1["Timestamp"] descending
                                 select new
                                 {
                                     CountryofOrigin = "",//(string)f1["Nationnality"], //CountryofOrigin = (string)sd["Nationnality"],
                                     ScopeofWork = (string)f1["WorkDisciplineType"],
                                     ContactDetails = (string)f1["TelephoneNumber"],
                                     RegistrationNo = (string)f1["CertificateNo"]
                                 }).FirstOrDefault();

                var jsonString = JsonConvert.SerializeObject(suppliers);
                return Json(jsonString);
            }
            else
            {
                var jsonString = JsonConvert.SerializeObject(new
                {
                    CountryofOrigin = "",
                    ScopeofWork = "",
                    ContactDetails = "",
                    RegistrationNo = ""
                });

                return Json(jsonString);
            }
        }

        public bool AppModelValidations(MainViewModel p)
        {
            bool AppFlag = false;

            if (p.SelectedCategory == null)
            {
                AppFlag = true;
                ModelState.AddModelError("p.SelectedCategory", "Please select Category");
            }

            return AppFlag;
        }
        public bool BusinessModelvalidations(MainViewModel p)
        {
            //int tempPercentage = 0;
            bool DFlag = false;

            if (p.WorkDiscipline == null)
            {
                DFlag = true;
                ModelState.AddModelError("p.WorkDiscipline", "Please select Category");
            }



            if (p.WorkDiscipline == "Buildings Specialist Works" || p.WorkDiscipline == "Civils Specialist Works" || p.WorkDiscipline == "Electrical Specialist Works" || p.WorkDiscipline == "Mechanical Specialist Works")
            {
                if (p.selectedsubcategory == 0)
                {
                    DFlag = true;
                    ModelState.AddModelError("p.selectedsubcategory", "Please select Specialist category");
                }
            }



            return DFlag;
        }


        public bool checkTab3(MainViewModel p, string next, string pre)
        {
            bool FFlag = false;
            bool aFlag = false;
            bool bFlag = false;

            if (p.Tab3MainSection != null)
            {
                if (p.Tab3MainSection.Count >= 1)
                {

                    for (int j = 0; j < p.Tab3MainSection.Count; j++)
                    {
                        if (p.Tab3MainSection[j].NameofConsultancyFirm != null)
                        {

                            if (p.Tab3MainSection[j].CountryofOrigin == null)
                            {
                                ModelState.AddModelError("Tab3MainSection[" + j + "].CountryofOrigin", "Please Enter Country of Origin");
                                FFlag = true;
                                //break;
                            }

                            if (p.Tab3MainSection[j].Contact == null)
                            {
                                ModelState.AddModelError("Tab3MainSection[" + j + "].Contact", "Please Enter Contact");
                                FFlag = true;
                                //break;
                            }
                            if (p.Tab3MainSection[j].CicRegistrationNo == null)
                            {
                                ModelState.AddModelError("Tab3MainSection[" + j + "].CicRegistrationNo", "Please Enter Cic Registration No");
                                FFlag = true;
                                //break;
                            }


                        }




                        //    if (p.Tab3MainSection.Count >= 1)
                        //      {
                        //    if (p.Tab3MainSection[j].NameofConsultancyFirm == null )
                        //     {
                        //        FFlag = true;
                        //        break;
                        //      }

                        //    else if (p.Tab3MainSection[j].CountryofOrigin == null)
                        //    {
                        //        FFlag = true;
                        //        break;
                        //    }

                        //    else if (p.Tab3MainSection[j].Contact == null)
                        //    {
                        //        FFlag = true;
                        //        break;
                        //    }
                        //    else if (p.Tab3MainSection[j].CicRegistrationNo == null)
                        //    {
                        //        FFlag = true;
                        //        break;
                        //    }
                        //}


                    }


                }
            }


            if (p.tab3SecSection != null)
            {
                if (p.tab3SecSection.Count > 0)
                {
                    for (int i = 0; i < p.tab3SecSection.Count; i++)
                    {

                        if (p.tab3SecSection.Count >= 1)
                        {
                            if (p.tab3SecSection[i].NameofSubContractors == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.tab3SecSection[i].CountryofOrigin == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.tab3SecSection[i].ScopeofWork == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.tab3SecSection[i].ContactDetails == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.tab3SecSection[i].RegistrationNo == null)
                            {
                                aFlag = true;
                                break;
                            }
                            //else
                            //{
                            //    aFlag = !cv.IsValidCICRegistrationNumber(p.tab3SecSection[i].RegistrationNo, StorageName, StorageKey);

                            //    if (aFlag)
                            //        ModelState.AddModelError("tab3SecSection[" + i + "].RegistrationNo", "Please Enter Valid Cic Registration No");
                            //}
                        }
                    }
                }
            }
            if (p.tab3ThirdSection != null)
            {
                if (p.tab3ThirdSection.Count > 0)
                {
                    for (int i = 0; i < p.tab3ThirdSection.Count; i++)
                    {

                        if (p.tab3ThirdSection.Count >= 1)
                        {
                            if (p.tab3ThirdSection[i].Supplier == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.tab3ThirdSection[i].CountryofOrigin == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.tab3ThirdSection[i].ScopeofWork == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.tab3ThirdSection[i].ContactDetails == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.tab3ThirdSection[i].RegistrationNo == null)
                            {
                                bFlag = true;
                                break;
                            }
                        }
                    }
                }
            }

            if ((FFlag == false && aFlag == false && bFlag == false) && next != null)
            {
                ViewBag.work = "active";
                return false;

            }
            if ((FFlag == false && aFlag == false && bFlag == false) && pre != null)
            {

                ViewBag.business = "active";
                return false;
            }
            if (FFlag == false && aFlag == false && bFlag == false)
            {

                ViewBag.doc = "active";
                return false;
            }
            else
            {

                ViewBag.fin = "active";

                return true;
            }

        }

        public IActionResult CicformReview()
        {
            return View();
        }

        public string getLevyPercentage(decimal value)
        {
            string levycaculationstr = "";
            decimal percentage = 0;
            decimal round = Decimal.Round(value);
            decimal totalProjectCost = 0, TotalProjectCostIncludingLevy = 0;


            //Decimal.Compare(0, value);
            //Decimal.Compare(value, 2000000);

            if (0 <= round && round <= 2000000)
            {
                percentage = 1;
            }
            else if (2000001 <= round && round <= 5000000)
            {
                percentage = 0.8M;
            }
            else if (5000001 <= round && round <= 10000000)
            {
                percentage = 0.75M;
            }
            else if (10000001 <= round && round <= 15000000)
            {
                percentage = 0.6M;
            }
            else if (15000001 <= round)
            {
                percentage = 0.5M;
            }

            totalProjectCost = value * (percentage / 100);
            TotalProjectCostIncludingLevy = value + totalProjectCost;

            levycaculationstr = percentage.ToString() + "," + totalProjectCost.ToString("0.##") + "," + TotalProjectCostIncludingLevy.ToString("0.##");

            return levycaculationstr;
        }
        public string Savedata(MainViewModel p1)
        {


            string FormRegNo = "";

            //Code for saving
            string response = "";
            SaveModelForm8 saveModelForm8 = new SaveModelForm8();


            string TableName = "CicForm8()";

            string jsonData;
            int tempMax = 0;
            if (p1.formval == "Edit")
            {
                tempMax = p1.FormRegistrationNo; //

                saveModelForm8.PartitionKey = p1.PartitionKey;
                saveModelForm8.RowKey = p1.RowKey;
                saveModelForm8.FormStatus = p1.FormStatus;
                FormRegNo = "PRN" + tempMax;

            }
            else
            {
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform", out jsonData);//Get data

                JObject myJObject = JObject.Parse(jsonData);
                int cntJson = myJObject["value"].Count();
                int tempRegNo;

                //int tempMax = (int)myJObject["value"][0]["ProjectRegistrationNo"]; ;
                tempMax = (int)myJObject["value"][0]["ProjectRegistrationNo"]; ;
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
                addNew.RowKey = "PRN" + tempMax.ToString();
                addNew.ProjectRegistrationNo = tempMax.ToString();
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));

                p1.ImagePath = "PRN" + tempMax;

                //
                if (p1.tabb1.Oraganization == null)
                {
                    p1.tabb1.Oraganization = "-";
                }
                saveModelForm8.PartitionKey = p1.tabb1.Oraganization;
                saveModelForm8.RowKey = "PRN" + tempMax.ToString();
                saveModelForm8.FormStatus = p1.FormStatus;

                FormRegNo = "PRN" + tempMax.ToString();

                saveModelForm8.FormRegistrationNo = tempMax;
                saveModelForm8.PartitionKey = p1.tabb1.Oraganization;
                saveModelForm8.RowKey = "PRN" + tempMax.ToString();
                saveModelForm8.FormStatus = p1.FormStatus;

            }

            //Saving data to storage
            saveModelForm8.FormName = "Form8";
            saveModelForm8.FormRegistrationNo = tempMax;
            //saveModelForm8.PartitionKey = p1.tabb1.Oraganization;
            //saveModelForm8.RowKey = "PRN"+tempMax.ToString();
            //saveModelForm8.FormStatus = p1.FormStatus;
            saveModelForm8.Reviewer = p1.Reviewer.Trim();
            saveModelForm8.CreatedBy = User.Identity.Name;
            saveModelForm8.Oraganization = p1.tabb1.Oraganization;
            saveModelForm8.CategoryId = p1.SelectedCategory;
            saveModelForm8.CertificateNo = p1.tabb1.CertificateNo;
            saveModelForm8.Grade = p1.tabb1.Grade;
            saveModelForm8.TypeGender = p1.tabb1.TypeGender;
            saveModelForm8.FirstName = p1.tabb1.FirstName;
            saveModelForm8.Surname = p1.tabb1.Surname;
            saveModelForm8.Designation = p1.tabb1.Designation;
            saveModelForm8.Telephone = p1.tabb1.Telephone;
            saveModelForm8.FaxNo = p1.tabb1.FaxNo;
            saveModelForm8.MobileNo = p1.tabb1.MobileNo;
            saveModelForm8.EmailAddress = p1.tabb1.EmailAddress;
            //saveModelForm8.PostalAddress = p1.tabb1.Form1PostalAddress;

            //2 tab details
            saveModelForm8.Name = p1.tab2.Name;
            saveModelForm8.IDNO = p1.tab2.IDNO;
            saveModelForm8.PassportNo = p1.tab2.PassportNo;
            saveModelForm8.PostalAddress = $"{p1.tabb1.Form1PostalAddress}|{p1.tab2.PostalAddress}" ;
            saveModelForm8.PhysicalAddress = p1.tab2.PhysicalAddress;
            saveModelForm8.PhysicalAddress = p1.tab2.PhysicalAddress;
            saveModelForm8.TelephoneWorkdiscipline = p1.tab2.Telephone;
            saveModelForm8.FaxNoWorkdiscipline = p1.tab2.FaxNo;
            saveModelForm8.EmailAdress = p1.tab2.EmailAdress;
            saveModelForm8.OwnerCategoryId = p1.OwnerCategory;
            saveModelForm8.WorkDisciplinefor = p1.WorkDiscipline;
            saveModelForm8.Subcategory = p1.selectedsubcategory;
            //if(p1.WorkDiscipline=="Other")
            //{
            //    saveModelForm8.WorkDisciplinefor=

            //}
            //if(p1.WorkDisciplineSubCategorry!=null)
            //{
            //    saveModelForm8.CLientSubcategory = p1.WorkDisciplineSubCategorry;
            //}

            // SaveModelForm8.CLientSubcategory = p1.selectedsubcategory;
            saveModelForm8.AuthoriseGende = p1.tab2.AuthoriseGende;
            saveModelForm8.AuthrisedFirstName = p1.tab2.AuthrisedFirstName;
            saveModelForm8.AuthorisedSurname = p1.tab2.AuthorisedSurname;
            saveModelForm8.DesignationWorkdiscipline = p1.tab2.Designation;
            saveModelForm8.AuthorisedFaxNo = p1.tab2.AuthorisedFaxNo;
            saveModelForm8.AuthorisedTelePhone = p1.tab2.TelePhoneNo;

            saveModelForm8.AuthorisedMobile = p1.tab2.AuthorisedMobile;
            saveModelForm8.AuthorisedEmail = p1.tab2.AuthorisedEmail;
            saveModelForm8.Other = p1.tab2.Other;
            memoryCache.Set("emailto", User.Identity.Name);
            saveModelForm8.CustNo = HttpContext.Session.GetString("CustNo");
            //Saving Data of 4 tab 
            saveModelForm8.ProjectType = p1.tab4.ProjectType;
            saveModelForm8.BidReference = p1.tab4.BidReference;
            saveModelForm8.ProjectTite = p1.tab4.ProjectTite;
            saveModelForm8.ProjectorFunder = p1.tab4.ProjectorFunder;
            saveModelForm8.ProjectLocation = p1.tab4.ProjectLocation;
            saveModelForm8.TownInkhundla = p1.tab4.TownInkhundla;
            saveModelForm8.Region = p1.tab4.Region;
            saveModelForm8.GPSCo = p1.tab4.GPSCo;
            saveModelForm8.DateofAward = p1.tab4.DateofAward;
            saveModelForm8.BriefDescriptionofProject = p1.tab4.BriefDescriptionofProject;
            saveModelForm8.ProposedCommencmentDate = p1.tab4.ProposedCommencmentDate;
            saveModelForm8.ProposedCompleteDate = p1.tab4.ProposedCompleteDate;
            saveModelForm8.RevisedDate = p1.tab4.RevisedDate;
            saveModelForm8.ContractVAlue = p1.tab4.ContractVAlue;
            saveModelForm8.LevyPaybale = p1.tab4.LevyPaybale;
            saveModelForm8.TotalProjectCost = p1.tab4.TotalProjectCost;
            saveModelForm8.TotalProjectCostIncludingLevy = p1.tab4.TotalProjectCostIncludingLevy;
            saveModelForm8.LevyPaymentOptions = p1.tab4.LevyPaymentOptions;
            saveModelForm8.TimeFrameoption = p1.tab4.TimeFrameoption;
            //Tab5 Declaration data 
            saveModelForm8.RepresentativeName = p1.tab5.RepresentativeName;
            saveModelForm8.CompName = p1.tab5.CompName;
            saveModelForm8.Position = p1.tab5.Position;
            saveModelForm8.Place = p1.tab5.Place;
            saveModelForm8.Day = p1.tab5.Day;
            saveModelForm8.Month = p1.tab5.Month;
            saveModelForm8.Year = p1.tab5.Year;
            saveModelForm8.WitnessName = p1.tab5.WitnessName;
            saveModelForm8.WitnessTitleDesignation = p1.tab5.WitnessTitleDesignation;
            saveModelForm8.WitnessName1 = p1.tab5.WitnessName1;
            saveModelForm8.WitnessTitleDesignation1 = p1.tab5.WitnessTitleDesignation1;
            saveModelForm8.CreateClearenceCertificate = p1.CreateClearenceCertificate;
            saveModelForm8.PartialInvoiceCount = p1.PartialInvoiceCount;
            saveModelForm8.projectCertificateCreated = p1.projectCertificateCreated;
            saveModelForm8.NoOfPartialCertificateCreated =  p1.NoOfPartialCertificateCreated;

            p1.ImagePath = "PRN" + tempMax;
            if (p1.tab5.Signature != null)
            {
                uploadFiles(p1.tab5.Signature, p1.ImagePath, "Signature1");

            }
            if (p1.tab5.WitnessSignature != null)
            {
                uploadFiles(p1.tab5.WitnessSignature, p1.ImagePath, "WitnessSignature");

            }
            if (p1.tab5.WitnessSignature1 != null)
            {
                uploadFiles(p1.tab5.WitnessSignature1, p1.ImagePath, "WitnessSignature1");

            }


            if (p1.tab5.SummaryPage != null)
            {
                uploadFiles(p1.tab5.SummaryPage, p1.ImagePath, "SummaryPage");

            }
            if (p1.tab5.LetterOfContract != null)
            {
                uploadFiles(p1.tab5.LetterOfContract, p1.ImagePath, "LetterOfContract");

            }
            if (p1.tab5.Letterindicating != null)
            {
                uploadFiles(p1.tab5.Letterindicating, p1.ImagePath, "Letterindicating");

            }
            if (p1.tab5.Signedletterupload != null)
            {
                uploadFiles(p1.tab5.Signedletterupload, p1.ImagePath, "Signedletterupload");
            }

            //Adding new concept

            if (filepath != "NA")
            {
                saveModelForm8.path = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    //saveModelForm8.path = @"https:\cicdatastorage.blob.core.windows.net\uploads\2022-02-21\" + filepath;
                    saveModelForm8.path = @"https:\cicdatastorage.blob.core.windows.net\uploads\" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }

            if (p1.formval == "Edit")
            {


                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm8", JsonConvert.SerializeObject(saveModelForm8, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), saveModelForm8.PartitionKey, saveModelForm8.RowKey);
            }

            else
            {
                saveModelForm8.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(saveModelForm8, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            }

            if (response == "Created" || response == "NoContent")
            {

                Tab3FirstSection tab3First = new Tab3FirstSection();
                if (p1.Tab3MainSection != null)
                {

                    for (int i = 0; i < p1.Tab3MainSection.Count; i++)
                    {

                        bool DFlag = cv.IsAnyNullOrEmpty(p1.Tab3MainSection[i]);
                        if (DFlag == false)
                        {

                            string Data;


                            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm8DetailsOfAllConsultamcyFirms", p1.Tab3MainSection[i].PartitionKey, p1.Tab3MainSection[i].RowKey, out Data);

                            JObject myJObject = JObject.Parse(Data);
                            int cntJson = myJObject["value"].Count();

                            if (cntJson != 0)
                            {

                                AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm8DetailsOfAllConsultamcyFirms", p1.Tab3MainSection[i].PartitionKey, p1.Tab3MainSection[i].RowKey, Data);


                            }

                            tab3First.PartitionKey = p1.Tab3MainSection[i].Profession;
                            tab3First.RowKey = "PRN" + tempMax.ToString();
                            tab3First.Profession = p1.Tab3MainSection[i].Profession;
                            tab3First.NameofConsultancyFirm = p1.Tab3MainSection[i].NameofConsultancyFirm;
                            tab3First.CountryofOrigin = p1.Tab3MainSection[i].CountryofOrigin;
                            tab3First.Contact = p1.Tab3MainSection[i].Contact;

                            tab3First.CicRegistrationNo = p1.Tab3MainSection[i].CicRegistrationNo;

                            tab3First.FormRegistrationNo = tempMax;
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm8DetailsOfAllConsultamcyFirms", JsonConvert.SerializeObject(tab3First, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                        }

                    }
                }

                Tab3SecSection tab3Sec = new Tab3SecSection();
                if (p1.tab3SecSection != null)
                {
                    //Delete all Subcontractors for the given RowKey.
                    AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", p1.RowKey, out jsonData);

                    JObject subContractorsObj = JObject.Parse(jsonData);
                    var subcontractorsList = subContractorsObj["value"];
                    if (subcontractorsList.Any())
                    {
                        for (int i = 0; i < subcontractorsList.Count(); i++)
                        { 
                            AzureTablesData.DeleteEntity(StorageName, StorageKey , "CicForm8DetailsOfAllSubContractors", (string)subContractorsObj["value"][i]["PartitionKey"], p1.RowKey, string.Empty);
                        }
                    }

                    //Workaround to set missing mandatory values
                    if (p1.tab3SecSection.Count > 1)
                    {
                        SetMandatoryColumns(p1);
                    }


                    for (int i = 0; i < p1.tab3SecSection.Count; i++)
                    {
                        bool DFlag = cv.IsAnyNullOrEmpty(p1.tab3SecSection[i]);
                        if (DFlag == false)
                        {
                            //Commenting: This code only inserts or updates the rows present in the Grid - But misses the rows that the user deleted

                            //string Data;

                            //AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", p1.tab3SecSection[i].PartitionKey, p1.tab3SecSection[i].RowKey, out Data);

                            //JObject myJObject = JObject.Parse(Data);
                            //int cntJson = myJObject["value"].Count();

                            //if (cntJson != 0)
                            //{
                            //    AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", p1.tab3SecSection[i].PartitionKey, p1.tab3SecSection[i].RowKey, Data);


                            //}

                            tab3Sec.PartitionKey = p1.tab3SecSection[i].NameofSubContractors;
                            tab3Sec.RowKey = "PRN" + tempMax.ToString();
                            tab3Sec.NameofSubContractors = p1.tab3SecSection[i].NameofSubContractors;
                            tab3Sec.CountryofOrigin = p1.tab3SecSection[i].CountryofOrigin;
                            tab3Sec.ScopeofWork = p1.tab3SecSection[i].ScopeofWork;
                            tab3Sec.ContactDetails = p1.tab3SecSection[i].ContactDetails;
                            tab3Sec.RegistrationNo = p1.tab3SecSection[i].RegistrationNo;
                            tab3Sec.FormRegistrationNo = tempMax;

                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", JsonConvert.SerializeObject(tab3Sec, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                        }

                    }
                }


                Tab3ThirdSection tab3Third = new Tab3ThirdSection();
                if (p1.tab3ThirdSection != null)
                {

                    //Delete all Suppliers for the given RowKey.
                    AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8AllSupplier", p1.RowKey, out jsonData);

                    JObject suppliersObj = JObject.Parse(jsonData);
                    var suppliersList = suppliersObj["value"];
                    if (suppliersList.Any())
                    {
                        for (int i = 0; i < suppliersList.Count(); i++)
                        {
                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm8AllSupplier", (string)suppliersObj["value"][i]["PartitionKey"], p1.RowKey, string.Empty);
                        }
                    }

                    //Workaround to set missing mandatory values
                    if (p1.tab3ThirdSection.Count > 1)
                    {
                        Settab3ThirdSectionMandatoryColumns(p1);
                    }

                    for (int i = 0; i < p1.tab3ThirdSection.Count; i++)
                    {

                        bool DFlag = cv.IsAnyNullOrEmpty(p1.tab3ThirdSection[i]);
                        if (DFlag == false)
                        {
                            //Commenting: This code only inserts or updates the rows present in the Grid - But misses the rows that the user deleted

                            //string Data;


                            //AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm8AllSupplier", p1.tab3ThirdSection[i].PartitionKey, p1.tab3ThirdSection[i].RowKey, out Data);

                            //JObject myJObject = JObject.Parse(Data);
                            //int cntJson = myJObject["value"].Count();

                            //if (cntJson != 0)
                            //{
                            //    AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm8AllSupplier", p1.tab3ThirdSection[i].PartitionKey, p1.tab3ThirdSection[i].RowKey, Data);


                            //}
                            //Saving data for Third section

                            tab3Third.PartitionKey = p1.tab3ThirdSection[i].Supplier;
                            tab3Third.RowKey = "PRN" + tempMax.ToString();
                            tab3Third.Supplier = p1.tab3ThirdSection[i].Supplier;
                            tab3Third.CountryofOrigin = p1.tab3ThirdSection[i].CountryofOrigin;
                            tab3Third.ScopeofWork = p1.tab3ThirdSection[i].ScopeofWork;
                            tab3Third.ContactDetails = p1.tab3ThirdSection[i].ContactDetails;
                            tab3Third.RegistrationNo = p1.tab3ThirdSection[i].RegistrationNo;
                            tab3Third.FormRegistrationNo = tempMax;
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm8AllSupplier", JsonConvert.SerializeObject(tab3Third, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));




                        }

                    }
                }



            }

            ////Patch data into BC
            //string jsonData1;
            //AzureTablesData.GetEntity(StorageName, StorageKey, "cicform8", FormRegNo, out jsonData1);//Get data
            //JObject myJObject1 = JObject.Parse(jsonData1);
            //int cntJson1 = myJObject1["value"].Count();
            //for (int i = 0; i < cntJson1; i++)
            //    AddCustinERP(myJObject1,i, FormRegNo);

            // return response;
            return FormRegNo;
        }

        private static void SetMandatoryColumns(MainViewModel p1)
        {
            var rowKey = p1.tab3SecSection[0].RowKey;
            var regNo = p1.tab3SecSection[0].FormRegistrationNo;

            foreach (var item in p1.tab3SecSection)
            {
                item.PartitionKey = item.NameofSubContractors;
                item.RowKey = rowKey;
                item.FormRegistrationNo = regNo;
            }
        }

        private static void Settab3ThirdSectionMandatoryColumns(MainViewModel p1)
        {
            var rowKey = p1.tab3ThirdSection[0].RowKey;
            var regNo = p1.tab3ThirdSection[0].FormRegistrationNo;

            foreach (var item in p1.tab3ThirdSection)
            {
                item.PartitionKey = item.Supplier;
                item.RowKey = rowKey;
                item.FormRegistrationNo = regNo;
            }
        }


        ////AK Registration Details
        //public string AddCustinERP(JObject myJObject,int i,string FormRegNo)
        //{       

        //    string custno = (string)myJObject["value"][i]["CustNo"];
        //    try
        //    {
        //        var data1 = JObject.FromObject(new
        //        {
        //            customerNo = (string)myJObject["value"][i]["CustNo"],
        //            formRegNo = FormRegNo,
        //            certificateNo = (string)myJObject["value"][i]["CertificateNo"],
        //            customerCategory = (string)myJObject["value"][i]["CategoryId"],
        //            grade = (string)myJObject["value"][i]["Grade"],
        //            projectNumber = (string)myJObject["value"][i]["ProjectTite"],
        //            contractSum = (decimal)myJObject["value"][i]["ContractVAlue"],
        //            levy = (decimal)myJObject["value"][i]["LevyPaybale"],                 
        //            classification = (string)myJObject["value"][i]["OwnerCategoryId"],
        //            levyPaymentOption = (string)myJObject["value"][i]["LevyPaymentOptions"],
        //            timeFrameForPaymentOfLevy = (string)myJObject["value"][i]["TimeFrameoption"],
        //            projectDetails = (string)myJObject["value"][i]["ProjectTite"],
        //            prcContactName = (string)myJObject["value"][i]["AuthrisedFirstName"],
        //            prcContactSurname = (string)myJObject["value"][i]["AuthorisedSurname"],
        //            prcDesignation = (string)myJObject["value"][i]["Designation"],
        //            prcEmail = (string)myJObject["value"][i]["AuthorisedEmail"],
        //            prcMobilePhoneNo = (string)myJObject["value"][i]["AuthorisedMobile"],
        //            prcPhoneNo = (string)myJObject["value"][i]["AuthorisedTelePhone"],                    
        //            pocContactName = (string)myJObject["value"][i]["FirstName"],
        //            pocContactSurname = (string)myJObject["value"][i]["Surname"],
        //            pocEmail = (string)myJObject["value"][i]["EmailAdress"],
        //            pocMobilePhoneNo = (string)myJObject["value"][i]["MobileNo"],
        //            pocPhoneNo = (string)myJObject["value"][i]["Telephone"],
        //            awardDate = ((DateTime)myJObject["value"][i]["DateofAward"]).ToString("yyyy-MM-dd"),
        //            startDate = ((DateTime)myJObject["value"][i]["ProposedCommencmentDate"]).ToString("yyyy-MM-dd"),
        //            completionDate = ((DateTime)myJObject["value"][i]["ProposedCompleteDate"]).ToString("yyyy-MM-dd")
        //        });

        //        var json = JsonConvert.SerializeObject(data1);
        //        var data = new StringContent(json, Encoding.UTF8, "application/json");

        //        using (var httpClient = new HttpClient())
        //        {
        //            var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
        //            httpClient.DefaultRequestHeaders.Clear();
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            string u = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract";
        //            HttpResponseMessage response = httpClient.PostAsync(@u, data).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string str = response.Content.ReadAsStringAsync().Result;

        //            }
        //        }


        //        //updating Blob Physical Address
        //        using (var httpClient = new HttpClient())
        //        {
        //            string BCUrl2 = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract(no='" + custno + "')/pocPhysicalAddress";
        //            Uri u = new Uri(BCUrl2);
        //            var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PhysicalAddress"], "text/plain"));
        //            t.Wait();
        //        }

        //        //updating Blob Postal Address
        //        using (var httpClient = new HttpClient())
        //        {
        //            string BCUrl2 = "https://api.businesscentral.dynamics.com/v2.0/0e1ed69c-0381-4157-9f35-d57e9facd3b1/Sandbox/api/cic/cic/v1.0/companies(048a694f-7727-ed11-97e8-0022481386b1)/customersContract(no='" + custno + "')/pocPostalAddress";
        //            Uri u = new Uri(BCUrl2);
        //            var t = Task.Run(() => PatchData(u, (string)myJObject["value"][i]["PostalAddress"], "text/plain"));
        //            t.Wait();
        //        }


        //        return custno;
        //    }
        //    catch
        //    { return ""; }

        //}

        //static async Task<HttpResponseMessage> PatchData(Uri u, string json,string appType)
        //{
        //    HttpClient client1 = new HttpClient();
        //    var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
        //    client1.DefaultRequestHeaders.Clear();
        //    client1.DefaultRequestHeaders.Add("If-Match", "*");
        //    client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    HttpContent c = new StringContent(json, Encoding.UTF8, appType);

        //    var method = "PATCH";
        //    var httpVerb = new HttpMethod(method);
        //    var httpRequestMessage =
        //        new HttpRequestMessage(httpVerb, u)
        //        {
        //            Content = c
        //        };

        //    var response = await client1.SendAsync(httpRequestMessage);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var responseCode = response.StatusCode;
        //        var responseJson = response.Content.ReadAsStringAsync();
        //    }
        //    return response;
        //}


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

            BlobStorageService objBlobService = new BlobStorageService();

            // path = objBlobService.UploadFileToBlob(tempFile.FileName, fileData, mimeType);


            filepath = objBlobService.UploadFileToBlob(TempFilename, fileData, mimeType, path);
            #endregion
        }

        bool validateIdNo(string ID)
        {
            if (ID != null)
            {
                if (ID.Length == 13)
                {
                    string tempstr = ID.Substring(0, 6);

                    bool isValid = regex.IsMatch(tempstr);

                    string str = ID.Substring(7, 1);

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
            return false;
        }

        public IActionResult IndexFromDashboard(string rowkey)
        {


            // ViewBag.type = "active";
            MainViewModel model = new MainViewModel();
            Tab1 model1 = new Tab1();
            int subCatid = 0;
            string c = "";
            model.Tab3Sec = cnt;
            model.Tab3Third = cnt;
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8", rowkey, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            //set master table values into model 
            for (int i = 0; i < cntJson; i++)
            {
                //adding for file name 
                path = (string)myJObject["value"][i]["path"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                //adding for files 
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                path = (string)myJObject["value"][i]["path"];

                string key;
                AllFileList = b.GetBlobList(path);
                string Signature1 = null, WitnessSignature = null, WitnessSignature1 = null, LetterOfContract = null,
                    SummaryPage = null, Letterindicating = null, Signedletterupload = null;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;

                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);

                        switch (key)
                        {
                            case "SummaryPage": SummaryPage = AllFileList[j].FileValue; break;
                            case "LetterOfContract": LetterOfContract = AllFileList[j].FileValue; break;
                            case "Letterindicating": Letterindicating = AllFileList[j].FileValue; break;
                            case "Signedletterupload": Signedletterupload = AllFileList[j].FileValue; break;
                            case "Signature1": Signature1 = AllFileList[j].FileValue; break;

                            case "WitnessSignature": WitnessSignature = AllFileList[j].FileValue; break;
                            case "WitnessSignature1": WitnessSignature1 = AllFileList[j].FileValue; break;

                        }
                    }
                }


                Tab1 tab1 = new Tab1
                {

                    Oraganization = (string)myJObject["value"][i]["Oraganization"],

                    //CategoryId = (string)myJObject["value"][i]["CategoryId"];
                    Grade = (string)myJObject["value"][i]["Grade"],
                    CertificateNo = (string)myJObject["value"][i]["CertificateNo"],
                    TypeGender = (string)myJObject["value"][i]["TypeGender"],
                    FirstName = (string)myJObject["value"][i]["FirstName"],
                    Surname = (string)myJObject["value"][i]["Surname"],
                    Designation = (string)myJObject["value"][i]["Designation"],
                    Telephone = (string)myJObject["value"][i]["Telephone"],
                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                    MobileNo = (string)myJObject["value"][i]["MobileNo"],
                    EmailAddress = (string)myJObject["value"][i]["EmailAddress"],
                    Form1PostalAddress = GetPostalAddress(myJObject, i, 0),
                };


                Tab2 tab2Details = new Tab2
                {


                    Name = (string)myJObject["value"][i]["Name"],
                    IDNO = (string)myJObject["value"][i]["IDNO"],
                    PassportNo = (string)myJObject["value"][i]["PassportNo"],
                    PostalAddress = GetPostalAddress(myJObject, i, 1),
                    PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"],
                    Telephone = (string)myJObject["value"][i]["TelephoneWorkdiscipline"],
                    FaxNo = (string)myJObject["value"][i]["FaxNoWorkdiscipline"],
                    EmailAdress = (string)myJObject["value"][i]["EmailAddress"],
                    Other = (string)myJObject["value"][i]["Other"],
                    AuthoriseGende = (string)myJObject["value"][i]["TypeGender"],
                    TelePhoneNo = (string)myJObject["value"][i]["AuthorisedTelePhone"],
                    AuthrisedFirstName = (string)myJObject["value"][i]["AuthrisedFirstName"],
                    AuthorisedSurname = (string)myJObject["value"][i]["AuthorisedSurname"],
                    Designation = (string)myJObject["value"][i]["DesignationWorkdiscipline"],
                    AuthorisedMobile = (string)myJObject["value"][i]["AuthorisedMobile"],
                    AuthorisedFaxNo = (string)myJObject["value"][i]["AuthorisedFaxNo"],
                    AuthorisedEmail = (string)myJObject["value"][i]["AuthorisedEmail"],


                };


                Tab4 tab4Details = new Tab4
                {


                    ProjectType = (string)myJObject["value"][i]["ProjectType"],
                    BidReference = (string)myJObject["value"][i]["BidReference"],
                    ProjectTite = (string)myJObject["value"][i]["ProjectTite"],
                    ProjectorFunder = (string)myJObject["value"][i]["ProjectorFunder"],
                    ProjectLocation = (string)myJObject["value"][i]["ProjectLocation"],
                    TownInkhundla = (string)myJObject["value"][i]["TownInkhundla"],
                    Region = (string)myJObject["value"][i]["Region"],
                    GPSCo = (string)myJObject["value"][i]["GPSCo"],
                    DateofAward = (DateTime)myJObject["value"][i]["DateofAward"],
                    BriefDescriptionofProject = (string)myJObject["value"][i]["BriefDescriptionofProject"],
                    ProposedCommencmentDate = (DateTime)myJObject["value"][i]["ProposedCommencmentDate"],
                    ProposedCompleteDate = (DateTime)myJObject["value"][i]["ProposedCompleteDate"],
                    RevisedDate = (DateTime)myJObject["value"][i]["RevisedDate"],
                    ContractVAlue = (decimal)myJObject["value"][i]["ContractVAlue"],
                    LevyPaybale = (decimal)myJObject["value"][i]["LevyPaybale"],

                    TotalProjectCost = (int)myJObject["value"][i]["TotalProjectCost"],
                    TotalProjectCostIncludingLevy = (int)myJObject["value"][i]["TotalProjectCostIncludingLevy"],
                    LevyPaymentOptions = (string)myJObject["value"][i]["LevyPaymentOptions"],
                    TimeFrameoption = (string)myJObject["value"][i]["TimeFrameoption"],

                };
                Tab5 tab5Details = new Tab5
                {
                    //CompName,Position,Place,day,Month ,Year

                    CompName = (string)myJObject["value"][i]["CompName"],
                    Place = (string)myJObject["value"][i]["Place"],
                    Position = (string)myJObject["value"][i]["Position"],
                    Day = (int)myJObject["value"][i]["Day"],
                    Month = (int)myJObject["value"][i]["Month"],
                    Year = (int)myJObject["value"][i]["Year"],
                    RepresentativeName = (string)myJObject["value"][i]["RepresentativeName"],
                    WitnessName = (string)myJObject["value"][i]["WitnessName"],
                    WitnessName1 = (string)myJObject["value"][i]["WitnessName1"],
                    WitnessTitleDesignation = (string)myJObject["value"][i]["WitnessTitleDesignation"],
                    WitnessTitleDesignation1 = (string)myJObject["value"][i]["WitnessTitleDesignation1"],

                };

                model.selectedsubcategory = (int)myJObject["value"][i]["Subcategory"];
                subCatid = (int)myJObject["value"][i]["Subcategory"];
                model.SummaryPage = SummaryPage;

                model.LetterOfContract = LetterOfContract;
                model.Letterindicating = Letterindicating;
                model.Signedletterupload = Signedletterupload;

                model.Signature1 = Signature1;
                model.WitnessSignature = WitnessSignature;
                model.WitnessSignature1 = WitnessSignature1;

                model.tab5 = tab5Details;
                model.tab4 = tab4Details;
                model.tab2 = tab2Details;
                model.tabb1 = tab1;
                model.SelectedCategory = (string)myJObject["value"][i]["CategoryId"];

                c = (string)myJObject["value"][i]["CategoryId"];
                model.OwnerCategory = (string)myJObject["value"][i]["OwnerCategoryId"];
                model.WorkDiscipline = (string)myJObject["value"][i]["WorkDisciplinefor"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.RowKey = (string)myJObject["value"][i]["RowKey"];
                model.PartitionKey = (string)myJObject["value"][i]["PartitionKey"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8DetailsOfAllConsultamcyFirms", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<Tab3FirstSection> d = new List<Tab3FirstSection>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new Tab3FirstSection
                {

                    Profession = (string)myJObject1["value"][i]["Profession"],
                    NameofConsultancyFirm = (string)myJObject1["value"][i]["NameofConsultancyFirm"],
                    //ShareFile = 
                    CountryofOrigin = (string)myJObject1["value"][i]["CountryofOrigin"],
                    Contact = (string)myJObject1["value"][i]["Contact"],
                    CicRegistrationNo = (string)myJObject1["value"][i]["CicRegistrationNo"],
                    PartitionKey = (string)myJObject1["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject1["value"][i]["RowKey"],

                    FormRegistrationNo = (int)myJObject1["value"][i]["FormRegistrationNo"]


                });
            }
            model.Tab3MainSection = d;

            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", rowkey, out string jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();

            var tab3SecSectionList = new List<Tab3SecSection>();
            if(cntJson2 == 0)
                tab3SecSectionList.Add( new Tab3SecSection() 
                    { 
                        ContactDetails = string.Empty,
                        CountryofOrigin = string.Empty,
                        FormRegistrationNo = 0,
                        NameofSubContractors = string.Empty,
                        RegistrationNo = string.Empty,
                        ScopeofWork = string.Empty,
                        PartitionKey = string.Empty,
                        RowKey = "DEFAULT" 
                    } );
            for (int i = 0; i < cntJson2; i++)
            {

                tab3SecSectionList.Add(new Tab3SecSection
                {

                    ContactDetails = (string)myJObject2["value"][i]["ContactDetails"],
                    CountryofOrigin = (string)myJObject2["value"][i]["CountryofOrigin"],
                    FormRegistrationNo = (int)myJObject2["value"][i]["FormRegistrationNo"],
                    NameofSubContractors = (string)myJObject2["value"][i]["NameofSubContractors"],
                    RegistrationNo = (string)myJObject2["value"][i]["RegistrationNo"],
                    ScopeofWork = (string)myJObject2["value"][i]["ScopeofWork"],
                    PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject2["value"][i]["RowKey"]


                });
            }
            if(tab3SecSectionList.Any()) model.tab3SecSection = tab3SecSectionList;
            model.Tab3Sec = tab3SecSectionList.Count;

            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm8AllSupplier", rowkey, out string jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();

            var tab3ThirdSectionList = new List<Tab3ThirdSection>();
            if(cntJson3 == 0)
                tab3ThirdSectionList.Add(new Tab3ThirdSection
                {
                    ScopeofWork = string.Empty, 
                    ContactDetails = string.Empty,
                    CountryofOrigin = string.Empty,
                    FormRegistrationNo = 0,
                    RegistrationNo = string.Empty,
                    Supplier = string.Empty,
                    PartitionKey = string.Empty,
                    RowKey = string.Empty
                });
            for (int i = 0; i < cntJson3; i++)
            {

                tab3ThirdSectionList.Add(new Tab3ThirdSection
                {

                    ScopeofWork = (string)myJObject3["value"][i]["ScopeofWork"],
                    ContactDetails = (string)myJObject3["value"][i]["ContactDetails"],
                    CountryofOrigin = (string)myJObject3["value"][i]["CountryofOrigin"],
                    FormRegistrationNo = (int)myJObject3["value"][i]["FormRegistrationNo"],
                    RegistrationNo = (string)myJObject3["value"][i]["RegistrationNo"],
                    Supplier = (string)myJObject3["value"][i]["Supplier"],
                    PartitionKey = (string)myJObject3["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject3["value"][i]["RowKey"]

                });
            }

            int categoryId = GetCategoryId(model.WorkDiscipline);


            LoadData(model, categoryId);
            model.selectedsubcategory = subCatid;



            List<Category> categorylistt = new List<Category>();
            categorylistt = ViewBag.ListofCategory;

            for (int k = 0; k < categorylistt.Count; k++)
            {
                if (categorylistt[k].CategoryName == c)
                {
                    model.SelectedCategory = categorylistt[k].CategoryName;
                }
            }

            //for sub category 
            List<subCategory> subcategorylistt = new List<subCategory>();
            subcategorylistt = ViewBag.ListSubCategory;

            for (int i = 0; i < subcategorylistt.Count; i++)
            {
                if (subcategorylistt[i].SubCategoryID == subCatid)
                {
                    //model.selectedsubcategory = subcategorylistt[i].SubCategoryID;
                    subcategorylistt[i].selected = true;


                }
            }


            model.tab3ThirdSection = tab3ThirdSectionList;
            model.Tab3Third = tab3ThirdSectionList.Count;
            model.formval = "Edit";
            List<Category> AList = new List<Category>();
            AList = ViewBag.ListofCategory;
            memoryCache.Set("ListofCategory", AList);
            //model.SelectedCategory = ViewBag.SelectedCategory;
            //memoryCache.Set("SelectedCategory", model.SelectedCategory);

            List<CertificatesList> listOfCertificates = ViewBag.ListOfCertificates;
            model.ListOfCertificates = listOfCertificates;
            memoryCache.Set("Form8", model);
            return RedirectToAction("CicForm8", "Form8");

        }

        private static string GetPostalAddress(JObject myJObject, int i, int pageId)
        {
            var postalAddressArray = ((string)myJObject["value"][i]["PostalAddress"])?.Split('|');
            if(postalAddressArray.Any() && postalAddressArray.Count() > 1) 
                return postalAddressArray[pageId];
            else 
                return string.Empty;
        }

        public void removeDatafromSession()
        {
            memoryCache.Remove("Form8");
            memoryCache.Remove("Signature1");
            memoryCache.Remove("WitnessSignature");
            memoryCache.Remove("WitnessSignature1");
            memoryCache.Remove("SummaryPage");
            memoryCache.Remove("LetterOfContract");
            memoryCache.Remove("Letterindicating");
            memoryCache.Remove("Signedletterupload");
            memoryCache.Remove("ListofCategory");
            memoryCache.Remove("ListOfSubContractors");


        }

        [HttpGet]
        public JsonResult GetData(string name)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm8 model = new SaveModelForm8();

            string jsonData;

            AzureTablesData.GetEntitybyCertificateJson(StorageName, StorageKey, "cicform1", name, out jsonData);

            var myJObject = jsonData;

            return Json(myJObject);
        }

        [HttpGet]
        public JsonResult GetCategoryData(string name)
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm8 model = new SaveModelForm8();

            string jsonData;

            AzureTablesData.GetEntitybyCertificateJson(StorageName, StorageKey, "cicform8", name, out jsonData);

            var myJObject = jsonData;

            return Json(myJObject);
        }

        //[HttpPost]
        //public JsonResult GetData(string name)
        //{

        //    string jsonData;
        //    string tCert = "";
        //    List<String> tempCertList = new List<String>();
        //    String[] clist;
        //    int count = 0;
        //    AzureTablesData.GetEntitybyCertificate(StorageName, StorageKey, "cicform1", out jsonData);
        //    JObject myJObject = JObject.Parse(jsonData);
        //    int cntJson = myJObject["value"].Count();
        //    string Grade = "NA";
        //    for (int i = 0; i < cntJson; i++)
        //    {

        //        tCert = (string)myJObject["value"][i]["CertificateNo"];
        //        if (tCert.Contains(','))
        //        {
        //            clist = tCert.Split(",");
        //            count = clist.Count();

        //            for (int j = 0; j <= count - 1; j++)
        //            {
        //                if (name == clist[j])
        //                {
        //                    Grade = (string)myJObject["value"][i]["Grade"];
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (name == tCert)
        //            {
        //                Grade = (string)myJObject["value"][i]["Grade"];
        //                break;
        //            }
        //        }
        //    }

        //    if (Grade.Contains(","))
        //    {
        //        clist = Grade.Split(",");
        //        count = clist.Count();

        //        for (int j = 0; j <= count - 1; j++)
        //        {
        //            if (name[0] == clist[j][0])
        //            {
        //                Grade = clist[j];
        //                break;
        //            }
        //        }

        //    }

        //    return Json(Grade);
        //}


        public MainViewModel loadData1(MainViewModel m, int CategoryID = 0)
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
            m.subCategoryModel = sub;
            //  m.businessModel = businessModel;
            return m;




        }
        public int GetCategoryId(string CategoryName)
        {
            //List<Category> categorylist = new List<Category>();
            int categoryId = (from Category in _context.Category
                              where Category.CategoryName == CategoryName && (Category.FormType == "F1" || Category.FormType == "F2")
                              select Category.CategoryID).FirstOrDefault();


            return categoryId;
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

            main.subCategoryModel = sub;
            return Json(main.subCategoryModel);
        }


        public JsonResult GetSubContractSupplierList()
        {
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            SaveModelForm8 model = new SaveModelForm8();

            string jsonData;
            AzureTablesData.GetSubcontractDetailsJson(StorageName, StorageKey, "CicForm8DetailsOfAllSubContractors", out jsonData);

            var myJObject = jsonData;

            return Json(myJObject);
        }



    }
}
