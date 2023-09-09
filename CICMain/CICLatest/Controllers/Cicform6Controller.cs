
using CICLatest.Helper;
using CICLatest.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{

    public class Cicform6Controller : Controller
    {
        private readonly ApplicationContext _context;
        int cnt = 1;
      
        Cicf6Model form1Model = new Cicf6Model();
        static string filepath = "NA";
        static string StorageName = "";
        static string StorageKey = "";
        private readonly UserManager<UserModel> _userManager;
        // string TableName = "CicForm6()";

        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        private readonly IMemoryCache memoryCache;
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public Cicform6Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
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


        public ActionResult AddCicform6()
        {
            //form1Model.c = cnt;
            //form1Model.d = cnt;

            //form1Model.educationBackground = new List<EducationBackGroundetails>();
            //form1Model.educationBackground.Add(new EducationBackGroundetails { HighestEducationLevel = "", EducationInstitution = "", QualificationsAttained = "" });
            //form1Model.backGroundetails = new List<BackGroundetails>();
            //form1Model.backGroundetails.Add(new BackGroundetails { NamesandSurname = "", TypeofWorkPerformed = "", TelephoneNo = "" });

            //ViewBag.type = "active";
            //loadData(form1Model);

            Cicf6Model form1EditModel = new Cicf6Model();

            form1Model.c = cnt;
            form1Model.d = cnt;
            bool isExist = memoryCache.TryGetValue("Form6", out form1EditModel);
            if (isExist)
            {
                List<AssociationList> AList = new List<AssociationList>();

                memoryCache.TryGetValue("listAssociation", out AList);
                ViewBag.ListofAssociation = AList;
                form1Model = form1EditModel;
            }
            if (!isExist)
            {

                form1Model.educationBackground = new List<EducationBackGroundetails>();
                form1Model.educationBackground.Add(new EducationBackGroundetails { HighestEducationLevel = "", EducationInstitution = "", QualificationsAttained = "",PartitionKey="-" ,RowKey="-" });
                form1Model.backGroundetails = new List<BackGroundetails>();
                form1Model.backGroundetails.Add(new BackGroundetails { NamesandSurname = "", TypeofWorkPerformed = "", TelephoneNo = "",PartitionKey="-",RowKey="-" });

              
            }
            ViewBag.type = "active";
            loadData(form1Model);
            return View(form1Model);
        }
        public IActionResult CicformReview()
        {
            return View();
        }
        public bool BusinessModelvalidations(Cicf6Model p)
        {
           
            bool DFlag = false;
            
            //if (p.personelDetails.NatureofTradeUpload == null)
            //{
            //    DFlag = true;
            //    ModelState.AddModelError("personelDetails.NatureofTradeUpload", "Please add Nature of Trade Upload");
            //}

            //if (p.personelDetails.IDNo != null)
            //{

            //    if (validateIdNo(p.personelDetails.IDNo))
            //    {
            //        ModelState.AddModelError("personelDetails.IDNo", "Invalid Id number!");
            //    }
            //}


            if (p.formval != "Edit")
            {
                if (p.personelDetails.NatureofTradeUpload == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("personelDetails.NatureofTradeUpload", "Please upload file");
                }
            }
            else
            {
                if (p.NatureofTradeUpload == null && p.personelDetails.NatureofTradeUpload == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("personelDetails.NatureofTradeUpload", "Please upload file");
                }
            }

            return DFlag;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCicform6(Cicf6Model p, string name, string next, string pre)
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
           
                //Grid section
                if (p.educationBackground != null)
                {
                    p.c = p.educationBackground.Count() + 1;


                }
                else
                {
                    p.c = cnt;
                    p.educationBackground = new List<EducationBackGroundetails>();
                    p.educationBackground.Add(new EducationBackGroundetails { HighestEducationLevel = null, EducationInstitution = null, QualificationsAttained = null,PartitionKey="-" ,RowKey="-"});
                }

                if (p.backGroundetails != null)
                {
                    p.d = p.backGroundetails.Count() + 1;


                }
                else
                {
                    p.d = cnt;
                    p.backGroundetails = new List<BackGroundetails>();
                    p.backGroundetails.Add(new BackGroundetails { NamesandSurname = null, TypeofWorkPerformed = null, TelephoneNo = null ,PartitionKey="-",RowKey="-"});
                }

            setDefault(p);
            fileDefault(p);
            //p.FormRegistrationNo = getRegNo(p);

            p.FormRegistrationNo = GenericHelper.GetRegNo(p.FormRegistrationNo, p.formval, _azureConfig); //AK

            // Model state no valid &next / Prev btn cicked -stay on same tab
            switch (name)
            {
                case "type":
                    bool AppFlag = IsAnyNullOrEmpty(p.App);
                    AppFlag = AppModelValidations(p);
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


                    //bool DFlag = IsAnyNullOrEmpty(p.personelDetails);
                    //DFlag= BusinessModelvalidations(p);

                    //if (DFlag == true)
                    //{
                    //    ViewBag.business = "active";
                    //}
                    //else if (DFlag == false && next != null)
                    //{
                    //    ViewBag.fin = "active";
                    //}
                    //else if (DFlag == false && pre != null)
                    //{
                    //    ViewBag.type = "active";
                    //}
                    bool BFlag = IsAnyNullOrEmpty(p.personelDetails);
                    bool DFlag;

                    DFlag = BusinessModelvalidations(p);

                    if (BFlag == true || DFlag == true)
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

                    UploadBlob(p, p.FormRegistrationNo);
                    break;


                case "finance":

                    bool FFlag = false;
                    bool aFlag = false;
                    //section c
                    if (p.educationBackground.Count >= 1)
                    { 
                      for (int j = 0; j < p.educationBackground.Count; j++)
                    {
                        if (p.educationBackground.Count >= 1)
                        {
                            if (p.educationBackground[j].HighestEducationLevel ==null )
                            {
                                FFlag = true;
                                break;
                            }
                           
                            else if (p.educationBackground[j].QualificationsAttained ==null)
                            {
                                FFlag = true;
                                break;
                            }

                            else if (p.educationBackground[j].EducationInstitution == null)
                             {
                                FFlag = true;
                                break;
                            }
                           
                        }


                    }
                    }
                    if (p.backGroundetails.Count > 0)
                    {
                        for (int i = 0; i < p.backGroundetails.Count; i++)
                        {

                            if (p.backGroundetails.Count >= 1)
                            {
                                if (p.backGroundetails[i].NamesandSurname == null)
                                {
                                    aFlag = true;
                                    break;
                                }
                                if (p.backGroundetails[i].TypeofWorkPerformed == null)
                                {
                                    aFlag = true;
                                    break;
                                }
                                if (p.backGroundetails[i].TelephoneNo == null)
                                {
                                    aFlag = true;
                                    break;
                                }

                            }
                        }
                    }

                    if ((FFlag == false && aFlag == false) && next != null)
                    {
                        ViewBag.work = "active";
                    }
                   
                    if ((FFlag == false && aFlag == false) && pre != null)
                    {
                        ViewBag.business = "active";
                    }
                    if (FFlag == true && aFlag == true)
                    {
                        ViewBag.fin = "active";
                    }
                    if (FFlag == false && aFlag == true)
                    {
                        ViewBag.fin = "active";
                    }
                    if (FFlag == true && aFlag == false)
                    {
                        ViewBag.fin = "active";
                    }
                    UploadBlob(p, p.FormRegistrationNo);
                    break;


                case "work":
                    //bool WFlag = IsAnyNullOrEmpty(p.declaration);

                    //if (WFlag == true)
                    //{
                    //    ViewBag.work = "active";
                    //}

                    //else if (WFlag == false && pre != null)
                    //{
                    //    ViewBag.fin = "active";
                    //}
                    bool docFlag =IsAnyNullOrEmpty(p.declaration);
                    docFlag = DocModelValidation(p);
                    if (docFlag == true)
                    {
                        ViewBag.work = "active";
                    }
                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.fin = "active";
                    }

                    UploadBlob(p, p.FormRegistrationNo);
                    break;

                case "draft":
                    p.Reviewer = "";
                    p.FormStatus = "Draft";
                    p.FormRegistrationNo = p.FormRegistrationNo;
                    UploadBlob(p, p.FormRegistrationNo);
                    string result3 = Savedata(p);
                    removeDatafromSession();

                    return RedirectToAction("Form6Result", "Cicform6", new { result = result3, text = "Draft" });
               

                case "final":
                    ModelState.Remove("p.ImagePath");
                   
                  //  fileDefault(p);
                    UploadBlob(p, p.FormRegistrationNo);
                    if (p.App.File==null)
                      {
                        p.App.File = "-";
                      }
                    //bool A = IsAnyNullOrEmpty(p.App);
                    //if (A == true)
                    //{
                    //    ViewBag.type = "active";
                    //    break;
                    //}
                    bool A =IsAnyNullOrEmpty(p.App);
                    A = AppModelValidations(p);
                    if (A == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }


                    //bool B = IsAnyNullOrEmpty(p.personelDetails);
                    //if (B == true)
                    //{
                    //    ViewBag.business = "active";
                    //    break;
                    //}

                    bool B = IsAnyNullOrEmpty(p.personelDetails);
                    B = BusinessModelvalidations(p);
                    if (B == true)
                    {
                        ViewBag.business = "active";
                        break;
                    }
                    bool E = checkTab3(p, next, pre);
                    if (E == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }

                    //bool C = IsAnyNullOrEmpty(p.declaration);
                    //if (C == true)
                    //{
                    //    ViewBag.work = "active";
                    //    break;
                    //}



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
                       //UploadBlob(p, p.FormRegistrationNo);
                        string result4 = Savedata(p);
                        removeDatafromSession();

                        return RedirectToAction("Form6Result", "Cicform6", new { result = result4, text = "Final" });

                    }
                    else
                    {
                        ViewBag.work = "active";
                    }
                    break;


                    #region

            }
            loadData(p);
           
            return View(p);
            
        }
        public bool checkTab3(Cicf6Model p, string next, string pre)
        {
          
            bool aFlag = false;
            bool bFlag = false;

          


            if (p.educationBackground != null)
            {
                if (p.educationBackground.Count > 0)
                {
                    for (int i = 0; i < p.educationBackground.Count; i++)
                    {

                        if (p.educationBackground.Count >= 1)
                        {
                            if (p.educationBackground[i].HighestEducationLevel == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.educationBackground[i].QualificationsAttained == null)
                            {
                                aFlag = true;
                                break;
                            }
                            if (p.educationBackground[i].EducationInstitution == null)
                            {
                                aFlag = true;
                                break;
                            }
                           
                        }
                    }
                }
            }
            if (p.backGroundetails != null)
            {
                if (p.backGroundetails.Count > 0)
                {
                    for (int i = 0; i < p.backGroundetails.Count; i++)
                    {

                        if (p.backGroundetails.Count >= 1)
                        {
                            if (p.backGroundetails[i].NamesandSurname == null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.backGroundetails[i].TypeofWorkPerformed== null)
                            {
                                bFlag = true;
                                break;
                            }
                            if (p.backGroundetails[i].TelephoneNo == null)
                            {
                                bFlag = true;
                                break;
                            }
                           
                        }
                    }
                }
            }

            if (( aFlag == false && bFlag == false) && next != null)
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
        public bool DocModelValidation(Cicf6Model p)
        {
            bool AppFlag = false;
            if (p.declaration.Namee == null)
            {
                AppFlag = true;
                ModelState.AddModelError("declaration.Namee", "Please enter Name");
            }
            if (p.formval != "Edit")
            {
                if (p.declaration.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("declaration.Signature", "Please upload signature");
                }
            }
            else
            {
                if ((p.Signature == null || p.Signature=="-") && p.declaration.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("declaration.Signature", "Please upload file");
                }
            }
            if (p.declaration.TitleDesignation == null)
            {
                AppFlag = true;
                ModelState.AddModelError("declaration.TitleDesignation", "Please enter title");
            }

            if (p.declaration.TermsAndConditions == false)
            {
                AppFlag = true;
                ModelState.AddModelError("declaration.TermsAndConditions", "Please accept Terms and conditions");
            }

            return AppFlag;
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
       

            public void setGetFileEdit(Cicf6Model p)
            {
            if (p.App.File != null)
            {
                memoryCache.Set("File", p.App.Filee);
                p.File = p.App.Filee.FileName;
                memoryCache.Set("File", p.App.Filee.FileName);
            }
            else
            {
                p.File = SetandGetFileEdit("File");
            }
            //
            if (p.personelDetails.NatureofTradeUpload != null)
            {
                memoryCache.Set("NatureofTradeUpload", p.personelDetails.NatureofTradeUpload);
                p.NatureofTradeUpload = p.personelDetails.NatureofTradeUpload.FileName;
                memoryCache.Set("NatureofTradeUpload", p.personelDetails.NatureofTradeUpload.FileName);
            }
            else
            {
                p.NatureofTradeUpload = SetandGetFileEdit("NatureofTradeUpload");
            }
            //lll
            if (p.documentUpload.fileupload1 != null)
            {
                memoryCache.Set("fileupload1", p.documentUpload.fileupload1);
                p.fileupload1 = p.documentUpload.fileupload1.FileName;
                memoryCache.Set("fileupload1", p.documentUpload.fileupload1.FileName);
            }
            else
            {
                p.fileupload1 = SetandGetFileEdit("fileupload1");
            }
            //
            if (p.documentUpload.fileupload2 != null)
            {
                memoryCache.Set("fileupload2", p.documentUpload.fileupload2);
                p.fileupload2 = p.documentUpload.fileupload2.FileName;
                memoryCache.Set("fileupload2", p.documentUpload.fileupload2.FileName);
            }
            else
            {
                p.fileupload2 = SetandGetFileEdit("fileupload2");
            }

            if (p.documentUpload.fileupload3 != null)
            {
                memoryCache.Set("fileupload3", p.documentUpload.fileupload3);
                p.fileupload3 = p.documentUpload.fileupload3.FileName;
                memoryCache.Set("fileupload3", p.documentUpload.fileupload3.FileName);
            }
            else
            {
                p.fileupload3 = SetandGetFileEdit("fileupload3");
            }


            //

            if (p.documentUpload.fileupload4 != null)
            {
                memoryCache.Set("fileupload4", p.documentUpload.fileupload4);
                p.fileupload4 = p.documentUpload.fileupload4.FileName;
                memoryCache.Set("fileupload4", p.documentUpload.fileupload4.FileName);
            }
            else
            {
                p.fileupload4 = SetandGetFileEdit("fileupload4");
            }
            //

            if (p.documentUpload.fileupload6 != null)
            {
                memoryCache.Set("fileupload6", p.documentUpload.fileupload6);
                p.fileupload6 = p.documentUpload.fileupload6.FileName;
                memoryCache.Set("fileupload6", p.documentUpload.fileupload6.FileName);
            }
            else
            {
                p.fileupload6 = SetandGetFileEdit("fileupload6");
            }
            if (p.declaration.Signature != null)
            {
                memoryCache.Set("Signature", p.declaration.Signature);
                p.Signature = p.declaration.Signature.FileName;
                memoryCache.Set("Signature", p.declaration.Signature.FileName);
            }
            else
            {
                p.Signature = SetandGetFileEdit("Signature");
            }
            
        }
        public IActionResult IndexFromDashboard(string rowkey)
        {
            ViewBag.type = "active";
            Cicf6Model model = new Cicf6Model();
            

            string c = "";
            string associate = "";
            //model.c = cnt;
            //model.d = cnt;
           
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm6", rowkey, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            //set master table values into model 
            for (int i = 0; i < cntJson; i++)
            {
                //adding for file name 
               // path = (string)myJObject["value"][i]["path"];

                //adding for files 
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                path = (string)myJObject["value"][i]["ImagePath"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                string key, value;
                AllFileList = _blobStorageService.GetBlobList(path);
                string Signature = null, fileupload1 = null, fileupload2 = null, fileupload3 = null, File = null, NatureofTradeUpload = null, fileupload6 = null, fileupload4 = null;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;

                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);

                        switch (key)
                        {
                            case "Filee": File = AllFileList[j].FileValue; break;
                            case "NatureofTradeUpload": NatureofTradeUpload = AllFileList[j].FileValue; break;


                          

                            case "fileupload1": fileupload1 = AllFileList[j].FileValue; break;
                            case "fileupload2": fileupload2 = AllFileList[j].FileValue; break;

                            case "fileupload3": fileupload3 = AllFileList[j].FileValue; break;


                            case "fileupload6": fileupload6 = AllFileList[j].FileValue; break;
                            case "Signature": Signature = AllFileList[j].FileValue; break;
                            case "fileupload4": fileupload4 = AllFileList[j].FileValue; break;


                        }
                    }
                }


                ApplicationTypeModell App = new ApplicationTypeModell
                {


                    AppType = (string)myJObject["value"][i]["AppType"],
                    AssociationName = (string)myJObject["value"][i]["AssociationName"],
                    AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"],
                    File = File,
                    Categories = (string)myJObject["value"][i]["Categories"],
                   
                    

                };


                c = (string)myJObject["value"][i]["Categories"];
                associate = (string)myJObject["value"][i]["AssociationName"];
                model.App = App;
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.RowKey = (string)myJObject["value"][i]["RowKey"];
                model.PartitionKey = (string)myJObject["value"][i]["PartitionKey"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
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


                //string Fax = "";
                PersonelDetailsModel persnel = new PersonelDetailsModel
                {


                    Name = (string)myJObject["value"][i]["Name"],
                    Surname = (string)myJObject["value"][i]["Surname"],
                    IDNo = (string)myJObject["value"][i]["IDNo"],
                    Nationality = (string)myJObject["value"][i]["Nationality"],
                    HomeArea = (string)myJObject["value"][i]["HomeArea"],
                    Chief = (string)myJObject["value"][i]["Chief"],
                    Indvuna = (string)myJObject["value"][i]["Indvuna"],
                    TemporalResidencePermitNo = (string)myJObject["value"][i]["TemporalResidencePermitNo"],
                    WorkPermitNo = (string)myJObject["value"][i]["WorkPermitNo"],
                    CellphoneNo = (string)myJObject["value"][i]["CellphoneNo"],
                    Emailaddress = (string)myJObject["value"][i]["Emailaddress"],
                    ResidentialAddress = (string)myJObject["value"][i]["ResidentialAddress"],
                    NextofKin = (string)myJObject["value"][i]["NextofKin"],
                    Relationship = (string)myJObject["value"][i]["Relationship"],
                    ContactNo = (string)myJObject["value"][i]["ContactNo"],
                    NatureofTradeExpertise = (string)myJObject["value"][i]["NatureofTradeExpertise"],
                    
                    




                };
                DocumentUpload doc = new DocumentUpload
                {

                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                    WitnessedName = (string)myJObject["value"][i]["WitnessedName"],
                    WitnessedSignatureTitleDesignation = (string)myJObject["value"][i]["WitnessedSignatureTitleDesignation"]

                };
                Declaration dec = new Declaration
                {
                    Namee = (string)myJObject["value"][i]["Namee"],
                    TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"]

                };
                model.declaration = dec;
                model.documentUpload = doc;

                model.personelDetails = persnel;
                model.NatureofTradeUpload = NatureofTradeUpload;
                model.fileupload1 = fileupload1;
                model.fileupload2 = fileupload2;
                model.fileupload3 = fileupload3;
                model.fileupload4 = fileupload4;
              
                model.Signature = Signature;
                model.fileupload6 = fileupload6;
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
                    QualificationsAttained = (string)myJObject1["value"][i]["QualificationsAttained"],
                    PartitionKey = (string)myJObject1["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject1["value"][i]["RowKey"]

                });
            }
            model.educationBackground = d;
            if (model.educationBackground.Count == 0)
            {
                model.c = 1;

                model.educationBackground = new List<EducationBackGroundetails>();
                model.educationBackground.Add(new EducationBackGroundetails { HighestEducationLevel = "", EducationInstitution = "", QualificationsAttained = "", PartitionKey = "-", RowKey = "-" });


            }
            else
            {
                model.c = model.educationBackground.Count;
            }

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
                    TelephoneNo = (string)myJObject2["value"][i]["TelephoneNo"],
                    PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject2["value"][i]["RowKey"]


                });
            }
            model.backGroundetails = a;
            if (model.backGroundetails.Count == 0)
            {
                model.d = 1;

                model.backGroundetails = new List<BackGroundetails>();
                model.backGroundetails.Add(new BackGroundetails { NamesandSurname = "", TypeofWorkPerformed = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });

            }
            else
            {
                model.d = model.backGroundetails.Count;



            }

            loadData(model);
            List<Category> categorylistt = new List<Category>();
            categorylistt = ViewBag.ListofCategory;
            //set category and association into dropdown 
            for (int k = 0; k < categorylistt.Count; k++)
            {
                if (categorylistt[k].CategoryName == c)
                {
                    model.App.Categories = c;
                }
            }
          
            List<AssociationList> AList = new List<AssociationList>();
            AList = ViewBag.ListofAssociation;
            memoryCache.Set("listAssociation", AList);

            model.formval = "Edit";
            memoryCache.Set("Form6", model);

            return RedirectToAction("AddCicform6", "Cicform6");


        }

        public IActionResult GetData(string apptype)
        {
            ViewBag.type = "active";
            Cicf6Model model = new Cicf6Model();
            string c = "";
            string associate = "";                     
            string jsonData;
            AzureTablesData.GetEntitybyLoginId(StorageName, StorageKey, "CicForm6", User.Identity.Name, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);

            var latestRecord = (from rec in myJObject["value"]
                                orderby (int)rec["FormRegistrationNo"] descending
                                select rec).FirstOrDefault();

            if(latestRecord != null)
            {
                model.RowKey = (string)latestRecord["RowKey"];
                model.CustNo = (string)latestRecord["CustNo"];
                ApplicationTypeModell App = new ApplicationTypeModell
                {
                    AppType = apptype,
                    AssociationName = (string)latestRecord["AssociationName"],
                    AuthorisedOfficerName = (string)latestRecord["AuthorisedOfficerName"],
                    Categories = (string)latestRecord["Categories"]
                };

                c = (string)latestRecord["Categories"];
                associate = (string)latestRecord["AssociationName"];

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

                model.App = App;

                PersonelDetailsModel persnel = new PersonelDetailsModel
                {
                    Name = (string)latestRecord["Name"],
                    Surname = (string)latestRecord["Surname"],
                    IDNo = (string)latestRecord["IDNo"],
                    Nationality = (string)latestRecord["Nationality"],
                    HomeArea = (string)latestRecord["HomeArea"],
                    Chief = (string)latestRecord["Chief"],
                    Indvuna = (string)latestRecord["Indvuna"],
                    TemporalResidencePermitNo = (string)latestRecord["TemporalResidencePermitNo"],
                    WorkPermitNo = (string)latestRecord["WorkPermitNo"],
                    CellphoneNo = (string)latestRecord["CellphoneNo"],
                    Emailaddress = (string)latestRecord["Emailaddress"],
                    ResidentialAddress = (string)latestRecord["ResidentialAddress"],
                    NextofKin = (string)latestRecord["NextofKin"],
                    Relationship = (string)latestRecord["Relationship"],
                    ContactNo = (string)latestRecord["ContactNo"],
                    NatureofTradeExpertise = (string)latestRecord["NatureofTradeExpertise"]
                };
                model.personelDetails = persnel;
                DocumentUpload doc = new DocumentUpload
                {
                    FaxNo = (string)latestRecord["FaxNo"],
                    WitnessedName = (string)latestRecord["WitnessedName"],
                    WitnessedSignatureTitleDesignation = (string)latestRecord["WitnessedSignatureTitleDesignation"]
                };
                Declaration dec = new Declaration
                {
                    Namee = (string)latestRecord["Namee"],
                    TitleDesignation = (string)latestRecord["TitleDesignation"]
                };
                model.declaration = dec;
                model.documentUpload = doc;

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6Education", model.RowKey, out jsonData1);
                JObject myJObject1 = JObject.Parse(jsonData1);
                int cntJson1 = myJObject1["value"].Count();
                List<EducationBackGroundetails> d = new List<EducationBackGroundetails>();
                for (int i = 0; i < cntJson1; i++)
                {

                    d.Add(new EducationBackGroundetails
                    {
                        EducationInstitution = (string)myJObject1["value"][i]["EducationInstitution"],
                        HighestEducationLevel = (string)myJObject1["value"][i]["HighestEducationLevel"],
                        QualificationsAttained = (string)myJObject1["value"][i]["QualificationsAttained"],
                        PartitionKey = "-",
                        RowKey = "-"

                    });
                }
                model.educationBackground = d;
                if (model.educationBackground.Count == 0)
                {
                    model.c = 1;
                    model.educationBackground = new List<EducationBackGroundetails>();
                    model.educationBackground.Add(new EducationBackGroundetails { HighestEducationLevel = "", EducationInstitution = "", QualificationsAttained = "", PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.c = model.educationBackground.Count;
                }

                string jsonData2;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6ReferenceDetails", model.RowKey, out jsonData2);
                JObject myJObject2 = JObject.Parse(jsonData2);
                int cntJson2 = myJObject2["value"].Count();
                List<BackGroundetails> a = new List<BackGroundetails>();
                for (int i = 0; i < cntJson2; i++)
                {

                    a.Add(new BackGroundetails
                    {
                        NamesandSurname = (string)myJObject2["value"][i]["NamesandSurname"],
                        TypeofWorkPerformed = (string)myJObject2["value"][i]["TypeofWorkPerformed"],
                        TelephoneNo = (string)myJObject2["value"][i]["TelephoneNo"],
                        PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                        RowKey = (string)myJObject2["value"][i]["RowKey"]


                    });
                }
                model.backGroundetails = a;
                if (model.backGroundetails.Count == 0)
                {
                    model.d = 1;
                    model.backGroundetails = new List<BackGroundetails>();
                    model.backGroundetails.Add(new BackGroundetails { NamesandSurname = "", TypeofWorkPerformed = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.d = model.backGroundetails.Count;
                }

                loadData(model);
                List<Category> categorylistt = new List<Category>();
                categorylistt = ViewBag.ListofCategory;

                for (int k = 0; k < categorylistt.Count; k++)
                {
                    if (categorylistt[k].CategoryName == c)
                    {
                        model.App.Categories = c;
                    }
                }

                List<AssociationList> AList = new List<AssociationList>();
                AList = ViewBag.ListofAssociation;
                memoryCache.Set("listAssociation", AList);
                memoryCache.Set("Form6", model);
            }

            return RedirectToAction("AddCicform6", "Cicform6");
        }

        public void removeDatafromSession()
        {
            memoryCache.Remove("File");
            memoryCache.Remove("NatureofTradeUpload");
            memoryCache.Remove("fileupload1");
            memoryCache.Remove("fileupload2");
            memoryCache.Remove("fileupload3");
            memoryCache.Remove("fileupload6");
            memoryCache.Remove("Signature");
            memoryCache.Remove("Form6");




        }
        public string Savedata(Cicf6Model p1)
             {

            string FormRegNo = "";
            string response = "";
            SaveModelForm6 saveModelForm6 = new SaveModelForm6();

           // p1.FormStatus = "Submit";
            string TableName = "CicForm6()";
            //int tempMax = 0;

            int tempMax = p1.FormRegistrationNo;
            AddNewRegistrationNo addNew = new AddNewRegistrationNo();
            addNew.PartitionKey = tempMax.ToString();
            addNew.RowKey = "Form" + tempMax.ToString();
            addNew.ProjectRegistrationNo = tempMax.ToString();
            response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));

            if (p1.App.AuthorisedOfficerName == null)
                {
                    p1.App.AuthorisedOfficerName = "-";
                }

                saveModelForm6.PartitionKey = p1.App.AuthorisedOfficerName;
                  FormRegNo = "Form" + p1.FormRegistrationNo.ToString(); //AK
            //}
            saveModelForm6.FormRegistrationNo = tempMax;
            saveModelForm6.FormName = "Form6";
            saveModelForm6.AssociationName = p1.App.AssociationName;
            
             saveModelForm6.AuthorisedOfficerName = p1.App.AuthorisedOfficerName;
               
                saveModelForm6.AppType = p1.App.AppType;
                saveModelForm6.Categories = p1.App.Categories;
                saveModelForm6.Name = p1.personelDetails.Name;
                saveModelForm6.Surname = p1.personelDetails.Surname;
                saveModelForm6.IDNo = p1.personelDetails.IDNo;
                saveModelForm6.Nationality = p1.personelDetails.Nationality;
                saveModelForm6.HomeArea = p1.personelDetails.HomeArea;
                saveModelForm6.Chief = p1.personelDetails.Chief;
                saveModelForm6.Indvuna = p1.personelDetails.Indvuna;
                saveModelForm6.TemporalResidencePermitNo = p1.personelDetails.TemporalResidencePermitNo;
                saveModelForm6.WorkPermitNo = p1.personelDetails.WorkPermitNo;
                saveModelForm6.CellphoneNo = p1.personelDetails.CellphoneNo;
                saveModelForm6.Emailaddress = p1.personelDetails.Emailaddress;
                saveModelForm6.FaxNo = p1.documentUpload.FaxNo;
                saveModelForm6.ResidentialAddress = p1.personelDetails.ResidentialAddress;
                saveModelForm6.NextofKin = p1.personelDetails.NextofKin;
                saveModelForm6.Relationship = p1.personelDetails.Relationship;
                saveModelForm6.ContactNo = p1.personelDetails.ContactNo;
                saveModelForm6.NatureofTradeExpertise = p1.personelDetails.NatureofTradeExpertise;
                saveModelForm6.Namee = p1.declaration.Namee;
               // saveModelForm6.Signature = p1.declaration.Signature;
                saveModelForm6.TitleDesignation = p1.declaration.TitleDesignation;
                saveModelForm6.WitnessedName = p1.documentUpload.WitnessedName;
            //  saveModelForm6.TitleDesignation = p1.documentUpload.WitnessedSignatureTitleDesignation;
                saveModelForm6.WitnessedSignatureTitleDesignation = p1.documentUpload.WitnessedSignatureTitleDesignation;

            saveModelForm6.FormStatus = p1.FormStatus;
                saveModelForm6.Reviewer = p1.Reviewer.Trim();
                saveModelForm6.CreatedBy = User.Identity.Name;
            saveModelForm6.CustNo = HttpContext.Session.GetString("CustNo");
            memoryCache.Set("emailto", User.Identity.Name);

            //storing upload file in blobe 

            //    if (p1.App.Filee != null)
            //    {
            //        uploadFiles(p1.App.Filee, p1.ImagePath, "Filee");
            //    }
            //    if (p1.personelDetails.NatureofTradeUpload != null)
            //    {
            //        uploadFiles(p1.personelDetails.NatureofTradeUpload, p1.ImagePath, "NatureofTradeUpload");
            //    }

            //    if (p1.documentUpload.fileupload1 != null)
            //    {
            //        uploadFiles(p1.documentUpload.fileupload1, p1.ImagePath, "fileupload1");

            //    }
            //    if (p1.documentUpload.fileupload2 != null)
            //    {
            //        uploadFiles(p1.documentUpload.fileupload2, p1.ImagePath, "fileupload2");

            //    }
            //    if (p1.documentUpload.fileupload3 != null)
            //    {
            //        uploadFiles(p1.documentUpload.fileupload3, p1.ImagePath, "fileupload3");

            //    }
            //if (p1.documentUpload.fileupload4 != null)
            //{
            //    uploadFiles(p1.documentUpload.fileupload4, p1.ImagePath, "fileupload4");

            //}


            //if (p1.declaration.Signature != null)
            //    {
            //        uploadFiles(p1.declaration.Signature, p1.ImagePath, "Signature");
            //    }
            //    if (p1.documentUpload.fileupload6 != null)
            //    {
            //        uploadFiles(p1.documentUpload.fileupload6, p1.ImagePath, "fileupload6");
            //    }


            //adding new for Uploding

            if (filepath != "NA")
                {
                    saveModelForm6.ImagePath = filepath;
                }
                else
                {
                    if (!filepath.Contains("https"))
                    {
                        var imagepath = _appSettingsReader.Read("ImagePath");
                        saveModelForm6.ImagePath = imagepath + @"2022-02-21\" + filepath;
                    }
                }




            
            //saving data for section c and d

            //TableName = "cicform6Education()";

            if (p1.formval == "Edit")
            {

                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm6", JsonConvert.SerializeObject(saveModelForm6, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), saveModelForm6.PartitionKey, saveModelForm6.RowKey);
            }

            else
            {
                saveModelForm6.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm6", JsonConvert.SerializeObject(saveModelForm6, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            }

            if (response == "Created" || response == "NoContent")
            {
                EducationBackGroundetails d = new EducationBackGroundetails();
                if(p1.educationBackground!=null)
                {
                    for (int i = 0; i < p1.educationBackground.Count; i++)
                    {

                        string Data;

                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform6Education", p1.educationBackground[i].PartitionKey, p1.educationBackground[i].RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        if (cntJson != 0)
                        {

                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform6Education", p1.educationBackground[i].PartitionKey, p1.educationBackground[i].RowKey, Data);


                        }


                        d.PartitionKey = p1.educationBackground[i].HighestEducationLevel;
                        d.RowKey = "Form" + tempMax.ToString(); //AK

                        d.HighestEducationLevel = p1.educationBackground[i].HighestEducationLevel;
                        d.QualificationsAttained = p1.educationBackground[i].QualificationsAttained;
                        d.EducationInstitution = p1.educationBackground[i].EducationInstitution;
                        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform6Education", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    }

                }
                BackGroundetails referenceDetails = new BackGroundetails();
                if (p1.backGroundetails != null)
                {
                    for (int i = 0; i < p1.backGroundetails.Count; i++)
                    {

                        string Data;

                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform6ReferenceDetails", p1.backGroundetails[i].PartitionKey, p1.backGroundetails[i].RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        if (cntJson != 0)
                        {

                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "cicform6ReferenceDetails", p1.backGroundetails[i].PartitionKey, p1.backGroundetails[i].RowKey, Data);


                        }


                        referenceDetails.PartitionKey = p1.backGroundetails[i].NamesandSurname;
                        referenceDetails.RowKey = "Form" + tempMax.ToString(); //AK

                        referenceDetails.TypeofWorkPerformed = p1.backGroundetails[i].TypeofWorkPerformed;
                        referenceDetails.TelephoneNo = p1.backGroundetails[i].TelephoneNo;
                        referenceDetails.NamesandSurname = p1.backGroundetails[i].NamesandSurname;
                        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform6ReferenceDetails", JsonConvert.SerializeObject(referenceDetails, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    }

                }


            }
            //EducationBackGroundetails d = new EducationBackGroundetails();
            //if (response == "Created")
            //{
            //    for (int i = 0; i < p1.educationBackground.Count; i++)
            //    {
            //        d.PartitionKey = p1.educationBackground[i].HighestEducationLevel;
            //        d.RowKey ="PRN"+tempMax.ToString();

            //        d.HighestEducationLevel = p1.educationBackground[i].HighestEducationLevel;
            //        d.QualificationsAttained = p1.educationBackground[i].QualificationsAttained;
            //        d.EducationInstitution = p1.educationBackground[i].EducationInstitution;
            //        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform6Education", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            //    }

            //    // TableName = "cicform6ReferenceDetails()";
            //    BackGroundetails referenceDetails = new BackGroundetails();

            //    for (int i = 0; i < p1.backGroundetails.Count; i++)
            //    {
            //        referenceDetails.PartitionKey = p1.backGroundetails[i].NamesandSurname;
            //        referenceDetails.RowKey ="PRN"+tempMax.ToString();

            //        referenceDetails.TypeofWorkPerformed = p1.backGroundetails[i].TypeofWorkPerformed;
            //        referenceDetails.TelephoneNo = p1.backGroundetails[i].TelephoneNo;
            //        referenceDetails.NamesandSurname = p1.backGroundetails[i].NamesandSurname;
            //        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform6ReferenceDetails", JsonConvert.SerializeObject(referenceDetails, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            //    }
            //}




            #endregion

            //}
            // return response;
            
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform6", FormRegNo, out jsonData1);//Get data
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            for (int i = 0; i < cntJson2; i++)
                AddCustinERP(myJObject2, i);

            return FormRegNo;

        }
        public bool AppModelValidations(Cicf6Model p)
        {
            bool AppFlag = false;
            //if (p.App.Filee == null)
            //{
            //    AppFlag = true;
            //    ModelState.AddModelError("App.Filee", "Please add Association Attachment");
            //}
            if (p.formval != "Edit")
            {
                if (p.App.Filee == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.Filee", "Please upload file");
                }
            }
            else
            {
                if (p.File == null && p.App.Filee == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.Filee", "Please upload file");
                }
            }

            if (p.App.Categories == "Select")
            {
                AppFlag = true;
                ModelState.AddModelError("App.Categories", "Please select category");
            }
            if (p.App.AssociationName == "Select association name")
            {
                AppFlag = true;
                ModelState.AddModelError("App.AssociationName", "Please select Association Name");
            }
            if (p.App.AuthorisedOfficerName == null)
            {
                AppFlag = true;
                ModelState.AddModelError("App.AuthorisedOfficerName", "Please enter Authorised Officer Name");
            }


            return AppFlag;
        }
        public IActionResult Form6Result(string result, string text)
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
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='" + domain + "'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
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
        bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }

            }
            return false;
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


        public void loadData(Cicf6Model m)
        {

            List<tblAssociation> associationlist = new List<tblAssociation>();
            List<AssociationList> AList = new List<AssociationList>();

            associationlist = (from association in _context.tblAssociation where association.formType == "Form1" select association).ToList();

           

            AList.Add(new AssociationList { AssociationName = "Select association name", AssociationName1 = "Select Association Name" });
            for (int i = 0; i < associationlist.Count; i++)
            {
                AList.Add(new AssociationList { AssociationName = associationlist[i].AssociationName, AssociationName1 = associationlist[i].AssociationName });
            }

            ViewBag.ListofAssociation = AList;

            //List<Category> categorylist = new List<Category>();
            //categorylist = (from Category in _context.Category
            //                where Category.FormType == "F6"
            //                select Category).ToList();




            List<Category> categorylistt = new List<Category>();

            //------- Getting data from database using EntityframeworkCore -------
            categorylistt = (from category in _context.Category where category.FormType == "F6" select category).ToList();

            //------- Inserting select items in list -------
            categorylistt.Insert(0, new Category { CategoryID = 0, CategoryName = "Select" });

            //------- Assigning categorylist to ViewBag.ListofCategory -------
            ViewBag.ListofCategory = categorylistt;

        }

        public Cicf6Model SetandGetFileAdd(Cicf6Model p)
        {
            IFormFile fsign;
            string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("File", out fsign);


            if (!isExist)
            {
                if (p.App.Filee != null)
                {
                    memoryCache.Set("File", p.App.Filee);
                    p.App.File = p.App.Filee.FileName;
                }
            }
            else
            {
                if (p.App.Filee != null)
                {
                    memoryCache.Set("File", p.App.Filee);
                }
                else
                {
                    p.App.Filee = fsign;
                }
                p.App.File = p.App.Filee.FileName;

            }


            isExist = memoryCache.TryGetValue("NatureofTradeUpload", out fsign);
            if (!isExist)
            {
                if (p.personelDetails.NatureofTradeUpload != null)
                {
                    memoryCache.Set("NatureofTradeUpload", p.personelDetails.NatureofTradeUpload);
                    p.NatureofTradeUpload = p.personelDetails.NatureofTradeUpload.FileName;
                }
            }
            else
            {
                if (p.personelDetails.NatureofTradeUpload != null)
                {
                    memoryCache.Set("NatureofTradeUpload", p.personelDetails.NatureofTradeUpload);
                }
                else
                {
                    p.personelDetails.NatureofTradeUpload = fsign;
                }
                p.NatureofTradeUpload = p.personelDetails.NatureofTradeUpload.FileName;

            }

            isExist = memoryCache.TryGetValue("fileupload1", out fsign);
            if (!isExist)
            {
                if (p.documentUpload.fileupload1 != null)
                {
                    memoryCache.Set("fileupload1", p.documentUpload.fileupload1);
                    p.fileupload1 = p.documentUpload.fileupload1.FileName;
                }
            }
            else
            {
                if (p.documentUpload.fileupload1 != null)
                {
                    memoryCache.Set("fileupload1", p.documentUpload.fileupload1);
                }
                else
                {
                    p.documentUpload.fileupload1 = fsign;
                }
                p.fileupload1 = p.documentUpload.fileupload1.FileName;

            }

            isExist = memoryCache.TryGetValue("fileupload2", out fsign);
            if (!isExist)
            {
                if (p.documentUpload.fileupload2 != null)
                {
                    memoryCache.Set("fileupload2", p.documentUpload.fileupload2);
                    p.fileupload2 = p.documentUpload.fileupload2.FileName;
                }
            }
            else
            {
                if (p.documentUpload.fileupload2 != null)
                {
                    memoryCache.Set("fileupload2", p.documentUpload.fileupload2);
                }
                else
                {
                    p.documentUpload.fileupload2 = fsign;
                }
                p.fileupload2 = p.documentUpload.fileupload2.FileName;

            }
            isExist = memoryCache.TryGetValue("fileupload3", out fsign);
            if (!isExist)
            {
                if (p.documentUpload.fileupload3 != null)
                {
                    memoryCache.Set("fileupload3", p.documentUpload.fileupload3);
                    p.fileupload3 = p.documentUpload.fileupload3.FileName;
                }
            }
            else
            {
                if (p.documentUpload.fileupload3 != null)
                {
                    memoryCache.Set("fileupload3", p.documentUpload.fileupload3);
                }
                else
                {
                    p.documentUpload.fileupload3 = fsign;
                }
                p.fileupload3 = p.documentUpload.fileupload3.FileName;

            }

            isExist = memoryCache.TryGetValue("fileupload4", out fsign);
            if (!isExist)
            {
                if (p.documentUpload.fileupload4 != null)
                {
                    memoryCache.Set("fileupload4", p.documentUpload.fileupload4);
                    p.fileupload4 = p.documentUpload.fileupload4.FileName;
                }
            }
            else
            {
                if (p.documentUpload.fileupload4 != null)
                {
                    memoryCache.Set("fileupload4", p.documentUpload.fileupload4);
                }
                else
                {
                    p.documentUpload.fileupload4 = fsign;
                }
                p.fileupload4 = p.documentUpload.fileupload4.FileName;

            }



            isExist = memoryCache.TryGetValue("fileupload6", out fsign);
            if (!isExist)
            {
                if (p.documentUpload.fileupload6 != null)
                {
                    memoryCache.Set("fileupload6", p.documentUpload.fileupload6);
                    p.fileupload6 = p.documentUpload.fileupload6.FileName;
                }
            }
            else
            {
                if (p.documentUpload.fileupload6 != null)
                {
                    memoryCache.Set("fileupload6", p.documentUpload.fileupload6);
                }
                else
                {
                    p.documentUpload.fileupload6 = fsign;
                }
                p.fileupload6 = p.documentUpload.fileupload6.FileName;

            }


            isExist = memoryCache.TryGetValue("Signature", out fsign);
            if (!isExist)
            {
                if (p.declaration.Signature != null)
                {
                    memoryCache.Set("Signature", p.declaration.Signature);
                    p.Signature = p.declaration.Signature.FileName;
                }
            }
            else
            {
                if (p.declaration.Signature != null)
                {
                    memoryCache.Set("Signature", p.declaration.Signature);
                }
                else
                {
                    p.declaration.Signature = fsign;
                }
                p.Signature = p.declaration.Signature.FileName;

            }

            return p;
        }
        public void UploadBlob(Cicf6Model p1, int tempMax)
        {
            if (p1.ImagePath == "NA" || p1.ImagePath == "-")
            {
                p1.ImagePath = "Form" + tempMax; //AK
            }
            if (p1.App.Filee != null)
            {
                uploadFiles1(p1.App.Filee, p1.ImagePath, "Filee");
            }
            if (p1.personelDetails.NatureofTradeUpload != null)
            {
                uploadFiles1(p1.personelDetails.NatureofTradeUpload, p1.ImagePath, "NatureofTradeUpload");
            }

            if (p1.documentUpload.fileupload1 != null)
            {
                uploadFiles1(p1.documentUpload.fileupload1, p1.ImagePath, "fileupload1");

            }
            if (p1.documentUpload.fileupload2 != null)
            {
                uploadFiles1(p1.documentUpload.fileupload2, p1.ImagePath, "fileupload2");

            }
            if (p1.documentUpload.fileupload3 != null)
            {
                uploadFiles1(p1.documentUpload.fileupload3, p1.ImagePath, "fileupload3");

            }
            if (p1.documentUpload.fileupload4 != null)
            {
                uploadFiles1(p1.documentUpload.fileupload4, p1.ImagePath, "fileupload4");

            }
            if (p1.declaration.Signature != null)
            {
                uploadFiles1(p1.declaration.Signature, p1.ImagePath, "Signature");
            }
            if (p1.documentUpload.fileupload6 != null)
            {
                uploadFiles1(p1.documentUpload.fileupload6, p1.ImagePath, "fileupload6");
            }



        }

        void setDefault(Cicf6Model p)
        {
            if (p.personelDetails.TemporalResidencePermitNo == null)
            {
                p.personelDetails.TemporalResidencePermitNo = "-";
            }
            if (p.personelDetails.WorkPermitNo == null)
            {
                p.personelDetails.WorkPermitNo = "-";
            }
        }

        public void fileDefault(Cicf6Model p)
        {
            p.App.File = (p.App.File != "") ? p.App.File : "-";
            p.NatureofTradeUpload = (p.NatureofTradeUpload != "") ? p.NatureofTradeUpload : "-";
            p.fileupload1 = (p.fileupload1 != "") ? p.fileupload1 : "-";
            p.fileupload2 = (p.fileupload2 != "") ? p.fileupload2 : "-";
            p.fileupload3 = (p.fileupload3 != "") ? p.fileupload3 : "-";
            p.fileupload4 = (p.fileupload4 != "") ? p.fileupload4 : "-";
            p.fileupload6 = (p.fileupload6 != "") ? p.fileupload6 : "-";
            p.Signature = (p.Signature != "") ? p.Signature : "-";
            
            //condition for null 

            p.App.File = (p.App.File != null) ? p.App.File : "-";
            p.NatureofTradeUpload = (p.NatureofTradeUpload != null) ? p.NatureofTradeUpload : "-";
            p.fileupload1 = (p.fileupload1 != null) ? p.fileupload1 : "-";
            p.fileupload2 = (p.fileupload2 != null) ? p.fileupload2 : "-";
            p.fileupload3 = (p.fileupload3 != null) ? p.fileupload3 : "-";
            p.fileupload4 = (p.fileupload4 != null) ? p.fileupload4 : "-";
            p.fileupload6 = (p.fileupload6 != null) ? p.fileupload6 : "-";
            p.Signature = (p.Signature != null) ? p.Signature : "-";
            
        }
        public int getRegNo(Cicf6Model p)
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

        public string AddCustinERP(JObject myJObject, int i)
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
                    adminFee = (decimal)myJObject["value"][i]["AdminFee"],
                    penalty = 0,
                    levy = 0,
                    credit = 0,
                    owing = 0,
                    total = 0,
                    dateofPay = "",
                    typeofPay = "",
                    bank = ""


                });
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
            var byteArray = Encoding.ASCII.GetBytes("SURYA:ikg+t/WFtLKGOW8VFRUy+dcJ+GXlWZb/VZKDq+oofO4=");
            client1.DefaultRequestHeaders.Clear();
            client1.DefaultRequestHeaders.Add("If-Match", "*");
            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
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


