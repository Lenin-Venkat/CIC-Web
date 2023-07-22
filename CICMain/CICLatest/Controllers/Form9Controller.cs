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
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Azure.Core;

namespace CICLatest.Controllers
{
    public class Form9Controller : Controller
    {      
        static string filepath = "NA";
        int cnt = 1;

        Form9ViewModel form9Model = new Form9ViewModel();
        private readonly UserManager<UserModel> _userManager;
        static string StorageName = "";
        static string StorageKey = "";
        //Adding for updates
        private readonly ApplicationContext _context;
        private readonly AzureStorageConfiguration _azureConfig;
        static string path = "";
        BlobStorageService b = new BlobStorageService();
        private readonly IMemoryCache memoryCache;
        public string accessToken = "";


        public Form9Controller(ApplicationContext context, AzureStorageConfiguration azureConfig, IMemoryCache memoryCache, UserManager<UserModel> userManager)
        {
            _context = context;
            _azureConfig = azureConfig;
            this.memoryCache = memoryCache;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _userManager = userManager;
        }

        public IActionResult CicForm9()
        {

            try
            {

                Form9ViewModel form1EditModel = new Form9ViewModel();

                form9Model.FirstGrid = cnt;
                form9Model.SecondGrid = cnt;
                form9Model.ThirdGrid = cnt;
                bool isExist = memoryCache.TryGetValue("Form9", out form1EditModel);
                if (isExist)
                {

                    //if (form1EditModel.buildingWorkForProject.Count == 0)
                    //{
                    //    form1EditModel.FirstGrid = cnt;
                    //    form1EditModel.buildingWorkForProject = new List<BuildingWorkForProject>();
                    //    form1EditModel.buildingWorkForProject.Add(new BuildingWorkForProject { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    //}
                    //if (form1EditModel.civilsWorksProjects.Count == 0)
                    //{
                    //    form1EditModel.SecondGrid = cnt;
                    //    form1EditModel.civilsWorksProjects = new List<CivilsWorksProjects>();
                    //    form1EditModel.civilsWorksProjects.Add(new CivilsWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    //}
                    //if (form1EditModel.mechanicalWorksProjects.Count == 0)
                    //{
                    //    form1EditModel.ThirdGrid = cnt;
                    //    form1EditModel.mechanicalWorksProjects = new List<MechanicalWorksProjects>();
                    //    form1EditModel.mechanicalWorksProjects.Add(new MechanicalWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    //}
                    form9Model = form1EditModel;
                }
                if (!isExist)
                {
                    form9Model.buildingWorkForProject = new List<BuildingWorkForProject>();
                    form9Model.buildingWorkForProject.Add(new BuildingWorkForProject { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    form9Model.civilsWorksProjects = new List<CivilsWorksProjects>();
                    form9Model.civilsWorksProjects.Add(new CivilsWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    form9Model.mechanicalWorksProjects = new List<MechanicalWorksProjects>();
                    form9Model.mechanicalWorksProjects.Add(new MechanicalWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });

                    form9Model.App = new Form9View();

                    form9Model.App.SignatureImage = form9Model.App.SignatureImage;
                    form9Model.App.PurchaseordersFilee = form9Model.App.PurchaseordersFilee;
                    form9Model.App.InvoicesFilee = form9Model.App.PurchaseordersFilee;
                    form9Model.App.SummarybillofquantitiesFilee = form9Model.App.PurchaseordersFilee;

                }

            }

            catch (Exception ex)
            {

                ex.Message.ToString();
            }
            return View(form9Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CicForm9(Form9ViewModel p, string name)
        {
            try
            {

                if (p.formval == "Edit")
                {

                    setGetFileEdit(p);

                }
                else
                {
                    //SetandGetFileAdd(p);
                    removeDatafromSession();
                    p.App.SignatureImage = "-";
                    p.App.PurchaseordersFilee = "-";
                    p.App.InvoicesFilee = "-";
                    p.App.SummarybillofquantitiesFilee = "-";

                    if (p.buildingWorkForProject != null)
                    {
                        p.FirstGrid = p.buildingWorkForProject.Count() + 1;
                        for (int i = 0; i < p.buildingWorkForProject.Count; i++)
                        {
                            p.buildingWorkForProject[i].PartitionKey = "-";
                            p.buildingWorkForProject[i].RowKey = "-";
                            p.buildingWorkForProject[i].FormRegistrationNo = 0;
                        }

                    }
                    else
                    {
                        p.FirstGrid = cnt;
                        p.buildingWorkForProject = new List<BuildingWorkForProject>();
                        p.buildingWorkForProject.Add(new BuildingWorkForProject { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    }

                    if (p.civilsWorksProjects != null)
                    {
                        p.SecondGrid = p.civilsWorksProjects.Count() + 1;
                        for (int i = 0; i < p.civilsWorksProjects.Count; i++)
                        {
                            p.civilsWorksProjects[i].PartitionKey = "-";
                            p.civilsWorksProjects[i].RowKey = "-";
                            p.civilsWorksProjects[i].FormRegistrationNo = 0;
                        }

                    }
                    else
                    {
                        p.SecondGrid = cnt;
                        p.civilsWorksProjects = new List<CivilsWorksProjects>();
                        p.civilsWorksProjects.Add(new CivilsWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    }
                    if (p.mechanicalWorksProjects != null)
                    {
                        p.ThirdGrid = p.mechanicalWorksProjects.Count() + 1;
                        for (int i = 0; i < p.mechanicalWorksProjects.Count; i++)
                        {
                            p.mechanicalWorksProjects[i].PartitionKey = "-";
                            p.mechanicalWorksProjects[i].RowKey = "-";
                            p.mechanicalWorksProjects[i].FormRegistrationNo = 0;
                        }

                    }
                    else
                    {
                        p.ThirdGrid = cnt;
                        p.mechanicalWorksProjects = new List<MechanicalWorksProjects>();
                        p.mechanicalWorksProjects.Add(new MechanicalWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });
                    }
                }


                if (p.App.FaxNo == null)
                {

                    p.App.FaxNo = "-";
                }


                CustomValidations cv = new CustomValidations();

                if (name == "draft")
                {
                    p.Reviewer = "";
                    p.FormRegistrationNo = p.FormRegistrationNo;
                    p.FormStatus = "Draft";
                    string result3 = Savedata(p);
                    ModelState.Clear();
                    removeDatafromSession();

                    return RedirectToAction("Form9Result", "Form9", new { result = result3, text = "Draft" });
                }
                if (name == "final")
                {
                    bool A = cv.IsAnyNullOrEmpty(p.App);
                    if (p.formval == "Edit")
                    {
                        if (p.App.SignatureImage != "")
                        {

                            ModelState.Remove("App.Signature");
                            A = false;
                        }
                        else
                        {
                            A = true;

                        }
                        if (p.App.PurchaseordersFilee != "")
                        {

                            ModelState.Remove("App.PurchaseordersFile");
                            A = false;
                        }
                        else
                        {
                            A = true;

                        }
                       
                        if (p.App.InvoicesFilee != "")
                        {

                            ModelState.Remove("App.InvoicesFile");
                            A = false;
                        }
                       else
                        {

                            A = true;
                        }
                        if (p.App.SummarybillofquantitiesFilee != "")
                        {

                            ModelState.Remove("App.SummarybillofquantitiesFile");
                            A = false;
                        }
                        else
                        {
                            A = true;

                        }
                       

                    }
                    else
                    {

                        if (p.App.Signature == null)
                        {
                            A = true;
                        }
                        if (p.App.PurchaseordersFile == null)
                        {
                            A = true;
                        }
                        if (p.App.InvoicesFile == null)
                        {
                            A = true;
                        }
                        if (p.App.SummarybillofquantitiesFile == null)
                        {
                            A = true;
                        }

                    }

                    //if (p.App.Signature==null)
                    //{
                    //    A = true;
                    //}
                    //if (p.App.PurchaseordersFile == null)
                    //{
                    //    A = true;
                    //}
                    //if (p.App.InvoicesFile == null)
                    //{
                    //    A = true;
                    //}
                    //if (p.App.SummarybillofquantitiesFile == null)
                    //{
                    //    A = true;
                    //}
                    if (A == true)
                    {
                        return View(p);
                    }
                    bool E = checkTab3(p);
                    if (E == true)
                    {
                        return View(p);

                    }
                    else
                    {

                        p.Reviewer = "Clerk";
                        p.FormStatus = "Submit";
                        string result3 = Savedata(p);
                        ModelState.Clear();
                        removeDatafromSession();

                        return RedirectToAction("Form9Result", "Form9", new { result = result3, text = "Final" });
                    }
                }

            }

            catch (Exception ex)
            {
                ex.Message.ToString();

            }
          

            return View(p);

          
        }
        public Form9ViewModel SetandGetFileAdd(Form9ViewModel p)
        {
            IFormFile fsign;
            //string fName;
            bool isExist;

            isExist = memoryCache.TryGetValue("Signature", out fsign);


            if (!isExist)
            {
                if (p.App.Signature != null)
                {
                    memoryCache.Set("Signature", p.App.Signature);
                    p.App.SignatureImage = p.App.Signature.FileName;
                }
            }
            else
            {
                if (p.App.Signature != null)
                {
                    memoryCache.Set("Signature", p.App.Signature);
                }
                else
                {
                    p.App.Signature = fsign;
                }
                p.App.SignatureImage = p.App.Signature.FileName;

            }

            isExist = memoryCache.TryGetValue("PurchaseordersFile", out fsign);
            if (!isExist)
            {
                if (p.App.PurchaseordersFile != null)
                {
                    memoryCache.Set("PurchaseordersFile", p.App.PurchaseordersFile);
                    p.App.PurchaseordersFilee = p.App.Signature.FileName;
                }
            }
            else
            {
                if (p.App.PurchaseordersFile != null)
                {
                    memoryCache.Set("PurchaseordersFile", p.App.PurchaseordersFile);
                }
                else
                {
                    p.App.PurchaseordersFile = fsign;
                }
                p.App.PurchaseordersFilee = p.App.Signature.FileName;

            }
            isExist = memoryCache.TryGetValue("InvoicesFile", out fsign);
            if (!isExist)
            {
                if (p.App.InvoicesFile != null)
                {
                    memoryCache.Set("InvoicesFile", p.App.InvoicesFile);
                    p.App.InvoicesFilee = p.App.InvoicesFile.FileName;
                }
            }
            else
            {
                if (p.App.InvoicesFile != null)
                {
                    memoryCache.Set("InvoicesFile", p.App.InvoicesFile);
                }
                else
                {
                    p.App.InvoicesFile = fsign;
                }
                p.App.InvoicesFilee = p.App.InvoicesFile.FileName;

            }

            isExist = memoryCache.TryGetValue("SummarybillofquantitiesFile", out fsign);
            if (!isExist)
            {
                if (p.App.SummarybillofquantitiesFile != null)
                {
                    memoryCache.Set("SummarybillofquantitiesFile", p.App.SummarybillofquantitiesFile);
                    p.App.SummarybillofquantitiesFilee = p.App.SummarybillofquantitiesFile.FileName;
                }
            }
            else
            {
                if (p.App.SummarybillofquantitiesFile != null)
                {
                    memoryCache.Set("SummarybillofquantitiesFile", p.App.SummarybillofquantitiesFile);
                }
                else
                {
                    p.App.SummarybillofquantitiesFile = fsign;
                }
                p.App.SummarybillofquantitiesFilee = p.App.SummarybillofquantitiesFile.FileName;

            }



            return p;
        }



        public IActionResult Form9Result(string result, string text)
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
            ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager);
            viewForm1.sendNotification(emailto, subject, body);
            memoryCache.Remove("emailto");
            return View();
        }

        public bool checkTab3(Form9ViewModel p)
        {
            bool FFlag = false;
            bool aFlag = false;
            bool bFlag = false;

            if (p.buildingWorkForProject.Count >= 1)
            {
                for (int j = 0; j < p.buildingWorkForProject.Count; j++)
                {
                    if (p.buildingWorkForProject[j].ProjectName != null)
                    {

                        if (p.buildingWorkForProject[j].NameOfResponsibleContractor == null)
                        {
                            ModelState.AddModelError("buildingWorkForProject[" + j + "].NameOfResponsibleContractor", "Please Enter Name Of Responsible Contractor");
                            FFlag = true;
                            //break;
                        }

                        if (p.buildingWorkForProject[j].EstProjectCost == 0)
                        {
                            ModelState.AddModelError("buildingWorkForProject[" + j + "].EstProjectCost", "Please Enter EstProjectCost");
                            FFlag = true;
                            //break;
                        }
                        if (p.buildingWorkForProject[j].ProposedCommencementDate == null)
                        {
                            ModelState.AddModelError("buildingWorkForProject[" + j + "].ProposedCommencementDate", "Please Enter ProposedCommencementDate");
                            FFlag = true;
                            //break;
                        }
                        if (p.buildingWorkForProject[j].ProposedCompletionDate == null)
                        {
                            ModelState.AddModelError("buildingWorkForProject[" + j + "].ProposedCompletionDate", "Please Enter ProposedCompletionDate");
                            FFlag = true;
                            //break;
                        }

                    }


                    //if (p.buildingWorkForProject.Count >= 1)
                    //{
                    //if (p.buildingWorkForProject[j].ProjectName == null)
                    //{
                    //    FFlag = true;
                    //    break;
                    //}

                    //else if (p.buildingWorkForProject[j].NameOfResponsibleContractor == null)
                    //{
                    //    FFlag = true;
                    //    break;
                    //}

                    //else if (p.buildingWorkForProject[j].EstProjectCost == 0)
                    //{
                    //    FFlag = true;
                    //    break;
                    //}
                    //else if (p.buildingWorkForProject[j].ProposedCommencementDate == null)
                    //{
                    //    FFlag = true;
                    //    break;
                    //}
                    //else if (p.buildingWorkForProject[j].ProposedCompletionDate == null)
                    //{
                    //    FFlag = true;
                    //    break;
                    //}
                    //}


                }
            }


            if (p.mechanicalWorksProjects.Count > 0)
            {
                for (int i = 0; i < p.mechanicalWorksProjects.Count; i++)
                {
                    if (p.mechanicalWorksProjects[i].ProjectName != null)
                    {
                        if (p.mechanicalWorksProjects[i].NameOfResponsibleContractor == null)
                        {
                            ModelState.AddModelError("mechanicalWorksProjects[" + i + "].NameOfResponsibleContractor", "Please Enter Name Of Responsible Contractor");
                            FFlag = true;
                            //break;
                        }

                        if (p.mechanicalWorksProjects[i].EstProjectCost == 0)
                        {
                            ModelState.AddModelError("mechanicalWorksProjects[" + i + "].EstProjectCost", "Please Enter EstProjectCost");
                            FFlag = true;
                            //break;
                        }
                        if (p.mechanicalWorksProjects[i].ProposedCommencementDate == null)
                        {
                            ModelState.AddModelError("mechanicalWorksProjects[" + i + "].ProposedCommencementDate", "Please Enter ProposedCommencementDate");
                            FFlag = true;
                            //break;
                        }
                        if (p.mechanicalWorksProjects[i].ProposedCompletionDate == null)
                        {
                            ModelState.AddModelError("mechanicalWorksProjects[" + i + "].ProposedCompletionDate", "Please Enter ProposedCompletionDate");
                            FFlag = true;
                            //break;
                        }
                    }


                    //if (p.mechanicalWorksProjects.Count >= 1)
                    //{
                    //    if (p.mechanicalWorksProjects[i].ProjectName == null)
                    //    {
                    //        aFlag = true;
                    //        break;
                    //    }
                    //    if (p.mechanicalWorksProjects[i].NameOfResponsibleContractor == null)
                    //    {
                    //        aFlag = true;
                    //        break;
                    //    }
                    //    //if (p.mechanicalWorksProjects[i].EstProjectCost == 0)
                    //    //{
                    //    //    aFlag = true;
                    //    //    break;
                    //    //}
                    //    if (p.mechanicalWorksProjects[i].ProposedCommencementDate == null)
                    //    {
                    //        aFlag = true;
                    //        break;
                    //    }
                    //    if (p.mechanicalWorksProjects[i].ProposedCompletionDate == null)
                    //    {
                    //        aFlag = true;
                    //        break;
                    //    }
                    //}
                }
            }
            if (p.civilsWorksProjects.Count > 0)
            {
                for (int i = 0; i < p.civilsWorksProjects.Count; i++)
                {
                    if (p.civilsWorksProjects[i].ProjectName != null)
                    {
                        if (p.civilsWorksProjects[i].NameOfResponsibleContractor == null)
                        {
                            ModelState.AddModelError("civilsWorksProjects[" + i + "].NameOfResponsibleContractor", "Please Enter Name Of Responsible Contractor");
                            FFlag = true;
                            //break;
                        }

                        if (p.civilsWorksProjects[i].EstProjectCost == 0)
                        {
                            ModelState.AddModelError("civilsWorksProjects[" + i + "].EstProjectCost", "Please Enter EstProjectCost");
                            FFlag = true;
                            //break;
                        }
                        if (p.civilsWorksProjects[i].ProposedCommencementDate == null)
                        {
                            ModelState.AddModelError("civilsWorksProjects[" + i + "].ProposedCommencementDate", "Please Enter ProposedCommencementDate");
                            FFlag = true;
                            //break;
                        }
                        if (p.civilsWorksProjects[i].ProposedCompletionDate == null)
                        {
                            ModelState.AddModelError("civilsWorksProjects[" + i + "].ProposedCompletionDate", "Please Enter ProposedCompletionDate");
                            FFlag = true;
                            //break;
                        }
                    }

                    //if (p.civilsWorksProjects.Count >= 1)
                    //{
                    //    if (p.civilsWorksProjects[i].ProjectName == null)
                    //    {
                    //        bFlag = true;
                    //        break;
                    //    }
                    //    if (p.civilsWorksProjects[i].NameOfResponsibleContractor == null)
                    //    {
                    //        bFlag = true;
                    //        break;
                    //    }
                    //    //if (p.civilsWorksProjects[i].EstProjectCost == 0)
                    //    //{
                    //    //    bFlag = true;
                    //    //    break;
                    //    //}
                    //    if (p.civilsWorksProjects[i].ProposedCommencementDate == null)
                    //    {
                    //        bFlag = true;
                    //        break;
                    //    }
                    //    if (p.civilsWorksProjects[i].ProposedCompletionDate == null)
                    //    {
                    //        bFlag = true;
                    //        break;
                    //    }
                    //}
                }
            }
            if (FFlag == false && aFlag == false && bFlag == false)
            {

                return false;

            }
            else
            {
                return true;
            }


        }

        public IActionResult CicformReview()
        {
            return View();
        }

        public string Savedata(Form9ViewModel p1)
        {
            string response = "";
            SaveModelForm9 saveModelForm9 = new SaveModelForm9();


            string TableName = "CicForm9()";

            string FormRegNo = "";

            int tempMax;
            if (p1.formval == "Edit")
            {

               
                saveModelForm9.PartitionKey = p1.PartitionKey;
                saveModelForm9.RowKey = p1.RowKey;
                saveModelForm9.FormStatus = p1.FormStatus;
                //FormRegNo = "PRN" + tempMax;
                FormRegNo = p1.RowKey;
                tempMax = p1.FormRegistrationNo;
            }
            else

            {
                string jsonData;

                AzureTablesData.GetAllEntity(StorageName, StorageKey, "cicform", out jsonData);//Get data

                JObject myJObject = JObject.Parse(jsonData);
                int cntJson = myJObject["value"].Count();
                int tempRegNo;

                tempMax = (int)myJObject["value"][0]["ProjectRegistrationNo"]; ;
                for (int i = 0; i < cntJson; i++)
                {
                    if (myJObject["value"][i]["ProjectRegistrationNo"] != null)
                    {
                        tempRegNo = (int)myJObject["value"][i]["ProjectRegistrationNo"];
                        if (tempRegNo > tempMax)
                        {
                            tempMax = tempRegNo;
                        }

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
                if(p1.App.CompanyName ==null)
                {
                    p1.App.CompanyName = "New";
                }
                saveModelForm9.PartitionKey = p1.App.CompanyName;
                saveModelForm9.RowKey = "PRN" + tempMax.ToString();
                saveModelForm9.FormStatus = p1.FormStatus;

                FormRegNo = "PRN" + tempMax.ToString();

              
                saveModelForm9.PartitionKey = p1.App.CompanyName;
                saveModelForm9.RowKey = "PRN" + tempMax.ToString();
                saveModelForm9.FormStatus = p1.FormStatus;
            }


            saveModelForm9.FormRegistrationNo = tempMax;
            saveModelForm9.FormName = "Form9";

            saveModelForm9.Reviewer = p1.Reviewer;

            saveModelForm9.CreatedBy = User.Identity.Name;
            saveModelForm9.FaxNo = p1.App.FaxNo;
            saveModelForm9.CompanyName = p1.App.CompanyName;
            saveModelForm9.InstitutionFocalPerson = p1.App.InstitutionFocalPerson;
            saveModelForm9.PostalAddress = p1.App.PostalAddress;
            saveModelForm9.PhysicalAddress = p1.App.PhysicalAddress;
            saveModelForm9.Email = p1.App.Email;
            saveModelForm9.TelephoneNumber = p1.App.TelephoneNumber;
            saveModelForm9.RepresentativeName = p1.App.RepresentativeName;
            saveModelForm9.CompName = p1.App.CompName;
            saveModelForm9.Position = p1.App.Position;
            saveModelForm9.Place = p1.App.Place;
            saveModelForm9.Day = Convert.ToInt32(p1.App.Day);
            saveModelForm9.Month = Convert.ToInt32(p1.App.Month);
            saveModelForm9.Year = Convert.ToInt32(p1.App.Year);
            memoryCache.Set("emailto", User.Identity.Name);
            saveModelForm9.CustNo = HttpContext.Session.GetString("CustNo");
            //Signature

            p1.ImagePath = "PRN" + tempMax;
            if (p1.App.Signature != null)
            {
                uploadFiles(p1.App.Signature, p1.ImagePath, "Signature");
            }

            if (p1.App.PurchaseordersFile != null)
            {
                uploadFiles(p1.App.PurchaseordersFile, p1.ImagePath, "PurchaseordersFile");
            }
            if (p1.App.InvoicesFile != null)
            {
                uploadFiles(p1.App.InvoicesFile, p1.ImagePath, "InvoicesFile");

            }
            if (p1.App.SummarybillofquantitiesFile != null)
            {
                uploadFiles(p1.App.SummarybillofquantitiesFile, p1.ImagePath, "SummarybillofquantitiesFile");

            }


            //addding new for file path 

            if (filepath != "NA")
            {
                saveModelForm9.path = filepath;
            }
            else
            {
                if (!filepath.Contains("https"))
                {
                    //saveModelForm9.path = @"https:\cicdatastorage.blob.core.windows.net\uploads\2022-02-21\" + filepath;
                    saveModelForm9.path= @"https:\cicdatastorage.blob.core.windows.net\uploads\" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + @"\" + filepath;
                }
            }
            if (p1.formval == "Edit")
            {

                response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "CicForm9", JsonConvert.SerializeObject(saveModelForm9, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), saveModelForm9.PartitionKey, saveModelForm9.RowKey);
            }

            else
            {
                saveModelForm9.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                response = AzureTablesData.InsertEntity(StorageName, StorageKey, TableName, JsonConvert.SerializeObject(saveModelForm9, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            }

            if (response == "Created" || response == "NoContent")
            {
                //Saving data for first section
                BuildingWorkForProject buildingworkFor = new BuildingWorkForProject();
                if (p1.buildingWorkForProject != null)
                {
                    for (int i = 0; i < p1.buildingWorkForProject.Count; i++)
                    {

                        string Data;

                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm9BuildingWorkForProject", p1.buildingWorkForProject[i].PartitionKey, p1.buildingWorkForProject[i].RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        if (cntJson != 0)
                        {

                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm9BuildingWorkForProject", p1.buildingWorkForProject[i].PartitionKey, p1.buildingWorkForProject[i].RowKey, Data);


                        }


                        buildingworkFor.PartitionKey = p1.buildingWorkForProject[i].ProjectName;
                        buildingworkFor.RowKey = "PRN" + tempMax.ToString();
                        buildingworkFor.ProjectName = p1.buildingWorkForProject[i].ProjectName;
                        buildingworkFor.NameOfResponsibleContractor = p1.buildingWorkForProject[i].NameOfResponsibleContractor;
                        buildingworkFor.EstProjectCost = p1.buildingWorkForProject[i].EstProjectCost;
                        buildingworkFor.ProposedCommencementDate = p1.buildingWorkForProject[i].ProposedCommencementDate;
                        buildingworkFor.ProposedCompletionDate = p1.buildingWorkForProject[i].ProposedCompletionDate;
                        buildingworkFor.FormRegistrationNo = tempMax;
                        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm9BuildingWorkForProject", JsonConvert.SerializeObject(buildingworkFor, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    }
                }



                CivilsWorksProjects CivilsWorks = new CivilsWorksProjects();
                if (p1.civilsWorksProjects != null)
                {
                    for (int i = 0; i < p1.civilsWorksProjects.Count; i++)
                    {
                        string Data;


                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm9CivilsWorksProjects", p1.civilsWorksProjects[i].PartitionKey, p1.civilsWorksProjects[i].RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        if (cntJson != 0)
                        {

                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm9CivilsWorksProjects", p1.civilsWorksProjects[i].PartitionKey, p1.civilsWorksProjects[i].RowKey, Data);


                        }


                        CivilsWorks.PartitionKey = p1.civilsWorksProjects[i].ProjectName;
                        CivilsWorks.RowKey = "PRN" + tempMax.ToString();
                        CivilsWorks.ProjectName = p1.civilsWorksProjects[i].ProjectName;
                        CivilsWorks.NameOfResponsibleContractor = p1.civilsWorksProjects[i].NameOfResponsibleContractor;
                        CivilsWorks.EstProjectCost = p1.civilsWorksProjects[i].EstProjectCost;
                        CivilsWorks.ProposedCommencementDate = p1.civilsWorksProjects[i].ProposedCommencementDate;
                        CivilsWorks.ProposedCompletionDate = p1.civilsWorksProjects[i].ProposedCompletionDate;
                        CivilsWorks.FormRegistrationNo = tempMax;
                        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm9CivilsWorksProjects", JsonConvert.SerializeObject(CivilsWorks, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    }
                }


                MechanicalWorksProjects mechanicalWork = new MechanicalWorksProjects();
                if (p1.mechanicalWorksProjects != null)
                {
                    for (int i = 0; i < p1.mechanicalWorksProjects.Count; i++)
                    {
                        string Data;


                        AzureTablesData.GetEntitybyRowPartition(StorageName, StorageKey, "CicForm9MechanicalWorksProjects", p1.mechanicalWorksProjects[i].PartitionKey, p1.mechanicalWorksProjects[i].RowKey, out Data);

                        JObject myJObject = JObject.Parse(Data);
                        int cntJson = myJObject["value"].Count();
                        if (cntJson != 0)
                        {

                            AzureTablesData.DeleteEntity(StorageName, StorageKey, "CicForm9MechanicalWorksProjects", p1.mechanicalWorksProjects[i].PartitionKey, p1.mechanicalWorksProjects[i].RowKey, Data);


                        }
                        mechanicalWork.PartitionKey = p1.mechanicalWorksProjects[i].ProjectName;
                        mechanicalWork.RowKey = "PRN" + tempMax.ToString();
                        mechanicalWork.RowKey = "PRN" + tempMax.ToString();
                        mechanicalWork.RowKey = "PRN" + tempMax.ToString();
                        mechanicalWork.ProjectName = p1.mechanicalWorksProjects[i].ProjectName;
                        mechanicalWork.NameOfResponsibleContractor = p1.mechanicalWorksProjects[i].NameOfResponsibleContractor;
                        mechanicalWork.EstProjectCost = p1.mechanicalWorksProjects[i].EstProjectCost;
                        mechanicalWork.ProposedCommencementDate = p1.mechanicalWorksProjects[i].ProposedCommencementDate;
                        mechanicalWork.ProposedCompletionDate = p1.mechanicalWorksProjects[i].ProposedCompletionDate;
                        mechanicalWork.FormRegistrationNo = tempMax;
                        response = AzureTablesData.InsertEntity(StorageName, StorageKey, "CicForm9MechanicalWorksProjects", JsonConvert.SerializeObject(mechanicalWork, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    }

                }
            }

            //return response;

            
            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, TableName, FormRegNo, out jsonData1);//Get data
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
                    businessName = "",
                    certificateNo = "",
                    monthofReg = (string)myJObject["value"][i]["CreatedDate"],
                    registration = "",
                    renewal = "",
                    adminFee = "",
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

        static async Task<HttpResponseMessage> PatchData(Uri u, string json, string accessToken)
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
        //update 
        public IActionResult IndexFromDashboard(string rowkey)
        {
            // ViewBag.type = "active";
            Form9ViewModel model = new Form9ViewModel();
            Form9View model1 = new Form9View();
            //string c = "";
            //form9Model.FirstGrid = cnt;
            //form9Model.SecondGrid = cnt;
            //form9Model.ThirdGrid = cnt;
            List<FileList> AllFileList = new List<FileList>();
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9", rowkey, out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            //set master table values into model 
            for (int i = 0; i < cntJson; i++)
            {
                //adding for file name 
                path = (string)myJObject["value"][i]["path"];

                //adding for files 
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                path = (string)myJObject["value"][i]["path"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                string key;
                AllFileList = b.GetBlobList(path);
                string Signature = null, PurchaseordersFile = null, InvoicesFile = null, SummarybillofquantitiesFile = null;

                if (AllFileList != null)
                {
                    for (int j = 0; j < AllFileList.Count; j++)
                    {
                        key = AllFileList[j].FileKey;

                        memoryCache.Set(AllFileList[j].FileKey, AllFileList[j].FileValue);

                        switch (key)
                        {
                            case "Signature": Signature = AllFileList[j].FileValue; break;

                            case "PurchaseordersFile": PurchaseordersFile = AllFileList[j].FileValue; break;
                            case "InvoicesFile": InvoicesFile = AllFileList[j].FileValue; break;

                            case "SummarybillofquantitiesFile": SummarybillofquantitiesFile = AllFileList[j].FileValue; break;


                        }
                    }
                }


                Form9View App = new Form9View
                {

                    CompanyName = (string)myJObject["value"][i]["CompanyName"],
                    InstitutionFocalPerson = (string)myJObject["value"][i]["InstitutionFocalPerson"],
                    PostalAddress = (string)myJObject["value"][i]["PostalAddress"],
                    PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"],
                    Email = (string)myJObject["value"][i]["Email"],
                    TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"],

                    RepresentativeName = (string)myJObject["value"][i]["RepresentativeName"],
                    CompName = (string)myJObject["value"][i]["CompName"],
                    Position = (string)myJObject["value"][i]["Position"],
                    Place = (string)myJObject["value"][i]["Place"],
                    Day = (int)myJObject["value"][i]["Day"],
                    Month = (int)myJObject["value"][i]["Month"],
                    Year = (int)myJObject["value"][i]["Year"],
                    FaxNo = (string)myJObject["value"][i]["FaxNo"],
                    SignatureImage = Signature,
                    PurchaseordersFilee = PurchaseordersFile,
                    InvoicesFilee = InvoicesFile,
                    SummarybillofquantitiesFilee = SummarybillofquantitiesFile


                };
                model.App = App;
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.RowKey = (string)myJObject["value"][i]["RowKey"];
                model.PartitionKey = (string)myJObject["value"][i]["PartitionKey"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9BuildingWorkForProject", rowkey, out jsonData1);
            JObject myJObject1 = JObject.Parse(jsonData1);
            int cntJson1 = myJObject1["value"].Count();
            List<BuildingWorkForProject> d = new List<BuildingWorkForProject>();
            //for (int i = 0; i < cntJson1; i++)
            for (int i = 0; i < cntJson1; i++)
           
            {

                d.Add(new BuildingWorkForProject
                {
                    EstProjectCost = (int)myJObject1["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject1["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject1["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject1["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject1["value"][i]["ProposedCompletionDate"],
                    PartitionKey = (string)myJObject1["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject1["value"][i]["RowKey"],
                    FormRegistrationNo= (int)myJObject1["value"][i]["FormRegistrationNo"]

                });
            }
            model.buildingWorkForProject = d;
           // model.FirstGrid = d.Count;

            if (model.buildingWorkForProject.Count == 0)
            {
                model.FirstGrid = 1;
                model.buildingWorkForProject = new List<BuildingWorkForProject>();
                model.buildingWorkForProject.Add(new BuildingWorkForProject { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });


            }
            else
            {
                model.FirstGrid = model.buildingWorkForProject.Count;
            }




            string jsonData2;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9CivilsWorksProjects", rowkey, out jsonData2);
            JObject myJObject2 = JObject.Parse(jsonData2);
            int cntJson2 = myJObject2["value"].Count();
            List<CivilsWorksProjects> a = new List<CivilsWorksProjects>();
            for (int i = 0; i < cntJson2; i++)
            {

                a.Add(new CivilsWorksProjects
                {
                    EstProjectCost = (int)myJObject2["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject2["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject2["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject2["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject2["value"][i]["ProposedCompletionDate"],
                    PartitionKey = (string)myJObject2["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject2["value"][i]["RowKey"]

                });
            }
            model.civilsWorksProjects = a;
            // form9Model.SecondGrid = a.Count;

            if (model.civilsWorksProjects.Count == 0)
            {
                model.SecondGrid = 1;
                model.civilsWorksProjects = new List<CivilsWorksProjects>();
                model.civilsWorksProjects.Add(new CivilsWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });



            }
            else
            {
                model.SecondGrid = model.civilsWorksProjects.Count;
            }





            string jsonData3;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm9MechanicalWorksProjects", rowkey, out jsonData3);
            JObject myJObject3 = JObject.Parse(jsonData3);
            int cntJson3 = myJObject3["value"].Count();
            List<MechanicalWorksProjects> w = new List<MechanicalWorksProjects>();
            for (int i = 0; i < cntJson3; i++)
            {

                w.Add(new MechanicalWorksProjects
                {
                    EstProjectCost = (int)myJObject3["value"][i]["EstProjectCost"],
                    NameOfResponsibleContractor = (string)myJObject3["value"][i]["NameOfResponsibleContractor"],
                    ProjectName = (string)myJObject3["value"][i]["ProjectName"],
                    ProposedCommencementDate = (DateTime)myJObject3["value"][i]["ProposedCommencementDate"],
                    ProposedCompletionDate = (DateTime)myJObject3["value"][i]["ProposedCompletionDate"],
                    PartitionKey = (string)myJObject3["value"][i]["PartitionKey"],
                    RowKey = (string)myJObject3["value"][i]["RowKey"]

                });
            }
            model.mechanicalWorksProjects = w;
            //form9Model.ThirdGrid = w.Count;


            if (model.mechanicalWorksProjects.Count == 0)
            {
                model.ThirdGrid = 1;
                model.mechanicalWorksProjects = new List<MechanicalWorksProjects>();
                model.mechanicalWorksProjects.Add(new MechanicalWorksProjects { ProjectName = "", NameOfResponsibleContractor = "", EstProjectCost = 0, ProposedCommencementDate = null, ProposedCompletionDate = null, FormRegistrationNo = 0, PartitionKey = "-", RowKey = "-" });



            }
            else
            {
                model.ThirdGrid = model.mechanicalWorksProjects.Count;
            }


            model.formval = "Edit";
            memoryCache.Set("Form9", model);

            return RedirectToAction("CicForm9", "Form9");


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
        public void setGetFileEdit(Form9ViewModel p)
        {
            if (p.App.Signature != null)
            {
                memoryCache.Set("Signature", p.App.Signature);
                p.App.SignatureImage = p.App.Signature.FileName;
                memoryCache.Set("Signature", p.App.Signature.FileName);
            }
            else
            {
                p.App.SignatureImage = SetandGetFileEdit("Signature");
            }

            if (p.App.PurchaseordersFile != null)
            {
                memoryCache.Set("PurchaseordersFile", p.App.PurchaseordersFile);
                p.App.PurchaseordersFilee = p.App.PurchaseordersFile.FileName;
                memoryCache.Set("PurchaseordersFile", p.App.PurchaseordersFile.FileName);
            }
            else
            {
                p.App.PurchaseordersFilee = SetandGetFileEdit("PurchaseordersFile");
            }


            if (p.App.InvoicesFile != null)
            {
                memoryCache.Set("InvoicesFile", p.App.InvoicesFile);
                p.App.InvoicesFilee = p.App.InvoicesFile.FileName;
                memoryCache.Set("InvoicesFile", p.App.InvoicesFile.FileName);
            }
            else
            {
                p.App.InvoicesFilee = SetandGetFileEdit("InvoicesFile");
            }

            if (p.App.SummarybillofquantitiesFile != null)
            {
                memoryCache.Set("SummarybillofquantitiesFile", p.App.SummarybillofquantitiesFile);
                p.App.SummarybillofquantitiesFilee = p.App.SummarybillofquantitiesFile.FileName;
                memoryCache.Set("SummarybillofquantitiesFile", p.App.SummarybillofquantitiesFile);
            }
            else
            {
                p.App.SummarybillofquantitiesFilee = SetandGetFileEdit("SummarybillofquantitiesFile");
            }



        }

        public void removeDatafromSession()
        {
            memoryCache.Remove("Form9");
            memoryCache.Remove("Signature");
            memoryCache.Remove("PurchaseordersFile");
            memoryCache.Remove("InvoicesFile");
            memoryCache.Remove("SummarybillofquantitiesFile");
        }
    }
}
