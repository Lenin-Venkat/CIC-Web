using Azure.Core;
using CICLatest.Contracts;
using CICLatest.Helper;
using CICLatest.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    public class CertificateForm7Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int ManufacturersCnt = 0, SuppliersCnt = 0;
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;

        public CertificateForm7Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context
            , AzureStorageConfiguration azureConfig, IHostingEnvironment _environment, IBlobStorageService blobStorageService)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            _context = context;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _blobStorageService = blobStorageService;
        }
        public IActionResult Index(string rowkey)
        {
            string jsonData;
            string tablename = "cicform7";
            List<CertificateModel> files = new List<CertificateModel>();

            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster","Form7", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {
                ManufacturersCnt = (int)myJObject1["value"][i]["ManufacturersNo"];
                SuppliersCnt = (int)myJObject1["value"][i]["SuppliersNo"];                
            }

            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            SaveForm7Model model = new SaveForm7Model();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string  filepdfpath="", grade = "", regNoName = "", headerContrcatorName = "";
            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.AppType = (string)myJObject["value"][i]["AppType"];
                model.AssociationName = (string)myJObject["value"][i]["AssociationName"];
                model.AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"];
                model.BusinessName = (string)myJObject["value"][i]["BusinessName"];
                model.TradingStyle = (string)myJObject["value"][i]["TradingStyle"];
                model.BusinessType = (string)myJObject["value"][i]["BusinessType"];
                model.CompanyRegistrationDate = (DateTime)myJObject["value"][i]["CompanyRegistrationDate"];
                model.CompanyRegistrationPlace = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.CompanyRegistrationNumber = (string)myJObject["value"][i]["CompanyRegistrationNumber"];
                //model.re = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.WorkDisciplineType = (string)myJObject["value"][i]["WorkDisciplineType"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                model.StateDetailsOfMaterials = (string)myJObject["value"][i]["StateDetailsOfMaterials"];
                model.StateOfAttainent = (string)myJObject["value"][i]["StateOfAttainent"];
                model.AnnualTurnoverYear1 = (long)myJObject["value"][i]["AnnualTurnoverYear1"];
                model.AnnualTurnoverYear2 = (long)myJObject["value"][i]["AnnualTurnoverYear2"];
                model.AnnualTurnoverYear3 = (long)myJObject["value"][i]["AnnualTurnoverYear3"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"];
                model.Title = (string)myJObject["value"][i]["Title"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                filepdfpath = (string)myJObject["value"][i]["path"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
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
            }

            decimal BestAnnualTurnover = 0;
            string cat = "";
            BestAnnualTurnover = ((model.AnnualTurnoverYear1 > model.AnnualTurnoverYear2 && model.AnnualTurnoverYear1 > model.AnnualTurnoverYear3) ? model.AnnualTurnoverYear1 : (model.AnnualTurnoverYear2 > model.AnnualTurnoverYear3) ? model.AnnualTurnoverYear2 : model.AnnualTurnoverYear3);

            if (1 <= BestAnnualTurnover && BestAnnualTurnover <= 500000)
            {
                cat = "7";
            }
            else if(500001 <= BestAnnualTurnover && BestAnnualTurnover <= 1000000)
            {
                cat = "6";
            }
            else if (1000001 <= BestAnnualTurnover && BestAnnualTurnover <= 2500000)
            {
                cat = "5";
            }
            else if (2500001 <= BestAnnualTurnover && BestAnnualTurnover <= 5000000)
            {
                cat = "4";
            }
            else if (5000001 <= BestAnnualTurnover && BestAnnualTurnover <= 7500000)
            {
                cat = "3";
            }
            else if (7500001 <= BestAnnualTurnover && BestAnnualTurnover <= 10000000)
            {
                cat = "2";
            }
            else if (10000001 <= BestAnnualTurnover && BestAnnualTurnover <= long.MaxValue)
            {
                cat = "1";
            }

            DateTime d = Convert.ToDateTime(model.CreatedDate).AddYears(1);
            d = d.AddDays(-1);
            string financialYear = GetCurrentFinancialYear();
            String[] yearlist = financialYear.Split("-");

            string createddate1 = yearlist[0] + "-04-01";// d.ToString("yyyy-MM-dd");
            String[] datelist = createddate1.Split("-");
            string year = datelist[0];
            string month = datelist[1];
            string date = datelist[2];

            string convertedDate = yearlist[1] + "-03-31";
            String[] datelist1 = convertedDate.Split("-");
            string year1 = datelist1[0];
            string month1 = datelist1[1];
            string date1 = datelist1[2];
            
            string tempPath1 = "";
            string path1 = "";

            //if (model.WorkDisciplineType.Contains("Manufacturer") == true)
            //{
            //    ManufacturersCnt++;
            //    if(model.BusinessType == "ForeignCompany")
            //    {
            //        regNoName = "FM" + ManufacturersCnt.ToString().PadLeft(5, '0');
            //        headerContrcatorName = "FOREIGN MANUFACTURER";
            //        grade = "FM-"+cat;
            //    }
            //    else
            //    {
            //        regNoName = "M" + ManufacturersCnt.ToString().PadLeft(5, '0');
            //        headerContrcatorName = "MANUFACTURER";
            //        grade = "M-" + cat;
            //    }

            //    tempPath1 = "Files/" + "FOREIGNMANUFACTURERSUPPLIER.pdf";                
            //}
            //else if (model.WorkDisciplineType.Contains("Supplier") == true)
            //{
            //    SuppliersCnt++;
            //    if (model.BusinessType == "ForeignCompany")
            //    {
            //        regNoName = "FS" + SuppliersCnt.ToString().PadLeft(5, '0');
            //        headerContrcatorName = "FOREIGN SUPPLIER";
            //        grade = "FS-" + cat;
            //    }
            //    else
            //    {
            //        regNoName = "S" + SuppliersCnt.ToString().PadLeft(5, '0');
            //        headerContrcatorName = "SUPPLIER";
            //        grade = "S-" + cat;
            //    }

            //    tempPath1 = "Files/" + "FOREIGNMANUFACTURERSUPPLIER.pdf";                
            //}

            ManufacturersCnt++;
            SuppliersCnt++;
            if (model.BusinessType == "ForeignCompany")
            {
                regNoName = "FM/S" + ManufacturersCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "FOREIGN MANUFACTURER/SUPPLIER";
                grade = "FM/S-" + cat;
                //model.Grade = "FM/S-" + cat;
            }
            else
            {
                regNoName = "M/S" + ManufacturersCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "MANUFACTURER/SUPPLIER";
                grade = "M/S-" + cat;
               // model.Grade = "M/S-" + cat;
            }
            string fName1 = "MS" + ManufacturersCnt.ToString().PadLeft(5, '0');
            tempPath1 = "Files/" + "FOREIGNMANUFACTURERSUPPLIER.pdf";

            //template file path
            path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
            PdfReader pdfReader = new PdfReader(path1);

            //Out pdf file path
            string tempPath = "Files/" + "Certificate_" + fName1 + ".pdf";
            string path = Path.Combine(this.Environment.WebRootPath, tempPath);

            //Signature file path
            string imgPath = "Files/" + "CEOSign.png";
            string path2 = Path.Combine(this.Environment.WebRootPath, imgPath);

            string RNo = getReceiptNumberfromDB().ToString().PadLeft(5, '0');// getReceiptNo(model.RowKey);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(path, FileMode.Create));
            AcroFields pdfFormFields = pdfStamper.AcroFields;

            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(path2);
            image1.SetAbsolutePosition(240, 185);
            image1.ScaleAbsolute(50, 40);

            // change the content on top of page 1
            PdfContentByte overContent = pdfStamper.GetOverContent(1);
            overContent.AddImage(image1);

            var tf = new TextField(pdfStamper.Writer, new iTextSharp.text.Rectangle(450, 264, 561, 150), "Vertical")
            {
                Alignment = Element.ALIGN_CENTER & Element.ALIGN_MIDDLE,
                FontSize = 8,
                Options = TextField.READ_ONLY,
                Text = "Issue Date :" + DateTime.Today.ToShortDateString()
            };
            pdfStamper.AddAnnotation(tf.GetTextField(), 1);

            AcroFields.Item item;
            item = pdfFormFields.GetFieldItem("Registration Number");
            item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

            item = pdfFormFields.GetFieldItem("undefined");
            item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

            pdfFormFields.SetField("Registration Number", regNoName);
            pdfFormFields.SetField("Type Work Discipline", model.WorkDisciplineType.ToUpper());
            pdfFormFields.SetField("Contractor Category", grade);
            pdfFormFields.SetField("Active", "A");
            pdfFormFields.SetField("undefined", headerContrcatorName.ToUpper()); //header contractor name
            pdfFormFields.SetField("undefined_2", model.BusinessName.ToUpper()); //contractor name
            pdfFormFields.SetField("undefined_3", model.TradingStyle.ToUpper());// trade
            pdfFormFields.SetField("Text1", date.Substring(0, 1)); //d
            pdfFormFields.SetField("Text2", date.Substring(1, 1));// d
            pdfFormFields.SetField("Text3", month.Substring(0, 1)); //d
            pdfFormFields.SetField("Text4", month.Substring(1, 1));// d
            pdfFormFields.SetField("Text5", year.Substring(0, 1)); //d
            pdfFormFields.SetField("Text6", year.Substring(1, 1));// d
            pdfFormFields.SetField("Text7", year.Substring(2, 1)); //d
            pdfFormFields.SetField("Text8", year.Substring(3, 1));// d

            pdfFormFields.SetField("Text9", date1.Substring(0, 1)); //d
            pdfFormFields.SetField("Text10", date1.Substring(1, 1));// d
            pdfFormFields.SetField("Text11", month1.Substring(0, 1)); //d
            pdfFormFields.SetField("Text12", month1.Substring(1, 1));// d
            pdfFormFields.SetField("Text13", year1.Substring(0, 1)); //d
            pdfFormFields.SetField("Text14", year1.Substring(1, 1));// d
            pdfFormFields.SetField("Text15", year1.Substring(2, 1)); //d
            pdfFormFields.SetField("Text16", year1.Substring(3, 1));// d            
            pdfFormFields.SetField("Receipt No", RNo);

            pdfStamper.FormFlattening = true;

            // close the pdf  
            pdfStamper.Close();
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string filepath = _blobStorageService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

            UpdateDB();
            
            string pdfnameServer = filepdfpath + @"\Files\Certificate_" + fName1 + ".pdf";
            string CertName = "Certificate_" + fName1 + ".pdf";
            files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
            memoryCache.Set("CertFiles", files);
            model.CertificateNo = regNoName;
            model.Grade = grade;
            model.FormStatus = "Finished";
            memoryCache.Set("Form7Model", model);


            return RedirectToAction("Index", "GenerateCertificate");
        }


        public static string GetCurrentFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;

            return FinYear.Trim();
        }
        public void UpdateDB()
        {
            CertForm7Model m = new CertForm7Model();

            m.ManufacturersNo = ManufacturersCnt;
            m.SuppliersNo = SuppliersCnt;           
            m.PartitionKey = "Certificate";
            m.RowKey = "Form7";

            memoryCache.Set("CertMaster", m);
        }

        public string GetSubCategorybyName(int subcategoryId)
        {
            string subName = "";
            subName = (from SubCategoryType in _context.SubCategory
                       where SubCategoryType.SubCategoryID == subcategoryId
                       select SubCategoryType.SubCategoryName).FirstOrDefault();

            return subName;
        }

        public int getReceiptNumberfromDB()
        {
            int RNo = 0;
            string jsonData;
            AzureTablesData.GetEntity(StorageName, StorageKey, "ReceiptTbl", "ReceiptNumber", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {
                RNo = (int)myJObject1["value"][i]["RNo"];
            }

            RNo++;

            ReceiptModel rmodel = new ReceiptModel();

            rmodel.PartitionKey = "CIC";
            rmodel.RowKey = "ReceiptNumber";
            rmodel.RNo = RNo;
            memoryCache.Set("ReceiptMaster", rmodel);

            return RNo;
        }

        public string getReceiptNo(string cust)
        {
            string RNo = "";
            CertificateForm1Controller certificateForm1 = new CertificateForm1Controller(memoryCache, _emailcofig, _azureConfig, Environment, _blobStorageService);
            accessToken = certificateForm1.GetAccessToken();

            try
            {

                var data1 = JObject.FromObject(new
                {
                    customerNumber = cust

                });
                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = _azureConfig.BCURL + "/cicSalesInvoices?customerNumber=" + cust;
                    HttpResponseMessage response = httpClient.GetAsync(@u).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string str = response.Content.ReadAsStringAsync().Result;
                        JObject myJObject = JObject.Parse(str);

                        int cntJson = myJObject["value"].Count();
                        for (int i = 0; i < cntJson; i++)
                        {
                            string doc = (string)myJObject["value"][i]["externalDocumentNumber"];
                            if (cust.ToUpper() == doc.ToUpper())
                            {
                                RNo = (string)myJObject["value"][i]["customerPurchaseOrderReference"];
                                break;
                            }
                        }

                    }

                }

                if (RNo != "")
                {
                    Form1Model model = new Form1Model();
                    bool isExist = memoryCache.TryGetValue("Form2Data", out model);
                    if (isExist)
                    {
                        model.ReceiptNo = RNo;
                        var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
                    }
                }

                return RNo;
            }
            catch
            { return ""; }
        }
    }
}
