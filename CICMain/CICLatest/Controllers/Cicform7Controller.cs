
using CICLatest.Helper;
using CICLatest.Models;

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
    public class CicForm7Controller : Controller
    {
        private readonly ApplicationContext _context;
        Cicf7Model Form7Model = new Cicf7Model();
        //BusinessDetails businessModel = new BusinessDetails();
        //static string formName;
        int cnt = 1;
        //int cntt = 1;
        //int cnttt = 1;
        //string FormStatus;
        static string filepath = "NA";
        Regex regex = new Regex(@"((\d\d)(0[1-9]|1[0-2])((0|1)[0-9]|2[0-9]|3[0-1]))$");
        private readonly UserManager<UserModel> _userManager;
        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        private readonly IMemoryCache memoryCache;
        CustomValidations cv = new CustomValidations();
        static string StorageName = "";
        static string StorageKey = "";
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public CicForm7Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache
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

        public ActionResult CicForm7()
        {

            ViewBag.type = "active";
            Form7Model.c = cnt;
            Form7Model.cFin = cnt;
            Form7Model.cWork = cnt;
            Cicf7Model form7EditModel = new Cicf7Model();

            bool isExist = memoryCache.TryGetValue("Form7", out form7EditModel);
            if (isExist)
            {
                List<AssociationList> AList = new List<AssociationList>();

                memoryCache.TryGetValue("listAssociation", out AList);
                ViewBag.ListofAssociation = AList;
                Form7Model = form7EditModel;
            }
            if (!isExist)
            {



                Form7Model.Sharelist = new List<DirectorshipShareDividendsSection>();
                Form7Model.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });

                Form7Model.companyBank = new List<CompanyBank>();
                Form7Model.companyBank.Add(new CompanyBank { BankName = "", BranchName = "", BranchCode = "", AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });

                Form7Model.listOfPrevousClent = new List<ListOfPreviousClient>();
                Form7Model.listOfPrevousClent.Add(new ListOfPreviousClient { NameofClient = "", ServiceProvided = "", DateOrPeriod = null, ContractValue = 0, PartitionKey = "-", RowKey = "-" });
                loadData(Form7Model);
            }
            return View(Form7Model);




        }

        public bool ShareValidation(List<DirectorshipShareDividendsSection> p)
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

            if (SwaziShare != 0 && ForeignShare != 0 && SwaziShare < 60)
            {
                return true;
            }
            else
            {
                return false;
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
        public void setGetFileEdit(Cicf7Model p)
        {
            if (p.App.AssociationCertificateAttachment != null)
            {
                memoryCache.Set("AssociationCertificateAttachment", p.App.AssociationCertificateAttachment);
                p.App.AssociationCertificateName = p.App.AssociationCertificateAttachment.FileName;
                memoryCache.Set("AssociationCertificateAttachment", p.App.AssociationCertificateAttachment.FileName);
            }
            else
            {
                p.App.AssociationCertificateName = SetandGetFileEdit("AssociationCertificateAttachment");
            }





            if (p.businessModel.Signature != null)
            {
                memoryCache.Set("Signature3", p.businessModel.Signature);
                p.Signature3 = p.businessModel.Signature.FileName;
                memoryCache.Set("Signature3", p.businessModel.Signature.FileName);
            }
            else
            {
                p.Signature3 = SetandGetFileEdit("Signature3");
            }

            //if (p.financialCapabilityForm7.StatmentFile != null)
            //{
            //    memoryCache.Set("StatmentFile", p.financialCapabilityForm7.StatmentFile);
            //    p.financialCapabilityForm7.statementFilename = p.financialCapabilityForm7.StatmentFile.FileName;
            //    memoryCache.Set("StatmentFile", p.financialCapabilityForm7.statementFilename);
            //}
            //else
            //{
            //    p.financialCapabilityForm7.statementFilename = SetandGetFileEdit("StatmentFile");
            //}


            if (p.declarationForm7.companyRegistration != null)
            {
                memoryCache.Set("companyRegistration", p.declarationForm7.companyRegistration);
                p.companyRegistration = p.declarationForm7.companyRegistration.FileName;
                memoryCache.Set("companyRegistration", p.declarationForm7.companyRegistration.FileName);
            }
            else
            {
                p.companyRegistration = SetandGetFileEdit("companyRegistration");
            }

            if (p.declarationForm7.shraeCertificate != null)
            {
                memoryCache.Set("shraeCertificate", p.declarationForm7.shraeCertificate);
                p.shraeCertificate = p.declarationForm7.shraeCertificate.FileName;
                memoryCache.Set("shraeCertificate", p.declarationForm7.shraeCertificate.FileName);
            }
            else
            {
                p.shraeCertificate = SetandGetFileEdit("shraeCertificate");
            }

            if (p.declarationForm7.TradingLicense != null)
            {
                memoryCache.Set("TradingLicense", p.declarationForm7.TradingLicense);
                p.TradingLicense = p.declarationForm7.TradingLicense.FileName;
                memoryCache.Set("TradingLicense", p.declarationForm7.TradingLicense.FileName);
            }
            else
            {
                p.TradingLicense = SetandGetFileEdit("TradingLicense");
            }
            if (p.declarationForm7.formj != null)
            {
                memoryCache.Set("formj", p.declarationForm7.formj);
                p.formj = p.declarationForm7.formj.FileName;
                memoryCache.Set("formj", p.declarationForm7.formj.FileName);
            }
            else
            {
                p.formj = SetandGetFileEdit("formj");
            }
            if (p.declarationForm7.FormC != null)
            {
                memoryCache.Set("FormC", p.declarationForm7.FormC);
                p.FormC = p.declarationForm7.FormC.FileName;
                memoryCache.Set("FormC", p.declarationForm7.FormC.FileName);
            }
            else
            {
                p.FormC = SetandGetFileEdit("FormC");
            }
            if (p.declarationForm7.RegistrationCertificate != null)
            {
                memoryCache.Set("RegistrationCertificate", p.declarationForm7.RegistrationCertificate);
                p.RegistrationCertificate = p.declarationForm7.RegistrationCertificate.FileName;
                memoryCache.Set("RegistrationCertificate", p.declarationForm7.RegistrationCertificate.FileName);
            }
            else
            {
                p.RegistrationCertificate = SetandGetFileEdit("RegistrationCertificate");
            }
            if (p.declarationForm7.CertifiedCompanyRegistration != null)
            {
                memoryCache.Set("CertifiedCompanyRegistration", p.declarationForm7.CertifiedCompanyRegistration);
                p.CertifiedCompanyRegistration = p.declarationForm7.CertifiedCompanyRegistration.FileName;
                memoryCache.Set("CertifiedCompanyRegistration", p.declarationForm7.CertifiedCompanyRegistration.FileName);
            }
            else
            {
                p.CertifiedCompanyRegistration = SetandGetFileEdit("CertifiedCompanyRegistration");
            }
            if (p.declarationForm7.CertifiedIdentityDocuments != null)
            {
                memoryCache.Set("CertifiedIdentityDocuments", p.declarationForm7.CertifiedIdentityDocuments);
                p.CertifiedIdentityDocuments = p.declarationForm7.CertifiedIdentityDocuments.FileName;
                memoryCache.Set("CertifiedIdentityDocuments", p.declarationForm7.CertifiedIdentityDocuments.FileName);
            }
            else
            {
                p.CertifiedIdentityDocuments = SetandGetFileEdit("CertifiedIdentityDocuments");
            }
            if (p.declarationForm7.RenewBusinessScopeOfwork != null)
            {
                memoryCache.Set("RenewBusinessScopeOfwork", p.declarationForm7.RenewBusinessScopeOfwork);
                p.RenewBusinessScopeOfwork = p.declarationForm7.RenewBusinessScopeOfwork.FileName;
                memoryCache.Set("RenewBusinessScopeOfwork", p.declarationForm7.RenewBusinessScopeOfwork.FileName);
            }
            else
            {
                p.RenewBusinessScopeOfwork = SetandGetFileEdit("RenewBusinessScopeOfwork");
            }
            if (p.declarationForm7.RenewCertificate != null)
            {
                memoryCache.Set("RenewCertificate", p.declarationForm7.RenewCertificate);
                p.RenewCertificate = p.declarationForm7.RenewCertificate.FileName;
                memoryCache.Set("RenewCertificate", p.declarationForm7.RenewCertificate.FileName);
            }
            else
            {
                p.RenewCertificate = SetandGetFileEdit("RenewCertificate");
            }
            if (p.declarationForm7.RenewFormJ != null)
            {
                memoryCache.Set("RenewFormJ", p.declarationForm7.RenewFormJ);
                p.RenewFormJ = p.declarationForm7.RenewFormJ.FileName;
                memoryCache.Set("RenewFormJ", p.declarationForm7.RenewFormJ.FileName);
            }
            else
            {
                p.RenewFormJ = SetandGetFileEdit("RenewFormJ");
            }
            if (p.declarationForm7.ReNewFormC != null)
            {
                memoryCache.Set("ReNewFormC", p.declarationForm7.ReNewFormC);
                p.ReNewFormC = p.declarationForm7.ReNewFormC.FileName;
                memoryCache.Set("ReNewFormC", p.declarationForm7.ReNewFormC.FileName);
            }
            else
            {
                p.ReNewFormC = SetandGetFileEdit("ReNewFormC");
            }
            if (p.declarationForm7.RenewFormBMCA != null)
            {
                memoryCache.Set("RenewFormBMCA", p.declarationForm7.RenewFormBMCA);
                p.RenewFormBMCA = p.declarationForm7.RenewFormBMCA.FileName;
                memoryCache.Set("RenewFormBMCA", p.declarationForm7.RenewFormBMCA.FileName);
            }
            else
            {
                p.RenewFormBMCA = SetandGetFileEdit("RenewFormBMCA");
            }

            if (p.declarationForm7.RenewFinancialRequirment != null)
            {
                memoryCache.Set("RenewFinancialRequirment", p.declarationForm7.RenewFinancialRequirment);
                p.RenewFinancialRequirment = p.declarationForm7.RenewFinancialRequirment.FileName;
                memoryCache.Set("RenewFinancialRequirment", p.declarationForm7.RenewFinancialRequirment.FileName);
            }
            else
            {
                p.RenewFinancialRequirment = SetandGetFileEdit("RenewFinancialRequirment");
            }
            if (p.declarationForm7.RenewIdentification != null)
            {
                memoryCache.Set("RenewIdentification", p.declarationForm7.RenewIdentification);
                p.RenewIdentification = p.declarationForm7.RenewIdentification.FileName;
                memoryCache.Set("RenewIdentification", p.declarationForm7.RenewIdentification.FileName);
            }
            else
            {
                p.RenewIdentification = SetandGetFileEdit("RenewIdentification");
            }
            if (p.declarationForm7.Signature != null)
            {
                memoryCache.Set("Signature", p.declarationForm7.Signature);
                p.Signature = p.declarationForm7.Signature.FileName;
                memoryCache.Set("Signature", p.declarationForm7.Signature.FileName);
            }
            else
            {
                p.Signature = SetandGetFileEdit("Signature");
            }
            if (p.documentsUpload.Signature2 != null)
            {
                memoryCache.Set("Signature2", p.documentsUpload.Signature2);
                p.Signature2 = p.documentsUpload.Signature2.FileName;
                memoryCache.Set("Signature2", p.documentsUpload.Signature2.FileName);
            }
            else
            {
                p.Signature2 = SetandGetFileEdit("Signature2");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm7(Cicf7Model p, string name, string next, string pre)
        {



            fileDefault(p);
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
            if (p.formval == "Edit")
            {
                if (p.Sharelist != null)
                {
                    p.c = p.Sharelist.Count() + 1;

                    for (int i = 0; i < p.Sharelist.Count; i++)
                    {
                        //p.Sharelist[i].PartitionKey = "-";
                        //p.Sharelist[i].RowKey = "-";
                        //if (p.Sharelist[i].Nationnality == "Select Account")
                        //{
                        //    ViewBag.Nationnality = "Please Select Account";

                        //    break;
                        //}

                        if (validateIdNo(p.Sharelist[i]))
                        {
                            ModelState.AddModelError(nameof(p.err), "Invalid Id number!");
                        }

                    }
                }
                else
                {
                    p.c = cnt;
                    p.Sharelist = new List<DirectorshipShareDividendsSection>();
                    p.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
                }





            }
            else
            {
                //Grid work
                if (p.Sharelist != null)
                {
                    p.c = p.Sharelist.Count() + 1;

                    for (int i = 0; i < p.Sharelist.Count; i++)
                    {
                        p.Sharelist[i].PartitionKey = "-";
                        p.Sharelist[i].RowKey = "-";
                        //if (p.Sharelist[i].Nationnality == "Select Account")
                        //{
                        //    ViewBag.Nationnality = "Please Select Account";

                        //    break;
                        //}

                        if (validateIdNo(p.Sharelist[i]))
                        {
                            ModelState.AddModelError(nameof(p.err), "Invalid Id number!");
                        }

                    }
                }
                else
                {
                    p.c = cnt;
                    p.Sharelist = new List<DirectorshipShareDividendsSection>();
                    p.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
                }

            }


            if (p.formval == "Edit")
            {
                if (p.companyBank != null)
                {
                    p.cFin = p.companyBank.Count() + 1;



                }
                else
                {
                    p.cFin = cnt;

                    p.companyBank = new List<CompanyBank>();
                    p.companyBank.Add(new CompanyBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
                }


            }
            else
            {
                if (p.companyBank != null)
                {
                    p.cFin = p.companyBank.Count() + 1;

                    for (int i = 0; i < p.companyBank.Count; i++)
                    {
                        p.companyBank[i].PartitionKey = "-";
                        p.companyBank[i].RowKey = "-";
                    }

                }
                else
                {
                    p.cFin = cnt;

                    p.companyBank = new List<CompanyBank>();
                    p.companyBank.Add(new CompanyBank { AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", BankName = "", BranchCode = "", BranchName = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });
                }


            }
            if (p.formval == "Edit")
            {
                //if (p.listOfPrevousClent != null)
                //{

                //    p.cWork = p.listOfPrevousClent.Count() + 1;


                //}
                //else
                //{
                //    p.cWork = cnt;
                //    p.listOfPrevousClent = new List<ListOfPreviousClient>();
                //    p.listOfPrevousClent.Add(new ListOfPreviousClient { NameofClient = "", ServiceProvided = "", DateOrPeriod = null, ContractValue = "", PartitionKey = "-", RowKey = "-" }); ;


                //}
            }
            else
            {
                if (p.listOfPrevousClent != null)
                {

                    p.cWork = p.listOfPrevousClent.Count() + 1;
                    for (int i = 0; i < p.listOfPrevousClent.Count; i++)
                    {
                        p.listOfPrevousClent[i].PartitionKey = "-";
                        p.listOfPrevousClent[i].RowKey = "-";
                    }


                }
                else
                {
                    p.cWork = cnt;
                    p.listOfPrevousClent = new List<ListOfPreviousClient>();
                    p.listOfPrevousClent.Add(new ListOfPreviousClient { NameofClient = "", ServiceProvided = "", DateOrPeriod = null, ContractValue = 0, PartitionKey = "-", RowKey = "-" }); ;


                }

            }



            if (p.businessModel.Other == null)
            {

                p.businessModel.Other = "-";
            }

            if (p.businessModel.FaxNo == null)
            {
                p.businessModel.FaxNo = "-";
            }
            if (p.businessModel.BusinessRepresentativeFax == null)
            {
                p.businessModel.BusinessRepresentativeFax = "-";
            }
            fileDefault(p);

            bool checkshares = ShareValidation(p.Sharelist);

            if (checkshares)
            {
                p.businessModel.BusinessType = "ForeignCompany";
                ViewBag.BType = "checked";
            }

            //p.FirmRegistrationNo = getRegNo(p);
            ModelState.Remove("financialCapabilityForm7.AnnualTurnoverYear2");
            ModelState.Remove("financialCapabilityForm7.AnnualTurnoverYear3");
            //if (p.App.AssociationName == "Select association name")
            //    ViewBag.AssociationName = "Please select Association Name";

            //Model state no valid & next/Prev btn cicked - stay on same tab
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
                    UploadBlob(p, p.FirmRegistrationNo);
                    break;

                case "business":


                    bool BFlag = IsAnyNullOrEmpty(p.businessModel);

                    //bool DFlag;

                    //DFlag = BusinessModelvalidations(p);

                    bool DFlag = false;
                    DFlag = BusinessModelvalidations1(p);



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

                    bool FFlag = IsAnyNullOrEmpty(p.technicalData);
                    bool aFlag = false;

                    for (int i = 0; i < p.listOfPrevousClent.Count; i++)
                    {
                        aFlag = IsAnyNullOrEmpty(p.listOfPrevousClent[i]);

                        if (aFlag == true)
                            break;
                    }
                    if (FFlag == false && aFlag == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if (FFlag == true && aFlag == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if (FFlag == true && aFlag == false)
                    {
                        ViewBag.fin = "active";
                        break;
                    }

                    if ((FFlag == false && aFlag == false) && next != null)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if ((FFlag == false && aFlag == true) && next != null)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if ((FFlag == true && aFlag == false) && next != null)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if ((FFlag == false && aFlag == true) && pre != null)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if ((FFlag == true && aFlag == false) && pre != null)
                    {
                        ViewBag.fin = "active";
                        break;
                    }
                    if ((FFlag == false && aFlag == false) && pre != null)
                    {
                        ViewBag.business = "active";
                        break; ;
                    }

                    else if (aFlag == true)
                    {
                        ModelState.AddModelError(nameof(p.err), "Add List of Previous Project Details!");
                        ViewBag.fin = "active";
                    }
                    UploadBlob(p, p.FirmRegistrationNo);
                    break;

                case "work":


                    bool WFlag = IsAnyNullOrEmpty(p.financialCapabilityForm7);
                    WFlag = FinanceModelValidations1(p);

                    bool WAFlag = false;

                    //for (int i = 0; i < p.companyBank.Count; i++)
                    //{
                    //    WAFlag = IsAnyNullOrEmpty(p.companyBank[i]);

                    //    if (WAFlag == true)
                    //        break;
                    //}
                    if (WFlag == false && WAFlag == true)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if (WFlag == true && WAFlag == true)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if (WFlag == true && WAFlag == false)
                    {
                        ViewBag.work = "active";
                        break;
                    }

                    if ((WFlag == false && WAFlag == false) && next != null)
                    {
                        ViewBag.doc = "active";
                        break;
                    }
                    if ((WFlag == false && WAFlag == true) && next != null)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if ((WFlag == true && WAFlag == false) && next != null)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if ((WFlag == false && WAFlag == true) && pre != null)
                    {
                        ViewBag.work = "active";
                        break;
                    }
                    if ((WFlag == true && WAFlag == false) && pre != null)
                    {
                        ViewBag.work = "active";
                        break; ;
                    }
                    if ((WFlag == false && WAFlag == false) && pre != null)
                    {
                        ViewBag.fin = "active";
                        break; ;
                    }
                    UploadBlob(p, p.FirmRegistrationNo);
                    break;

                case "doc":


                    // bool decFlag = IsAnyNullOrEmpty(p.declarationForm7);


                    bool docFlag = cv.IsAnyNullOrEmpty(p.declarationForm7);
                    bool docFlag1 = DocModelValidation(p);
                    if (docFlag == true || docFlag1 == true)
                    {
                        ViewBag.doc = "active";
                    }
                    else if (docFlag == false && pre != null)
                    {
                        ViewBag.work = "active";
                    }

                    //if (decFlag == true)
                    //{
                    //    ViewBag.doc = "active";
                    //}
                    //else if (decFlag == false && pre != null)
                    //{
                    //    ViewBag.work = "active";
                    //}
                    UploadBlob(p, p.FirmRegistrationNo);

                    break;
                case "draft":
                    p.Reviewer = "";
                    p.FormStatus = "Draft";
                    UploadBlob(p, p.FirmRegistrationNo);
                    string result = Savedata(p);
                    removeDatafromSession();
                    ModelState.Clear();
                    return RedirectToAction("Form7Result", "Cicform7", new { result = result, text = "Draft" });


                case "final":

                    ModelState.Remove("p.ImagePath");
                    ModelState.Remove("p.businessModel.FaxNo");
                    ModelState.Remove("p.businessModel.BusinessRepresentativeFax");
                    ModelState.Remove("p.FormStatus");
                    ModelState.Remove("p.err");
                    ModelState.Remove("p.PartitionKey");
                    ModelState.Remove("p.RowKey");

                    UploadBlob(p, p.FirmRegistrationNo);
                    //bool A = IsAnyNullOrEmpty(p.App);
                    //if (A == true)
                    //{
                    //    ViewBag.type = "active";
                    //    break;
                    //}
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    bool A1 = AppModelValidations(p);
                    if (A == true || A1 == true)
                    {
                        ViewBag.type = "active";
                        break;
                    }
                    //bool B = IsAnyNullOrEmpty(p.businessModel);
                    //if (B == true)
                    //{
                    //    ViewBag.business = "active";
                    //    break;
                    //}
                    bool B = cv.IsAnyNullOrEmpty(p.businessModel);
                    bool B1 = BusinessModelvalidations(p);
                    if (B == true || B1 == true)
                    {
                        ViewBag.business = "active";
                        break;
                    }

                    bool F = IsAnyNullOrEmpty(p.technicalData);
                    if (F == true)
                    {
                        ViewBag.fin = "active";
                        break;
                    }

                    bool G = cv.IsAnyNullOrEmpty(p.financialCapabilityForm7);
                    G = FinanceModelValidations1(p);
                    if (G == true)
                    {
                        ViewBag.fin = "active";
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
                        p.Reviewer = "Clerk";
                        p.FormStatus = "Submit";
                        //  UploadBlob(p, p.FirmRegistrationNo);
                        string result3 = Savedata(p);
                        removeDatafromSession();
                        ModelState.Clear();
                        return RedirectToAction("Form7Result", "Cicform7", new { result = result3, text = "Final" });
                    }
                    else
                    {
                        ViewBag.doc = "active";
                    }

                    break;
            }
            loadData(p);
            if (p.formval == "Edit")
            {
                if (p.businessModel.Signature != null)
                {
                    memoryCache.Set("Signature3", p.businessModel.Signature);
                    p.Signature = p.businessModel.Signature.FileName;
                    memoryCache.Set("Signature3", p.businessModel.Signature);
                }
                else
                {
                    p.Signature3 = SetandGetFileEdit("Signature3");
                }
            }
            else
            {
                IFormFile fsign;
                bool isExist = memoryCache.TryGetValue("Signature3", out fsign);
                if (!isExist)
                {
                    if (p.businessModel.Signature != null)
                    {
                        memoryCache.Set("Signature3", p.businessModel.Signature);
                        p.Signature3 = p.businessModel.Signature.FileName;
                    }
                }
                else
                {
                    if (p.businessModel.Signature != null)
                    {
                        memoryCache.Set("Signature3", p.businessModel.Signature);
                    }
                    else
                    {
                        p.businessModel.Signature = fsign;
                    }
                    p.Signature3 = p.businessModel.Signature.FileName;

                }
            }

            if (p.App.AssociationCertificateAttachment != null)
            {
                ModelState.Remove("App.AssociationCertificateAttachment");
            }
            if (p.businessModel.Signature != null)
            {
                ModelState.Remove("businessModel.Signature");
            }
            //if (p.financialCapabilityModel.StatmentFile != null)
            //{
            //    ModelState.Remove("financialCapabilityModel.StatmentFile");
            //}

            return View(p);
        }

        public bool DocModelValidation(Cicf7Model p)
        {
            bool AppFlag = false;
            if (p.declarationForm7.Namee == null)
            {
                AppFlag = true;
                ModelState.AddModelError("declarationForm7.Namee", "Please enter Name");
            }
            if (p.formval != "Edit")
            {
                if (p.declarationForm7.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("declarationForm7.Signature", "Please upload signature");
                }
            }
            else
            {
                if (p.declarationForm7.Signature == null && p.Signature == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("declarationForm7.Signature", "Please upload file");
                }
            }
            if (p.declarationForm7.TitleDesignation == null)
            {
                AppFlag = true;
                ModelState.AddModelError("declarationForm7.TitleDesignation", "Please enter title");
            }

            if (p.declarationForm7.TermsAndConditions == false)
            {
                AppFlag = true;
                ModelState.AddModelError("declarationForm7.TermsAndConditions", "Please accept Terms and conditions");
            }

            return AppFlag;
        }
        public bool AppModelValidations(Cicf7Model p)
        {

            bool AppFlag = false;

            if (p.formval != "Edit")
            {
                if (p.App.AssociationCertificateAttachment == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.AssociationCertificateAttachment", "Please upload file");
                }
            }
            else
            {
                if (p.App.AssociationCertificateName == null && p.App.AssociationCertificateAttachment == null)
                {
                    AppFlag = true;
                    ModelState.AddModelError("App.AssociationCertificateAttachment", "Please upload file");
                }
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

        public bool BusinessModelvalidations1(Cicf7Model p)
        {
            int tempPercentage = 0;
            bool DFlag = false;

            if (p.formval != "Edit")
            {
                if (p.businessModel.Signature == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("businessModel.Signature", "Please upload file");
                }
            }
            else
            {
                if (p.Signature3 == null && p.businessModel.Signature == null)
                {
                    DFlag = true;
                    ModelState.AddModelError("businessModel.Signature", "Please upload file");
                }
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
        public bool BusinessModelvalidations(Cicf7Model p)
        {
            int tempPercentage = 0;
            bool DFlag = false;

            if (p.businessModel.Signature == null)
            {
                DFlag = true;
                ModelState.AddModelError("businessModel.Signature", "Please upload file");
            }
            if (p.Sharelist.Count >= 1)
            {
                for (int i = 0; i < p.Sharelist.Count; i++)
                {
                    DFlag = cv.IsAnyNullOrEmpty(p.Sharelist[i]);

                    if (p.Sharelist[i].Nationnality == "Select Nationnality")
                    {
                        DFlag = true;
                        ModelState.AddModelError("Sharelist[" + i + "].Nationnality", "Please Select Nationnality!");
                    }

                    if (p.Sharelist[i].Nationnality == "Swazi")
                    {
                        if (validateIdNo(p.Sharelist[i]))
                        {
                            DFlag = true;
                            ModelState.AddModelError("Sharelist[" + i + "].IdNO", "Invalid Id number!");
                        }
                    }

                    if (ModelState["Sharelist[" + i + "].IdNO"].Errors.Any())
                    {
                        DFlag = true;
                    }

                    if (ModelState["Sharelist[" + i + "].SharePercent"].Errors.Any())
                    {
                        DFlag = true;
                        ModelState.AddModelError("Sharelist[" + i + "].SharePercent", "Please use values between 1 to 100 for % Shares!");
                    }
                    tempPercentage = tempPercentage + p.Sharelist[i].SharePercent;
                    if (tempPercentage > 100)
                    {
                        DFlag = true;
                        ModelState.AddModelError("Sharelist[" + i + "].SharePercent", "Shares can not be greater than 100%");
                    }
                }

            }

            return DFlag;
        }

        public bool FinanceModelValidations(Cicf7Model p)
        {
            bool FFlag = false;
            if (p.financialCapabilityForm7.AnnualTurnoverYear1 == 0)
            {
                FFlag = true;
                ModelState.AddModelError("financialCapabilityForm7.AnnualTurnoverYear1", "Financial Year 1 end Total Turnover can not be 0");
            }
            if (p.financialCapabilityForm7.AvailableCapital == 0)
            {
                FFlag = true;
                ModelState.AddModelError("financialCapabilityForm7.AvailableCapital", "Available Capital can not be 0");
            }

            //if (p.financialCapabilityForm7.StatmentFile == null)
            //{
            //    FFlag = true;
            //    ModelState.AddModelError("financialCapabilityForm7.StatmentFile", "Please upload file");
            //}

            return FFlag;
        }

        public bool FinanceModelValidations1(Cicf7Model p)
        {
            bool FFlag = false;
            if (p.financialCapabilityForm7.AnnualTurnoverYear1 == 0)
            {
                FFlag = true;
                ModelState.AddModelError("p.financialCapabilityForm7.AnnualTurnoverYear1", "Financial Year 1 end Total Turnover can not be 0");
            }
            //if (p.financialCapabilityForm7.AvailableCapital == 0)
            //{
            //    FFlag = true;
            //    ModelState.AddModelError("p.financialCapabilityForm7.AvailableCapital", "Available Capital can not be 0");
            //}
            ////FinancialValue

            //if (p.financialCapabilityForm7.FinancialValue == 0)
            //{
            //    FFlag = true;
            //    ModelState.AddModelError("p.financialCapabilityForm7.FinancialValue", "FinancialValue can not be 0");
            //}



            //if (p.formval != "Edit")
            //{
            //    if (p.financialCapabilityModel.StatmentFile == null)
            //    {
            //        FFlag = true;
            //        ModelState.AddModelError("financialCapabilityModel.StatmentFile", "Please upload file");
            //    }
            //}
            //else
            //{
            //    if (p.financialCapabilityModel.statementFilename == null && p.financialCapabilityModel.StatmentFile == null)
            //    {
            //        FFlag = true;
            //        ModelState.AddModelError("financialCapabilityModel.StatmentFile", "Please upload file");
            //    }
            //}


            return FFlag;
        }
        public void removeDatafromSession()
        {
            memoryCache.Remove("Form7");
            memoryCache.Remove("AssociationCertificateAttachment");
            memoryCache.Remove("Signature3");
            memoryCache.Remove("shraeCertificate");
            memoryCache.Remove("companyRegistration");
            memoryCache.Remove("TradingLicense");
            memoryCache.Remove("formj");
            memoryCache.Remove("FormC");
            memoryCache.Remove("RegistrationCertificate");
            memoryCache.Remove("CertifiedCompanyRegistration");
            memoryCache.Remove("CertifiedIdentityDocuments");
            memoryCache.Remove("RenewBusinessScopeOfwork");
            memoryCache.Remove("RenewCertificate");
            memoryCache.Remove("RenewFormJ");
            memoryCache.Remove("ReNewFormC");
            memoryCache.Remove("RenewFormBMCA");
            memoryCache.Remove("RenewFinancialRequirment");
            memoryCache.Remove("RenewIdentification");
            memoryCache.Remove("Signature");
            memoryCache.Remove("Signature2");

        }


        public IActionResult Form7Result(string result, string text)
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
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context, _userManager, _appSettingsReader, _blobStorageService);
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

        public IActionResult AddRow()
        {
            Form7Model.Sharelist = new List<DirectorshipShareDividendsSection>();
            Form7Model.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });
            return View(Form7Model);
        }

        public string Savedata(Cicf7Model model)
        {

            //string TableName = "CicForm7()";

            string response = "";
            string FormRegNo = "";
            int tempMax;
            SaveForm7Model saveModel = new SaveForm7Model();
            if (model.formval == "Edit")
            {
                saveModel.PartitionKey = model.PartitionKey;
                saveModel.RowKey = model.RowKey;
                saveModel.FormStatus = model.FormStatus;

                FormRegNo = model.RowKey;
                tempMax = model.FirmRegistrationNo;
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
                //tempMax++;

                tempMax = GenericHelper.GetRegNo(model.FirmRegistrationNo, model.formval, _azureConfig);

                //Adding new rEgistration no 
                AddNewRegistrationNo addNew = new AddNewRegistrationNo();

                addNew.PartitionKey = tempMax.ToString();
                addNew.RowKey = "Form" + tempMax.ToString(); //AK
                FormRegNo = "Form" + tempMax.ToString(); //AK

                addNew.ProjectRegistrationNo = tempMax.ToString();

                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform", JsonConvert.SerializeObject(addNew));
                saveModel.PartitionKey = model.App.AssociationName;
                saveModel.RowKey = "Form" + tempMax.ToString(); //AK
            }

            saveModel.FormRegistrationNo = tempMax;
            saveModel.FormStatus = model.FormStatus;
            saveModel.AppType = model.App.AppType;
            saveModel.AssociationName = model.App.AssociationName;
            saveModel.AuthorisedOfficerName = model.App.AuthorisedOfficerName;

            //2nd tab details
            saveModel.FormName = "Form7";
            saveModel.BusinessName = model.businessModel.BusinessName;
            saveModel.TradingStyle = model.businessModel.TradingStyle;
            saveModel.BusinessType = model.businessModel.BusinessType;
            saveModel.Other = model.businessModel.Other;
            saveModel.CompanyRegistrationDate = model.businessModel.CompanyRegistrationDate;
            saveModel.CompanyRegistrationPlace = model.businessModel.CompanyRegistrationPlace;
            saveModel.CompanyRegistrationNumber = model.businessModel.CompanyRegistrationNumber;
            saveModel.PhysicalAddress = model.businessModel.PhysicalAddress;
            saveModel.CompanyHOPhysicalAddress = model.businessModel.CompanyHOPhysicalAddress;
            saveModel.PostalAddress = model.businessModel.PostalAddress;
            saveModel.TelephoneNumber = model.businessModel.TelephoneNumber;
            saveModel.FaxNo = model.businessModel.FaxNo;
            saveModel.Email = model.businessModel.Email;
            saveModel.WorkDisciplineType = model.businessModel.WorkDisciplineType;
            saveModel.BusinessRepresentativeName = model.businessModel.BusinessRepresentativeName;
            saveModel.BusinessRepresentativePositionNumber = model.businessModel.BusinessRepresentativePositionNumber;
            saveModel.BusinessRepresentativeCellNo = model.businessModel.BusinessRepresentativeCellNo;
            saveModel.BusinessRepresentativeEmail = model.businessModel.BusinessRepresentativeEmail;
            saveModel.BusinessRepresentativeFax = model.businessModel.BusinessRepresentativeFax;
            //BusinessRepresentativeFax
            //Finiancial capability
            saveModel.AnnualTurnoverYear1 = model.financialCapabilityForm7.AnnualTurnoverYear1;
            saveModel.AnnualTurnoverYear2 = model.financialCapabilityForm7.AnnualTurnoverYear2;
            saveModel.AnnualTurnoverYear3 = model.financialCapabilityForm7.AnnualTurnoverYear3;
            //saveModel.FinancialValue = model.financialCapabilityForm7.FinancialValue;
            //saveModel.FinancialInstitutionName = model.financialCapabilityForm7.FinancialInstitutionName;
            //saveModel.AvailableCapital = model.financialCapabilityForm7.AvailableCapital;

            //Declaration section
            saveModel.Namee = model.declarationForm7.Namee;
            saveModel.Namee = model.declarationForm7.Namee;

            saveModel.TitleDesignation = model.declarationForm7.TitleDesignation;


            //technial data 
            saveModel.StateDetailsOfMaterials = model.technicalData.StateDetailsOfMaterials;
            saveModel.StateOfAttainent = model.technicalData.StateOfAttainent;
            saveModel.WitnessedTitle = model.documentsUpload.WitnessedTitle;
            saveModel.WitnessedName = model.documentsUpload.WitnessedName;
            saveModel.Reviewer = model.Reviewer.Trim();
            saveModel.CreatedBy = User.Identity.Name;
            //File Upload section for New Application
            model.ImagePath = "Form" + tempMax; //AK
            memoryCache.Set("emailto", User.Identity.Name);
            saveModel.CustNo = HttpContext.Session.GetString("CustNo");

            if (filepath != "NA")
            {
                saveModel.path = filepath;

            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    var imgPath = _appSettingsReader.Read("ImagePath");
                    saveModel.path = imgPath + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }

            }

            if (model.formval == "Edit")
            {
                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm7", JsonConvert.SerializeObject(saveModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), saveModel.PartitionKey, saveModel.RowKey);
            }
            else
            {
                saveModel.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm7", JsonConvert.SerializeObject(saveModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }

            if (response == "Created" || response == "NoContent")
            {
                // TableName = "cicform1ShareDividends()";
                DirectorshipShareDividendsSection d = new DirectorshipShareDividendsSection();
                if (model.Sharelist != null)
                {
                    for (int i = 0; i < model.Sharelist.Count; i++)
                    {
                        bool DFlag = cv.IsAnyNullOrEmpty(model.Sharelist[i]);
                        DFlag = false;
                        if (DFlag == false)
                        {
                            string Data;

                            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "cicform1ShareDividends", model.Sharelist[i].PartitionKey, model.Sharelist[i].RowKey, out Data);
                            JObject myJObject = JObject.Parse(Data);
                            int cntJson = myJObject["value"].Count();
                            if (cntJson != 0)
                            {
                                d.PartitionKey = model.Sharelist[i].PartitionKey;
                                d.RowKey = model.Sharelist[i].RowKey;
                                d.DirectorName = model.Sharelist[i].DirectorName;
                                d.Nationnality = model.Sharelist[i].Nationnality;
                                d.IdNO = model.Sharelist[i].IdNO;
                                d.Country = model.Sharelist[i].Country;
                                d.CellphoneNo = model.Sharelist[i].CellphoneNo;
                                d.SharePercent = model.Sharelist[i].SharePercent;

                                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1ShareDividends", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), d.PartitionKey, d.RowKey);
                            }
                            else
                            {

                                d.PartitionKey = model.Sharelist[i].DirectorName;
                                d.RowKey = "Form" + tempMax.ToString(); //AK
                                d.DirectorName = model.Sharelist[i].DirectorName;
                                d.Nationnality = model.Sharelist[i].Nationnality;
                                d.IdNO = model.Sharelist[i].IdNO;
                                d.Country = model.Sharelist[i].Country;
                                d.CellphoneNo = model.Sharelist[i].CellphoneNo;
                                d.SharePercent = model.Sharelist[i].SharePercent;

                                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "cicform1ShareDividends", JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                                if (model.Sharelist[i].ShareFile != null)
                                {
                                    // string tempn = "ShareFile" + i.ToString();
                                    uploadFiles(model.Sharelist[i].ShareFile, model.ImagePath, "ShareFile");
                                }

                            }

                        }


                    }
                }


                // TableName = "CicForm7ListOfPreviousProject()";
                ListOfPreviousClient listOfprevious = new ListOfPreviousClient();
                if (model.listOfPrevousClent != null)
                {
                    for (int i = 0; i < model.listOfPrevousClent.Count; i++)
                    {
                        bool DFlag = cv.IsAnyNullOrEmpty(model.listOfPrevousClent[i]);
                        DFlag = false;
                        if (DFlag == false)
                        {
                            // k.mapBankDetails(AppB, model.applicantBank[i], tempMax);
                            string Data;
                            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm7ListOfPreviousProject", model.listOfPrevousClent[i].PartitionKey, model.listOfPrevousClent[i].RowKey, out Data);

                            JObject myJObject = JObject.Parse(Data);
                            int cntJson = myJObject["value"].Count();

                            if (cntJson != 0)
                            {
                                listOfprevious.PartitionKey = model.listOfPrevousClent[i].PartitionKey;
                                listOfprevious.RowKey = model.listOfPrevousClent[i].RowKey;

                                listOfprevious.NameofClient = model.listOfPrevousClent[i].NameofClient;
                                listOfprevious.ServiceProvided = model.listOfPrevousClent[i].ServiceProvided;
                                listOfprevious.DateOrPeriod = model.listOfPrevousClent[i].DateOrPeriod;
                                listOfprevious.ContractValue = model.listOfPrevousClent[i].ContractValue;
                                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm7ListOfPreviousProject", JsonConvert.SerializeObject(listOfprevious, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), listOfprevious.PartitionKey, listOfprevious.RowKey);
                            }
                            else
                            {
                                listOfprevious.PartitionKey = model.listOfPrevousClent[i].NameofClient;
                                listOfprevious.RowKey = "Form" + tempMax.ToString(); //AK

                                listOfprevious.NameofClient = model.listOfPrevousClent[i].NameofClient;
                                listOfprevious.ServiceProvided = model.listOfPrevousClent[i].ServiceProvided;
                                listOfprevious.DateOrPeriod = model.listOfPrevousClent[i].DateOrPeriod;
                                listOfprevious.ContractValue = model.listOfPrevousClent[i].ContractValue;
                                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm7ListOfPreviousProject", JsonConvert.SerializeObject(listOfprevious, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }

                    }
                }

                //TableName = "CicForm7ApplicationBankDetails()";
                CompanyBank compnyBankDetails = new CompanyBank();
                if (model.companyBank != null)
                {
                    for (int i = 0; i < model.companyBank.Count; i++)
                    {

                        bool DFlag = cv.IsAnyNullOrEmpty(model.companyBank[i]);
                        DFlag = false;
                        if (DFlag == false)
                        {
                            //k.mapWorkDetails(workdata, model.worksCapability[i], tempMax);
                            string Data;
                            AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm7ApplicationBankDetails", model.companyBank[i].PartitionKey, model.companyBank[i].RowKey, out Data);

                            JObject myJObject = JObject.Parse(Data);
                            int cntJson = myJObject["value"].Count();


                            if (cntJson != 0)
                            {
                                compnyBankDetails.PartitionKey = model.companyBank[i].PartitionKey;
                                compnyBankDetails.RowKey = model.companyBank[i].RowKey;

                                compnyBankDetails.BankName = model.companyBank[i].BankName;
                                compnyBankDetails.BranchName = model.companyBank[i].BranchName;
                                compnyBankDetails.BranchCode = model.companyBank[i].BranchCode;
                                compnyBankDetails.AccountHoulderName = model.companyBank[i].AccountHoulderName;
                                compnyBankDetails.AccountNo = model.companyBank[i].AccountNo;
                                compnyBankDetails.AccountTYpe = model.companyBank[i].AccountTYpe;
                                compnyBankDetails.TelephoneNo = model.companyBank[i].TelephoneNo;

                                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm7ApplicationBankDetails", JsonConvert.SerializeObject(compnyBankDetails, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), compnyBankDetails.PartitionKey, compnyBankDetails.RowKey);
                            }
                            else
                            {
                                compnyBankDetails.PartitionKey = model.companyBank[i].BankName;
                                compnyBankDetails.RowKey = "Form" + tempMax.ToString(); //AK

                                compnyBankDetails.BankName = model.companyBank[i].BankName;
                                compnyBankDetails.BranchName = model.companyBank[i].BranchName;
                                compnyBankDetails.BranchCode = model.companyBank[i].BranchCode;
                                compnyBankDetails.AccountHoulderName = model.companyBank[i].AccountHoulderName;
                                compnyBankDetails.AccountNo = model.companyBank[i].AccountNo;
                                compnyBankDetails.AccountTYpe = model.companyBank[i].AccountTYpe;
                                compnyBankDetails.TelephoneNo = model.companyBank[i].TelephoneNo;

                                response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm7ApplicationBankDetails", JsonConvert.SerializeObject(compnyBankDetails, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                            }



                        }

                    }
                }


            }
            //return response;

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7", FormRegNo, out jsonData1);//Get data
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            for (int i = 0; i < cntJson2; i++)
                AddCustinERP(myJObject2, i);
            return FormRegNo;
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
        public IActionResult CicformReview()
        {
            return View();
        }
        public IActionResult Form7Resultt(string result)
        {
            ViewBag.Result = result;
            return View();
        }
        bool validateIdNo(DirectorshipShareDividendsSection p)
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

        public void loadData(Cicf7Model m)
        {

            List<tblAssociation> associationlist = new List<tblAssociation>();
            List<AssociationList> AList = new List<AssociationList>();

            //associationlist = (from association in _context.tblAssociation select association).ToList();
            associationlist = (from association in _context.tblAssociation where association.formType == "Form7" select association).ToList();
            AList.Add(new AssociationList { AssociationName = "Select association name", AssociationName1 = "Select Association Name" });
            for (int i = 0; i < associationlist.Count; i++)
            {
                AList.Add(new AssociationList { AssociationName = associationlist[i].AssociationName, AssociationName1 = associationlist[i].AssociationName });
            }

            ViewBag.ListofAssociation = AList;

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
        public IActionResult IndexFromDashboard(string rowkey)
        {
            ViewBag.type = "active";
            Cicf7Model model = new Cicf7Model();
            string c = "";

            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7", rowkey, out jsonData);

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
                string key;
                AllFileList = _blobStorageService.GetBlobList(path);
                string AppFile = null, Signature3 = null, StatmentFile = null, shraeCertificate = null, companyRegistration = null, TradingLicense = null,
                    formj = null, FormC = null, RegistrationCertificate = null, CertifiedCompanyRegistration = null, CertifiedIdentityDocuments = null, RenewBusinessScopeOfwork = null, RenewCertificate = null,
                    RenewFormJ = null, ReNewFormC = null, RenewFormBMCA = null, RenewFinancialRequirment = null, RenewIdentification = null, Signature = null,
                    Signature2 = null;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;
                        // AppFile = AllFileList[j].FileValue;
                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);


                        switch (key)
                        {
                            case "AssociationCertificateAttachment": AppFile = AllFileList[j].FileValue; break;

                            case "Signature3": Signature3 = AllFileList[j].FileValue; break;
                            case "StatmentFile": StatmentFile = AllFileList[j].FileValue; break;
                            case "shraeCertificate": shraeCertificate = AllFileList[j].FileValue; break;
                            case "companyRegistration": companyRegistration = AllFileList[j].FileValue; break;
                            case "TradingLicense": TradingLicense = AllFileList[j].FileValue; break;
                            case "formj": formj = AllFileList[j].FileValue; break;
                            case "FormC": FormC = AllFileList[j].FileValue; break;
                            case "RegistrationCertificate": RegistrationCertificate = AllFileList[j].FileValue; break;
                            case "CertifiedCompanyRegistration": CertifiedCompanyRegistration = AllFileList[j].FileValue; break;
                            case "CertifiedIdentityDocuments": CertifiedIdentityDocuments = AllFileList[j].FileValue; break;
                            case "RenewBusinessScopeOfwork": RenewBusinessScopeOfwork = AllFileList[j].FileValue; break;
                            case "RenewCertificate": RenewCertificate = AllFileList[j].FileValue; break;
                            case "RenewFormJ": RenewFormJ = AllFileList[j].FileValue; break;
                            case "ReNewFormC": ReNewFormC = AllFileList[j].FileValue; break;
                            case "RenewFormBMCA": RenewFormBMCA = AllFileList[j].FileValue; break;
                            case "RenewFinancialRequirment": RenewFinancialRequirment = AllFileList[j].FileValue; break;
                            case "RenewIdentification": RenewIdentification = AllFileList[j].FileValue; break;
                            case "Signature": Signature = AllFileList[j].FileValue; break;
                            case "Signature2": Signature2 = AllFileList[j].FileValue; break;

                        }
                    }
                }

                ApplicationType App = new ApplicationType
                {
                    AppType = (string)myJObject["value"][i]["AppType"],
                    AssociationName = (string)myJObject["value"][i]["AssociationName"],
                    AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"],
                    AssociationCertificateName = AppFile

                };

                model.App = App;
                model.ImagePath = path;
                model.path = path;

                //model.AssociationCertificateAttachment = AppFile;
                c = (string)myJObject["value"][i]["Category"];
                BusinessDetails businessModel = new BusinessDetails
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
                    WorkDisciplineType = (string)myJObject["value"][i]["WorkDisciplineType"],

                    BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"],
                    Other = (string)myJObject["value"][i]["Other"],

                };
                model.businessModel = businessModel;
                model.Signature3 = Signature3;

                FinancialCapabilityForm7 fin = new FinancialCapabilityForm7
                {
                    AnnualTurnoverYear1 = (long)myJObject["value"][i]["AnnualTurnoverYear1"],
                    AnnualTurnoverYear2 = (long)myJObject["value"][i]["AnnualTurnoverYear2"],
                    AnnualTurnoverYear3 = (long)myJObject["value"][i]["AnnualTurnoverYear3"]
                    //,
                    //FinancialValue = (long)myJObject["value"][i]["FinancialValue"],
                    //FinancialInstitutionName = (string)myJObject["value"][i]["FinancialInstitutionName"],
                    //AvailableCapital = (long)myJObject["value"][i]["AvailableCapital"],
                    // statementFilename = StatmentFile
                };

                model.financialCapabilityForm7 = fin;


                DocumentsUpload doc = new DocumentsUpload
                {

                    Name = (string)myJObject["value"][i]["Name"],
                    Title = (string)myJObject["value"][i]["Title"],
                    WitnessedName = (string)myJObject["value"][i]["WitnessedName"],
                    WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"],
                    //Signature1=
                    //Signature2



                };
                model.documentsUpload = doc;



                DeclarationForm7 dec = new DeclarationForm7
                {

                    Namee = (string)myJObject["value"][i]["Namee"],
                    TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"]
                    //Images to upload 

                };
                model.declarationForm7 = dec;
                model.companyRegistration = companyRegistration;
                model.shraeCertificate = shraeCertificate;
                model.TradingLicense = TradingLicense;
                model.formj = formj;
                model.RegistrationCertificate = RegistrationCertificate;
                model.FormC = FormC;
                model.CertifiedCompanyRegistration = CertifiedCompanyRegistration;
                model.CertifiedIdentityDocuments = CertifiedIdentityDocuments;
                model.RenewBusinessScopeOfwork = RenewBusinessScopeOfwork;
                model.RenewCertificate = RenewCertificate;
                model.RenewFormJ = RenewFormJ;
                model.ReNewFormC = ReNewFormC;
                model.RenewFormBMCA = RenewFormBMCA;
                model.RenewFinancialRequirment = RenewFinancialRequirment;
                model.RenewIdentification = RenewIdentification;
                model.Signature = Signature;
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.Signature2 = Signature2;


                TechnicalData tecData = new TechnicalData
                {
                    StateDetailsOfMaterials = (string)myJObject["value"][i]["StateDetailsOfMaterials"],
                    StateOfAttainent = (string)myJObject["value"][i]["StateOfAttainent"]

                };
                model.technicalData = tecData;



                model.declarationForm7 = dec;
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];


                model.FirmRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.ImagePath = (string)myJObject["value"][i]["path"];
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<DirectorshipShareDividendsSection> d = new List<DirectorshipShareDividendsSection>();
            for (int i = 0; i < cntJson1; i++)
            {

                d.Add(new DirectorshipShareDividendsSection
                {
                    DirectorName = (string)myJObject1["value"][i]["DirectorName"],
                    IdNO = (string)myJObject1["value"][i]["IdNO"],
                    Nationnality = (string)myJObject1["value"][i]["Nationnality"],
                    CellphoneNo = (string)myJObject1["value"][i]["CellphoneNo"],
                    Country = (string)myJObject1["value"][i]["Country"],
                    //ShareFile = 
                    SharePercent = (int)myJObject1["value"][i]["SharePercent"],
                    PartitionKey = (string)myJObject1["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject1["value"][i]["RowKey"],


                });
            }
            model.Sharelist = d;
            if (model.Sharelist.Count == 0)
            {
                model.c = 1;
                model.Sharelist = new List<DirectorshipShareDividendsSection>();
                model.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });

            }
            else
            {
                model.c = model.Sharelist.Count;
            }
            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7ListOfPreviousProject", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<ListOfPreviousClient> a = new List<ListOfPreviousClient>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new ListOfPreviousClient
                {

                    NameofClient = (string)myJObject2["value"][i]["NameofClient"],
                    ServiceProvided = (string)myJObject2["value"][i]["ServiceProvided"],
                    DateOrPeriod = (DateTime)myJObject2["value"][i]["DateOrPeriod"],
                    ContractValue = (decimal)myJObject2["value"][i]["ContractValue"],
                    PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject2["value"][i]["RowKey"]

                });
            }
            model.listOfPrevousClent = a;

            if (model.listOfPrevousClent.Count == 0)
            {
                //  model.cFin = 1;
                model.cWork = 1;
                Form7Model.listOfPrevousClent = new List<ListOfPreviousClient>();
                Form7Model.listOfPrevousClent.Add(new ListOfPreviousClient { NameofClient = "", ServiceProvided = "", DateOrPeriod = null, ContractValue = 0, PartitionKey = "-", RowKey = "-" });
            }
            else
            {
                // model.cFin = model.listOfPrevousClent.Count;
                model.cWork = model.listOfPrevousClent.Count; ;
            }
            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7ApplicationBankDetails", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<CompanyBank> w = new List<CompanyBank>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new CompanyBank
                {
                    AccountHoulderName = (string)myJObject3["value"][i]["AccountHoulderName"],
                    AccountNo = (int)myJObject3["value"][i]["AccountNo"],
                    AccountTYpe = (string)myJObject3["value"][i]["AccountTYpe"],
                    BankName = (string)myJObject3["value"][i]["BankName"],
                    BranchCode = (string)myJObject3["value"][i]["BranchCode"],
                    BranchName = (string)myJObject3["value"][i]["BranchName"],
                    TelephoneNo = (string)myJObject3["value"][i]["TelephoneNo"],
                    PartitionKey = (string)myJObject3["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject3["value"][i]["RowKey"]

                });
            }
            model.companyBank = w;
            if (model.companyBank.Count == 0)
            {
                //model.cWork = 1;
                model.cFin = 1;
                Form7Model.companyBank = new List<CompanyBank>();
                Form7Model.companyBank.Add(new CompanyBank { BankName = "", BranchName = "", BranchCode = "", AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });

            }
            else
            {
                //model.cWork = model.companyBank.Count;
                model.cFin = model.companyBank.Count;

            }

            loadData(model);
            model.formval = "Edit";

            List<AssociationList> AList = new List<AssociationList>();
            AList = ViewBag.ListofAssociation;



            memoryCache.Set("listAssociation", AList);
            memoryCache.Set("Form7", model);

            return RedirectToAction("CicForm7", "Cicform7");
        }

        public IActionResult Getdata(string apptype)
        {
            ViewBag.type = "active";
            Cicf7Model model = new Cicf7Model();
            string c = "";
            string jsonData;
            AzureTablesData.GetEntitybyLoginId(StorageName, StorageKey, "CicForm7", User.Identity.Name, out jsonData);

            JObject myJObject = JObject.Parse(jsonData);
            var latestRecord = (from rec in myJObject["value"]
                                orderby (int)rec["FormRegistrationNo"] descending
                                select rec).FirstOrDefault();

            if (latestRecord != null)
            {
                model.RowKey = (string)latestRecord["RowKey"];
                model.CustNo = (string)latestRecord["CustNo"];
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
                ApplicationType App = new ApplicationType
                {
                    AppType = apptype,
                    AssociationName = (string)latestRecord["AssociationName"],
                    AuthorisedOfficerName = (string)latestRecord["AuthorisedOfficerName"],
                };

                model.App = App;

                c = (string)latestRecord["Category"];
                BusinessDetails businessModel = new BusinessDetails
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
                    WorkDisciplineType = (string)latestRecord["WorkDisciplineType"],
                    BusinessRepresentativeName = (string)latestRecord["BusinessRepresentativeName"],
                    BusinessRepresentativePositionNumber = (string)latestRecord["BusinessRepresentativePositionNumber"],
                    BusinessRepresentativeCellNo = (string)latestRecord["BusinessRepresentativeCellNo"],
                    BusinessRepresentativeFax = (string)latestRecord["BusinessRepresentativeFax"],
                    BusinessRepresentativeEmail = (string)latestRecord["BusinessRepresentativeEmail"],
                    Other = (string)latestRecord["Other"],

                };
                model.businessModel = businessModel;

                FinancialCapabilityForm7 fin = new FinancialCapabilityForm7
                {
                    AnnualTurnoverYear1 = (long)latestRecord["AnnualTurnoverYear1"],
                    AnnualTurnoverYear2 = (long)latestRecord["AnnualTurnoverYear2"],
                    AnnualTurnoverYear3 = (long)latestRecord["AnnualTurnoverYear3"]
                };

                model.financialCapabilityForm7 = fin;

                DocumentsUpload doc = new DocumentsUpload
                {

                    Name = (string)latestRecord["Name"],
                    Title = (string)latestRecord["Title"],
                    WitnessedName = (string)latestRecord["WitnessedName"],
                    WitnessedTitle = (string)latestRecord["WitnessedTitle"]
                };
                model.documentsUpload = doc;

                DeclarationForm7 dec = new DeclarationForm7
                {

                    Namee = (string)latestRecord["Namee"],
                    TitleDesignation = (string)latestRecord["TitleDesignation"]

                };
                model.declarationForm7 = dec;

                TechnicalData tecData = new TechnicalData
                {
                    StateDetailsOfMaterials = (string)latestRecord["StateDetailsOfMaterials"],
                    StateOfAttainent = (string)latestRecord["StateOfAttainent"]

                };
                model.technicalData = tecData;

                string jsonData1;
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1ShareDividends", model.RowKey, out jsonData1);
                JObject myJObject1 = JObject.Parse(jsonData1);
                int cntJson1 = myJObject1["value"].Count();
                List<DirectorshipShareDividendsSection> d = new List<DirectorshipShareDividendsSection>();
                for (int i = 0; i < cntJson1; i++)
                {

                    d.Add(new DirectorshipShareDividendsSection
                    {
                        DirectorName = (string)myJObject1["value"][i]["DirectorName"],
                        IdNO = (string)myJObject1["value"][i]["IdNO"],
                        Nationnality = (string)myJObject1["value"][i]["Nationnality"],
                        CellphoneNo = (string)myJObject1["value"][i]["CellphoneNo"],
                        Country = (string)myJObject1["value"][i]["Country"],
                        //ShareFile = 
                        SharePercent = (int)myJObject1["value"][i]["SharePercent"],
                        PartitionKey = "-",
                        RowKey = "-",


                    });
                }
                model.Sharelist = d;
                if (model.Sharelist.Count == 0)
                {
                    model.c = 1;
                    model.Sharelist = new List<DirectorshipShareDividendsSection>();
                    model.Sharelist.Add(new DirectorshipShareDividendsSection { DirectorName = "", CellphoneNo = "", Country = "", IdNO = "", Nationnality = "", SharePercent = 0, PartitionKey = "-", RowKey = "-" });

                }
                else
                {
                    model.c = model.Sharelist.Count;
                }


                string jsonData2;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7ListOfPreviousProject", model.RowKey, out jsonData2);
                JObject myJObject2 = JObject.Parse(jsonData2);
                int cntJson2 = myJObject2["value"].Count();
                List<ListOfPreviousClient> a = new List<ListOfPreviousClient>();
                for (int i = 0; i < cntJson2; i++)
                {

                    a.Add(new ListOfPreviousClient
                    {

                        NameofClient = (string)myJObject2["value"][i]["NameofClient"],
                        ServiceProvided = (string)myJObject2["value"][i]["ServiceProvided"],
                        DateOrPeriod = (DateTime)myJObject2["value"][i]["DateOrPeriod"],
                        ContractValue = (decimal)myJObject2["value"][i]["ContractValue"],
                        PartitionKey = "-",
                        RowKey = "-"

                    });
                }
                model.listOfPrevousClent = a;

                if (model.listOfPrevousClent.Count == 0)
                {
                    model.cWork = 1;
                    Form7Model.listOfPrevousClent = new List<ListOfPreviousClient>();
                    Form7Model.listOfPrevousClent.Add(new ListOfPreviousClient { NameofClient = "", ServiceProvided = "", DateOrPeriod = null, ContractValue = 0, PartitionKey = "-", RowKey = "-" });
                }
                else
                {
                    model.cWork = model.listOfPrevousClent.Count; ;
                }
                string jsonData3;
                AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm7ApplicationBankDetails", model.RowKey, out jsonData3);
                JObject myJObject3 = JObject.Parse(jsonData3);
                int cntJson3 = myJObject3["value"].Count();
                List<CompanyBank> w = new List<CompanyBank>();
                for (int i = 0; i < cntJson3; i++)
                {

                    w.Add(new CompanyBank
                    {
                        AccountHoulderName = (string)myJObject3["value"][i]["AccountHoulderName"],
                        AccountNo = (int)myJObject3["value"][i]["AccountNo"],
                        AccountTYpe = (string)myJObject3["value"][i]["AccountTYpe"],
                        BankName = (string)myJObject3["value"][i]["BankName"],
                        BranchCode = (string)myJObject3["value"][i]["BranchCode"],
                        BranchName = (string)myJObject3["value"][i]["BranchName"],
                        TelephoneNo = (string)myJObject3["value"][i]["TelephoneNo"],
                        PartitionKey = "-",
                        RowKey = "-"

                    });
                }
                model.companyBank = w;
                if (model.companyBank.Count == 0)
                {
                    model.cFin = 1;
                    Form7Model.companyBank = new List<CompanyBank>();
                    Form7Model.companyBank.Add(new CompanyBank { BankName = "", BranchName = "", BranchCode = "", AccountHoulderName = "", AccountNo = 0, AccountTYpe = "", TelephoneNo = "", PartitionKey = "-", RowKey = "-" });

                }
                else
                {
                    model.cFin = model.companyBank.Count;

                }

                loadData(model);

                List<AssociationList> AList = new List<AssociationList>();
                AList = ViewBag.ListofAssociation;
                memoryCache.Set("listAssociation", AList);
                memoryCache.Set("Form7", model);



            }

            return RedirectToAction("CicForm7", "Cicform7");
        }
        public Cicf7Model SetandGetFileAdd(Cicf7Model p)
        {
            IFormFile fsign;
            string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("AssociationCertificateAttachment", out fsign);

            if (!isExist)
            {
                if (p.App.AssociationCertificateAttachment != null)
                {
                    memoryCache.Set("AssociationCertificateAttachment", p.App.AssociationCertificateAttachment);
                    p.App.AssociationCertificateName = p.App.AssociationCertificateAttachment.FileName;
                }
            }
            else
            {
                if (p.App.AssociationCertificateAttachment != null)
                {
                    memoryCache.Set("AssociationCertificateAttachment", p.App.AssociationCertificateAttachment);
                }
                else
                {
                    p.App.AssociationCertificateAttachment = fsign;
                }
                p.App.AssociationCertificateName = p.App.AssociationCertificateAttachment.FileName;

            }
            isExist = memoryCache.TryGetValue("Signature3", out fsign);

            if (!isExist)
            {
                if (p.businessModel.Signature != null)
                {
                    memoryCache.Set("Signature3", p.businessModel.Signature);
                    p.Signature3 = p.businessModel.Signature.FileName;
                }
            }
            else
            {
                if (p.businessModel.Signature != null)
                {
                    memoryCache.Set("Signature3", p.businessModel.Signature);
                }
                else
                {
                    p.businessModel.Signature = fsign;
                }
                p.Signature3 = p.businessModel.Signature.FileName;

            }
            //Fin capability 
            //isExist = memoryCache.TryGetValue("StatmentFile", out fsign);
            //if (!isExist)
            //{
            //    if (p.financialCapabilityForm7.StatmentFile != null)
            //    {
            //        memoryCache.Set("StatmentFile", p.financialCapabilityForm7.StatmentFile);
            //        p.financialCapabilityForm7.statementFilename = p.financialCapabilityForm7.StatmentFile.FileName;
            //    }
            //}
            //else
            //{
            //    if (p.financialCapabilityForm7.StatmentFile != null)
            //    {
            //        memoryCache.Set("StatmentFile", p.financialCapabilityForm7.StatmentFile);
            //    }
            //    else
            //    {
            //        p.financialCapabilityForm7.StatmentFile = fsign;
            //    }
            //    p.financialCapabilityForm7.statementFilename = p.financialCapabilityForm7.StatmentFile.FileName;

            //}

            isExist = memoryCache.TryGetValue("companyRegistration", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.companyRegistration != null)
                {
                    memoryCache.Set("companyRegistration", p.declarationForm7.companyRegistration);
                    p.companyRegistration = p.declarationForm7.companyRegistration.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.companyRegistration != null)
                {
                    memoryCache.Set("companyRegistration", p.declarationForm7.companyRegistration);
                }
                else
                {
                    p.declarationForm7.companyRegistration = fsign;
                }
                p.companyRegistration = p.declarationForm7.companyRegistration.FileName;

            }

            //shere 
            isExist = memoryCache.TryGetValue("shraeCertificate", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.shraeCertificate != null)
                {
                    memoryCache.Set("shraeCertificate", p.declarationForm7.shraeCertificate);
                    p.shraeCertificate = p.declarationForm7.shraeCertificate.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.shraeCertificate != null)
                {
                    memoryCache.Set("shraeCertificate", p.declarationForm7.shraeCertificate);
                }
                else
                {
                    p.declarationForm7.shraeCertificate = fsign;
                }
                p.shraeCertificate = p.declarationForm7.shraeCertificate.FileName;

            }


            isExist = memoryCache.TryGetValue("TradingLicense", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.TradingLicense != null)
                {
                    memoryCache.Set("TradingLicense", p.declarationForm7.TradingLicense);
                    p.TradingLicense = p.declarationForm7.TradingLicense.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.TradingLicense != null)
                {
                    memoryCache.Set("TradingLicense", p.declarationForm7.TradingLicense);
                }
                else
                {
                    p.declarationForm7.TradingLicense = fsign;
                }
                p.TradingLicense = p.declarationForm7.TradingLicense.FileName;

            }
            isExist = memoryCache.TryGetValue("formj", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.formj != null)
                {
                    memoryCache.Set("formj", p.declarationForm7.formj);
                    p.formj = p.declarationForm7.formj.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.formj != null)
                {
                    memoryCache.Set("formj", p.declarationForm7.formj);
                }
                else
                {
                    p.declarationForm7.formj = fsign;
                }
                p.formj = p.declarationForm7.formj.FileName;

            }

            isExist = memoryCache.TryGetValue("FormC", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.FormC != null)
                {
                    memoryCache.Set("FormC", p.declarationForm7.FormC);
                    p.FormC = p.declarationForm7.FormC.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.FormC != null)
                {
                    memoryCache.Set("FormC", p.declarationForm7.FormC);
                }
                else
                {
                    p.declarationForm7.FormC = fsign;
                }
                p.FormC = p.declarationForm7.FormC.FileName;

            }

            isExist = memoryCache.TryGetValue("RegistrationCertificate", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.RegistrationCertificate != null)
                {
                    memoryCache.Set("RegistrationCertificate", p.declarationForm7.RegistrationCertificate);
                    p.RegistrationCertificate = p.declarationForm7.RegistrationCertificate.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RegistrationCertificate != null)
                {
                    memoryCache.Set("RegistrationCertificate", p.declarationForm7.RegistrationCertificate);
                }
                else
                {
                    p.declarationForm7.RegistrationCertificate = fsign;
                }
                p.RegistrationCertificate = p.declarationForm7.RegistrationCertificate.FileName;

            }

            isExist = memoryCache.TryGetValue("CertifiedCompanyRegistration", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.CertifiedCompanyRegistration != null)
                {
                    memoryCache.Set("CertifiedCompanyRegistration", p.declarationForm7.CertifiedCompanyRegistration);
                    p.CertifiedCompanyRegistration = p.declarationForm7.CertifiedCompanyRegistration.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.CertifiedCompanyRegistration != null)
                {
                    memoryCache.Set("CertifiedCompanyRegistration", p.declarationForm7.CertifiedCompanyRegistration);
                }
                else
                {
                    p.declarationForm7.CertifiedCompanyRegistration = fsign;
                }
                p.CertifiedCompanyRegistration = p.declarationForm7.CertifiedCompanyRegistration.FileName;

            }
            isExist = memoryCache.TryGetValue("CertifiedIdentityDocuments", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.CertifiedIdentityDocuments != null)
                {
                    memoryCache.Set("CertifiedIdentityDocuments", p.declarationForm7.CertifiedIdentityDocuments);
                    p.CertifiedIdentityDocuments = p.declarationForm7.CertifiedIdentityDocuments.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.CertifiedIdentityDocuments != null)
                {
                    memoryCache.Set("CertifiedIdentityDocuments", p.declarationForm7.CertifiedIdentityDocuments);
                }
                else
                {
                    p.declarationForm7.CertifiedIdentityDocuments = fsign;
                }
                p.CertifiedIdentityDocuments = p.declarationForm7.CertifiedIdentityDocuments.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewBusinessScopeOfwork", out fsign);

            if (!isExist)
            {
                if (p.declarationForm7.RenewBusinessScopeOfwork != null)
                {
                    memoryCache.Set("RenewBusinessScopeOfwork", p.declarationForm7.RenewBusinessScopeOfwork);
                    p.RenewBusinessScopeOfwork = p.declarationForm7.RenewBusinessScopeOfwork.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewBusinessScopeOfwork != null)
                {
                    memoryCache.Set("RenewBusinessScopeOfwork", p.declarationForm7.RenewBusinessScopeOfwork);
                }
                else
                {
                    p.declarationForm7.RenewBusinessScopeOfwork = fsign;
                }
                p.RenewBusinessScopeOfwork = p.declarationForm7.RenewBusinessScopeOfwork.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewCertificate", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.RenewCertificate != null)
                {
                    memoryCache.Set("RenewCertificate", p.declarationForm7.RenewCertificate);
                    p.RenewCertificate = p.declarationForm7.RenewCertificate.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewCertificate != null)
                {
                    memoryCache.Set("RenewCertificate", p.declarationForm7.RenewCertificate);
                }
                else
                {
                    p.declarationForm7.RenewCertificate = fsign;
                }
                p.RenewCertificate = p.declarationForm7.RenewCertificate.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewFormJ", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.RenewFormJ != null)
                {
                    memoryCache.Set("RenewFormJ", p.declarationForm7.RenewFormJ);
                    p.RenewFormJ = p.declarationForm7.RenewFormJ.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewFormJ != null)
                {
                    memoryCache.Set("RenewFormJ", p.declarationForm7.RenewFormJ);
                }
                else
                {
                    p.declarationForm7.RenewFormJ = fsign;
                }
                p.RenewFormJ = p.declarationForm7.RenewFormJ.FileName;

            }
            isExist = memoryCache.TryGetValue("ReNewFormC", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.ReNewFormC != null)
                {
                    memoryCache.Set("ReNewFormC", p.declarationForm7.ReNewFormC);
                    p.ReNewFormC = p.declarationForm7.ReNewFormC.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.ReNewFormC != null)
                {
                    memoryCache.Set("ReNewFormC", p.declarationForm7.ReNewFormC);
                }
                else
                {
                    p.declarationForm7.ReNewFormC = fsign;
                }
                p.ReNewFormC = p.declarationForm7.ReNewFormC.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewFormBMCA", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.RenewFormBMCA != null)
                {
                    memoryCache.Set("RenewFormBMCA", p.declarationForm7.RenewFormBMCA);
                    p.RenewFormBMCA = p.declarationForm7.RenewFormBMCA.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewFormBMCA != null)
                {
                    memoryCache.Set("RenewFormBMCA", p.declarationForm7.RenewFormBMCA);
                }
                else
                {
                    p.declarationForm7.RenewFormBMCA = fsign;
                }
                p.RenewFormBMCA = p.declarationForm7.RenewFormBMCA.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewFinancialRequirment", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.RenewFinancialRequirment != null)
                {
                    memoryCache.Set("RenewFinancialRequirment", p.declarationForm7.RenewFinancialRequirment);
                    p.RenewFinancialRequirment = p.declarationForm7.RenewFinancialRequirment.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewFinancialRequirment != null)
                {
                    memoryCache.Set("RenewFinancialRequirment", p.declarationForm7.RenewFinancialRequirment);
                }
                else
                {
                    p.declarationForm7.RenewFinancialRequirment = fsign;
                }
                p.RenewFinancialRequirment = p.declarationForm7.RenewFinancialRequirment.FileName;

            }
            isExist = memoryCache.TryGetValue("RenewIdentification", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.RenewIdentification != null)
                {
                    memoryCache.Set("RenewIdentification", p.declarationForm7.RenewIdentification);
                    p.RenewIdentification = p.declarationForm7.RenewIdentification.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.RenewIdentification != null)
                {
                    memoryCache.Set("RenewIdentification", p.declarationForm7.RenewIdentification);
                }
                else
                {
                    p.declarationForm7.RenewIdentification = fsign;
                }
                p.RenewIdentification = p.declarationForm7.RenewIdentification.FileName;

            }
            isExist = memoryCache.TryGetValue("Signature", out fsign);
            if (!isExist)
            {
                if (p.declarationForm7.Signature != null)
                {
                    memoryCache.Set("Signature", p.declarationForm7.Signature);
                    p.Signature = p.declarationForm7.Signature.FileName;
                }
            }
            else
            {
                if (p.declarationForm7.Signature != null)
                {
                    memoryCache.Set("Signature", p.declarationForm7.Signature);
                }
                else
                {
                    p.declarationForm7.Signature = fsign;
                }
                p.Signature = p.declarationForm7.Signature.FileName;

            }
            isExist = memoryCache.TryGetValue("Signature2", out fsign);
            if (!isExist)
            {
                if (p.documentsUpload.Signature2 != null)
                {
                    memoryCache.Set("Signature2", p.documentsUpload.Signature2);
                    p.Signature2 = p.documentsUpload.Signature2.FileName;
                }
            }
            else
            {
                if (p.documentsUpload.Signature2 != null)
                {
                    memoryCache.Set("Signature2", p.documentsUpload.Signature2);
                }
                else
                {
                    p.documentsUpload.Signature2 = fsign;
                }
                p.Signature2 = p.documentsUpload.Signature2.FileName;

            }
            //isExist = memoryCache.TryGetValue("FinaiancialCapital", out fsign);
            //if (!isExist)
            //{
            //    if (p.declarationForm7.fi != null)
            //    {
            //        memoryCache.Set("Signature2", p.documentsUpload.Signature2);
            //        p.Signature2 = p.documentsUpload.Signature2.FileName;
            //    }
            //}
            //else
            //{
            //    if (p.documentsUpload.Signature2 != null)
            //    {
            //        memoryCache.Set("Signature2", p.documentsUpload.Signature2);
            //    }
            //    else
            //    {
            //        p.documentsUpload.Signature2 = fsign;
            //    }
            //    p.Signature2 = p.documentsUpload.Signature2.FileName;

            //}
            return p;
        }


        public void UploadBlob(Cicf7Model model, int tempMax)
        {
            if (model.ImagePath == "NA" || model.ImagePath == "-")
            {
                model.ImagePath = "Form" + tempMax; //AK
            }

            uploadFiles1(model.App.AssociationCertificateAttachment, model.ImagePath, "AssociationCertificateAttachment");
            uploadFiles1(model.businessModel.Signature, model.ImagePath, "Signature3");
            uploadFiles1(model.financialCapabilityForm7.StatmentFile, model.ImagePath, "StatmentFile");

            uploadFiles1(model.declarationForm7.companyRegistration, model.ImagePath, "companyRegistration");
            uploadFiles1(model.declarationForm7.shraeCertificate, model.ImagePath, "shraeCertificate");
            uploadFiles1(model.declarationForm7.TradingLicense, model.ImagePath, "TradingLicense");
            uploadFiles1(model.declarationForm7.formj, model.ImagePath, "formj");
            uploadFiles1(model.declarationForm7.FormC, model.ImagePath, "FormC");
            uploadFiles1(model.declarationForm7.RegistrationCertificate, model.ImagePath, "RegistrationCertificate");
            uploadFiles1(model.declarationForm7.CertifiedCompanyRegistration, model.ImagePath, "CertifiedCompanyRegistration");
            uploadFiles1(model.declarationForm7.CertifiedIdentityDocuments, model.ImagePath, "CertifiedIdentityDocuments");
            uploadFiles1(model.declarationForm7.RenewBusinessScopeOfwork, model.ImagePath, "RenewBusinessScopeOfwork");
            uploadFiles1(model.declarationForm7.RenewCertificate, model.ImagePath, "RenewCertificate");
            uploadFiles1(model.declarationForm7.RenewFormJ, model.ImagePath, "RenewFormJ");
            uploadFiles1(model.declarationForm7.ReNewFormC, model.ImagePath, "ReNewFormC");
            uploadFiles1(model.declarationForm7.RenewFormBMCA, model.ImagePath, "RenewFormBMCA");
            uploadFiles1(model.declarationForm7.RenewFinancialRequirment, model.ImagePath, "RenewFinancialRequirment");
            uploadFiles1(model.declarationForm7.RenewIdentification, model.ImagePath, "RenewIdentification");
            uploadFiles1(model.declarationForm7.Signature, model.ImagePath, "Signature");
            uploadFiles1(model.documentsUpload.Signature2, model.ImagePath, "Signature2");

            if (model.Sharelist != null)
            {
                for (int i = 0; i < model.Sharelist.Count; i++)
                {
                    string tempn = "ShareFile" + i.ToString();
                    uploadFiles1(model.Sharelist[i].ShareFile, model.ImagePath, tempn);
                }
            }

        }
        public int getRegNo(Cicf7Model p)
        {
            int tempMax = 0;

            if (p.formval == "Edit")
            {
                tempMax = p.FirmRegistrationNo;
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
        public void fileDefault(Cicf7Model p)
        {
            p.App.AssociationCertificateName = (p.App.AssociationCertificateName != "") ? p.App.AssociationCertificateName : "-";
            p.Signature3 = (p.Signature3 != "") ? p.Signature3 : "-";
            //p.financialCapabilityModel.statementFilename = (p.financialCapabilityModel.statementFilename != "") ? p.financialCapabilityModel.statementFilename : "-";
            p.financialCapabilityForm7.statementFilename = (p.financialCapabilityForm7.statementFilename != "") ? p.financialCapabilityForm7.statementFilename : "-";
            p.companyRegistration = (p.companyRegistration != "") ? p.companyRegistration : "-";

            p.shraeCertificate = (p.shraeCertificate != "") ? p.shraeCertificate : "-";
            p.TradingLicense = (p.TradingLicense != "") ? p.TradingLicense : "-";
            p.formj = (p.formj != "") ? p.formj : "-";
            p.FormC = (p.FormC != "") ? p.FormC : "-";
            p.RegistrationCertificate = (p.RegistrationCertificate != "") ? p.RegistrationCertificate : "-";
            p.CertifiedCompanyRegistration = (p.CertifiedCompanyRegistration != "") ? p.CertifiedCompanyRegistration : "-";
            p.CertifiedIdentityDocuments = (p.CertifiedIdentityDocuments != "") ? p.CertifiedIdentityDocuments : "-";
            p.RenewBusinessScopeOfwork = (p.RenewBusinessScopeOfwork != "") ? p.RenewBusinessScopeOfwork : "-";
            p.RenewCertificate = (p.RenewCertificate != "") ? p.RenewCertificate : "-";
            p.RenewFormJ = (p.RenewFormJ != "") ? p.RenewFormJ : "-";
            p.ReNewFormC = (p.ReNewFormC != "") ? p.ReNewFormC : "-";
            p.RenewFormBMCA = (p.RenewFormBMCA != "") ? p.RenewFormBMCA : "-";
            p.RenewFinancialRequirment = (p.RenewFinancialRequirment != "") ? p.RenewFinancialRequirment : "-";

            p.RenewIdentification = (p.RenewIdentification != "") ? p.RenewIdentification : "-";
            p.Signature = (p.Signature != "") ? p.Signature : "-";
            p.Signature2 = (p.Signature2 != "") ? p.Signature2 : "-";
            //condition for null 

            p.App.AssociationCertificateName = (p.App.AssociationCertificateName != null) ? p.App.AssociationCertificateName : "-";
            p.Signature3 = (p.Signature3 != null) ? p.Signature3 : "-";
            p.financialCapabilityForm7.statementFilename = (p.financialCapabilityForm7.statementFilename != "") ? p.financialCapabilityForm7.statementFilename : "-";
            p.companyRegistration = (p.companyRegistration != null) ? p.companyRegistration : "-";
            p.shraeCertificate = (p.shraeCertificate != null) ? p.shraeCertificate : "-";
            p.TradingLicense = (p.TradingLicense != null) ? p.TradingLicense : "-";
            p.formj = (p.formj != null) ? p.formj : "-";
            p.FormC = (p.FormC != null) ? p.FormC : "-";
            p.RegistrationCertificate = (p.RegistrationCertificate != null) ? p.RegistrationCertificate : "-";
            p.CertifiedCompanyRegistration = (p.CertifiedCompanyRegistration != null) ? p.CertifiedCompanyRegistration : "-";
            p.CertifiedIdentityDocuments = (p.CertifiedIdentityDocuments != null) ? p.CertifiedIdentityDocuments : "-";
            p.RenewBusinessScopeOfwork = (p.RenewBusinessScopeOfwork != null) ? p.RenewBusinessScopeOfwork : "-";
            p.RenewCertificate = (p.RenewCertificate != null) ? p.RenewCertificate : "-";
            p.RenewFormJ = (p.RenewFormJ != null) ? p.RenewFormJ : "-";
            p.ReNewFormC = (p.ReNewFormC != null) ? p.ReNewFormC : "-";
            p.RenewFormBMCA = (p.RenewFormBMCA != null) ? p.RenewFormBMCA : "-";
            p.RenewFinancialRequirment = (p.RenewFinancialRequirment != null) ? p.RenewFinancialRequirment : "-";

            p.RenewIdentification = (p.RenewIdentification != null) ? p.RenewIdentification : "-";
            p.Signature = (p.Signature != null) ? p.Signature : "-";
            p.Signature2 = (p.Signature2 != null) ? p.Signature2 : "-";
        }
    }
}
