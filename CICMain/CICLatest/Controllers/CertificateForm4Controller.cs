using Azure.Core;
using CICLatest.Contracts;
using CICLatest.Helper;
using CICLatest.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
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
    public class CertificateForm4Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int ArchitectureCnt = 0, CivilCnt = 0, MechanicalCnt = 0, ElectricalCnt = 0, QuantityCnt=0, AlliedCnt=0;
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;


        public CertificateForm4Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context, AzureStorageConfiguration azureConfig
            , IHostingEnvironment _environment, IBlobStorageService blobStorageService)
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
            string tablename = "cicform4";
            List<CertificateModel> files = new List<CertificateModel>();

            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster","Form4", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {
                ArchitectureCnt = (int)myJObject1["value"][i]["ArchitectureNo"];
                QuantityCnt = (int)myJObject1["value"][i]["QuantityNo"];
                CivilCnt = (int)myJObject1["value"][i]["CivilNo"];
                MechanicalCnt = (int)myJObject1["value"][i]["MechanicalNo"];
                ElectricalCnt = (int)myJObject1["value"][i]["ElectricalNo"];
                AlliedCnt = (int)myJObject1["value"][i]["AlliedNo"];
            }

            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            Form4Model model = new Form4Model();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string  filepdfpath="", grade = "",  regNoName = "", headerContrcatorName = "";
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
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.subCategoryName = (int)myJObject["value"][i]["subCategoryName"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];                
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
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                filepdfpath = (string)myJObject["value"][i]["path"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.RegistrationID = (string)myJObject["value"][i]["RegistrationID"];
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
            string subCategoryName = GetSubCategorybyName(model.subCategoryName);

            string tempPath1 = "", tempPath = "";
            string path1 = "";
            string CivilFilename = "";
            if (model.Category.Contains("Architecture") == true)
            {
                ArchitectureCnt++;
                if(model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FARC" + ArchitectureCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN ARCHITECTURE";
                    grade = "FARC";
                }
                else
                {
                    regNoName = "ARC" + ArchitectureCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "ARCHITECTURE";
                    grade = "ARC";
                }
                
                tempPath1 = "Files/" + "ARCHITECTURE.pdf";                
            }
            else if (model.Category.Contains("Quantity") == true)
            {
                QuantityCnt++;
                if (model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FQS" + QuantityCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN QUANTITY SURVEYING";
                    grade = "FQS";
                }
                else
                {
                    regNoName = "QS" + QuantityCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "QUANTITY SURVEYING";
                    grade = "QS";
                }
                
                tempPath1 = "Files/" + "QUANTITY SURVEYING.pdf";                
            }
            else if (model.Category.Contains("Electrical") == true)
            {
                ElectricalCnt++;
                if (model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FEE" + ElectricalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN ELECTRICAL ENGINEERING";
                    grade = "FEE";
                }
                else
                {
                    regNoName = "EE" + ElectricalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "ELECTRICAL ENGINEERING";
                    grade = "EE";
                }
                
                tempPath1 = "Files/" + "ELECTRICAL ENGINEERING.pdf";
            }
            else if (model.Category.Contains("Civil") == true)
            {
                CivilCnt++;
                if (model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FC/SE" + CivilCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN CIVIL/STRUCTURAL ENGINEERING";
                    grade = "FC/SE";
                    CivilFilename = "FCSE" + CivilCnt.ToString().PadLeft(5, '0');
                }
                else
                {
                    regNoName = "C/SE" + CivilCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "CIVIL/STRUCTURAL ENGINEERING";
                    grade = "C/SE";
                    CivilFilename = "CSE" + CivilCnt.ToString().PadLeft(5, '0');
                }
               
                tempPath1 = "Files/" + "CIVIL STRUCTURAL ENGINEERING.pdf";
            }
            else if (model.Category.Contains("Mechanical") == true)
            {
                MechanicalCnt++;
                if (model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FME" + MechanicalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN MECHANICAL ENGINEERING";
                    grade = "FME";
                }
                else
                {
                    regNoName = "ME" + MechanicalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "MECHANICAL ENGINEERING";
                    grade = "ME";
                }
                
                tempPath1 = "Files/" + "MECHANICAL ENGINEERING.pdf";
            }
            else if (model.Category.Contains("Allied") == true)
            {
                AlliedCnt++;
                if (model.BusinessType == "ForeignCompany")
                {
                    regNoName = "FAP" + AlliedCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN ALLIED PROFESSIONALS";
                    grade = "FAP";
                }
                else
                {
                    regNoName = "AP" + AlliedCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "ALLIED PROFESSIONALS";
                    grade = "AP";
                }
                
                tempPath1 = "Files/" + "ALLIED PROFESSIONALS.pdf";
            }

            model.Grade = grade;    
            //template file path
            path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
            PdfReader pdfReader = new PdfReader(path1);

            //Out pdf file path
            if (model.Category.Contains("Civil") == true)
            {
                tempPath = "Files/" + "Certificate_" + CivilFilename + ".pdf";
            }
            else
            {
                tempPath = "Files/" + "Certificate_" + regNoName + ".pdf";
            }
            string RNo = getReceiptNumberfromDB().ToString().PadLeft(5, '0');// getReceiptNo(model.RowKey);

            string path = Path.Combine(this.Environment.WebRootPath, tempPath);

            //Signature file path
            string imgPath = "Files/" + "CEOSign.png";
            string path2 = Path.Combine(this.Environment.WebRootPath, imgPath);


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
            pdfFormFields.SetField("Type Work Discipline", subCategoryName.ToUpper());
            pdfFormFields.SetField("Contractor Category", grade);
            pdfFormFields.SetField("Active", "A");
            pdfFormFields.SetField("undefined", headerContrcatorName.ToUpper()); //contractor name
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
            string pdfnameServer = "", CertName="";
            if (model.Category.Contains("Civil") == true)
            {
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + CivilFilename + ".pdf";
                CertName = "Certificate_" + CivilFilename + ".pdf";
            }
            else
            {
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                CertName = "Certificate_" + regNoName + ".pdf";
            }
                       
            
            files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
            memoryCache.Set("CertFiles", files);
            model.CertificateNo = regNoName;
            model.Grade = grade;
            model.FormStatus = "Finished";
            memoryCache.Set("Form4Model", model);


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
            CertForm4Model m = new CertForm4Model();

            m.ArchitectureNo = ArchitectureCnt;
            m.CivilNo = CivilCnt;
            m.MechanicalNo = MechanicalCnt;
            m.ElectricalNo = ElectricalCnt;
            m.QuantityNo = QuantityCnt;
            m.AlliedNo = AlliedCnt;
            m.PartitionKey = "Certificate";
            m.RowKey = "Form4";

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
