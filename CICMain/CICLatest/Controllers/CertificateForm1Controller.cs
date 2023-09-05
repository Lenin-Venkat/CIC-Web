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
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    public class CertificateForm1Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        //private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int CivilCnt = 0, MechanicalCnt = 0, ElectricalCnt = 0, BuildingCnt = 0;
        static string BGrade = "", CGrade = "", EGrade = "", MGrade = "";
        static string filepdfpath = "";
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;

        public CertificateForm1Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, AzureStorageConfiguration azureConfig
            , IHostingEnvironment _environment, IBlobStorageService blobStorageService)
        {
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            Environment = _environment;
            _emailcofig = emailconfig;
            StorageName = _azureConfig.StorageAccount;
            StorageKey = _azureConfig.StorageKey1;
            _blobStorageService = blobStorageService;
        }


        public IActionResult Index(string rowkey)
        {
            string jsonData;
            CertificateModel CertModel = new CertificateModel();
            List<CertificateModel> files = new List<CertificateModel>();
            AzureTablesData.GetEntity(StorageName, StorageKey, "certificateMaster", "RegistrationNumber", out jsonData);

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
                model.ScoreStr = (string)myJObject["value"][i]["ScoreStr"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.newGradecomment = (string)myJObject["value"][i]["newGradecomment"];
                if(myJObject["value"][i]["AdminFee"] != null)
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
                model.RegistrationID = (string)myJObject["value"][i]["RegistrationID"];
            }
            memoryCache.Set("Form1Data", model);
            string pdfnameServer = "";
            string TempCertificateNo = "";
            string CertName = "";
            //string tempPath = "Files/Certificate_";
            string wwwrootpath = "";


            if (category.Contains(','))
            {
                String[] strList = category.Split(",");
                int len = strList.Length;

                if (grade.Contains(','))
                {
                    String[] GradeList = grade.Split(",");
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


                for (int i = 0; i < len; i++)
                {
                    WorkDiscipline = strList[i];
                    regNoName = createPDFForm1(createddate, WorkDiscipline, name, emailTo, contractorName, trade, model.BusinessType, model.RowKey);
                    pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                    string tempPath = "Files/" + "Certificate_" + regNoName + ".pdf";
                    wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath);
                    CertName = "Certificate_" + regNoName + ".pdf";
                    files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = model.Grade });
                    if (TempCertificateNo == "")
                    {
                        TempCertificateNo = regNoName;
                    }
                    else
                    {
                        TempCertificateNo = TempCertificateNo + "," + regNoName;
                    }

                }

            }
            else
            {
                if (grade.Contains("B") == true)
                {
                    BGrade = grade;
                }
                else if (grade.Contains("C") == true)
                {
                    CGrade = grade;
                }
                else if (grade.Contains("M") == true)
                {
                    MGrade = grade;
                }
                else if (grade.Contains("E") == true)
                {
                    EGrade = grade;
                }
                WorkDiscipline = category;
                TempCertificateNo = createPDFForm1(createddate, WorkDiscipline, name, emailTo, contractorName, trade, model.BusinessType,model.RowKey);
                
                pdfnameServer = filepdfpath + @"\Files\Certificate_" + TempCertificateNo + ".pdf";
                //tempPath = tempPath + TempCertificateNo + ".pdf";
                //wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath);
                string tempPath = "Files/" + "Certificate_" + TempCertificateNo + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath);
                CertName = "Certificate_" + TempCertificateNo+ ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = model.Grade });
                
            }
            UpdateRegNumberToBC(TempCertificateNo, model.RegistrationID);
            memoryCache.Set("CertFiles",files);
            model.CertificateNo = TempCertificateNo;
            model.FormStatus = "Finished";
            memoryCache.Set("Form1Model", model);
           
           
            //var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "cicform1", JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), model.PartitionKey, model.RowKey);
            return RedirectToAction("Index", "GenerateCertificate");
        }

        public string createPDFForm1(string createddate, string WorkDiscipline, string name, string emailTo, string contractorName, string trade, string businessType, string cust)
        {            
            string regNoName = "", headerContrcatorName = "", grade = "";
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
            getRegistrationNo(WorkDiscipline, businessType, out regNoName, out headerContrcatorName, out grade);

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
            pdfFormFields.SetField("Receipt No", RNo); //d
            pdfStamper.FormFlattening = true;             
            pdfStamper.Close();
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string filepath = _blobStorageService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

            
            return regNoName;
        }

        private void getRegistrationNo(string WorkDiscipline, string businessType, out string regNoName, out string headerContrcatorName, out string grade)
        {
            headerContrcatorName = ""; regNoName = ""; grade = "";
            if (WorkDiscipline.Contains("Building") == true)
            {
                BuildingCnt++;
                
                if(businessType == "ForeignCompany")
                {
                    headerContrcatorName = "FOREIGN BUILDING WORKS CONTRACTOR";
                    regNoName = "BF" + BuildingCnt.ToString().PadLeft(5, '0');
                }
                else
                {
                    headerContrcatorName = "BUILDING WORKS CONTRACTOR";
                    regNoName = "B" + BuildingCnt.ToString().PadLeft(5, '0');
                }
                
                grade = BGrade;
            }
            else if (WorkDiscipline.Contains("Civil") == true)
            {
                CivilCnt++;
                
                if (businessType == "ForeignCompany")
                {
                    regNoName = "CF" + CivilCnt.ToString().PadLeft(5, '0');
                    headerContrcatorName = "FOREIGN CIVIL WORKS CONTRACTOR";
                }
                else
                {
                    headerContrcatorName = "CIVIL WORKS CONTRACTOR";
                    regNoName = "C" + CivilCnt.ToString().PadLeft(5, '0');
                }
                    
                grade = CGrade;
            }
            else if (WorkDiscipline.Contains("Electrical") == true)
            {
                ElectricalCnt++;
                
                if (businessType == "ForeignCompany")
                {
                    headerContrcatorName = "FOREIGN ELECTRICAL WORKS CONTRACTOR";
                    regNoName = "EF" + ElectricalCnt.ToString().PadLeft(5, '0');
                }
                else
                {
                    headerContrcatorName = "ELECTRICAL WORKS CONTRACTOR";
                    regNoName = "E" + ElectricalCnt.ToString().PadLeft(5, '0');
                }
                
                grade = EGrade;
            }
            else if (WorkDiscipline.Contains("Mechanical") == true)
            {
                MechanicalCnt++;
                
                if (businessType == "ForeignCompany")
                {
                    headerContrcatorName = "FOREIGN MECHANICAL WORKS CONTRACTOR";
                    regNoName = "MF" + MechanicalCnt.ToString().PadLeft(5, '0');
                }
                else
                {
                    headerContrcatorName = "MECHANICAL WORKS CONTRACTOR";
                    regNoName = "M" + MechanicalCnt.ToString().PadLeft(5, '0');
                }
                
                grade = MGrade;
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
            m.RowKey = "RegistrationNumber";

            memoryCache.Set("CertMaster",m);
            //var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, "certificateMaster", JsonConvert.SerializeObject(data1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), "Certificate", "RegistrationNumber");
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

        public string getReceiptNo(string cust)
        {
            string RNo = "";
            accessToken = GetAccessToken();
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
                    bool isExist = memoryCache.TryGetValue("Form1Data", out model);
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

        public string UpdateRegNumberToBC(string regNumber,string custno)
        {
            try
            {
                var data1 = JObject.FromObject(new
                {
                    certificateNo = regNumber
                }) ;

                var json = JsonConvert.SerializeObject(data1);

                using (var httpClient = new HttpClient())
                {
                    GetAccessToken();
                    string BCUrl2 = _azureConfig.BCURL + "/customersContract1(" + custno + ")";
                    Uri u = new Uri(BCUrl2);
                    var t = Task.Run(() => PatchData(u, json, "application/json",accessToken));
                    t.Wait();

                }

                return "";
            }
            catch
            { return ""; }

        }

        static async Task<HttpResponseMessage> PatchData(Uri u, string json, string appType,string accessToken)
        {
            HttpClient client1 = new HttpClient();
            client1.DefaultRequestHeaders.Clear();
            client1.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client1.DefaultRequestHeaders.Add("If-Match", "*");
            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent c = new StringContent(json, Encoding.UTF8, appType);

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
    }
}

