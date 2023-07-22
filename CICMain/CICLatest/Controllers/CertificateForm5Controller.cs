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
    public class CertificateForm5Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int JVCnt = 0;
        public static string accessToken;

        public CertificateForm5Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            _context = context;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
        }
        public IActionResult Index(string rowkey)
        {
            string jsonData;
            string tablename = "cicform5";
            List<CertificateModel> files = new List<CertificateModel>();

            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster","Form3", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {
                JVCnt = (int)myJObject1["value"][i]["JVNo"];                                
            }
            JVCnt++;
            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            SaveModelForm5 model = new SaveModelForm5();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string filepdfpath="", grade = "", regNoName = "", headerContrcatorName = "";
            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.AppType = (string)myJObject["value"][i]["AppType"];
                model.NameOFJoinVenture = (string)myJObject["value"][i]["NameOFJoinVenture"];
                model.TypeofJoointVenture = (string)myJObject["value"][i]["TypeofJoointVenture"];
                model.BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"];
                model.BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Phyaddress = (string)myJObject["value"][i]["Phyaddress"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.SurName = (string)myJObject["value"][i]["SurName"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.Fax = (string)myJObject["value"][i]["Fax"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.SubcatogoryId = (string)myJObject["value"][i]["SubcatogoryId"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.DateofRegistration = (DateTime)myJObject["value"][i]["DateofRegistration"];
                model.TaxIdentityNo = (string)myJObject["value"][i]["TaxIdentityNo"];
                //model.SubcatogoryId = (string)myJObject["value"][i]["Subcatogory"];                
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                filepdfpath = (string)myJObject["value"][i]["ImagePath"];
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

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", model.RowKey, out jsonData1);
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            List<DetailOfProjects> JV = new List<DetailOfProjects>();
            for (int i = 0; i < cntJson1; i++)
            {

                JV.Add(new DetailOfProjects
                {
                    NameofApplicant = (string)myJObject2["value"][i]["NameofApplicant"],
                    CountryOfOrigin = (string)myJObject2["value"][i]["CountryOfOrigin"],
                    ContactDetails = (string)myJObject2["value"][i]["ContactDetails"],
                    CICRegistrationNo = (string)myJObject2["value"][i]["CICRegistrationNo"],
                    Shareholding = (int)myJObject2["value"][i]["Shareholding"]

                });
            }
            model.detailOfProjects = JV;

            string subCategoryName = "";
            if (model.SubcatogoryId != "0")
            {
               subCategoryName = GetSubCategorybyName(Convert.ToInt32(model.SubcatogoryId));
            }

            bool SFlag= ShareValidation(model.detailOfProjects);

            string foreignstr = "";
            if (SFlag)
            {
                foreignstr = "F";
            }

            if (model.TypeofJoointVenture == "Foreign/Foreign" || foreignstr == "F")
            {
                regNoName = "JVF" + JVCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "FOREIGN JOINT VENTURE";                
                foreignstr = "-F";
            }
            else
            {
                regNoName = "JV" + JVCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "JOINT VENTURE";                
            }

            if (model.Category.Contains("Architecture") == true)
            {                
                grade = "JVARC"+ foreignstr;
            }
            else if(model.Category.Contains("Civil") == true)
            {
                grade = "JVC/SE" + foreignstr;
            }
            else if (model.Category.Contains("Quantity") == true)
            {
                grade = "JVQS" + foreignstr;
            }
            else if (model.Category.Contains("Electrical") == true)
            {
                grade = "JVEE" + foreignstr;
            }
            else if (model.Category.Contains("Mechanical") == true)
            {
                grade = "JVME" + foreignstr;
            }
            else if (model.Category.Contains("Allied") == true)
            {
                grade = "JVAP" + foreignstr;
            }

            string RNo = getReceiptNumberfromDB().ToString().PadLeft(5, '0');// getReceiptNo(model.RowKey);

            DateTime d = Convert.ToDateTime(model.CreatedDate).AddYears(1);
            d = d.AddDays(-1);
            string convertedDate = d.ToString("yyyy-MM-dd");
            String[] datelist = model.CreatedDate.Split("-");
            string year = datelist[0];
            string month = datelist[1];
            string date = datelist[2];

            String[] datelist1 = convertedDate.Split("-");
            string year1 = datelist1[0];
            string month1 = datelist1[1];
            string date1 = datelist1[2];
            
            string tempPath1 = "";
            string path1 = "";

            tempPath1 = "Files/" + "JOINT VENTURE CONSULTANCY PRACTICE.pdf";

                       
            //template file path
            path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
            PdfReader pdfReader = new PdfReader(path1);

            //Out pdf file path
            string tempPath = "Files/" + "Certificate_" + regNoName + ".pdf";
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

            if (grade.Trim().Length >= 7)
            {
                foreach (String name in pdfFormFields.Fields.Keys)
                {
                    if (name == "Contractor Category")
                    {
                        pdfFormFields.SetFieldProperty(name, "textsize", (float)8, null);
                        pdfFormFields.SetField(name, grade.Trim());
                    }

                }

            }
            else
            {
                pdfFormFields.SetField("Contractor Category", grade.Trim());
            }

            String[] sub = new String[3];
            if (subCategoryName.Contains("("))
            {
                sub = subCategoryName.Split("(");
                subCategoryName = sub[0];
            }


            if (subCategoryName.Length > 60)
            {
                
                    foreach (String name in pdfFormFields.Fields.Keys)
                    {
                        if (name == "Type Work Discipline")
                        {
                            pdfFormFields.SetFieldProperty(name, "textsize", (float)10, null);
                            pdfFormFields.SetField(name, sub[0].ToUpper());
                        }

                    }                                
            }
            else
            {
                pdfFormFields.SetField("Type Work Discipline", subCategoryName.ToUpper());
            }

            pdfFormFields.SetField("Registration Number", regNoName);
            
           // pdfFormFields.SetField("Contractor Category", grade.Trim());
            pdfFormFields.SetField("Active", "A");
            pdfFormFields.SetField("undefined", headerContrcatorName.ToUpper()); //header contractor name
            pdfFormFields.SetField("undefined_2", model.NameOFJoinVenture.ToUpper()); //contractor name
            pdfFormFields.SetField("undefined_3", model.NameOFJoinVenture.ToUpper());// trade
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
            //pdfFormFields.SetFieldProperty("Contractor Category", "textsize", 3, null);
            // close the pdf  
            pdfStamper.Close();
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            BlobStorageService objBlobService = new BlobStorageService();

            string filepath = objBlobService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

            UpdateDB();
            
            string pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
            string CertName = "Certificate_" + regNoName + ".pdf";
            files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
            memoryCache.Set("CertFiles", files);
            model.CertificateNo = regNoName;
            model.FormStatus = "Finished";
            model.detailOfProjects = null;
            memoryCache.Set("Form5Model", model);


            return RedirectToAction("Index", "GenerateCertificate");
        }

        

        public void UpdateDB()
        {
            CertForm3Model m = new CertForm3Model();

            m.JVNo = JVCnt;                    
            m.PartitionKey = "Certificate";
            m.RowKey = "Form3";

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

        public bool ShareValidation(List<DetailOfProjects> p)
        {
            int ForeignShare = 0, SwaziShare = 0;

            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].CountryOfOrigin != "Swazi")
                {
                    ForeignShare = p[i].Shareholding;
                }

                if (p[i].CountryOfOrigin == "Swazi")
                {
                    SwaziShare = SwaziShare+ p[i].Shareholding;
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
            CertificateForm1Controller certificateForm1 = new CertificateForm1Controller(memoryCache, _emailcofig, _azureConfig, Environment);
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
