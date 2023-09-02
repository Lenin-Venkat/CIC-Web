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
    public class CertificateForm6Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int ArtisanCnt = 0;
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;

        public CertificateForm6Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context
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
            string tablename = "cicform6";
            List<CertificateModel> files = new List<CertificateModel>();

            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster","Form6", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {
                ArtisanCnt = (int)myJObject1["value"][i]["ArtisanNo"];                                
            }

            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            SaveModelForm6 model = new SaveModelForm6();
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
                model.Categories = (string)myJObject["value"][i]["Categories"];
                model.AssociationName = (string)myJObject["value"][i]["AssociationName"];
                model.AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Surname = (string)myJObject["value"][i]["Surname"];
                model.IDNo = (string)myJObject["value"][i]["IDNo"];
                model.Nationality = (string)myJObject["value"][i]["Nationality"];
                model.HomeArea = (string)myJObject["value"][i]["HomeArea"];
                model.Chief = (string)myJObject["value"][i]["Chief"];
                model.Indvuna = (string)myJObject["value"][i]["Indvuna"];
                model.TemporalResidencePermitNo = (string)myJObject["value"][i]["TemporalResidencePermitNo"];
                model.WorkPermitNo = (string)myJObject["value"][i]["WorkPermitNo"];
                model.CellphoneNo = (string)myJObject["value"][i]["CellphoneNo"];
                model.Emailaddress = (string)myJObject["value"][i]["Emailaddress"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                // model.FaxNo = (string)myJObject["value"][i]["Email"];
                model.ResidentialAddress = (string)myJObject["value"][i]["ResidentialAddress"];
                model.NextofKin = (string)myJObject["value"][i]["NextofKin"];
                model.Relationship = (string)myJObject["value"][i]["Relationship"];
                model.ContactNo = (string)myJObject["value"][i]["ContactNo"];
                model.NatureofTradeExpertise = (string)myJObject["value"][i]["NatureofTradeExpertise"]; model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.path = (string)myJObject["value"][i]["ImagePath"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                filepdfpath = (string)myJObject["value"][i]["ImagePath"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
            }

            string subcategory = "";
            string Cname = model.Name + " " + model.Surname;
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

            String[] strList = model.Categories.Split("-");
            int len = strList.Length;
            subcategory = strList[0].Trim(); // Subcategory Name
            string WorkDiscipline = strList[1];

            regNoName = "IA" + ArtisanCnt.ToString().PadLeft(5, '0');
            headerContrcatorName = "INDIVIDUAL ARTISAN";
            grade = subcategory;
            tempPath1 = "Files/" + "INDIVIDUAL ARTISAN.pdf";

            //template file path
            path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
            PdfReader pdfReader = new PdfReader(path1);

            //Out pdf file path
            string tempPath = "Files/" + "Certificate_" + regNoName + ".pdf";
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
            pdfFormFields.SetField("Type Work Discipline", WorkDiscipline.ToUpper());
            pdfFormFields.SetField("Contractor Category", grade);
            pdfFormFields.SetField("Active", "A");
            pdfFormFields.SetField("undefined", headerContrcatorName.ToUpper()); //header contractor name
            pdfFormFields.SetField("undefined_2", Cname.ToUpper()); //contractor name
            pdfFormFields.SetField("undefined_3", Cname.ToUpper());// trade
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
            
            string pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
            string CertName = "Certificate_" + regNoName + ".pdf";
            files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
            memoryCache.Set("CertFiles", files);
            model.CertificateNo = regNoName;
            model.FormStatus = "Finished";
            memoryCache.Set("Form6Model", model);


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
            CertForm6Model m = new CertForm6Model();

            m.ArtisanNo = ArtisanCnt;                    
            m.PartitionKey = "Certificate";
            m.RowKey = "Form6";

            memoryCache.Set("CertMaster", m);
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
