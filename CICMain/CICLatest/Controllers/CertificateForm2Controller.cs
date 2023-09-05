using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using CICLatest.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CICLatest.Models;
using System.IO;
using iTextSharp.text;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    public class CertificateForm2Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int CivilCnt = 0, MechanicalCnt = 0, ElectricalCnt = 0, BuildingCnt = 0;
       // static string BGrade = "", CGrade = "", EGrade = "", MGrade = "";
        string filepdfpath = "";
        private readonly UserManager<UserModel> _userManager;
        public static string accessToken;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;


        public CertificateForm2Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, AzureStorageConfiguration azureConfig
            , IHostingEnvironment _environment, ApplicationContext context
            ,UserManager<UserModel> userManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _userManager = userManager;
            _context = context;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }


        public IActionResult Index(string rowkey)
        {
            string jsonData;
            CertificateModel CertModel = new CertificateModel();
            List<CertificateModel> files = new List<CertificateModel>();
            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster", "Form2", out jsonData);

            JObject myJObject1 = JObject.Parse(jsonData);
            int cntJson1 = myJObject1["value"].Count();
            for (int i = 0; i < cntJson1; i++)
            {               
                BuildingCnt = (int)myJObject1["value"][i]["BuildingNo"];
                CivilCnt = (int)myJObject1["value"][i]["CivilNo"];
                MechanicalCnt = (int)myJObject1["value"][i]["MechanicalNo"];
                ElectricalCnt = (int)myJObject1["value"][i]["ElectricalNo"];                
            }
                       
            AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1", rowkey, out jsonData);
            Form1Model model = new Form1Model();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string RegNo = "", category = "", WorkDiscipline = "", emailTo = "", name = "", grade = "", contractorName = "", trade = "", regNoName = "", headerContrcatorName = "", createddate = "";
            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                RegNo = (string)myJObject["value"][i]["FirmRegistrationNo"];
                category = (string)myJObject["value"][i]["Category"];
                grade = (string)myJObject["value"][i]["Grade"];
                contractorName = (string)myJObject["value"][i]["BusinessName"];
                trade = (string)myJObject["value"][i]["TradingStyle"];
                createddate = (string)myJObject["value"][i]["CreatedDate"];
                emailTo = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                name = (string)myJObject["value"][i]["BusinessRepresentativeName"];

                model.PartitionKey = partitionkey;
                model.RowKey = (string)myJObject["value"][i]["RowKey"]; ;
                model.AppType = (string)myJObject["value"][i]["AppType"];
                model.AssociationName = (string)myJObject["value"][i]["AssociationName"];
                model.AuthorisedOfficerName = (string)myJObject["value"][i]["AuthorisedOfficerName"];
                model.BusinessName = (string)myJObject["value"][i]["BusinessName"];
                model.TradingStyle = (string)myJObject["value"][i]["TradingStyle"];
                model.BusinessType = (string)myJObject["value"][i]["BusinessType"];
                model.CompanyRegistrationDate = (DateTime)myJObject["value"][i]["CompanyRegistrationDate"];
                model.CompanyRegistrationPlace = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.CompanyRegistrationNumber = (string)myJObject["value"][i]["CompanyRegistrationPlace"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.CompanyHOPhysicalAddress = (string)myJObject["value"][i]["CompanyHOPhysicalAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.Category = (string)myJObject["value"][i]["Category"];
                model.PresentGrade = (string)myJObject["value"][i]["PresentGrade"];
                model.BusinessRepresentativeName = (string)myJObject["value"][i]["BusinessRepresentativeName"];
                model.BusinessRepresentativePositionNumber = (string)myJObject["value"][i]["BusinessRepresentativePositionNumber"];
                model.BusinessRepresentativeCellNo = (string)myJObject["value"][i]["BusinessRepresentativeCellNo"];
                model.BusinessRepresentativeFax = (string)myJObject["value"][i]["BusinessRepresentativeFax"];
                model.BusinessRepresentativeEmail = (string)myJObject["value"][i]["BusinessRepresentativeEmail"];
                model.AnnualTurnoverYear1 = (long)myJObject["value"][i]["AnnualTurnoverYear1"];
                model.AnnualTurnoverYear2 = (long)myJObject["value"][i]["AnnualTurnoverYear2"];
                model.AnnualTurnoverYear3 = (long)myJObject["value"][i]["AnnualTurnoverYear3"];
                model.FinancialValue = (long)myJObject["value"][i]["FinancialValue"];
                model.FinancialInstitutionName = (string)myJObject["value"][i]["FinancialInstitutionName"];
                model.AvailableCapital = (long)myJObject["value"][i]["AvailableCapital"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessedName = (string)myJObject["value"][i]["WitnessedName"];
                model.WitnessedTitle = (string)myJObject["value"][i]["WitnessedTitle"];
                model.Title = (string)myJObject["value"][i]["Title"];
                model.FirmRegistrationNo = (int)myJObject["value"][i]["FirmRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.comment = (string)myJObject["value"][i]["comment"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                filepdfpath = (string)myJObject["value"][i]["path"];
                model.ElectricalSubCategory = (int)myJObject["value"][i]["ElectricalSubCategory"];
                model.CivilSubCategory = (int)myJObject["value"][i]["CivilSubCategory"];
                model.BuildingSubCategory = (int)myJObject["value"][i]["BuildingSubCategory"];
                model.MechanicalSubCategory = (int)myJObject["value"][i]["MechanicalSubCategory"];
                model.ScoreStr = (string)myJObject["value"][i]["ScoreStr"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.newGradecomment = (string)myJObject["value"][i]["newGradecomment"];
                model.RegistrationID = (string)myJObject["value"][i]["RegistrationID"];
            }
            memoryCache.Set("Form2Data", model);
            string pdfnameServer = "";
            string TempCertificateNo = "";
            string CertName = "";
            string tempPath = "Files/Certificate_";
            string wwwrootpath = "";

            string subcategory = "";
            ViewForm4Controller vf4 = new ViewForm4Controller(memoryCache, _azureConfig, _context,_userManager, _appSettingsReader, _blobStorageService);
            string numericgrade = "";
            string BGrade = "", CGrade = "", EGrade = "", MGrade = "", BCat = "", CCat = "", ECat = "", MCat = "";

            if (model.Grade.Contains(','))
            {
                String[] GradeList = model.Grade.Split(",");
                int len1 = GradeList.Length;

                for (int k = 0; k < len1; k++)
                {
                    if (GradeList[k].Contains("B") == true)
                    {
                        BGrade = GradeList[k];
                    }
                    else if (GradeList[k].Contains("C") == true)
                    {
                        CGrade = GradeList[k];
                    }
                    else if (GradeList[k].Contains("M") == true)
                    {
                        MGrade = GradeList[k];
                    }
                    else if (GradeList[k].Contains("E") == true)
                    {
                        EGrade = GradeList[k];
                    }
                }
            }
            else
            {
                if (model.Grade.Contains("B") == true)
                {
                    BGrade = model.Grade;
                }
                else if (model.Grade.Contains("C") == true)
                {
                    CGrade = model.Grade;
                }
                else if (model.Grade.Contains("M") == true)
                {
                    MGrade = model.Grade;
                }
                else if (model.Grade.Contains("E") == true)
                {
                    EGrade = model.Grade;
                }
            }

            if (model.Category.Contains(','))
            {
                String[] CList = model.Category.Split(",");
                int len1 = CList.Length;

                for (int k = 0; k < len1; k++)
                {
                    if (CList[k].Contains("Building") == true)
                    {
                        BCat = CList[k];
                    }
                    else if (CList[k].Contains("Civil") == true)
                    {
                        CCat = CList[k];
                    }
                    else if (CList[k].Contains("Mechanical") == true)
                    {
                        MCat = CList[k];
                    }
                    else if (CList[k].Contains("Electrical") == true)
                    {
                        ECat = CList[k];
                    }
                }
            }
            else
            {
                if (model.Category.Contains("Building") == true)
                {
                    BCat = model.Category;
                }
                else if (model.Category.Contains("Civil") == true)
                {
                    CCat = model.Category;
                }
                else if (model.Category.Contains("Mechanical") == true)
                {
                    MCat = model.Category;
                }
                else if (model.Category.Contains("Electrical") == true)
                {
                    ECat = model.Category;
                }
            }


            if (model.ElectricalSubCategory !=0)
            {
                subcategory = vf4.GetSubCategorybyName(model.ElectricalSubCategory);
                String[] strList = subcategory.Split("-");
                int len = strList.Length;               
                subcategory = strList[1].Trim(); // Subcategory Name

                if(model.BusinessType == "ForeignCompany")
                {
                    numericgrade = strList[0].Trim() + "-F" ;
                }
                else
                {
                    numericgrade = strList[0].Trim() + "-" + new String(EGrade.Where(Char.IsDigit).ToArray()); //Grade
                }
                
                regNoName = createPDFForm1(createddate, subcategory, numericgrade, emailTo, contractorName, trade, ECat, model.BusinessType, model.RowKey);
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = EGrade });

            }
            if(model.BuildingSubCategory !=0)
            {
                subcategory = vf4.GetSubCategorybyName(model.BuildingSubCategory);
                String[] strList = subcategory.Split("-");
                int len = strList.Length;
                subcategory = strList[1].Trim(); // Subcategory Name
                if (model.BusinessType == "ForeignCompany")
                {
                    numericgrade = strList[0].Trim() + "-F";
                }
                else
                {
                    numericgrade = strList[0].Trim() + "-" + new String(BGrade.Where(Char.IsDigit).ToArray()); //Grade
                }
                regNoName = createPDFForm1(createddate, subcategory, numericgrade, emailTo, contractorName, trade, BCat, model.BusinessType, model.RowKey);
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = BGrade });
            }
            if (model.MechanicalSubCategory != 0)
            {
                subcategory = vf4.GetSubCategorybyName(model.MechanicalSubCategory);
                String[] strList = subcategory.Split("-");
                int len = strList.Length;
                subcategory = strList[1].Trim(); // Subcategory Name
                if (model.BusinessType == "ForeignCompany")
                {
                    numericgrade = strList[0].Trim() + "-F";
                }
                else
                {
                    numericgrade = strList[0].Trim() + "-" + new String(MGrade.Where(Char.IsDigit).ToArray()); //Grade
                }
                regNoName = createPDFForm1(createddate, subcategory, numericgrade, emailTo, contractorName, trade, MCat, model.BusinessType, model.RowKey);
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = MGrade });
            }
            if (model.CivilSubCategory != 0)
            {
                subcategory = vf4.GetSubCategorybyName(model.CivilSubCategory);
                String[] strList = subcategory.Split("-");
                int len = strList.Length;
                subcategory = strList[1].Trim(); // Subcategory Name
                if (model.BusinessType == "ForeignCompany")
                {
                    numericgrade = strList[0].Trim() + "-F";
                }
                else
                {
                    numericgrade = strList[0].Trim() + "-" + new String(CGrade.Where(Char.IsDigit).ToArray()); //Grade
                }
                regNoName = createPDFForm1(createddate, subcategory, numericgrade, emailTo, contractorName, trade, CCat, model.BusinessType,model.RowKey);
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = CGrade });
            }

            CertificateForm1Controller certificateForm1 = new CertificateForm1Controller(memoryCache,_emailcofig,_azureConfig, Environment, _blobStorageService);
            certificateForm1.UpdateRegNumberToBC(regNoName, model.RegistrationID);

            memoryCache.Set("CertFiles",files);
            model.CertificateNo = regNoName;
            model.FormStatus = "Finished";
            memoryCache.Set("Form2Model", model);
           
            return RedirectToAction("Index", "GenerateCertificate");
        }

        public string createPDFForm1(string createddate, string WorkDiscipline, string grade, string emailTo, string contractorName, string trade, string category, string Btype, string cust)
        {
            string regNoName = "", headerContrcatorName = "";
            DateTime d = Convert.ToDateTime(createddate).AddYears(1);
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
            string RNo = getReceiptNumberfromDB().ToString().PadLeft(5, '0');// getReceiptNo(cust);
            getRegistrationNo(category, Btype, out regNoName, out headerContrcatorName);

            //template file path
            string tempPath1 = "Files/" + "Certificate.pdf";
            string path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
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

            pdfFormFields.SetField("Registration Number", regNoName);
            pdfFormFields.SetField("Type Work Discipline", WorkDiscipline.ToUpper());
            pdfFormFields.SetField("Contractor Category", grade);
            pdfFormFields.SetField("Active", "A");
            pdfFormFields.SetField("undefined", headerContrcatorName.ToUpper()); //contractor name
            pdfFormFields.SetField("undefined_2", contractorName.ToUpper()); //contractor name
            pdfFormFields.SetField("undefined_3", trade.ToUpper());// trade
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
            // pdfFormFields.RemoveField("Receipt No");
            pdfFormFields.SetField("Receipt No", RNo); //d

            pdfStamper.FormFlattening = true;             
            pdfStamper.Close();
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string filepath = _blobStorageService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

            return regNoName;
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
        private void getRegistrationNo(string WorkDiscipline, string businessType, out string regNoName, out string headerContrcatorName)
        {
            headerContrcatorName = ""; regNoName = "";
            if (WorkDiscipline.Contains("Building") == true)
            {
                BuildingCnt++;
                if (businessType == "ForeignCompany")
                {
                    regNoName = "BSF" + BuildingCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN BUILDING SPECIALIST WORKS CONTRACTOR";
                }
                else
                {
                    regNoName = "BS" + BuildingCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "BUILDING SPECIALIST WORKS CONTRACTOR";
                }                               
            }
            else if (WorkDiscipline.Contains("Civil") == true)
            {
                CivilCnt++;
                if (businessType == "ForeignCompany")
                {
                    regNoName = "CSF" + CivilCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN CIVIL SPECIALIST WORKS CONTRACTOR";
                }
                else
                {
                    regNoName = "CS" + CivilCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "CIVIL SPECIALIST WORKS CONTRACTOR";
                }                
            }
            else if (WorkDiscipline.Contains("Electrical") == true)
            {
                ElectricalCnt++;
                if (businessType == "ForeignCompany")
                {
                    regNoName = "ESF" + ElectricalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN ELECTRICAL SPECIALIST WORKS CONTRACTOR";
                }
                else
                {
                    regNoName = "ES" + ElectricalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "ELECTRICAL SPECIALIST WORKS CONTRACTOR";
                }                
            }
            else if (WorkDiscipline.Contains("Mechanical") == true)
            {
                MechanicalCnt++;
                if (businessType == "ForeignCompany")
                {
                    regNoName = "MSF" + MechanicalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN MECHANICAL SPECIALIST WORKS CONTRACTOR";
                }
                else
                {
                    regNoName = "MS" + MechanicalCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "MECHANICAL SPECIALIST WORKS CONTRACTOR";
                }                
            }

            UpdateDB();
        }

        public void UpdateDB()
        {
            CertMasterModel m = new CertMasterModel();

            m.BuildingNo = BuildingCnt;
            m.CivilNo = CivilCnt;
            m.MechanicalNo = MechanicalCnt;
            m.ElectricalNo = ElectricalCnt;            
            m.PartitionKey = "Certificate";
            m.RowKey = "Form2";

            memoryCache.Set("CertMaster",m);
            //var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "RegistrationNumber");
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
                            if (cust.ToUpper() ==  doc.ToUpper())
                            {
                                RNo = (string)myJObject["value"][i]["customerPurchaseOrderReference"]; //(string)myJObject["value"][i]["number"];
                                break;
                            }
                        }
                                                   
                    }

                }

                if(RNo!="")
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
    }
}
