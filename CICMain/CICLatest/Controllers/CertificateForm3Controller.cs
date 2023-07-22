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
    public class CertificateForm3Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        private readonly UserManager<UserModel> _userManager;
        static int JVCnt = 0;
        public static string accessToken;
        public CertificateForm3Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment, UserManager<UserModel> userManager)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            _context = context;
            _userManager = userManager;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
        }
        public IActionResult Index(string rowkey)
        {
            string jsonData;
            string tablename = "cicform3";
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
            Form3Model model = new Form3Model();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string filepdfpath="", grade = "", trade = "", regNoName = "", headerContrcatorName = "";
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
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.Fax = (string)myJObject["value"][i]["Fax"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Phyaddress = (string)myJObject["value"][i]["Phyaddress"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.NameType = (string)myJObject["value"][i]["NameType"];
                model.SurName = (string)myJObject["value"][i]["SurName"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.BusinessTelephone = (string)myJObject["value"][i]["BusinessTelephone"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.BusinessEmail = (string)myJObject["value"][i]["BusinessEmail"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.CategoryId = (int)myJObject["value"][i]["CategoryId"];
                model.Subcatogory = (int)myJObject["value"][i]["Subcatogory"];
                model.SubcatogoryId = (string)myJObject["value"][i]["SubcatogoryId"];
                model.NameofApplicant = (string)myJObject["value"][i]["NameofApplicant"];
                model.CountryOfOrigin = (string)myJObject["value"][i]["CountryOfOrigin"];
                model.ContactDetails = (string)myJObject["value"][i]["ContactDetails"];
                model.CICRegistrationNo = (string)myJObject["value"][i]["CICRegistrationNo"];
                model.Shareholding = (int)myJObject["value"][i]["Shareholding"];
                model.NameofContractor = (string)myJObject["value"][i]["NameofContractor"];
                model.CountryyofOrigin = (string)myJObject["value"][i]["CountryyofOrigin"];
                model.CICRegistrationNoConsultant = (string)myJObject["value"][i]["CICRegistrationNoConsultant"];
                model.DescriptionOfWork = (string)myJObject["value"][i]["DescriptionOfWork"];
                model.ContractValueOfWork = (decimal)myJObject["value"][i]["ContractValueOfWork"];
                model.customerCheck = (int)myJObject["value"][i]["customerCheck"];
                model.BidReferenceNo = (string)myJObject["value"][i]["BidReferenceNo"];
                model.ProjectTitle = (string)myJObject["value"][i]["ProjectTitle"];
                model.DateofAward = (DateTime)myJObject["value"][i]["DateofAward"];
                model.CommencementDate = (DateTime)myJObject["value"][i]["CommencementDate"];
                model.CompletionDate = (DateTime)myJObject["value"][i]["CompletionDate"];
                model.DescriptionofProject = (string)myJObject["value"][i]["DescriptionofProject"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitleDesignation = (string)myJObject["value"][i]["WitnessedTitleDesignation"];
                model.ClientName = (string)myJObject["value"][i]["ClientName"];
                model.ImagePath = (string)myJObject["value"][i]["ImagePath"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.ContractValue = (decimal)myJObject["value"][i]["ContractValue"];
                model.TitleDesignation = (string)myJObject["value"][i]["TitleDesignation"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.comment = (string)myJObject["value"][i]["comment"];
                filepdfpath = (string)myJObject["value"][i]["ImagePath"];
                model.JVGrade = (string)myJObject["value"][i]["JVGrade"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.RegistrationID = (string)myJObject["value"][i]["RegistrationID"];
            }

            string jsonData1;
            AzureTablesData.GetEntity(StorageName, StorageKey, "CicForm5ProjectDetails", model.RowKey, out jsonData1);
            JObject myJObject2 = JObject.Parse(jsonData1);
            int cntJson2 = myJObject2["value"].Count();
            List<ParticularsofJointVentureParties> JV = new List<ParticularsofJointVentureParties>();
            for (int i = 0; i < cntJson1; i++)
            {

                JV.Add(new ParticularsofJointVentureParties
                {
                    NameofApplicant = (string)myJObject2["value"][i]["NameofApplicant"],
                    CountryOfOrigin = (string)myJObject2["value"][i]["CountryOfOrigin"],
                    ContactDetails = (string)myJObject2["value"][i]["ContactDetails"],
                    CICRegistrationNo = (string)myJObject2["value"][i]["CICRegistrationNo"],
                    Shareholding = (int)myJObject2["value"][i]["Shareholding"]

                });
            }
            model.JointVenturePartiesModel = JV;

            bool SFlag= ShareValidation(model.JointVenturePartiesModel);

            string foreignstr = "";

            if (SFlag)
            {
                foreignstr = "F";
            }

            if (model.TypeofJoointVenture == "foreign" || foreignstr == "F")
            {
                regNoName = "JVF" + JVCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "FOREIGN JOINT VENTURE";
                //grade = "JV-";
                foreignstr = "F";
            }
            else
            {
                regNoName = "JV" + JVCnt.ToString().PadLeft(5, '0');
                headerContrcatorName = "JOINT VENTURE";
                //grade = "M-";
            }

            bool numFlag = model.JVGrade.Any(Char.IsDigit);
            string numericgrade = "";
            if (numFlag)
            {
                numericgrade = "-" + new String(model.JVGrade.Where(Char.IsDigit).ToArray()); //Grade
            }
            
            string CategoryName = "";
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context,_userManager);
            if (model.CategoryId != 0)
            {
                CategoryName = vf4.GetCategorybyName(model.CategoryId);
            }


            if (CategoryName.Contains("General Building") == true)
            {
                CategoryName = "BUILDING WORKS ("+ model.ProjectTitle + ")";
                grade = "JVB" + foreignstr + numericgrade;
            }
            else if(CategoryName.Contains("General Civil") == true)
            {
                CategoryName = "CIVIL WORKS (" + model.ProjectTitle + ")";
                grade = "JVC" + foreignstr + numericgrade;
            }
            else if (CategoryName.Contains("General Electrical") == true)
            {
                CategoryName = "ELECTRICAL WORKS (" + model.ProjectTitle + ")";
                grade = "JVE" + foreignstr + numericgrade;
            }
            else if (CategoryName.Contains("General Mechanical") == true)
            {
                CategoryName = "MECHANICAL WORKS (" + model.ProjectTitle + ")";
                grade = "JVM" + foreignstr + numericgrade;
            }
            bool SpFlag = false;

            if(CategoryName.Contains("Specialist") == true)
            {
                SpFlag = true;
                if (CategoryName.Contains("Mechanical Specialist") == true)
                {                    
                    CategoryName = GetSubCategorybyName(model.Subcatogory);
                    String[] strList = CategoryName.Split("-");
                    CategoryName = strList[1].Trim(); // Subcategory Name

                    if (foreignstr =="F")
                    {
                        foreignstr = "-" + foreignstr;
                        grade = "JV" + strList[0].Trim() + foreignstr;//Grade
                    }
                    else
                    {
                        grade = "JV"+ strList[0].Trim() + numericgrade;
                    }
                        

                }
                else if (CategoryName.Contains("Civils Specialist") == true)
                {
                    CategoryName = GetSubCategorybyName(model.Subcatogory);
                    String[] strList = CategoryName.Split("-");
                    CategoryName = strList[1].Trim(); // Subcategory Name
                    
                    if (foreignstr == "F")
                    {
                        foreignstr = "-" + foreignstr;
                        grade = "JV" + strList[0].Trim() + foreignstr;//Grade
                    }
                    else
                    {
                        grade = "JV" + strList[0].Trim() + numericgrade;
                    }

                }
                else if (CategoryName.Contains("Electrical Specialist") == true)
                {
                    CategoryName = GetSubCategorybyName(model.Subcatogory);
                    String[] strList = CategoryName.Split("-");
                    CategoryName = strList[1].Trim(); // Subcategory Name
                   
                    if (foreignstr == "F")
                    {
                        foreignstr = "-" + foreignstr;
                        grade = "JV" + strList[0].Trim() + foreignstr;//Grade
                    }
                    else
                    {
                        grade = "JV" + strList[0].Trim() + numericgrade;
                    }
                }
                else if (CategoryName.Contains("Buildings Specialist") == true)
                {
                    CategoryName = GetSubCategorybyName(model.Subcatogory);
                    String[] strList = CategoryName.Split("-");
                    CategoryName = strList[1].Trim(); // Subcategory Name
                    
                    if (foreignstr == "F")
                    {
                        foreignstr = "-" + foreignstr;
                        grade = "JV" + strList[0].Trim() + foreignstr;//Grade
                    }
                    else
                    {
                        grade = "JV" + strList[0].Trim() + numericgrade;
                    }
                }

                CategoryName = CategoryName + "(" + model.ProjectTitle + ")";
            }

            

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

            string RNo = getReceiptNumberfromDB().ToString().PadLeft(5, '0');// getReceiptNo(model.RowKey);

            tempPath1 = "Files/" + "JOINT VENTURE CONSTRUCTION FIRMS.pdf";

                       
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

            //if (SpFlag)
            //{
            //    var tp = new TextField(pdfStamper.Writer, new iTextSharp.text.Rectangle(150, 600, 561, 150), "pTitle")
            //    {
            //        Alignment = Element.ALIGN_RIGHT & Element.ALIGN_RIGHT,
            //        FontSize = 8,
            //        Options = TextField.READ_ONLY,
            //        Text = model.ProjectTitle.ToUpper()
            //    };
            //    pdfStamper.AddAnnotation(tp.GetTextField(), 1);
            //}
            

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

            if (CategoryName.ToUpper().Length > 65)
            {
                foreach (String name in pdfFormFields.Fields.Keys)
                {
                    if (name == "Type Work Discipline")
                    {
                        pdfFormFields.SetFieldProperty(name, "textsize", (float)8, null);
                        pdfFormFields.SetField(name, CategoryName.ToUpper());
                    }

                }
            }
            else
            {
                pdfFormFields.SetField("Type Work Discipline", CategoryName.ToUpper());
            }

            pdfFormFields.SetField("Registration Number", regNoName);
            //pdfFormFields.SetField("Type Work Discipline", CategoryName.ToUpper());
            //pdfFormFields.SetField("Contractor Category", grade.Trim());
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
            model.JointVenturePartiesModel = null;
            memoryCache.Set("Form3Model", model);


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

        public bool ShareValidation(List<ParticularsofJointVentureParties> p)
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
                    SwaziShare = SwaziShare + p[i].Shareholding;
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
