using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using CICLatest.MappingConfigurations;

namespace CICLatest.Controllers
{
    public class GenerateCertificateController : Controller
    {
        private readonly IMemoryCache memoryCache;
        private readonly EmailConfiguration _emailcofig;
        static string StorageName = "";
        static string StorageKey = "";
        private IHostingEnvironment Environment;
        private readonly AzureStorageConfiguration _azureConfig;

        public GenerateCertificateController(IMemoryCache memoryCache, EmailConfiguration emailconfig, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
        }

        public IActionResult Index()
        {
            List<CertificateModel> files = new List<CertificateModel>();
            bool isexist = memoryCache.TryGetValue("CertFiles", out files);
            if (isexist)
            {
                memoryCache.Remove("CertFiles");
            }
            return View(files);
        }
        public string GetFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString().Substring(2);
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "/" + NexYear;
            else
                FinYear = PreYear + "/" + CurYear;

            return FinYear.Trim();
        }


        [HttpPost]
        public IActionResult Index(List<CertificateModel> model)
        {
            string PKey = "", RKey = "";
            BlobStorageService objBlobService = new BlobStorageService();
            string tempPath = "Files/";
            string body = "";
            string yr = GetFinancialYear();

            ReceiptModel rdata = new ReceiptModel();
            bool isRexist = memoryCache.TryGetValue("ReceiptMaster", out rdata);
            if (isRexist)
            {
                var r = AzureTablesData.UpdateEntity(StorageName, StorageKey, "ReceiptTbl", JsonConvert.SerializeObject(rdata, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "CIC", "ReceiptNumber");
            }

            Form1Model form1Model = new Form1Model();
            bool isexist = memoryCache.TryGetValue("Form1Model", out form1Model);
            if (isexist)
            {
                foreach (var item in model)
                {

                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form1Model.PartitionKey;
                RKey = form1Model.RowKey;
                form1Model.ReceiptNo = rdata.RNo.ToString();
                CertMasterModel data1 = new CertMasterModel();
                isexist = memoryCache.TryGetValue("CertMaster", out data1);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "RegistrationNumber");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(form1Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form1Model.PartitionKey, form1Model.RowKey);
                memoryCache.Set("Cer", "Success");
                memoryCache.Remove("Form1Model");
                memoryCache.Remove("CertMaster");
            }

            Form1Model form2Model = new Form1Model();
            bool isexist2 = memoryCache.TryGetValue("Form2Model", out form2Model);
            if (isexist2)
            {
                foreach (var item in model)
                {

                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form2Model.PartitionKey;
                RKey = form2Model.RowKey;
                form2Model.ReceiptNo = rdata.RNo.ToString();
                CertMasterModel data1 = new CertMasterModel();
                isexist = memoryCache.TryGetValue("CertMaster", out data1);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form2");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(form2Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form2Model.PartitionKey, form2Model.RowKey);
                memoryCache.Set("Cer", "Success");
                memoryCache.Remove("Form2Model");
                memoryCache.Remove("CertMaster");
            }

            //Form4
            Form4Model form4Model = new Form4Model();
            bool isexist4 = memoryCache.TryGetValue("Form4Model", out form4Model);
            if (isexist4)
            {
                foreach (var item in model)
                {

                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form4Model.PartitionKey;
                RKey = form4Model.RowKey;
                form4Model.ReceiptNo = rdata.RNo.ToString();
                CertForm4Model data1 = new CertForm4Model();
                isexist = memoryCache.TryGetValue("CertMaster", out data1);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form4");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform4", JsonConvert.SerializeObject(form4Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form4Model.PartitionKey, form4Model.RowKey);
                memoryCache.Set("Cer", "Success");
                memoryCache.Remove("Form4Model");
                memoryCache.Remove("CertMaster");
            }

            //Form7
            SaveForm7Model form7Model = new SaveForm7Model();
            bool isexist7 = memoryCache.TryGetValue("Form7Model", out form7Model);
            if (isexist7)
            {
                foreach (var item in model)
                {

                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form7Model.PartitionKey;
                RKey = form7Model.RowKey;
                form7Model.ReceiptNo = rdata.RNo.ToString();
                CertForm7Model data1 = new CertForm7Model();
                isexist = memoryCache.TryGetValue("CertMaster", out data1);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form7");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform7", JsonConvert.SerializeObject(form7Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form7Model.PartitionKey, form7Model.RowKey);
                memoryCache.Set("Cer", "Success");
                memoryCache.Remove("Form7Model");
                memoryCache.Remove("CertMaster");
            }

            //Form6
            SaveModelForm6 form6Model = new SaveModelForm6();
            bool isexist6 = memoryCache.TryGetValue("Form6Model", out form6Model);
            if (isexist6)
            {
                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form6Model.PartitionKey;
                RKey = form6Model.RowKey;
                form6Model.ReceiptNo = rdata.RNo.ToString();
                CertForm6Model data1 = new CertForm6Model();
                isexist = memoryCache.TryGetValue("CertMaster", out data1);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form6");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform6", JsonConvert.SerializeObject(form6Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form6Model.PartitionKey, form6Model.RowKey);
                memoryCache.Set("Cer", "Success");
                memoryCache.Remove("Form6Model");
                memoryCache.Remove("CertMaster");
            }

            //Form8
            SaveModelForm8 form8Model = new SaveModelForm8();
            bool isexist8 = memoryCache.TryGetValue("Form8Model", out form8Model);
            if (isexist8)
            {
                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form8Model.PartitionKey;
                RKey = form8Model.RowKey;
                form8Model.ReceiptNo = rdata.RNo.ToString();
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform8", JsonConvert.SerializeObject(form8Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form8Model.PartitionKey, form8Model.RowKey);

                ////post release updates: invoice generation and update Project Details
                //Form8Helpers form8Helpers = new Form8Helpers();
                //form8Helpers.PostReleaseUpdates(form8Model, StorageName, StorageKey);

                memoryCache.Remove("Form8Model");
                memoryCache.Remove("CertMaster");
            }

            //Form3
            Form3Model form3Model = new Form3Model();
            bool isexist3 = memoryCache.TryGetValue("Form3Model", out form3Model);
            if (isexist3)
            {
                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form3Model.PartitionKey;
                RKey = form3Model.RowKey;
                form3Model.ReceiptNo = rdata.RNo.ToString();
                CertForm3Model data3 = new CertForm3Model();
                isexist = memoryCache.TryGetValue("CertMaster", out data3);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data3, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form3");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform3", JsonConvert.SerializeObject(form3Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form3Model.PartitionKey, form3Model.RowKey);
                memoryCache.Remove("Form3Model");
                memoryCache.Remove("CertMaster");
            }

            //Form5
            SaveModelForm5 form5Model = new SaveModelForm5();
            bool isexist5 = memoryCache.TryGetValue("Form5Model", out form5Model);
            if (isexist5)
            {
                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);

                    body = "<p>Dear Valuable Contractor, you have been successfully registered as a " + item.grade + " contractor for the " + yr + " financial year. Please find attached CIC Certificate. <br/><br/>Thank you,<br/>CIC Team</p>";
                    Email emailobj = new Email(_emailcofig);
                    emailobj.SendAsync(item.emailTo, "CIC certificate", body, bytes);
                }

                PKey = form5Model.PartitionKey;
                RKey = form5Model.RowKey;
                form5Model.ReceiptNo = rdata.RNo.ToString();
                CertForm3Model data3 = new CertForm3Model();
                isexist = memoryCache.TryGetValue("CertMaster", out data3);
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data3, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "Form3");
                var response1 = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform5", JsonConvert.SerializeObject(form5Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form5Model.PartitionKey, form5Model.RowKey);
                memoryCache.Remove("Form5Model");
                memoryCache.Remove("CertMaster");
            }



            string jsond = "";
            var Deleteresponse = AzureTablesData.DeleteEntity(StorageName, StorageKey, "ApplicationLock", PKey, RKey, jsond);
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public IActionResult RemoveCert(List<CertificateModel> model)
        {
            Form1Model form1Model = new Form1Model();
            BlobStorageService objBlobService = new BlobStorageService();
            bool isexist = memoryCache.TryGetValue("Form1Model", out form1Model);

            if (isexist)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form1Model");
                memoryCache.Remove("CertMaster");
            }

            Form1Model form2Model = new Form1Model();
            bool isexist2 = memoryCache.TryGetValue("Form2Model", out form2Model);

            if (isexist2)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form2Model");
                memoryCache.Remove("CertMaster");
            }

            Form4Model form4Model = new Form4Model();
            bool isexist4 = memoryCache.TryGetValue("Form4Model", out form4Model);
            if (isexist4)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                }
                memoryCache.Remove("Form4Model");
                memoryCache.Remove("CertMaster");
            }

            SaveForm7Model form7Model = new SaveForm7Model();
            bool isexist7 = memoryCache.TryGetValue("Form7Model", out form7Model);
            if (isexist7)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form7Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm6 form6Model = new SaveModelForm6();
            bool isexist6 = memoryCache.TryGetValue("Form6Model", out form6Model);
            if (isexist6)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form6Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm8 form8Model = new SaveModelForm8();
            bool isexist8 = memoryCache.TryGetValue("Form8Model", out form6Model);
            if (isexist8)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form8Model");

            }

            Form3Model form3Model = new Form3Model();
            bool isexist3 = memoryCache.TryGetValue("Form3Model", out form3Model);
            if (isexist3)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form3Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm5 form5Model = new SaveModelForm5();
            bool isexist5 = memoryCache.TryGetValue("Form5Model", out form5Model);
            if (isexist5)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                memoryCache.Remove("Form5Model");
                memoryCache.Remove("CertMaster");
            }

            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }

        public void RemoveCertFromServer(List<CertificateModel> model, string comment)
        {
            Form1Model form1Model = new Form1Model();
            BlobStorageService objBlobService = new BlobStorageService();
            bool isexist = memoryCache.TryGetValue("Form1Model", out form1Model);

            if (isexist)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }


                form1Model.Reviewer = "Ops Manager";
                form1Model.FormStatus = "Rejected";
                form1Model.comment = comment;
                form1Model.CertificateNo = null;
                form1Model.Sharelist = null;
                form1Model.applicantBank = null;
                form1Model.worksCapability = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(form1Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form1Model.PartitionKey, form1Model.RowKey);

                memoryCache.Remove("Form1Model");
                memoryCache.Remove("CertMaster");
            }

            Form1Model form2Model = new Form1Model();
            bool isexist2 = memoryCache.TryGetValue("Form2Model", out form2Model);

            if (isexist2)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                form2Model.Reviewer = "Ops Manager";
                form2Model.FormStatus = "Rejected";
                form2Model.comment = comment;
                form2Model.Sharelist = null;
                form2Model.applicantBank = null;
                form2Model.worksCapability = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(form2Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form2Model.PartitionKey, form2Model.RowKey);
                memoryCache.Remove("Form2Model");
                memoryCache.Remove("CertMaster");
            }

            Form4Model form4Model = new Form4Model();
            bool isexist4 = memoryCache.TryGetValue("Form4Model", out form4Model);
            if (isexist4)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                }

                form4Model.Reviewer = "Ops Manager";
                form4Model.FormStatus = "Rejected";
                form4Model.comment = comment;
                form4Model.Sharelist = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform4", JsonConvert.SerializeObject(form4Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form4Model.PartitionKey, form4Model.RowKey);
                memoryCache.Remove("Form4Model");
                memoryCache.Remove("CertMaster");
            }

            SaveForm7Model form7Model = new SaveForm7Model();
            bool isexist7 = memoryCache.TryGetValue("Form7Model", out form7Model);
            if (isexist7)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                form7Model.Reviewer = "Ops Manager";
                form7Model.FormStatus = "Rejected";
                form7Model.comment = comment;
                form7Model.Sharelist = null;
                form7Model.listOfPrevousClent = null;
                form7Model.companyBank = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform7", JsonConvert.SerializeObject(form7Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form7Model.PartitionKey, form7Model.RowKey);
                memoryCache.Remove("Form7Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm6 form6Model = new SaveModelForm6();
            bool isexist6 = memoryCache.TryGetValue("Form6Model", out form6Model);
            if (isexist6)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                form6Model.Reviewer = "Ops Manager";
                form6Model.FormStatus = "Rejected";
                form6Model.comment = comment;
                form6Model.educationBackground = null;
                form6Model.backGroundetails = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform6", JsonConvert.SerializeObject(form6Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form6Model.PartitionKey, form6Model.RowKey);
                memoryCache.Remove("Form6Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm8 form8Model = new SaveModelForm8();
            bool isexist8 = memoryCache.TryGetValue("Form8Model", out form8Model);
            if (isexist8)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                form8Model.Reviewer = "CIC Compliance";
                form8Model.FormStatus = "Rejected";
                form8Model.comment = comment;
                form8Model.Tab3MainSection = null;
                form8Model.tab3SecSection = null;
                form8Model.tab3ThirdSection = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform8", JsonConvert.SerializeObject(form8Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form8Model.PartitionKey, form8Model.RowKey);
                memoryCache.Remove("Form8Model");

            }

            Form3Model form3Model = new Form3Model();
            bool isexist3 = memoryCache.TryGetValue("Form3Model", out form3Model);
            if (isexist3)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                form3Model.Reviewer = "Ops Manager";
                form3Model.FormStatus = "Rejected";
                form3Model.comment = comment;
                form3Model.JointVenturePartiesModel = null;
                form3Model.TechnicalAdministrativeStaffModel = null;
                form3Model.projectStaffModel = null;
                form3Model.LabourForceModel = null;
                form3Model.SubContractorModel = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform3", JsonConvert.SerializeObject(form3Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form3Model.PartitionKey, form3Model.RowKey);
                memoryCache.Remove("Form3Model");
                memoryCache.Remove("CertMaster");
            }

            SaveModelForm5 form5Model = new SaveModelForm5();
            bool isexist5 = memoryCache.TryGetValue("Form5Model", out form5Model);
            if (isexist5)
            {
                string tempPath = "Files/";

                foreach (var item in model)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, tempPath) + item.FileName;
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                form5Model.Reviewer = "Ops Manager";
                form5Model.FormStatus = "Rejected";
                form5Model.comment = comment;
                form5Model.detailOfProjects = null;
                form5Model.subConsultantDetail = null;
                var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform5", JsonConvert.SerializeObject(form5Model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), form5Model.PartitionKey, form5Model.RowKey);
                memoryCache.Remove("Form5Model");
                memoryCache.Remove("CertMaster");
            }
        }

        public IActionResult RejectCert(List<CertificateModel> model)
        {
            string comment = Request.Form["comment"];
            if (comment == "")
            {
                ViewBag.commenterr = "Please enter Comment";
                return View("Index", model);
            }
            else
            {
                ViewBag.commenterr = " ";
            }
            comment = "CEO comment - " + comment;
            RemoveCertFromServer(model, comment);
            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
        }
    }
}
