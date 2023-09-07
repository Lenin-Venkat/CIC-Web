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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Identity;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    [Authorize]
    public class Form1Controller : Controller
    {
        private readonly ApplicationContext _context;
        CICForm1Model form1Model = new CICForm1Model();
        BusinessDetailsModel businessModel = new BusinessDetailsModel();
        static string formName;
        int cnt = 1;
        static string StorageName = "";
        static int subcat =0;
        static string StorageKey = "";
        static bool IsProjectExist = false;
        string Grade = "";
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");
        static string filepath = "NA";
        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        private readonly IMemoryCache memoryCache;
        CustomValidations cv = new CustomValidations();
        private readonly UserManager<UserModel> _userManager;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;


        public Form1Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
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

        public IActionResult Index(string id)
        {
            CICForm1Model form1EditModel = new CICForm1Model();            
            formName = "";

            if (id == "Form1")
            {
                formName = "CONSTRUCTION FIRMS REGISTRATION FORM - CICF 1";
                
            }
            else
            {
                formName = "SPECIALIST WORKS CONTRACTORS REGISTRATION FORM - CICF 2";
            }

            memoryCache.Set("fId", id);
            bool isExist = memoryCache.TryGetValue("Form1", out form1EditModel);

            ViewBag.FormTitle = formName;
            ViewBag.type = "active";
            form1Model.FormName = id;
            form1Model.c = cnt;
            form1Model.cFin = cnt;
            form1Model.cWork = cnt;

            if (isExist)
            {
                form1Model = form1EditModel;
                if (id == "Form1")
                {
                    ViewBag.sType = false;
                }
                else
                {
                    ViewBag.sType = true;
                }
                List<AssociationList> AList = new List<AssociationList>();
               
                memoryCache.TryGetValue("listAssociation", out AList);
                ViewBag.ListofAssociation = AList;
            }

            if (!isExist)
            {
                form1Model.Sharelist = new List<DirectorshipShareDividends>();
                form1Model.Sharelist.Add(new DirectorshipShareDividends { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });

                form1Model.applicantBank = new List<ApplicantBank>();
                form1Model.applicantBank.Add(new ApplicantBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });

                form1Model.worksCapability = new List<WorksCapability>();
                form1Model.worksCapability.Add(new WorksCapability { ProjectName = "", Location = "", ContractSum = 0, TypeofInvolvement = "", RegistationNo = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-", CompletionDate = DateTime.MinValue });

                form1Model = loadData(form1Model);
                if (id == "Form2")
                {
                    form1Model = loadDataBuilding(form1Model, 0);
                    form1Model = loadDataCivil(form1Model, 0);
                    form1Model = loadDataElectrical(form1Model, 0);
                    form1Model = loadDataMechanical(form1Model, 0);
                }

                form1Model.financialCapabilityModel = new FinancialCapability();
                form1Model.worksCapability = new List<WorksCapability>();
                form1Model.worksCapability.Add(new WorksCapability { ContractSum = 0, Location = "", PartitionKey = "-", RegistationNo = "", ProjectName = "", TelephoneNo = "", RowKey = "-", TypeofInvolvement = "", CompletionDate = DateTime.MinValue });
                
            }
            
            return View(form1Model);
        }

        public void setGetFileEdit(CICForm1Model p)
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

            //
            if (p.financialCapabilityModel.StatmentFile != null)
            {
                memoryCache.Set("StatmentFile", p.financialCapabilityModel.StatmentFile);
                p.financialCapabilityModel.statementFilename = p.financialCapabilityModel.StatmentFile.FileName;
                memoryCache.Set("StatmentFile", p.financialCapabilityModel.statementFilename);
            }
            else
            {
                p.financialCapabilityModel.statementFilename = SetandGetFileEdit("StatmentFile");
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

            if (p.docs.ShareholdersFile2 != null)
            {
                memoryCache.Set("ShareholdersFile2", p.docs.ShareholdersFile2);
                p.docs.ShareholdersFile2Name = p.docs.ShareholdersFile2.FileName;
                memoryCache.Set("ShareholdersFile2", p.docs.ShareholdersFile2Name);
            }
            else
            {
                p.docs.ShareholdersFile2Name = SetandGetFileEdit("ShareholdersFile2");
            }

            if (p.docs.ShareholdersFile3 != null)
            {
                memoryCache.Set("ShareholdersFile3", p.docs.ShareholdersFile3);
                p.docs.ShareholdersFile3Name = p.docs.ShareholdersFile3.FileName;
                memoryCache.Set("ShareholdersFile3", p.docs.ShareholdersFile3Name);
            }
            else
            {
                p.docs.ShareholdersFile3Name = SetandGetFileEdit("ShareholdersFile3");
            }

            if (p.docs.FinancialFile1 != null)
            {
                memoryCache.Set("FinancialFile1", p.docs.FinancialFile1);
                p.docs.FinancialFile1Name = p.docs.FinancialFile1.FileName;
                memoryCache.Set("FinancialFile1", p.docs.FinancialFile1Name);
            }
            else
            {
                p.docs.FinancialFile1Name = SetandGetFileEdit("FinancialFile1");
            }

            if (p.docs.FinancialFile2 != null)
            {
                memoryCache.Set("FinancialFile2", p.docs.FinancialFile2);
                p.docs.FinancialFile2Name = p.docs.FinancialFile2.FileName;
                memoryCache.Set("FinancialFile2", p.docs.FinancialFile2Name);
            }
            else
            {
                p.docs.FinancialFile2Name = SetandGetFileEdit("FinancialFile2");
            }


            if (p.docs.FinancialFile3 != null)
            {
                memoryCache.Set("FinancialFile3", p.docs.FinancialFile3);
                p.docs.FinancialFile3Name = p.docs.FinancialFile3.FileName;
                memoryCache.Set("FinancialFile3", p.docs.FinancialFile3Name);
            }
            else
            {
                p.docs.FinancialFile3Name = SetandGetFileEdit("FinancialFile3");
            }

            if (p.docs.TrackRecordFile1 != null)
            {
                memoryCache.Set("TrackRecordFile1", p.docs.TrackRecordFile1);
                p.docs.TrackRecordFile1Name = p.docs.TrackRecordFile1.FileName;
                memoryCache.Set("TrackRecordFile1", p.docs.TrackRecordFile1Name);
            }
            else
            {
                p.docs.TrackRecordFile1Name = SetandGetFileEdit("TrackRecordFile1");
            }



            if (p.docs.TrackRecordFile2 != null)
            {
                memoryCache.Set("TrackRecordFile2", p.docs.TrackRecordFile2);
                p.docs.TrackRecordFile2Name = p.docs.TrackRecordFile2.FileName;
                memoryCache.Set("TrackRecordFile2", p.docs.TrackRecordFile2Name);
            }
            else
            {
                p.docs.TrackRecordFile2Name = SetandGetFileEdit("TrackRecordFile2");
            }

            if (p.docs.TrackRecordFile3 != null)
            {
                memoryCache.Set("TrackRecordFile3", p.docs.TrackRecordFile3);
                p.docs.TrackRecordFile3Name = p.docs.TrackRecordFile3.FileName;
                memoryCache.Set("TrackRecordFile3", p.docs.TrackRecordFile3Name);
            }
            else
            {
                p.docs.TrackRecordFile3Name = SetandGetFileEdit("TrackRecordFile3");
            }

            if (p.docs.JointVentureFile1 != null)
            {
                memoryCache.Set("JointVentureFile1", p.docs.JointVentureFile1);
                p.docs.JointVentureFile1Name = p.docs.JointVentureFile1.FileName;
                memoryCache.Set("JointVentureFile1", p.docs.JointVentureFile1Name);
            }
            else
            {
                p.docs.JointVentureFile1Name = SetandGetFileEdit("JointVentureFile1");
            }

            if (p.docs.JointVentureFile2 != null)
            {
                memoryCache.Set("JointVentureFile2", p.docs.JointVentureFile2);
                p.docs.JointVentureFile2Name = p.docs.JointVentureFile2.FileName;
                memoryCache.Set("JointVentureFile2", p.docs.JointVentureFile2Name);
            }
            else
            {
                p.docs.JointVentureFile2Name = SetandGetFileEdit("JointVentureFile2");
            }

            if (p.docs.JointVentureFile3 != null)
            {
                memoryCache.Set("JointVentureFile3", p.docs.JointVentureFile3);
                p.docs.JointVentureFile3Name = p.docs.JointVentureFile3.FileName;
                memoryCache.Set("JointVentureFile3", p.docs.JointVentureFile3Name);
            }
            else
            {
                p.docs.JointVentureFile3Name = SetandGetFileEdit("JointVentureFile3");
            }

            if (p.docs.JointVentureFile4 != null)
            {
                memoryCache.Set("JointVentureFile4", p.docs.JointVentureFile4);
                p.docs.JointVentureFile4Name = p.docs.JointVentureFile4.FileName;
                memoryCache.Set("JointVentureFile4", p.docs.JointVentureFile4Name);
            }
            else
            {
                p.docs.JointVentureFile4Name = SetandGetFileEdit("JointVentureFile4");
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

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> Index(CICForm1Model p, string name, string next, string pre)
        {
           ViewBag.FormTitle = formName;
            p.FormName = p.FormName;
            //p.businessModel.Other = "Other";
            

            if(p.formval == "Edit")
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
                    p.Sharelist[i].ShareFileName = "-";
                }                               
            }
            else
            {
                p.c = cnt;
                p.Sharelist = new List<DirectorshipShareDividends>();
                p.Sharelist.Add(new DirectorshipShareDividends { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0 , PartitionKey = "-", RowKey = "-" ,ShareFileName=""});
            }

           if(p.applicantBank !=null)
            {
                p.cFin = p.applicantBank.Count() + 1;
                
                for(int i =0; i < p.applicantBank.Count;i++)
                {
                    p.applicantBank[i].PartitionKey = "-";
                    p.applicantBank[i].RowKey = "-";
                }
                
            }
           else
            {
                p.cFin = cnt;                
                p.applicantBank = new List<ApplicantBank>();
                p.applicantBank.Add(new ApplicantBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
            }

            if (p.worksCapability != null)
            {
                p.cWork = p.worksCapability.Count() + 1;
                for (int i = 0; i < p.worksCapability.Count; i++)
                {
                    p.worksCapability[i].PartitionKey = "-";
                    p.worksCapability[i].RowKey = "-";
                }
            }
            else
            {
                
                p.cWork = cnt;
                p.worksCapability = new List<WorksCapability>();
                p.worksCapability.Add(new WorksCapability { ProjectName = "", Location = "", ContractSum = 0, TypeofInvolvement = "", RegistationNo = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
            }



            //if (p.App.AssociationName == "Select association name")
            //    ViewBag.AssociationName = "Please select Association Name";

            setDefault(p);
            fileDefault(p);

            bool checkshares = ShareValidation(p.Sharelist);

            if (checkshares)
            {
                p.businessModel.BusinessType = "ForeignCompany";
                ViewBag.BType = "checked";
            }

            
            if (p.FirmRegistrationNo == 0)
            {
                p.FirmRegistrationNo = getRegNo(p);
            }
                

            switch (name)
            {
                case "type":
                    bool AppFlag = cv.IsAnyNullOrEmpty(p.App);
                    AppFlag = AppModelValidations(p);                    

                    if (AppFlag == true)
                    {                       
                        ViewBag.type = "active";
                    }
                    else if (AppFlag == false && next != null)
                    {
                        ViewBag.business = "active";                        
                    }

                    UploadBlob(p, p.FirmRegistrationNo);                  
                    break;

                case "business":                    
                    bool BFlag = cv.IsAnyNullOrEmpty(p.businessModel);                                        
                    bool DFlag;

                    DFlag= BusinessModelvalidations(p);
                    
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
                    UploadBlob(p, p.FirmRegistrationNo);
                    break;

                case "finance":
                    bool FFlag = cv.IsAnyNullOrEmpty(p.financialCapabilityModel);

                    FFlag = FinanceModelValidations(p);
                    bool aFlag = false;
                    
                    //for (int i = 0;i < p.applicantBank.Count;i++)
                    //{
                    //    aFlag = cv.IsAnyNullOrEmpty(p.applicantBank[i]);

                    //    if (aFlag == true)
                    //        break;
                    //}
                                                            
                    if (FFlag == true)
                    {
                        ViewBag.fin = "active";
                    }
                    else if (FFlag == false && next != null)
                    {
                        ViewBag.work = "active";
                    }
                    else if (FFlag == false && pre != null)
                    {
                        ViewBag.business = "active";
                    }
                    else if (aFlag == true)
                    {
                        ModelState.AddModelError(nameof(p.err), "Add Applicant Bank Details!");
                        ViewBag.fin = "active";
                    }
                    UploadBlob(p, p.FirmRegistrationNo);

                    break;


                case "work":                    
                    bool WFlag = WorkModelValidations(p);
                    
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
                        ViewBag.finance = "active";
                    }
                    break;

                case "doc":
                    bool docFlag = cv.IsAnyNullOrEmpty(p.docs);
                    docFlag = DocModelValidation(p);
                    if (docFlag == true)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.work = "active";
                    }
                    UploadBlob(p, p.FirmRegistrationNo);
                    break;

                case "draft":
                    p.FormStatus = "Draft";                    
                    UploadBlob(p, p.FirmRegistrationNo);
                    string result = Savedata(p);
                    removeDatafromSession();
                    return RedirectToAction("Form1Result", "Form1", new { result = result, text = "Draft" } );                    

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
                    UploadBlob(p, p.FirmRegistrationNo);
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    A = AppModelValidations(p);
                    if (A == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }
                                      
                    bool B = cv.IsAnyNullOrEmpty(p.businessModel);
                    B = BusinessModelvalidations(p);
                    if (B == true)
                    {
                        ViewBag.business = "active";
                        break;
                    }

                    bool F = cv.IsAnyNullOrEmpty(p.financialCapabilityModel);
                    F = FinanceModelValidations(p);
                    if (F == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    bool D = DocModelValidation(p);
                    if (D == true)
                    {
                        ViewBag.doc = "active";
                    }
                    ModelState.Clear();

                    if (ModelState.IsValid)                    
                    {
                        if (p.businessModel.BusinessType != "ForeignCompany")
                        {
                            p.Grade = GetDataForGrade(p);
                        }
                        else
                        {
                            p.Grade = GetGradeforForeign(p);
                        }
                        p.FormStatus = "Submit";
                        p.Reviewer = "Clerk";                                                
                        string result1 = Savedata(p);                        
                        removeDatafromSession();
                        return RedirectToAction("Form1Result", "Form1", new { result = result1 , text = "Final" });
                    }
                    else
                    {
                        ViewBag.doc = "active";
                    }                    
                    break;
            }


            int Bcat = p.businessModel.selectedBuildingSubcategory;
            int Ccat = p.businessModel.selectedCivilSubcategory;
            int Ecat = p.businessModel.selectedElectSubcategory;
            int Mcat = p.businessModel.selectedMechSubcategory;
            List<categoryType> afterCategory = new List<categoryType>();
            afterCategory = p.businessModel.Category;

            loadData(p);

            if (p.FormName == "Form2")
            {
                p = loadDataBuilding(p, 0);
                p = loadDataCivil(p, 0);
                p = loadDataElectrical(p, 0);
                p = loadDataMechanical(p, 0);
                for (int k = 0; k < afterCategory.Count; k++)
                {
                    if (afterCategory[k].Selected == true)
                    {
                        if (afterCategory[k].CategoryName.Contains("Building"))
                        {
                            p = loadDataBuilding(p, afterCategory[k].CategoryID);
                        }
                        if (afterCategory[k].CategoryName.Contains("Civil"))
                        {
                            p = loadDataCivil(p, afterCategory[k].CategoryID);
                        }
                        if (afterCategory[k].CategoryName.Contains("Electrical"))
                        {
                            p = loadDataElectrical(p, afterCategory[k].CategoryID);
                        }
                        if (afterCategory[k].CategoryName.Contains("Mechanical"))
                        {
                            p = loadDataMechanical(p, afterCategory[k].CategoryID);
                        }
                    }
                }

                p.businessModel.selectedBuildingSubcategory = Bcat;
                p.businessModel.selectedCivilSubcategory = Ccat;
                p.businessModel.selectedElectSubcategory = Ecat;
                p.businessModel.selectedMechSubcategory = Mcat;
            }

            if (p.formval=="Edit")
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
            if (p.financialCapabilityModel.StatmentFile != null)
            {
                ModelState.Remove("financialCapabilityModel.StatmentFile");
            }
            return View(p);
        }

        public int[] getcatId(CICForm1Model p)
        {
            int[] cat = new int[5];
            for (int i = 0; i < p.businessModel.Category.Count; i++)
            {
                if (p.businessModel.Category[i].Selected)
                {
                    cat = cat.Append(p.businessModel.Category[i].CategoryID).ToArray();
                }
            }

            return cat;
        }
        public bool AppModelValidations(CICForm1Model p)
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

        public bool DocModelValidation(CICForm1Model p)
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

        public bool BusinessModelvalidations(CICForm1Model p)
        {
            int tempPercentage = 0;
            bool DFlag = false;

            //Add BusinessName and Type combo validation
            if (p.FormName == "Form1")
            {
                AzureTablesData.GetEntitybyBusinessName(StorageName, StorageKey, "cicform1", p.businessModel.BusinessName, out string data);
                if (!string.IsNullOrEmpty(data)) 
                {
                    var myJObject = JObject.Parse(data);
                    var cntJson = myJObject["value"].Count();

                    for (int i = 0; i < cntJson; i++)
                    {
                        var formStatus = (string)myJObject["value"][i]["FormStatus"];
                        if (formStatus == "Finished")
                        {
                            var categoryString = (string)myJObject["value"][i]["Category"];
                            var categoryList = categoryString.Split(',');
                            DFlag = ValidateBusinessCategory(categoryList, p.businessModel.Category.Where(cat => cat.Selected).ToList());

                            if (DFlag) 
                                ModelState.AddModelError("businessModel.businessName", "Application already exists for the selected Categories and Business Name. Cannot proceed further.");

                            break;
                        }
                    }
                }
            }

            if (p.FormName == "Form2")
            {
                for (int k = 0; k < p.businessModel.Category.Count; k++)
                {
                    if (p.businessModel.Category[k].CategoryName.Contains("Building"))
                    {
                        if (p.businessModel.selectedBuildingSubcategory == 0 && p.businessModel.Category[k].Selected == true)
                        {
                            DFlag = true;
                            ModelState.AddModelError("businessModel.selectedBuildingSubcategory", "Please select Building sub category");
                        }
                        else
                        {
                            ModelState.Remove("businessModel.selectedBuildingSubcategory");
                        }
                    }
                    if (p.businessModel.Category[k].CategoryName.Contains("Civil"))
                    {
                        if (p.businessModel.selectedCivilSubcategory == 0 && p.businessModel.Category[k].Selected == true)
                        {
                            DFlag = true;
                            ModelState.AddModelError("businessModel.selectedCivilSubcategory", "Please select Civil sub category");
                        }
                        else
                        {
                            ModelState.Remove("businessModel.selectedCivilSubcategory");
                        }
                    }
                    if (p.businessModel.Category[k].CategoryName.Contains("Mechanical"))
                    {
                        if (p.businessModel.selectedMechSubcategory == 0 && p.businessModel.Category[k].Selected == true)
                        {
                            DFlag = true;
                            ModelState.AddModelError("businessModel.selectedMechSubcategory", "Please select Mechanical sub category");
                        }
                        else
                        {
                            ModelState.Remove("businessModel.selectedMechSubcategory");
                        }
                    }
                    if (p.businessModel.Category[k].CategoryName.Contains("Electrical"))
                    {
                        if (p.businessModel.selectedElectSubcategory == 0 && p.businessModel.Category[k].Selected == true)
                        {
                            DFlag = true;
                            ModelState.AddModelError("businessModel.selectedElectSubcategory", "Please select Electrical sub category");
                        }
                        else
                        {
                            ModelState.Remove("businessModel.selectedElectSubcategory");
                        }
                    }

                }
            }

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

            if(ModelState["businessModel.Category"]!=null )
            {
                DFlag = true;
                ModelState.AddModelError("businessModel.Category", "Please select category");
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
            if (DFlag1)
            {
                DFlag = true;
            }
            return DFlag;
        }

        private bool ValidateBusinessCategory(string[] existingCategories, List<categoryType> selectedCategories)
        {
            if (existingCategories.Any())
            {
                var matchingCategories = selectedCategories.Where(scat => existingCategories.Any(ecat => scat.CategoryName.Trim().Equals(ecat.Trim(), StringComparison.InvariantCultureIgnoreCase)));
                return matchingCategories.Any();
            }
            return false;
        }

        public bool WorkModelValidations(CICForm1Model p)
        {
            bool WFlag = false;
            if (p.App.AppType != "NewApplication" && p.businessModel.BusinessType != "ForeignCompany")
            {
                for (int i = 0; i < p.worksCapability.Count; i++)
                {
                    WFlag = cv.IsAnyNullOrEmpty(p.worksCapability[i]);
                    if (ModelState["worksCapability[" + i + "].ContractSum"].Errors.Any())
                    {
                        WFlag = true;
                        ModelState.AddModelError("worksCapability[" + i + "].ContractSum", "Contract Sum can not be NA!");
                    }
                    if (WFlag == true)
                        break;
                }
            }
            else
            {
                for (int i = 0; i < p.worksCapability.Count; i++)
                {                                                                 
                    ModelState.Remove("worksCapability[" + i + "].CompletionDate");
                    ModelState.Remove("worksCapability[" + i + "].ContractSum");
                    ModelState.Remove("worksCapability[" + i + "].Location");
                    ModelState.Remove("worksCapability[" + i + "].ProjectName");
                    ModelState.Remove("worksCapability[" + i + "].RegistationNo");
                    ModelState.Remove("worksCapability[" + i + "].TelephoneNo");
                    ModelState.Remove("worksCapability[" + i + "].TypeofInvolvement");
                }
            }
            return WFlag;
        }

        public bool FinanceModelValidations(CICForm1Model p)
        {
            bool FFlag = false;

            if (p.App.AppType != "NewApplication" && p.businessModel.BusinessType != "ForeignCompany")
            {
                if (p.financialCapabilityModel.AnnualTurnoverYear1 == 0)
                {
                    FFlag = true;
                    ModelState.AddModelError("financialCapabilityModel.AnnualTurnoverYear1", "Financial Year 1 end Total Turnover can not be 0");
                }
                if (p.financialCapabilityModel.AvailableCapital == 0)
                {
                    FFlag = true;
                    ModelState.AddModelError("financialCapabilityModel.AvailableCapital", "Available Capital can not be 0");
                }

                if (p.formval != "Edit")
                {
                    if (p.financialCapabilityModel.StatmentFile == null)
                    {
                        FFlag = true;
                        ModelState.AddModelError("financialCapabilityModel.StatmentFile", "Please upload file");
                    }
                }
                else
                {
                    if (p.financialCapabilityModel.statementFilename == null && p.financialCapabilityModel.StatmentFile == null)
                    {
                        FFlag = true;
                        ModelState.AddModelError("financialCapabilityModel.StatmentFile", "Please upload file");
                    }
                    else
                    {
                        ModelState.Remove("financialCapabilityModel.StatmentFile");
                    }
                }
            }
            else
            {
                if ((p.App.AppType == "NewApplication" || p.businessModel.BusinessType == "ForeignCompany") && p.financialCapabilityModel.FinancialInstitutionName ==null)
                {
                    FFlag = true;
                    ModelState.AddModelError("financialCapabilityModel.FinancialInstitutionName", "Please enter Financial Institution Name");
                }
                else
                {
                    ModelState.Remove("financialCapabilityModel.FinancialInstitutionName");
                }
                ModelState.Remove("financialCapabilityModel.AvailableCapital");               
                ModelState.Remove("financialCapabilityModel.StatmentFile");
            }

            

                return FFlag;
        }

        public void removeDatafromSession()
        {
            memoryCache.Remove("Form1");
            memoryCache.Remove("Filesignature");
            memoryCache.Remove("Businesssignature");
            memoryCache.Remove("StatmentFile");
            memoryCache.Remove("BusinessFile1");
            memoryCache.Remove("BusinessFile2");
            memoryCache.Remove("BusinessFile3");
            memoryCache.Remove("BusinessFile4");
            memoryCache.Remove("BusinessFile5");
            memoryCache.Remove("BusinessFile6");
            memoryCache.Remove("ShareholdersFile1");
            memoryCache.Remove("ShareholdersFile2");
            memoryCache.Remove("ShareholdersFile3");
            memoryCache.Remove("FinancialFile1");
            memoryCache.Remove("FinancialFile2");
            memoryCache.Remove("FinancialFile3");
            memoryCache.Remove("TrackRecordFile1");
            memoryCache.Remove("TrackRecordFile2");
            memoryCache.Remove("TrackRecordFile3");
            memoryCache.Remove("JointVentureFile1");
            memoryCache.Remove("JointVentureFile2");
            memoryCache.Remove("JointVentureFile3");
            memoryCache.Remove("JointVentureFile4");
            memoryCache.Remove("Signature1");
            memoryCache.Remove("Signature2");
            memoryCache.Remove("TaxLaw");
            memoryCache.Remove("Evidence");
            memoryCache.Remove("Compliance");
        }

        public void fileDefault(CICForm1Model p)
        {
            p.App.signaturefilename = (p.App.signaturefilename != "") ? p.App.signaturefilename : "-";
            p.businessModel.BusinessFileSignatureName = (p.businessModel.BusinessFileSignatureName != "") ? p.businessModel.BusinessFileSignatureName : "-";
            p.financialCapabilityModel.statementFilename = (p.financialCapabilityModel.statementFilename != "") ? p.financialCapabilityModel.statementFilename : "-";
            p.docs.BusinessFile1Name = (p.docs.BusinessFile1Name != "") ? p.docs.BusinessFile1Name : "-";
            p.docs.BusinessFile2Name = (p.docs.BusinessFile2Name != "") ? p.docs.BusinessFile2Name : "-";            
            p.docs.BusinessFile3Name = (p.docs.BusinessFile3Name != "") ? p.docs.BusinessFile3Name : "-";
            p.docs.BusinessFile4Name = (p.docs.BusinessFile4Name != "") ? p.docs.BusinessFile4Name : "-";
            p.docs.BusinessFile5Name = (p.docs.BusinessFile5Name != "") ? p.docs.BusinessFile5Name : "-";
            p.docs.BusinessFile6Name = (p.docs.BusinessFile6Name != "") ? p.docs.BusinessFile6Name : "-";
            p.docs.ShareholdersFile1Name = (p.docs.ShareholdersFile1Name != "") ? p.docs.ShareholdersFile1Name : "-";
            p.docs.ShareholdersFile2Name = (p.docs.ShareholdersFile2Name != "") ? p.docs.ShareholdersFile2Name : "-";
            p.docs.ShareholdersFile3Name = (p.docs.ShareholdersFile3Name != "") ? p.docs.ShareholdersFile3Name : "-";
            p.docs.FinancialFile1Name = (p.docs.FinancialFile1Name != "") ? p.docs.FinancialFile1Name : "-";
            p.docs.FinancialFile2Name = (p.docs.FinancialFile2Name != "") ? p.docs.FinancialFile2Name : "-";
            p.docs.FinancialFile3Name = (p.docs.FinancialFile3Name != "") ? p.docs.FinancialFile3Name : "-";
            p.docs.TrackRecordFile1Name = (p.docs.TrackRecordFile1Name != "") ? p.docs.TrackRecordFile1Name : "-";
            p.docs.TrackRecordFile2Name = (p.docs.TrackRecordFile2Name != "") ? p.docs.TrackRecordFile2Name : "-";
            p.docs.TrackRecordFile3Name = (p.docs.TrackRecordFile3Name != "") ? p.docs.TrackRecordFile3Name : "-";
            p.docs.JointVentureFile1Name = (p.docs.JointVentureFile1Name != "") ? p.docs.JointVentureFile1Name : "-";
            p.docs.JointVentureFile2Name = (p.docs.JointVentureFile2Name != "") ? p.docs.JointVentureFile2Name : "-";
            p.docs.JointVentureFile3Name = (p.docs.JointVentureFile3Name != "") ? p.docs.JointVentureFile3Name : "-";
            p.docs.JointVentureFile4Name = (p.docs.JointVentureFile4Name != "") ? p.docs.JointVentureFile4Name : "-";
            p.docs.Signature1Name = (p.docs.Signature1Name != "") ? p.docs.Signature1Name : "-";
            p.docs.Signature2Name = (p.docs.Signature2Name != "") ? p.docs.Signature2Name : "-";
            p.docs.TaxLawName = (p.docs.TaxLawName != "") ? p.docs.TaxLawName : "-";
            p.docs.EvidenceName = (p.docs.EvidenceName != "") ? p.docs.EvidenceName : "-";
            p.docs.ComplianceName = (p.docs.ComplianceName != "") ? p.docs.ComplianceName : "-";

            p.App.signaturefilename = (p.App.signaturefilename != null) ? p.App.signaturefilename : "-";
            p.businessModel.BusinessFileSignatureName = (p.businessModel.BusinessFileSignatureName != null) ? p.businessModel.BusinessFileSignatureName : "-";
            p.financialCapabilityModel.statementFilename = (p.financialCapabilityModel.statementFilename != null) ? p.financialCapabilityModel.statementFilename : "-";
            p.docs.BusinessFile1Name = (p.docs.BusinessFile1Name != null) ? p.docs.BusinessFile1Name : "-";
            p.docs.BusinessFile2Name = (p.docs.BusinessFile2Name != null) ? p.docs.BusinessFile2Name : "-";
            p.docs.BusinessFile3Name = (p.docs.BusinessFile3Name != null) ? p.docs.BusinessFile3Name : "-";
            p.docs.BusinessFile4Name = (p.docs.BusinessFile4Name != null) ? p.docs.BusinessFile4Name : "-";
            p.docs.BusinessFile5Name = (p.docs.BusinessFile5Name != null) ? p.docs.BusinessFile5Name : "-";
            p.docs.BusinessFile6Name = (p.docs.BusinessFile6Name != null) ? p.docs.BusinessFile6Name : "-";
            p.docs.ShareholdersFile1Name = (p.docs.ShareholdersFile1Name != null) ? p.docs.ShareholdersFile1Name : "-";
            p.docs.ShareholdersFile2Name = (p.docs.ShareholdersFile2Name != null) ? p.docs.ShareholdersFile2Name : "-";
            p.docs.ShareholdersFile3Name = (p.docs.ShareholdersFile3Name != null) ? p.docs.ShareholdersFile3Name : "-";
            p.docs.FinancialFile1Name = (p.docs.FinancialFile1Name != null) ? p.docs.FinancialFile1Name : "-";
            p.docs.FinancialFile2Name = (p.docs.FinancialFile2Name != null) ? p.docs.FinancialFile2Name : "-";
            p.docs.FinancialFile3Name = (p.docs.FinancialFile3Name != null) ? p.docs.FinancialFile3Name : "-";
            p.docs.TrackRecordFile1Name = (p.docs.TrackRecordFile1Name != null) ? p.docs.TrackRecordFile1Name : "-";
            p.docs.TrackRecordFile2Name = (p.docs.TrackRecordFile2Name != null) ? p.docs.TrackRecordFile2Name : "-";
            p.docs.TrackRecordFile3Name = (p.docs.TrackRecordFile3Name != null) ? p.docs.TrackRecordFile3Name : "-";
            p.docs.JointVentureFile1Name = (p.docs.JointVentureFile1Name != null) ? p.docs.JointVentureFile1Name : "-";
            p.docs.JointVentureFile2Name = (p.docs.JointVentureFile2Name != null) ? p.docs.JointVentureFile2Name : "-";
            p.docs.JointVentureFile3Name = (p.docs.JointVentureFile3Name != null) ? p.docs.JointVentureFile3Name : "-";
            p.docs.JointVentureFile4Name = (p.docs.JointVentureFile4Name != null) ? p.docs.JointVentureFile4Name : "-";
            p.docs.Signature1Name = (p.docs.Signature1Name != null) ? p.docs.Signature1Name : "-";
            p.docs.Signature2Name = (p.docs.Signature2Name != null) ? p.docs.Signature2Name : "-";
            p.docs.TaxLawName = (p.docs.TaxLawName != null) ? p.docs.TaxLawName : "-";
            p.docs.EvidenceName = (p.docs.EvidenceName != null) ? p.docs.EvidenceName : "-";
            p.docs.ComplianceName = (p.docs.ComplianceName != null) ? p.docs.ComplianceName : "-";
        }
        public CICForm1Model SetandGetFileAdd(CICForm1Model p)
        {
            IFormFile fsign;
            //string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("Filesignature", out fsign);


            if(!isExist)
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

            isExist = memoryCache.TryGetValue("StatmentFile", out fsign);
            if (!isExist)
            {
                if (p.financialCapabilityModel.StatmentFile != null)
                {
                    memoryCache.Set("StatmentFile", p.financialCapabilityModel.StatmentFile);
                    p.financialCapabilityModel.statementFilename = p.financialCapabilityModel.StatmentFile.FileName;
                }
            }
            else
            {
                if (p.financialCapabilityModel.StatmentFile != null)
                {
                    memoryCache.Set("StatmentFile", p.financialCapabilityModel.StatmentFile);
                }
                else
                {
                    p.financialCapabilityModel.StatmentFile = fsign;
                }
                p.financialCapabilityModel.statementFilename = p.financialCapabilityModel.StatmentFile.FileName;

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

            isExist = memoryCache.TryGetValue("ShareholdersFile2", out fsign);
            if (!isExist)
            {
                if (p.docs.ShareholdersFile2 != null)
                {
                    memoryCache.Set("ShareholdersFile2", p.docs.ShareholdersFile2);
                    p.docs.ShareholdersFile2Name = p.docs.ShareholdersFile2.FileName;
                }
            }
            else
            {
                if (p.docs.ShareholdersFile2 != null)
                {
                    memoryCache.Set("ShareholdersFile2", p.docs.ShareholdersFile2);
                }
                else
                {
                    p.docs.ShareholdersFile2 = fsign;
                }
                p.docs.ShareholdersFile2Name = p.docs.ShareholdersFile2.FileName;

            }

            isExist = memoryCache.TryGetValue("ShareholdersFile3", out fsign);
            if (!isExist)
            {
                if (p.docs.ShareholdersFile3 != null)
                {
                    memoryCache.Set("ShareholdersFile3", p.docs.ShareholdersFile3);
                    p.docs.ShareholdersFile3Name = p.docs.ShareholdersFile3.FileName;
                }
            }
            else
            {
                if (p.docs.ShareholdersFile3 != null)
                {
                    memoryCache.Set("ShareholdersFile3", p.docs.ShareholdersFile3);
                }
                else
                {
                    p.docs.ShareholdersFile3 = fsign;
                }
                p.docs.ShareholdersFile3Name = p.docs.ShareholdersFile3.FileName;

            }

            isExist = memoryCache.TryGetValue("FinancialFile1", out fsign);
            if (!isExist)
            {
                if (p.docs.FinancialFile1 != null)
                {
                    memoryCache.Set("FinancialFile1", p.docs.FinancialFile1);
                    p.docs.FinancialFile1Name = p.docs.FinancialFile1.FileName;
                }
            }
            else
            {
                if (p.docs.FinancialFile1 != null)
                {
                    memoryCache.Set("FinancialFile1", p.docs.FinancialFile1);
                }
                else
                {
                    p.docs.FinancialFile1 = fsign;
                }
                p.docs.FinancialFile1Name = p.docs.FinancialFile1.FileName;

            }

            isExist = memoryCache.TryGetValue("FinancialFile2", out fsign);
            if (!isExist)
            {
                if (p.docs.FinancialFile2 != null)
                {
                    memoryCache.Set("FinancialFile2", p.docs.FinancialFile2);
                    p.docs.FinancialFile2Name = p.docs.FinancialFile2.FileName;
                }
            }
            else
            {
                if (p.docs.FinancialFile2 != null)
                {
                    memoryCache.Set("FinancialFile2", p.docs.FinancialFile2);
                }
                else
                {
                    p.docs.FinancialFile2 = fsign;
                }
                p.docs.FinancialFile2Name = p.docs.FinancialFile2.FileName;

            }

            isExist = memoryCache.TryGetValue("FinancialFile3", out fsign);
            if (!isExist)
            {
                if (p.docs.FinancialFile3 != null)
                {
                    memoryCache.Set("FinancialFile3", p.docs.FinancialFile3);
                    p.docs.FinancialFile3Name = p.docs.FinancialFile3.FileName;
                }
            }
            else
            {
                if (p.docs.FinancialFile3 != null)
                {
                    memoryCache.Set("FinancialFile3", p.docs.FinancialFile3);
                }
                else
                {
                    p.docs.FinancialFile3 = fsign;
                }
                p.docs.FinancialFile3Name = p.docs.FinancialFile3.FileName;

            }

            isExist = memoryCache.TryGetValue("TrackRecordFile1", out fsign);
            if (!isExist)
            {
                if (p.docs.TrackRecordFile1 != null)
                {
                    memoryCache.Set("TrackRecordFile1", p.docs.TrackRecordFile1);
                    p.docs.TrackRecordFile1Name = p.docs.TrackRecordFile1.FileName;
                }
            }
            else
            {
                if (p.docs.TrackRecordFile1 != null)
                {
                    memoryCache.Set("TrackRecordFile1", p.docs.TrackRecordFile1);
                }
                else
                {
                    p.docs.TrackRecordFile1 = fsign;
                }
                p.docs.TrackRecordFile1Name = p.docs.TrackRecordFile1.FileName;

            }

            isExist = memoryCache.TryGetValue("TrackRecordFile2", out fsign);
            if (!isExist)
            {
                if (p.docs.TrackRecordFile2 != null)
                {
                    memoryCache.Set("TrackRecordFile2", p.docs.TrackRecordFile2);
                    p.docs.TrackRecordFile2Name = p.docs.TrackRecordFile2.FileName;
                }
            }
            else
            {
                if (p.docs.TrackRecordFile2 != null)
                {
                    memoryCache.Set("TrackRecordFile2", p.docs.TrackRecordFile2);
                }
                else
                {
                    p.docs.TrackRecordFile2 = fsign;
                }
                p.docs.TrackRecordFile2Name = p.docs.TrackRecordFile2.FileName;

            }

            isExist = memoryCache.TryGetValue("TrackRecordFile3", out fsign);
            if (!isExist)
            {
                if (p.docs.TrackRecordFile3 != null)
                {
                    memoryCache.Set("TrackRecordFile3", p.docs.TrackRecordFile3);
                    p.docs.TrackRecordFile3Name = p.docs.TrackRecordFile3.FileName;
                }
            }
            else
            {
                if (p.docs.TrackRecordFile3 != null)
                {
                    memoryCache.Set("TrackRecordFile3", p.docs.TrackRecordFile3);
                }
                else
                {
                    p.docs.TrackRecordFile3 = fsign;
                }
                p.docs.TrackRecordFile3Name = p.docs.TrackRecordFile3.FileName;

            }

            isExist = memoryCache.TryGetValue("JointVentureFile1", out fsign);
            if (!isExist)
            {
                if (p.docs.JointVentureFile1 != null)
                {
                    memoryCache.Set("JointVentureFile1", p.docs.JointVentureFile1);
                    p.docs.JointVentureFile1Name = p.docs.JointVentureFile1.FileName;
                }
            }
            else
            {
                if (p.docs.JointVentureFile1 != null)
                {
                    memoryCache.Set("JointVentureFile1", p.docs.JointVentureFile1);
                }
                else
                {
                    p.docs.JointVentureFile1 = fsign;
                }
                p.docs.JointVentureFile1Name = p.docs.JointVentureFile1.FileName;

            }

            isExist = memoryCache.TryGetValue("JointVentureFile2", out fsign);
            if (!isExist)
            {
                if (p.docs.JointVentureFile2 != null)
                {
                    memoryCache.Set("JointVentureFile2", p.docs.JointVentureFile2);
                    p.docs.JointVentureFile2Name = p.docs.JointVentureFile2.FileName;
                }
            }
            else
            {
                if (p.docs.JointVentureFile2 != null)
                {
                    memoryCache.Set("JointVentureFile2", p.docs.JointVentureFile2);
                }
                else
                {
                    p.docs.JointVentureFile2 = fsign;
                }
                p.docs.JointVentureFile2Name = p.docs.JointVentureFile2.FileName;

            }

            isExist = memoryCache.TryGetValue("JointVentureFile3", out fsign);
            if (!isExist)
            {
                if (p.docs.JointVentureFile3 != null)
                {
                    memoryCache.Set("JointVentureFile3", p.docs.JointVentureFile3);
                    p.docs.JointVentureFile3Name = p.docs.JointVentureFile3.FileName;
                }
            }
            else
            {
                if (p.docs.JointVentureFile3 != null)
                {
                    memoryCache.Set("JointVentureFile3", p.docs.JointVentureFile3);
                }
                else
                {
                    p.docs.JointVentureFile3 = fsign;
                }
                p.docs.JointVentureFile3Name = p.docs.JointVentureFile3.FileName;

            }

            isExist = memoryCache.TryGetValue("JointVentureFile4", out fsign);
            if (!isExist)
            {
                if (p.docs.JointVentureFile4 != null)
                {
                    memoryCache.Set("JointVentureFile4", p.docs.JointVentureFile4);
                    p.docs.JointVentureFile4Name = p.docs.JointVentureFile4.FileName;
                }
            }
            else
            {
                if (p.docs.JointVentureFile4 != null)
                {
                    memoryCache.Set("JointVentureFile4", p.docs.JointVentureFile4);
                }
                else
                {
                    p.docs.JointVentureFile4 = fsign;
                }
                p.docs.JointVentureFile4Name = p.docs.JointVentureFile4.FileName;

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

        public void SetFiles(string key, IFormFile fsign, IFormFile modelFile, out string FName)
        {
            FName = null;
            if (fsign != null && modelFile == null)
            {
                modelFile = fsign;
                FName = modelFile.FileName;
            }
            else
            {
                if (modelFile != null)
                {
                    memoryCache.Set(key, modelFile);
                    FName = modelFile.FileName;
                }
            }            
        }

        public string SetandGetFileEdit( string key)
        {
            string tempFilename;
            bool isExist = memoryCache.TryGetValue(key, out tempFilename);
            if (isExist && tempFilename != null)
            {
                return tempFilename;
            }
            
            
            return "";
        }

        
        public IActionResult Getdata(string apptype)
        {
            string id = "";
            bool isExist = memoryCache.TryGetValue("fId", out id);

            ViewBag.type = "active";
            CICForm1Model model = new CICForm1Model();
            string c = "";            
            List<FileList> AllFileList = new List<FileList>();
            string jsonData,newgrade = "NA";
            AzureTablesData.GetEntitybyLoginIdwithForm(StorageName, StorageKey, "cicform1", User.Identity.Name,id, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);

            var latestRecord = (from p in myJObject["value"]
                             orderby (long)p["FirmRegistrationNo"] descending
                             select p).FirstOrDefault();

            if(latestRecord !=null)
            {
                model.RowKey = (string)latestRecord["RowKey"];
                newgrade = (string)latestRecord["Grade"];

                ApplicationTypeModel App = new ApplicationTypeModel
                {
                    AppType = apptype,
                    AssociationName = (string)latestRecord["AssociationName"],
                    AuthorisedOfficerName = (string)latestRecord["AuthorisedOfficerName"],
                    // signaturefilename = AppFile,
                    ImagePath = path
                };

                model.App = App;

                c = (string)latestRecord["Category"];
                businessModel = new BusinessDetailsModel
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
                    // Category = (string)latestRecord["Category"],
                    PresentGrade = newgrade,// (string)latestRecord["PresentGrade"],
                    BusinessRepresentativeName = (string)latestRecord["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)latestRecord["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)latestRecord["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)latestRecord["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)latestRecord["BusinessRepresentativeEmail"],
                    Other = (string)latestRecord["Other"],
                    // BusinessFileSignatureName = Businesssignature,
                    selectedElectSubcategory = (int)latestRecord["ElectricalSubCategory"],
                    selectedCivilSubcategory = (int)latestRecord["CivilSubCategory"],
                    selectedBuildingSubcategory = (int)latestRecord["BuildingSubCategory"],
                    selectedMechSubcategory = (int)latestRecord["MechanicalSubCategory"]
                };
                model.businessModel = businessModel;
                model.CustNo = (string)latestRecord["CustNo"];
                model.newGradecomment = (string)latestRecord["newGradecomment"];
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
                
                FinancialCapability fin = new FinancialCapability
                {
                    AnnualTurnoverYear1 = (decimal)latestRecord["AnnualTurnoverYear1"],
                    AnnualTurnoverYear2 = (decimal)latestRecord["AnnualTurnoverYear2"],
                    AnnualTurnoverYear3 = (decimal)latestRecord["AnnualTurnoverYear3"],
                    FinancialValue = (decimal)latestRecord["FinancialValue"],
                    FinancialInstitutionName = (string)latestRecord["FinancialInstitutionName"],
                    AvailableCapital = (decimal)latestRecord["AvailableCapital"],
                    // statementFilename = StatmentFile
                };

                model.financialCapabilityModel = fin;

                Documents doc = new Documents
                {
                    Name = (string)latestRecord["Name"],
                    WitnessedName = (string)latestRecord["WitnessedName"],
                    WitnessedTitle = (string)latestRecord["WitnessedTitle"],
                    Title = (string)latestRecord["Title"]
                };
                model.docs = doc;

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", model.RowKey, out jsonData1);
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
                        SharePercent = (int)myJObject1["value"][i]["SharePercent"]
                    });
                }
                model.Sharelist = d;
                if (model.Sharelist.Count == 0)
                {
                    model.c = 1;
                    model.Sharelist = new List<DirectorshipShareDividends>();
                    model.Sharelist.Add(new DirectorshipShareDividends { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-", ShareFileName = "" });
                }
                else
                {
                    model.c = model.Sharelist.Count;
                }
                string jsonData2;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1BankDetails", model.RowKey, out jsonData2);
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

                if (model.applicantBank.Count == 0)
                {
                    model.cFin = 1;
                    model.applicantBank = new List<ApplicantBank>();
                    model.applicantBank.Add(new ApplicantBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.cFin = model.applicantBank.Count;
                }
                string jsonData3;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1WorkDetails", model.RowKey, out jsonData3);
                JObject myJObject3 = JObject.Parse(jsonData3);
                int cntJson3 = myJObject3["value"].Count();
                List<WorksCapability> w = new List<WorksCapability>();
                for (int i = 0; i < cntJson3; i++)
                {

                    w.Add(new WorksCapability
                    {
                        CompletionDate = (DateTime)myJObject3["value"][i]["CompletionDate"],
                        ContractSum = (int)myJObject3["value"][i]["ContractSum"],
                        Location = (string)myJObject3["value"][i]["Location"],
                        ProjectName = (string)myJObject3["value"][i]["ProjectName"],
                        RegistationNo = (string)myJObject3["value"][i]["RegistationNo"],
                        TelephoneNo = (string)myJObject3["value"][i]["TelephoneNo"],
                        TypeofInvolvement = (string)myJObject3["value"][i]["TypeofInvolvement"]

                    });
                }
                model.worksCapability = w;
                if (model.worksCapability.Count == 0)
                {
                    model.cWork = 1;
                    model.worksCapability = new List<WorksCapability>();
                    model.worksCapability.Add(new WorksCapability { ProjectName = "", Location = "", ContractSum = 0, TypeofInvolvement = "", RegistationNo = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.cWork = model.worksCapability.Count;
                }

                model.FormName = id;

                model = loadData(model);

                if (c != null)
                {
                    if (c.Contains(','))
                    {
                        String[] strList = c.Split(",");
                        int len = strList.Length;
                        model = loadDataBuilding(model, 0);
                        model = loadDataCivil(model, 0);
                        model = loadDataElectrical(model, 0);
                        model = loadDataMechanical(model, 0);
                        for (int i = 0; i < len; i++)
                        {
                            for (int k = 0; k < model.businessModel.Category.Count; k++)
                            {
                                if (model.businessModel.Category[k].CategoryName == strList[i])
                                {
                                    model.businessModel.Category[k].Selected = true;
                                    if (model.FormName == "Form2")
                                    {
                                        if (model.businessModel.Category[k].CategoryName.Contains("Building"))
                                        {
                                            model = loadDataBuilding(model, model.businessModel.Category[k].CategoryID);
                                        }
                                        if (model.businessModel.Category[k].CategoryName.Contains("Civil"))
                                        {
                                            model = loadDataCivil(model, model.businessModel.Category[k].CategoryID);
                                        }
                                        if (model.businessModel.Category[k].CategoryName.Contains("Electrical"))
                                        {
                                            model = loadDataElectrical(model, model.businessModel.Category[k].CategoryID);
                                        }
                                        if (model.businessModel.Category[k].CategoryName.Contains("Mechanical"))
                                        {
                                            model = loadDataMechanical(model, model.businessModel.Category[k].CategoryID);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        model = loadDataBuilding(model, 0);
                        model = loadDataCivil(model, 0);
                        model = loadDataElectrical(model, 0);
                        model = loadDataMechanical(model, 0);
                        for (int k = 0; k < model.businessModel.Category.Count; k++)
                        {
                            if (model.businessModel.Category[k].CategoryName == c)
                            {
                                model.businessModel.Category[k].Selected = true;
                                if (model.FormName == "Form2")
                                {
                                    if (model.businessModel.Category[k].CategoryName.Contains("Building"))
                                    {
                                        model = loadDataBuilding(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Civil"))
                                    {
                                        model = loadDataCivil(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Electrical"))
                                    {
                                        model = loadDataElectrical(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Mechanical"))
                                    {
                                        model = loadDataMechanical(model, model.businessModel.Category[k].CategoryID);
                                    }
                                }
                            }
                        }
                    }
                }

                if (c == null)
                {
                    model = loadDataBuilding(model, 0);
                    model = loadDataCivil(model, 0);
                    model = loadDataElectrical(model, 0);
                    model = loadDataMechanical(model, 0);
                }

                List<AssociationList> AList = new List<AssociationList>();
                AList = ViewBag.ListofAssociation;
                memoryCache.Set("listAssociation", AList);
                memoryCache.Set("Form1", model);
                memoryCache.Remove("fId");                
            }

            return RedirectToAction("Index", "Form1", new { id = id });
        }
        public IActionResult IndexFromDashboard(string rowkey)
        {
            ViewBag.type = "active";
            CICForm1Model model = new CICForm1Model();
            string c = "";
            //model.c = cnt;
            //model.cFin = cnt;
            //model.cWork = cnt;
            List<FileList> AllFileList = new List<FileList>();
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
                path = (string)myJObject["value"][i]["path"];
                
                string key;
                AllFileList = _blobStorageService.GetBlobList(path);
                string AppFile = null, Businesssignature = null, StatmentFile = null, BusinessFile1 = null, BusinessFile2 = null,
                    BusinessFile3 = null, BusinessFile4 = null,BusinessFile5 = null, BusinessFile6 = null, ShareholdersFile1 = null, ShareholdersFile2 = null, ShareholdersFile3 = null,
                    FinancialFile1 = null, FinancialFile2 = null, FinancialFile3 = null, TrackRecordFile1 = null, TrackRecordFile2 = null, TrackRecordFile3 = null,
                    JointVentureFile1 = null, JointVentureFile2 = null, JointVentureFile3 = null, JointVentureFile4 = null, Signature1 = null, Signature2 = null, TaxLaw = null,
                     Evidence = null,Compliance = null;

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

                            case "StatmentFile": StatmentFile = AllFileList[j].FileValue; break;
                            case "BusinessFile1": BusinessFile1 = AllFileList[j].FileValue; break;
                            case "BusinessFile2": BusinessFile2 = AllFileList[j].FileValue; break;
                            case "BusinessFile3": BusinessFile3 = AllFileList[j].FileValue; break;
                            case "BusinessFile4": BusinessFile4 = AllFileList[j].FileValue; break;
                            case "BusinessFile5": BusinessFile5 = AllFileList[j].FileValue; break;
                            case "BusinessFile6": BusinessFile6 = AllFileList[j].FileValue; break;
                            case "ShareholdersFile1": ShareholdersFile1 = AllFileList[j].FileValue; break;
                            case "ShareholdersFile2": ShareholdersFile2 = AllFileList[j].FileValue; break;
                            case "ShareholdersFile3": ShareholdersFile3 = AllFileList[j].FileValue; break;
                            case "FinancialFile1": FinancialFile1 = AllFileList[j].FileValue; break;
                            case "FinancialFile2": FinancialFile2 = AllFileList[j].FileValue; break;
                            case "FinancialFile3": FinancialFile3 = AllFileList[j].FileValue; break;
                            case "TrackRecordFile1": TrackRecordFile1 = AllFileList[j].FileValue; break;
                            case "TrackRecordFile2": TrackRecordFile2 = AllFileList[j].FileValue; break;
                            case "TrackRecordFile3": TrackRecordFile3 = AllFileList[j].FileValue; break;
                            case "JointVentureFile1": JointVentureFile1 = AllFileList[j].FileValue; break;
                            case "JointVentureFile2": JointVentureFile2 = AllFileList[j].FileValue; break;
                            case "JointVentureFile3": JointVentureFile3 = AllFileList[j].FileValue; break;
                            case "JointVentureFile4": JointVentureFile4 = AllFileList[j].FileValue; break;
                            case "Signature1": Signature1 = AllFileList[j].FileValue; break;
                            case "Signature2": Signature2 = AllFileList[j].FileValue; break;
                            case "TaxLaw": TaxLaw = AllFileList[j].FileValue; break;
                            case "Evidence": Evidence = AllFileList[j].FileValue; break;
                            case "Compliance": Compliance = AllFileList[j].FileValue; break;
                        }
                    }
                }

                ApplicationTypeModel App = new ApplicationTypeModel
                {
                    AppType = (string)myJObject["value"][i]["AppType"],
                    AssociationName = (string)myJObject["value"][i]["AssociationName"],
                    AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"],
                   // OldRegNo = (string)myJObject["value"][i]["OldRegNo"],
                    signaturefilename = AppFile,
                    ImagePath = path
                };

                model.App = App;

                c = (string)myJObject["value"][i]["Category"];
                businessModel = new BusinessDetailsModel
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
                    // Category = (string)myJObject["value"][i]["Category"],
                    PresentGrade = (string)myJObject["value"][i]["PresentGrade"],
                    BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"],
                    Other = (string)myJObject["value"][i]["Other"],
                    BusinessFileSignatureName = Businesssignature,
                    selectedElectSubcategory = (int)myJObject["value"][i]["ElectricalSubCategory"],
                    selectedCivilSubcategory = (int)myJObject["value"][i]["CivilSubCategory"],
                    selectedBuildingSubcategory = (int)myJObject["value"][i]["BuildingSubCategory"],
                    selectedMechSubcategory = (int)myJObject["value"][i]["MechanicalSubCategory"]
                };
                model.businessModel = businessModel;
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
                FinancialCapability fin = new FinancialCapability
                {
                    AnnualTurnoverYear1 = (decimal)myJObject["value"][i]["AnnualTurnoverYear1"],
                    AnnualTurnoverYear2 = (decimal)myJObject["value"][i]["AnnualTurnoverYear2"],
                    AnnualTurnoverYear3 = (decimal)myJObject["value"][i]["AnnualTurnoverYear3"],
                    FinancialValue = (decimal)myJObject["value"][i]["FinancialValue"],
                    FinancialInstitutionName = (string)myJObject["value"][i]["FinancialInstitutionName"],
                    AvailableCapital = (decimal)myJObject["value"][i]["AvailableCapital"],
                    statementFilename = StatmentFile
                };

                model.financialCapabilityModel = fin;

                Documents doc = new Documents
                {
                    BusinessFile1Name = BusinessFile1,
                    BusinessFile2Name = BusinessFile2,
                    BusinessFile3Name = BusinessFile3,
                    BusinessFile4Name = BusinessFile4,
                    BusinessFile5Name = BusinessFile5,
                    BusinessFile6Name = BusinessFile6,
                    ComplianceName = Compliance,
                    EvidenceName = Evidence,
                    FinancialFile1Name = FinancialFile1,
                    FinancialFile2Name = FinancialFile2,
                    FinancialFile3Name = FinancialFile3,
                    JointVentureFile1Name = JointVentureFile1,
                    JointVentureFile2Name = JointVentureFile2,
                    JointVentureFile3Name = JointVentureFile3,
                    JointVentureFile4Name = JointVentureFile4,
                    ShareholdersFile1Name = ShareholdersFile1,
                    ShareholdersFile2Name = ShareholdersFile2,
                    ShareholdersFile3Name = ShareholdersFile3,
                    Signature1Name = Signature1,
                    Signature2Name = Signature2,
                    TrackRecordFile1Name = TrackRecordFile1,
                    TrackRecordFile2Name = TrackRecordFile2,
                    TrackRecordFile3Name = TrackRecordFile3,
                    TaxLawName = TaxLaw,
                    Name = (string)myJObject["value"][i]["Name"],
                    WitnessedName = (string)myJObject["value"][i]["WitnessedName"],
                    WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"],
                    Title = (string)myJObject["value"][i]["Title"]
                };
                model.docs = doc;
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.FirmRegistrationNo = (long)myJObject["value"][i]["FirmRegistrationNo"];
                // model.path = (string)myJObject["value"][i]["path"];
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
                    SharePercent = (int)myJObject1["value"][i]["SharePercent"]

                });
            }
            model.Sharelist = d;
            if (model.Sharelist.Count == 0)
            {
                model.c = 1;
                model.Sharelist = new List<DirectorshipShareDividends>();
                model.Sharelist.Add(new DirectorshipShareDividends { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-", ShareFileName = "" });
            }
            else
            {
                model.c = model.Sharelist.Count;
            }
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
            
            if (model.applicantBank.Count == 0)
            {
                model.cFin = 1;
                model.applicantBank = new List<ApplicantBank>();
                model.applicantBank.Add(new ApplicantBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.cFin = model.applicantBank.Count;
            }
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
                    ContractSum = (int)myJObject3["value"][i]["ContractSum"],
                    Location = (string)myJObject3["value"][i]["Location"],
                    ProjectName = (string)myJObject3["value"][i]["ProjectName"],
                    RegistationNo = (string)myJObject3["value"][i]["RegistationNo"],
                    TelephoneNo = (string)myJObject3["value"][i]["TelephoneNo"],
                    TypeofInvolvement = (string)myJObject3["value"][i]["TypeofInvolvement"]

                });
            }
            model.worksCapability = w;
            if (model.worksCapability.Count == 0)
            {
                model.cWork = 1;
                model.worksCapability = new List<WorksCapability>();
                model.worksCapability.Add(new WorksCapability { ProjectName = "", Location = "", ContractSum = 0, TypeofInvolvement = "", RegistationNo = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                model.cWork = model.worksCapability.Count;
            }

            model = loadData(model);
            model.formval = "Edit";
            
            if(c!=null)
            {
                if (c.Contains(','))
                {
                    String[] strList = c.Split(",");
                    int len = strList.Length;
                    model = loadDataBuilding(model, 0);
                    model = loadDataCivil(model, 0);
                    model = loadDataElectrical(model, 0);
                    model = loadDataMechanical(model, 0);
                    for (int i = 0; i < len; i++)
                    {
                        for (int k = 0; k < model.businessModel.Category.Count; k++)
                        {
                            if (model.businessModel.Category[k].CategoryName == strList[i])
                            {
                                model.businessModel.Category[k].Selected = true;
                                if (model.FormName == "Form2")
                                {
                                    if (model.businessModel.Category[k].CategoryName.Contains("Building"))
                                    {
                                        model = loadDataBuilding(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Civil"))
                                    {
                                        model = loadDataCivil(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Electrical"))
                                    {
                                        model = loadDataElectrical(model, model.businessModel.Category[k].CategoryID);
                                    }
                                    if (model.businessModel.Category[k].CategoryName.Contains("Mechanical"))
                                    {
                                        model = loadDataMechanical(model, model.businessModel.Category[k].CategoryID);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    model = loadDataBuilding(model, 0);
                    model = loadDataCivil(model, 0);
                    model = loadDataElectrical(model, 0);
                    model = loadDataMechanical(model, 0);
                    for (int k = 0; k < model.businessModel.Category.Count; k++)
                    {
                        if (model.businessModel.Category[k].CategoryName == c)
                        {
                            model.businessModel.Category[k].Selected = true;
                            if (model.FormName == "Form2")
                            {
                                if (model.businessModel.Category[k].CategoryName.Contains("Building"))
                                {
                                    model = loadDataBuilding(model, model.businessModel.Category[k].CategoryID);
                                }
                                if (model.businessModel.Category[k].CategoryName.Contains("Civil"))
                                {
                                    model = loadDataCivil(model, model.businessModel.Category[k].CategoryID);
                                }
                                if (model.businessModel.Category[k].CategoryName.Contains("Electrical"))
                                {
                                    model = loadDataElectrical(model, model.businessModel.Category[k].CategoryID);
                                }
                                if (model.businessModel.Category[k].CategoryName.Contains("Mechanical"))
                                {
                                    model = loadDataMechanical(model, model.businessModel.Category[k].CategoryID);
                                }
                            }
                        }
                    }
                }
            }

            if(c==null)
            {
                model = loadDataBuilding(model, 0);
                model = loadDataCivil(model, 0);
                model = loadDataElectrical(model, 0);
                model = loadDataMechanical(model, 0);
            }
            
            List<AssociationList> AList = new List<AssociationList>();
            AList = ViewBag.ListofAssociation;
            memoryCache.Set("listAssociation", AList);
            memoryCache.Set("Form1", model);
            if (model.App.AppType == "NewApplication")
            {
                model.ReadOnlyField1 = true;
            }
            return RedirectToAction("Index", "Form1", new { id = model.FormName });
        }

        public CICForm1Model loadDataBuilding(CICForm1Model m, int CategoryID = 0)
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
            businessModel.subCategoryBuilding = sub;

            return m;
        }

        public CICForm1Model loadDataElectrical(CICForm1Model m, int CategoryID = 0)
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
            businessModel.subCategoryElect = sub;

            return m;
        }
        public CICForm1Model loadDataCivil(CICForm1Model m, int CategoryID = 0)
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
            businessModel.subCategoryCivil = sub;

            return m;
        }
        public CICForm1Model loadDataMechanical(CICForm1Model m, int CategoryID = 0)
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
            businessModel.subCategoryMech = sub;

            return m;
        }
        public void DownloadFile(string name)
        {

            if (path != "")
            {
                _blobStorageService.DownloadFile(path, name);
            }
        }


        string GetDataForGrade(CICForm1Model p)
        {
            int LargesrContract = 0;
            decimal BestAnnualTurnover = 0;
            int AvailableCapital = 0;
            int temp = 0;
            string tempCategory = "", gradeCategory = "", grade="";

            if (p.App.AppType != "NewApplication")
            {
                BestAnnualTurnover = ((p.financialCapabilityModel.AnnualTurnoverYear1 > p.financialCapabilityModel.AnnualTurnoverYear2 && p.financialCapabilityModel.AnnualTurnoverYear1 > p.financialCapabilityModel.AnnualTurnoverYear3) ? p.financialCapabilityModel.AnnualTurnoverYear1 : (p.financialCapabilityModel.AnnualTurnoverYear2 > p.financialCapabilityModel.AnnualTurnoverYear3) ? p.financialCapabilityModel.AnnualTurnoverYear2 : p.financialCapabilityModel.AnnualTurnoverYear3);


                for (int i = 0; i < p.worksCapability.Count; i++)
                {
                    LargesrContract = Convert.ToInt32(p.worksCapability[i].ContractSum);
                    if (LargesrContract > temp)
                    {
                        temp = LargesrContract;
                    }
                }

                AvailableCapital = Convert.ToInt32(p.financialCapabilityModel.AvailableCapital);

            }

            for (int i = 0; i < p.businessModel.Category.Count; i++)
            {
                if (p.businessModel.Category[i].Selected)
                {
                    if (p != null)
                    {
                        tempCategory = p.businessModel.Category[i].CategoryName;
                        if (tempCategory.Contains("Building") == true)
                        {
                            gradeCategory = "Building" + p.FormName;
                        }
                        else if (tempCategory.Contains("Civil") == true)
                        {
                            gradeCategory = "Civil" + p.FormName;
                        }
                        else if (tempCategory.Contains("Electrical") == true)
                        {
                            gradeCategory = "Electrical" + p.FormName;
                        }
                        else if (tempCategory.Contains("Mechanical") == true)
                        {
                            gradeCategory = "Mechanical" + p.FormName;
                        }

                        if (grade == "")
                        {
                            grade = GetGrade(Convert.ToInt32(BestAnnualTurnover), temp, AvailableCapital, gradeCategory);
                            if(grade.Contains("-"))
                            {
                                String[] gList = grade.Split("-");
                                p.GradeStr = gList[1];
                                p.ScoreStr = gList[0];
                            }
                        }
                        else
                        {
                            string tempstr = GetGrade(Convert.ToInt32(BestAnnualTurnover), temp, AvailableCapital, gradeCategory);
                            grade = grade +","+ tempstr;
                            if (tempstr.Contains("-"))
                            {
                                String[] gList = tempstr.Split("-");
                                p.GradeStr = p.GradeStr +","+ gList[1];
                                p.ScoreStr = p.ScoreStr + "," + gList[0];
                            }
                        }
                    }
                }
            }

            
            return p.GradeStr;
        }

        string GetGradeforForeign(CICForm1Model p)
        {
            
            //int temp = 0;
            string tempCategory = "",  grade = "";

            for (int i = 0; i < p.businessModel.Category.Count; i++)
            {
                if (p.businessModel.Category[i].Selected)
                {
                    if (p != null)
                    {
                        tempCategory = p.businessModel.Category[i].CategoryName;
                        if(p.FormName == "Form1")
                        {
                            if (tempCategory.Contains("Building") == true)
                            {
                                if(grade=="")
                                {
                                    grade = "BF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",BF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                                
                            }
                            else if (tempCategory.Contains("Civil") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "CF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",CF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                            else if (tempCategory.Contains("Electrical") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "EF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",EF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                            else if (tempCategory.Contains("Mechanical") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "MF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",MF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                        }

                        if (p.FormName == "Form2")
                        {
                            if (tempCategory.Contains("Building") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "BSF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",BSF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }

                            }
                            else if (tempCategory.Contains("Civil") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "CSF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",CSF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                            else if (tempCategory.Contains("Electrical") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "ESF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",ESF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                            else if (tempCategory.Contains("Mechanical") == true)
                            {
                                if (grade == "")
                                {
                                    grade = "MSF";
                                    p.ScoreStr = "0";
                                }
                                else
                                {
                                    grade = grade + ",MSF";
                                    p.ScoreStr = p.ScoreStr + ",0";
                                }
                            }
                        }

                    }
                }
            }


            return grade;
        }

        void setDefault(CICForm1Model p)
        {
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
            if (p.businessModel.PresentGrade == null)
            {
                p.businessModel.PresentGrade = "NA";
            }
            if (p.businessModel.Other == null)
            {
                p.businessModel.Other = "-";
            }            
            //
        }

        bool validateIdNo(DirectorshipShareDividends p)
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

        public bool ShareValidation(List<DirectorshipShareDividends> p)
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
                    SwaziShare = SwaziShare + p[i].SharePercent;
                }
            }

            if (SwaziShare != 0 && ForeignShare !=0 && SwaziShare < 60)
            {
                return true;                
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult IsProjectNumberExist(string projectNo)
        {
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform8", projectNo.ToUpper(), out jsonData);//Get data
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            
            string d = "";
            for (int i = 0; i < cntJson; i++)
            {
                d = (string)myJObject["value"][i]["ProjectTite"] + "," + (string)myJObject["value"][i]["ProjectLocation"] + "," +
                    (string)myJObject["value"][i]["ProposedCompleteDate"] + "," + (string)myJObject["value"][i]["TotalProjectCostIncludingLevy"] + "," +
                    (string)myJObject["value"][i]["AuthorisedMobile"] + "," + "Main" + "," +(string)myJObject["value"][i]["RowKey"];
            }            
            return Json(d);
        }

        //public JsonResult IsProjectNumberExist(string projectNo)
        //{
        //    string jsonData;
        //    AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform", out jsonData);//Get data
        //    JObject myJObject = JObject.Parse(jsonData);
        //    int cntJson = myJObject["value"].Count();
        //    List<string> add_list = new List<string>();
        //    string prn;
        //    for (int i = 0; i < cntJson; i++)
        //    {
        //        prn = (string)myJObject["value"][i]["RowKey"];
        //        prn = prn.ToLower();
        //        add_list.Add(prn);

        //    }

        //    if(add_list.Contains(projectNo.ToLower()))
        //    {
        //        IsProjectExist = true;
        //    }
        //    else
        //    {
        //        IsProjectExist = false;
        //    }
        //    return Json(IsProjectExist);
        //}



        [HttpPost]
        public JsonResult GetProjectDetails(string projectNo)
        {
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform8", projectNo, out jsonData);//Get data
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();


            WorksCapability tempwork = new WorksCapability();

            if(cntJson > 0)
            {
                tempwork.TypeofInvolvement = (string)myJObject["value"]["TypeofInvolvement"];
                tempwork.Location = (string)myJObject["value"]["Location"];
                tempwork.CompletionDate = (DateTime)myJObject["value"]["CompletionDate"];
                tempwork.ContractSum = (long)myJObject["value"]["ContractSum"];
                tempwork.ProjectName = (string)myJObject["value"]["ProjectName"];
                tempwork.TelephoneNo = (string)myJObject["value"]["TelephoneNo"];
            }

            
            return Json(tempwork);
        }

        public CICForm1Model loadData(CICForm1Model m)
        {

            List<tblAssociation> associationlist = new List<tblAssociation>();
            List<AssociationList> AList = new List<AssociationList>();

            associationlist = (from association in _context.tblAssociation 
                               where association.formType == "Form1"
                               select association).ToList();

            AList.Add(new AssociationList { AssociationName = " ", AssociationName1 = "Select Association Name" });
            for (int i = 0; i < associationlist.Count; i++)
            {
                AList.Add(new AssociationList { AssociationName = associationlist[i].AssociationName, AssociationName1 = associationlist[i].AssociationName });
            }
                        
            ViewBag.ListofAssociation = AList;

            string FTYpe = "";
            if(m.FormName == "Form1")
            {
                ViewBag.sType = false;
                FTYpe = "F1";
            }else
            {
                ViewBag.sType = true;
                FTYpe= "F2";
            }

            List<Category> categorylist = new List<Category>();
            categorylist = (from Category in _context.Category
                            where Category.FormType == FTYpe
                            select Category).ToList();
          

            List<categoryType> categoryTypeList = new List<categoryType>();

            for (int i=0;i<categorylist.Count;i++)
            {
                categoryTypeList.Add(new categoryType { CategoryID = categorylist[i].CategoryID, CategoryName = categorylist[i].CategoryName });
            }

            businessModel.Category = categoryTypeList;
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
            businessModel.subCategory = subCategorylist;
            //form1Model.businessModel = businessModel;
            return Json(businessModel.subCategory);
            //return Json(new SelectList(subCategorylist, "SubCategoryID", "SubCategoryName"));
        }

        public CICForm1Model GetSubCategorybyName(int Categoryid, CICForm1Model p)
        {
            List<SubCategoryType> subCategorylist = new List<SubCategoryType>();

            subCategorylist = (from SubCategoryType in _context.SubCategory
                               where SubCategoryType.CategoryID == Categoryid
                               select SubCategoryType).ToList();
            
            subCategorylist.Insert(0, new SubCategoryType { SubCategoryID = 0, SubCategoryName = "Select" });
            businessModel.subCategory = subCategorylist;
            p.businessModel = businessModel;
            return p;
        }

        public Int64 getRegNo(CICForm1Model p)
        {
            Int64 tempMax = 0;

            if (p.formval == "Edit")
            {
                tempMax = p.FirmRegistrationNo;
            }
            else
            {
                string jsonData;
                
                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform1", out jsonData);//Get data

                JObject myJObject = JObject.Parse(jsonData);
                int cntJson = myJObject["value"].Count();
                Int64 tempRegNo;


                try
                {
                    if (cntJson != 0)
                    {
                        tempMax = (long)myJObject["value"][0]["FirmRegistrationNo"];
                    }
                    for (int i = 0; i < cntJson; i++)
                    {
                        var regNo = myJObject["value"][i]["FirmRegistrationNo"];
                        Console.WriteLine(regNo);
                        tempRegNo = regNo == null ? 0 : (Int64)regNo;

                        if (tempRegNo > tempMax)
                        {
                            tempMax = tempRegNo;
                        }
                    }
                    tempMax++;
                }
                catch (Exception ex)
                {

                    throw;
                }


            }

            return tempMax;
           
        }

        public void UploadBlob(CICForm1Model p, Int64 tempMax)
        {
            if (p.App.ImagePath == "NA" || p.App.ImagePath == "-")
            {
                p.App.ImagePath = "Form" + tempMax;
            }
                       

            uploadFiles1(p.App.Filesignature, p.App.ImagePath, "Filesignature");
            uploadFiles1(p.businessModel.Businesssignature, p.App.ImagePath, "Businesssignature");
            uploadFiles1(p.financialCapabilityModel.StatmentFile, p.App.ImagePath, "StatmentFile");
            uploadFiles1(p.docs.BusinessFile1, p.App.ImagePath, "BusinessFile1");
            uploadFiles1(p.docs.BusinessFile2, p.App.ImagePath, "BusinessFile2");
            uploadFiles1(p.docs.BusinessFile3, p.App.ImagePath, "BusinessFile3");
            uploadFiles1(p.docs.BusinessFile4, p.App.ImagePath, "BusinessFile4");
            uploadFiles1(p.docs.BusinessFile5, p.App.ImagePath, "BusinessFile5");
            uploadFiles1(p.docs.BusinessFile6, p.App.ImagePath, "BusinessFile6");
            uploadFiles1(p.docs.ShareholdersFile1, p.App.ImagePath, "ShareholdersFile1");
            uploadFiles1(p.docs.ShareholdersFile2, p.App.ImagePath, "ShareholdersFile2");
            uploadFiles1(p.docs.ShareholdersFile3, p.App.ImagePath, "ShareholdersFile3");
            uploadFiles1(p.docs.FinancialFile1, p.App.ImagePath, "FinancialFile1");
            uploadFiles1(p.docs.FinancialFile2, p.App.ImagePath, "FinancialFile2");
            uploadFiles1(p.docs.FinancialFile3, p.App.ImagePath, "FinancialFile3");
            uploadFiles1(p.docs.TrackRecordFile1, p.App.ImagePath, "TrackRecordFile1");
            uploadFiles1(p.docs.TrackRecordFile2, p.App.ImagePath, "TrackRecordFile2");
            uploadFiles1(p.docs.TrackRecordFile3, p.App.ImagePath, "TrackRecordFile3");
            uploadFiles1(p.docs.JointVentureFile1, p.App.ImagePath, "JointVentureFile1");
            uploadFiles1(p.docs.JointVentureFile2, p.App.ImagePath, "JointVentureFile2");
            uploadFiles1(p.docs.JointVentureFile3, p.App.ImagePath, "JointVentureFile3");
            uploadFiles1(p.docs.JointVentureFile4, p.App.ImagePath, "JointVentureFile4");
            uploadFiles1(p.docs.Signature1, p.App.ImagePath, "Signature1");
            uploadFiles1(p.docs.Signature2, p.App.ImagePath, "Signature2");
            uploadFiles1(p.docs.TaxLaw, p.App.ImagePath, "TaxLaw");
            uploadFiles1(p.docs.Evidence, p.App.ImagePath, "Evidence");
            uploadFiles1(p.docs.Compliance, p.App.ImagePath, "Compliance");

            //for (int i = 0; i < p.Sharelist.Count; i++)
            //{
            //    string tempn = "ShareFile" + i.ToString();
            //    uploadFiles1(p.Sharelist[i].ShareFile, p.App.ImagePath, tempn);
            //}
        }
        public string Savedata(CICForm1Model model)
        {
            CustomValidations cv = new CustomValidations();
            string TableName = "cicform1()";
            string response = "";
            Form1Model newModel = new Form1Model();

            Form1Mapper k = new Form1Mapper();
            
            //string jsonData;
            long tempMax = 0;

            if (model.formval == "Edit")
            {
                tempMax = model.FirmRegistrationNo;
            }
            else
            {
                //AzureTablesData.GetAllEntity(StorageName, StorageKey, TableName, out jsonData);//Get data

                //JObject myJObject = JObject.Parse(jsonData);
                //int cntJson = myJObject["value"].Count();
                //int tempRegNo;

                //if (cntJson != 0)
                //{
                //    tempMax = (int)myJObject["value"][0]["FirmRegistrationNo"];
                //}


                //for (int i = 0; i < cntJson; i++)
                //{
                //    tempRegNo = (int)myJObject["value"][i]["FirmRegistrationNo"];

                //    if (tempRegNo > tempMax)
                //    {
                //        tempMax = tempRegNo;
                //    }
                //}
                //tempMax++;


                model.App.ImagePath = "Form" + model.FirmRegistrationNo;
                tempMax = model.FirmRegistrationNo;

            }

            //uploadFiles1(model.App.Filesignature, model.App.ImagePath, "Filesignature");
            //uploadFiles1(model.businessModel.Businesssignature, model.App.ImagePath, "Businesssignature");
            //uploadFiles1(model.financialCapabilityModel.StatmentFile, model.App.ImagePath, "StatmentFile");
            //uploadFiles1(model.docs.BusinessFile1, model.App.ImagePath, "BusinessFile1");
            //uploadFiles1(model.docs.BusinessFile2, model.App.ImagePath, "BusinessFile2");
            //uploadFiles1(model.docs.BusinessFile3, model.App.ImagePath, "BusinessFile3");
            //uploadFiles1(model.docs.BusinessFile4, model.App.ImagePath, "BusinessFile4");
            //uploadFiles1(model.docs.BusinessFile5, model.App.ImagePath, "BusinessFile5");
            //uploadFiles1(model.docs.BusinessFile6, model.App.ImagePath, "BusinessFile6");
            //uploadFiles1(model.docs.ShareholdersFile1, model.App.ImagePath, "ShareholdersFile1");
            //uploadFiles1(model.docs.ShareholdersFile2, model.App.ImagePath, "ShareholdersFile2");
            //uploadFiles1(model.docs.ShareholdersFile3, model.App.ImagePath, "ShareholdersFile3");
            //uploadFiles1(model.docs.FinancialFile1, model.App.ImagePath, "FinancialFile1");
            //uploadFiles1(model.docs.FinancialFile2, model.App.ImagePath, "FinancialFile2");
            //uploadFiles1(model.docs.FinancialFile3, model.App.ImagePath, "FinancialFile3");
            //uploadFiles1(model.docs.TrackRecordFile1, model.App.ImagePath, "TrackRecordFile1");
            //uploadFiles1(model.docs.TrackRecordFile2, model.App.ImagePath, "TrackRecordFile2");
            //uploadFiles1(model.docs.TrackRecordFile3, model.App.ImagePath, "TrackRecordFile3");
            //uploadFiles1(model.docs.JointVentureFile1, model.App.ImagePath, "JointVentureFile1");
            //uploadFiles1(model.docs.JointVentureFile2, model.App.ImagePath, "JointVentureFile2");
            //uploadFiles1(model.docs.JointVentureFile3, model.App.ImagePath, "JointVentureFile3");
            //uploadFiles1(model.docs.JointVentureFile4, model.App.ImagePath, "JointVentureFile4");
            //uploadFiles1(model.docs.Signature1, model.App.ImagePath, "Signature1");
            //uploadFiles1(model.docs.Signature2, model.App.ImagePath, "Signature2");
            //uploadFiles1(model.docs.TaxLaw, model.App.ImagePath, "TaxLaw");
            //uploadFiles1(model.docs.Evidence, model.App.ImagePath, "Evidence");
            //uploadFiles1(model.docs.Compliance, model.App.ImagePath, "Compliance");


            var imagePath = _appSettingsReader.Read("ImagePath");
            if (filepath != "NA")
            {
                model.App.ImagePath = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    model.App.ImagePath = imagePath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }
            model.CustNo = HttpContext.Session.GetString("CustNo");
            model.CreatedBy = User.Identity.Name;
            memoryCache.Set("emailto", User.Identity.Name);
            memoryCache.Set("QGrade", model.Grade);
            k.mapData(model, newModel, tempMax);
            string firmNo = newModel.RowKey;
            if (model.formval == "Edit")
            {
                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(newModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
            }
            else
            {
                newModel.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(newModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }

            if (response == "Created" || response == "NoContent") 
            {
                TableName = "cicform1ShareDividends()";
                DirectorshipShareDividends d = new DirectorshipShareDividends();
                for (int i = 0; i < model.Sharelist.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(model.Sharelist[i]);

                    if (DFlag == false)
                    {
                        string Data;
                        k.mapShareDetails(d, model.Sharelist[i], tempMax);
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1ShareDividends", d.PartitionKey, d.RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        
                        if (cntJson !=0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1ShareDividends", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), d.PartitionKey, d.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                            //string tempn = "ShareFile" + i.ToString();
                            //uploadFiles1(model.Sharelist[i].ShareFile, model.App.ImagePath, tempn);
                        }
                    }
                }

                TableName = "cicform1BankDetails()";
                ApplicantBank AppB = new ApplicantBank();
                for (int i = 0; i < model.applicantBank.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(model.applicantBank[i]);

                    if (DFlag == false)
                    {
                        k.mapBankDetails(AppB, model.applicantBank[i], tempMax);
                        string Data;                        
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1BankDetails", AppB.PartitionKey, AppB.RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();

                        if (cntJson !=0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1BankDetails", JsonConvert.SerializeObject(AppB, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), AppB.PartitionKey, AppB.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(AppB, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }

                TableName = "cicform1WorkDetails()";
                WorksCapability workdata = new WorksCapability();
                for (int i = 0; i < model.worksCapability.Count; i++)
                {
                    bool DFlag = cv.IsAnyNullOrEmpty(model.worksCapability[i]);

                    if (DFlag == false)
                    {
                        k.mapWorkDetails(workdata, model.worksCapability[i], tempMax);
                        string Data;
                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1WorkDetails", workdata.PartitionKey, workdata.RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();


                        if (cntJson != 0)
                        {
                            response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1WorkDetails", JsonConvert.SerializeObject(workdata, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), workdata.PartitionKey, workdata.RowKey);
                        }
                        else
                        {
                            response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(workdata, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                }
            }
            return firmNo;
        }


        public void uploadFiles(IFormFile tempFile, string path, string name)
        {
            if (tempFile != null)
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

                filepath = _blobStorageService.UploadFileToBlob(tempFile.FileName, fileData, mimeType,path);
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
                    var imagePath = _appSettingsReader.Read("ImagePath");
                    filepath = imagePath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + path;
                }
                else
                {
                    filepath = path;
                }
            }
            return filepath;
        }

        public async Task<string> uploadFiles2(IFormFile tempFile, string path, string name)
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

        public IActionResult Form1Result(string result, string text)
        {
            CICCommonService cmsrv = new CICCommonService(_userManager);
            string yr = cmsrv.GetFinancialYear();
            string body = "", subject = "", emailto="",grade ="";
            ViewBag.Result = result;
            ViewBag.sts = text;
            ViewBag.yr = yr;
            memoryCache.TryGetValue("QGrade", out grade);
            ViewBag.grade = grade;
            var domain = _appSettingsReader.Read("Domain");

            if (text == "Draft")
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been saved as draft. To edit your application, please log in <a href='"+ domain +"'>CIC Portal</a> and continue with your application and submit. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been saved as draft";
            }
            else
            {
                body = "<p>Dear Valuable Contractor, your application - " + result + " for the financial year " + yr + " CIC registration/renewal has been successfully submitted.<br/> You are qualified for grade - " + grade + ". To view your application status, please log in <a href='" + domain + "'>CIC Portal</a> and view your dashboard. <br/><br/>Thank you,<br/>CIC Team</p>";
                subject = "CIC registration/renewal has been successfully submitted";
            }
            memoryCache.TryGetValue("emailto", out emailto);
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig,_context,_userManager, _appSettingsReader, _blobStorageService);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
        }

        [HttpPost]
        public void Form8Registration (CICForm1Model p)
        {
            setDefault(p);
            fileDefault(p);
            for (int i = 0; i < p.Sharelist.Count; i++)
            {
                if (p.Sharelist[i].PartitionKey == null)
                    p.Sharelist[i].PartitionKey = "-";
                if (p.Sharelist[i].RowKey == null)
                    p.Sharelist[i].RowKey = "-";                
                if (p.Sharelist[i].ShareFileName == null)
                    p.Sharelist[i].ShareFileName = "-";
            }

            for (int i = 0; i < p.applicantBank.Count; i++)
            {
                if (p.applicantBank[i].PartitionKey == null)
                    p.applicantBank[i].PartitionKey = "-";
                if (p.applicantBank[i].RowKey == null)
                    p.applicantBank[i].RowKey = "-";                
            }

            for (int i = 0; i < p.worksCapability.Count; i++)
            {
                if (p.worksCapability[i].PartitionKey == null)
                    p.worksCapability[i].PartitionKey = "-";
                if (p.worksCapability[i].RowKey == null)
                    p.worksCapability[i].RowKey = "-";
                
            }


            p.FormStatus = "Draft";
            string result = Savedata(p);
            //return RedirectToRoute( RedirectToAction("CicForm8", "Form8");
            //return RedirectToRoute(new { controller = "Form8", action = "CicForm8" });
        }

        public string GetGrade(int? BestAnnualTurnover, int? LargesrContract, int? AvailableCapital, string category)
        {

            using (var httpClient = new HttpClient())
            {

                  HttpResponseMessage response = httpClient.GetAsync("https://gradeapi.azurewebsites.net/api/GradeFactor/GetGrading?BestAnnualTurnover=" + BestAnnualTurnover + "&LargesrContract=" + LargesrContract + "&AvailableCapital=" + AvailableCapital + "&category=" + category).Result;
                //HttpResponseMessage response = httpClient.GetAsync("https://localhost:44372/api/GradeFactor/GetGrading?BestAnnualTurnover=" + BestAnnualTurnover + "&LargesrContract=" + LargesrContract + "&AvailableCapital=" + AvailableCapital + "&category=" + category).Result;
                if (response.IsSuccessStatusCode)
                {
                    Grade = response.Content.ReadAsStringAsync().Result;                    
                }

            }
            return Grade;
        }

    }


}
