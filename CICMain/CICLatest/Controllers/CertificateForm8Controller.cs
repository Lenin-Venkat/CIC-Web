using Azure.Core;
using CICLatest.Helper;
using CICLatest.Migrations;
using CICLatest.Models;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Crypto.Tls;
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
    public class CertificateForm8Controller : Controller
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

        public CertificateForm8Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment)
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
            string tablename = "cicform8";
            List<CertificateModel> files = new List<CertificateModel>();


            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            SaveModelForm8 model = new SaveModelForm8();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string filepdfpath = "", grade = "", regNoName = "";
         
            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                model.Timestamp = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.Oraganization = (string)myJObject["value"][i]["Oraganization"];
                model.CategoryId = (string)myJObject["value"][i]["CategoryId"];
                model.Grade = (string)myJObject["value"][i]["Grade"];
                model.CertificateNo = (string)myJObject["value"][i]["CertificateNo"];
                model.TypeGender = (string)myJObject["value"][i]["TypeGender"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.Surname = (string)myJObject["value"][i]["Surname"];
                model.Designation = (string)myJObject["value"][i]["Designation"];
                model.Telephone = (string)myJObject["value"][i]["Telephone"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.EmailAdress = (string)myJObject["value"][i]["EmailAdress"];
                model.MobileNo = (string)myJObject["value"][i]["MobileNo"];
                model.EmailAddress = (string)myJObject["value"][i]["EmailAddress"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.IDNO = (string)myJObject["value"][i]["IDNO"];
                model.PassportNo = (string)myJObject["value"][i]["PassportNo"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.TelephoneWorkdiscipline = (string)myJObject["value"][i]["TelephoneWorkdiscipline"];
                model.FaxNoWorkdiscipline = (string)myJObject["value"][i]["FaxNoWorkdiscipline"];
                model.EmailAddress = (string)myJObject["value"][i]["EmailAddress"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.FirstName = (string)myJObject["value"][i]["FirstName"];
                model.AuthrisedFirstName = (string)myJObject["value"][i]["AuthrisedFirstName"];
                model.AuthorisedSurname = (string)myJObject["value"][i]["AuthorisedSurname"];
                model.DesignationWorkdiscipline = (string)myJObject["value"][i]["DesignationWorkdiscipline"];
                model.AuthorisedMobile = (string)myJObject["value"][i]["AuthorisedMobile"];
                model.AuthorisedFaxNo = (string)myJObject["value"][i]["AuthorisedFaxNo"];
                model.AuthorisedEmail = (string)myJObject["value"][i]["AuthorisedEmail"];
                model.OwnerCategoryId = (string)myJObject["value"][i]["OwnerCategoryId"];//adding new for owner category
                model.DateofAward = (DateTime)myJObject["value"][i]["DateofAward"];
                model.WorkDisciplinefor = (string)myJObject["value"][i]["WorkDisciplinefor"];
                model.ProjectType = (string)myJObject["value"][i]["ProjectType"];
                model.BidReference = (string)myJObject["value"][i]["BidReference"];
                model.ProjectTite = (string)myJObject["value"][i]["ProjectTite"];
                model.ProjectorFunder = (string)myJObject["value"][i]["ProjectorFunder"];
                model.ProjectLocation = (string)myJObject["value"][i]["ProjectLocation"];
                model.TownInkhundla = (string)myJObject["value"][i]["TownInkhundla"];
                model.Region = (string)myJObject["value"][i]["Region"];
                model.GPSCo = (string)myJObject["value"][i]["GPSCo"];
                model.BriefDescriptionofProject = (string)myJObject["value"][i]["BriefDescriptionofProject"];
                model.ProposedCommencmentDate = (DateTime)myJObject["value"][i]["ProposedCommencmentDate"];
                model.ProposedCompleteDate = (DateTime)myJObject["value"][i]["ProposedCompleteDate"];
                model.RevisedDate = (DateTime)myJObject["value"][i]["RevisedDate"];
                model.ContractVAlue = (decimal)myJObject["value"][i]["ContractVAlue"];
                model.LevyPaybale = (decimal)myJObject["value"][i]["LevyPaybale"];
                model.TotalProjectCost = (int)myJObject["value"][i]["TotalProjectCost"];
                model.TotalProjectCostIncludingLevy = (int)myJObject["value"][i]["TotalProjectCostIncludingLevy"];
                model.LevyPaymentOptions = (string)myJObject["value"][i]["LevyPaymentOptions"];
                model.TimeFrameoption = (string)myJObject["value"][i]["TimeFrameoption"];
                model.RevisedDate = (DateTime)myJObject["value"][i]["RevisedDate"];
                model.AuthorisedTelePhone = (string)myJObject["value"][i]["AuthorisedTelePhone"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.Name = (string)myJObject["value"][i]["Name"];
                model.Other = (string)myJObject["value"][i]["Other"];
                model.WitnessName = (string)myJObject["value"][i]["WitnessName"];
                model.WitnessName1 = (string)myJObject["value"][i]["WitnessName1"];
                model.WitnessTitleDesignation = (string)myJObject["value"][i]["WitnessTitleDesignation"];
                model.WitnessTitleDesignation1 = (string)myJObject["value"][i]["WitnessTitleDesignation1"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.path = (string)myJObject["value"][i]["path"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
                model.comment = (string)myJObject["value"][i]["comTotalProjectCostment"];
                filepdfpath = (string)myJObject["value"][i]["path"];
                model.CustNo = (string)myJObject["value"][i]["CustNo"];
                model.TotalProjectCostIncludingLevy = (int)myJObject["value"][i]["TotalProjectCostIncludingLevy"];
             
                model.CreateClearenceCertificate = (int)myJObject["value"][i]["CreateClearenceCertificate"];   
                model.PartialInvoiceCount = (int)myJObject["value"][i]["PartialInvoiceCount"];
                model.NoOfPartialCertificateCreated = (int)myJObject["value"][i]["NoOfPartialCertificateCreated"];
                model.projectCertificateCreated = (int)myJObject["value"][i]["projectCertificateCreated"];
            }
            memoryCache.Set("Form2Data", model);


            string wwwrootpath = "";
            DateTime d = Convert.ToDateTime(model.CreatedDate).AddYears(1);
            DateTime m = Convert.ToDateTime(model.Timestamp).AddMonths(3);
            m = m.AddDays(-1);
            d = d.AddDays(-1);
            string financialYear = GetCurrentFinancialYear();
            String[] yearlist = financialYear.Split("-");

            string createddate1 = DateTime.Today.ToString("yyyy-MM-dd");// yearlist[0] + "-04-01";// d.ToString("yyyy-MM-dd");
            String[] datelist = createddate1.Split("-");
            string year = datelist[0];
            string month = datelist[1];
            string date = datelist[2];

            string convertedDate = DateTime.Now.ToString("yyyy-MM-dd"); ;
            String[] datelist1 = convertedDate.Split("-");
            string year1 = datelist1[0];
            string month1 = datelist1[1];
            string date1 = datelist1[2];

            string convertedDate2 = model.ProposedCommencmentDate.ToString("yyyy-MM-dd"); ;
            String[] datelist2 = convertedDate2.Split("-");
            string year2 = datelist2[0];
            string month2 = datelist2[1];
            string date2 = datelist2[2];

            string convertedDate3 = model.ProposedCompleteDate.ToString("yyyy-MM-dd"); ;
            String[] datelist3 = convertedDate3.Split("-");
            string year3 = datelist3[0];
            string month3 = datelist3[1];
            string date3 = datelist3[2];

            string convertedDate4 = model.Timestamp.ToString("yyyy-MM-dd");
            String[] datelist4 = convertedDate4.Split("-");
            string year4 = datelist4[0];
            string month4 = datelist4[1];
            string date4 = datelist4[2];

            string convertedDate5 = m.ToString("yyyy-MM-dd");
            String[] datelist5 = convertedDate5.Split("-");
            string year5 = datelist5[0];
            string month5 = datelist5[1];
            string date5 = datelist5[2];

            string tempPath1 = "";
            string path1 = "";
            
            tempPath1 = "Files/" + "ProjectRegistrationCertificate2.pdf";

            regNoName = "PRN" + model.FormRegistrationNo.ToString().PadLeft(5, '0');

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
            image1.SetAbsolutePosition(240, 80);
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
            item = pdfFormFields.GetFieldItem("Project Registration Number");
            item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

            //item = pdfFormFields.GetFieldItem("undefined");
            //item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

            pdfFormFields.SetField("Project Registration Number", regNoName);

            pdfFormFields.SetField("Project Name", model.ProjectTite.ToUpper());
            pdfFormFields.SetField("Project Location", model.ProjectLocation.ToUpper());
            pdfFormFields.SetField("Project Owner s", model.Name.ToUpper());
            pdfFormFields.SetField("Address", model.PostalAddress.ToUpper());
            pdfFormFields.SetField("Contractor", model.Oraganization.ToUpper()); //contractor name
            pdfFormFields.SetField("Contractor CIC Registration Certificate No", model.CertificateNo);// cert
            pdfFormFields.SetField("Value of Project", model.TotalProjectCostIncludingLevy.ToString()); //contractor name
            pdfFormFields.SetField("undefined", model.Grade.ToUpper());//Category of Contractor


            //dates filling
            //registration date
            pdfFormFields.SetField("Text1", date1.Substring(0, 1));
            pdfFormFields.SetField("Text2", date1.Substring(1, 1));
            pdfFormFields.SetField("Text3", month1.Substring(0, 1));
            pdfFormFields.SetField("Text4", month1.Substring(1, 1));
            pdfFormFields.SetField("Text5", year1.Substring(0, 1));
            pdfFormFields.SetField("Text7", year1.Substring(1, 1));
            pdfFormFields.SetField("Text8", year1.Substring(2, 1));
            pdfFormFields.SetField("Text9", year1.Substring(3, 1));

            //commencement date
            pdfFormFields.SetField("Text10", date2.Substring(0, 1));
            pdfFormFields.SetField("Text11", date2.Substring(1, 1));
            pdfFormFields.SetField("Text12", month2.Substring(0, 1));
            pdfFormFields.SetField("Text13", month2.Substring(1, 1));
            pdfFormFields.SetField("Text14", year2.Substring(0, 1));
            pdfFormFields.SetField("Text15", year2.Substring(1, 1));
            pdfFormFields.SetField("Text16", year2.Substring(2, 1));
            pdfFormFields.SetField("Text17", year2.Substring(3, 1));

            //completion date
            pdfFormFields.SetField("Text18", date3.Substring(0, 1));
            pdfFormFields.SetField("Text19", date3.Substring(1, 1));
            pdfFormFields.SetField("Text20", month3.Substring(0, 1));
            pdfFormFields.SetField("Text21", month3.Substring(1, 1));
            pdfFormFields.SetField("Text22", "");
            pdfFormFields.SetField("Text23", year3.Substring(0, 1));
            pdfFormFields.SetField("Text24", year3.Substring(1, 1));
            pdfFormFields.SetField("Text25", year3.Substring(2, 1));
            pdfFormFields.SetField("Text26", year3.Substring(3, 1));


            pdfFormFields.SetField("Receipt No", RNo);

            pdfStamper.FormFlattening = true;

            // close the pdf  
            pdfStamper.Close();
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            if (model.projectCertificateCreated == 0)
            {
                BlobStorageService objBlobService = new BlobStorageService();

                string filepath = objBlobService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

                string pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                string CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
                model.projectCertificateCreated = 1;
            }

             int fontSize = 8;

            //Levy Partial certificate
            for (int i = 1; i <= model.PartialInvoiceCount; i++)
            {
                tempPath1 = "Files/" + "CICPartial.pdf";

                regNoName = "PRN" + model.FormRegistrationNo.ToString().PadLeft(5, '0');

                //template file path
                path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
                pdfReader = new PdfReader(path1);

                //Out pdf file path
                tempPath = "Files/" + "Partial_Certificate_" + regNoName + "-" + i + ".pdf";
                path = Path.Combine(this.Environment.WebRootPath, tempPath);

                pdfStamper = new PdfStamper(pdfReader, new FileStream(path, FileMode.Create));
                pdfFormFields = pdfStamper.AcroFields;

                image1 = iTextSharp.text.Image.GetInstance(path2);
                image1.SetAbsolutePosition(240, 80);
                image1.ScaleAbsolute(50, 40);

                // change the content on top of page 1
                overContent = pdfStamper.GetOverContent(1);
                overContent.AddImage(image1);

                tf = new TextField(pdfStamper.Writer, new iTextSharp.text.Rectangle(450, 264, 561, 150), "Vertical")
                {
                    Alignment = Element.ALIGN_CENTER & Element.ALIGN_MIDDLE,
                    FontSize = 8,
                    Options = TextField.READ_ONLY,
                    Text = "Issue Date :" + DateTime.Today.ToShortDateString()
                };
                pdfStamper.AddAnnotation(tf.GetTextField(), 1);

                item = pdfFormFields.GetFieldItem("Project Registration Number");
                item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

                //item = pdfFormFields.GetFieldItem("undefined");
                //item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));


                pdfFormFields.SetField("Project Registration Number", regNoName);

                pdfFormFields.SetField("undefined", model.ProjectTite.ToUpper());
                pdfFormFields.SetFieldProperty("undefined", "textsize", fontSize, null);
                pdfFormFields.SetField("Project Location", model.ProjectLocation.ToUpper());
                pdfFormFields.SetFieldProperty("Project Location", "textsize", fontSize, null);
                pdfFormFields.SetField("Project Owner s", model.Name.ToUpper());
                pdfFormFields.SetFieldProperty("Project Owner s", "textsize", fontSize, null);
                pdfFormFields.SetField("undefined_2", model.PostalAddress.ToUpper());
                pdfFormFields.SetFieldProperty("undefined_2", "textsize", fontSize, null);
                pdfFormFields.SetField("undefined_3", model.Oraganization.ToUpper()); //contractor name
                pdfFormFields.SetFieldProperty("undefined_3", "textsize", fontSize, null);
                pdfFormFields.SetField("fill_9", model.CertificateNo);// cert
                pdfFormFields.SetFieldProperty("fill_9", "textsize", fontSize, null);


                //dates filling
                //registration date
                pdfFormFields.SetField("dd1", date4.Substring(0, 1));
                pdfFormFields.SetField("dd2", date4.Substring(1, 1));
                pdfFormFields.SetField("mm1", month4.Substring(0, 1));
                pdfFormFields.SetField("mm2", month4.Substring(1, 1));
                pdfFormFields.SetField("y1", year4.Substring(0, 1));
                pdfFormFields.SetField("y2", year4.Substring(1, 1));
                pdfFormFields.SetField("y3", year4.Substring(2, 1));
                pdfFormFields.SetField("y4", year4.Substring(3, 1));

                //commencement date
                pdfFormFields.SetField("dd3", date5.Substring(0, 1));
                pdfFormFields.SetField("dd4", date5.Substring(1, 1));
                pdfFormFields.SetField("mm3", month5.Substring(0, 1));
                pdfFormFields.SetField("mm4", month5.Substring(1, 1));
                pdfFormFields.SetField("y5", year5.Substring(0, 1));
                pdfFormFields.SetField("y6", year5.Substring(1, 1));
                pdfFormFields.SetField("y7", year5.Substring(2, 1));
                pdfFormFields.SetField("y8", year5.Substring(3, 1));

                pdfFormFields.SetField("Receipt  No", RNo);
                pdfFormFields.SetFieldProperty("Receipt  No", "textsize", fontSize, null);

                pdfStamper.FormFlattening = true;

                // close the pdf  
                pdfStamper.Close();
                bytes = System.IO.File.ReadAllBytes(path);

                if (i > model.NoOfPartialCertificateCreated)
                {
                    BlobStorageService objBlobService = new BlobStorageService();

                    string filepath = objBlobService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

                    string pdfnameServer = filepdfpath + @"\Files\Partial_Certificate_" + regNoName + "-" + i + ".pdf";
                    wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + "-" + i + ".pdf";
                    string CertName = "Partial_Certificate_" + regNoName + "-" + i + ".pdf";
                    files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
                    model.NoOfPartialCertificateCreated = i;

                    string jsonData1;
                    string tablename1 = "cicform8rcptdetails";

                    AzureTablesData.GetEntity(StorageName, StorageKey, tablename1, rowkey, out jsonData1);
                    JObject myJObject1 = JObject.Parse(jsonData1);
                    int cntJson1 = myJObject1["value"].Count();

                    for (int k = 0; k < cntJson1; k++)
                    {
                        var tempReceiptNo = (string)myJObject1["value"][k]["ReceiptNo"];
                        if (tempReceiptNo == "1")
                        {
                            var PartiKey = (string)myJObject1["value"][k]["PartitionKey"];
                            var RowKey = (string)myJObject1["value"][k]["RowKey"];
                            ReceiptNoDetails ReceiptNoDetailsmodel = new ReceiptNoDetails();
                            ReceiptNoDetailsmodel.ReceiptNo = RNo;
                            ReceiptNoDetailsmodel.CertificateNo = model.CertificateNo;
                            ReceiptNoDetailsmodel.Amount = (decimal)myJObject1["value"][k]["Amount"];
                            var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, tablename1, JsonConvert.SerializeObject(ReceiptNoDetailsmodel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), PartiKey, RowKey);

                            updateReceiptNoDetails(ReceiptNoDetailsmodel.ReceiptNo, PartiKey, RowKey, ReceiptNoDetailsmodel.Amount, model.CertificateNo);
                        }
                    }
                }                
            }
            if (model.CreateClearenceCertificate == 1)
            {
                //Levy Clearence Certificate
                tempPath1 = "Files/" + "LEVY CLEARANCE CERTIFICATE_1.pdf";

                regNoName = "PRN" + model.FormRegistrationNo.ToString().PadLeft(5, '0');

                //template file path
                path1 = Path.Combine(this.Environment.WebRootPath, tempPath1);
                pdfReader = new PdfReader(path1);

                //Out pdf file path
                tempPath = "Files/" + "LEVY_CLEARANCE_CERTIFICATE_" + regNoName + ".pdf";
                path = Path.Combine(this.Environment.WebRootPath, tempPath);

                pdfStamper = new PdfStamper(pdfReader, new FileStream(path, FileMode.Create));
                pdfFormFields = pdfStamper.AcroFields;

                image1 = iTextSharp.text.Image.GetInstance(path2);
                image1.SetAbsolutePosition(240, 80);
                image1.ScaleAbsolute(50, 40);

                // change the content on top of page 1
                overContent = pdfStamper.GetOverContent(1);
                overContent.AddImage(image1);

                tf = new TextField(pdfStamper.Writer, new iTextSharp.text.Rectangle(450, 264, 561, 150), "Vertical")
                {
                    Alignment = Element.ALIGN_CENTER & Element.ALIGN_MIDDLE,
                    FontSize = 8,
                    Options = TextField.READ_ONLY,
                    Text = "Issue Date :" + DateTime.Today.ToShortDateString()
                };
                pdfStamper.AddAnnotation(tf.GetTextField(), 1);

                item = pdfFormFields.GetFieldItem("Project Registration Number");
                item.GetMerged(0).Put(PdfName.Q, new PdfNumber(PdfFormField.Q_CENTER));

                pdfFormFields.SetField("Project Registration Number", regNoName);

                pdfFormFields.SetField("Project Name Give name of project as it appears in the contract document or letter of award", model.ProjectTite.ToUpper());
                pdfFormFields.SetField("Project Location", model.ProjectLocation.ToUpper());
                pdfFormFields.SetField("Project Owner s", model.Name.ToUpper());
                pdfFormFields.SetField("undefined", model.PostalAddress.ToUpper());
                pdfFormFields.SetField("undefined_2", model.Oraganization.ToUpper()); //contractor name
                pdfFormFields.SetField("CertificateNo", model.CertificateNo);// cert

                //dates filling
                //registration date
                pdfFormFields.SetField("Text17", date1.Substring(0, 1));
                pdfFormFields.SetField("Text18", date1.Substring(1, 1));
                pdfFormFields.SetField("Text19", month1.Substring(0, 1));
                pdfFormFields.SetField("Text20", month1.Substring(1, 1));
                pdfFormFields.SetField("Text21", year1.Substring(0, 1));
                pdfFormFields.SetField("Text22", year1.Substring(1, 1));
                pdfFormFields.SetField("Text23", year1.Substring(2, 1));
                pdfFormFields.SetField("Text24", year1.Substring(3, 1));

                //completion date
                pdfFormFields.SetField("Text33", date3.Substring(0, 1));
                pdfFormFields.SetField("Text34", date3.Substring(1, 1));
                pdfFormFields.SetField("Text35", month3.Substring(0, 1));
                pdfFormFields.SetField("Text36", month3.Substring(1, 1));
                pdfFormFields.SetField("Text37", year3.Substring(0, 1));
                pdfFormFields.SetField("Text38", year3.Substring(1, 1));
                pdfFormFields.SetField("Text39", year3.Substring(2, 1));
                pdfFormFields.SetField("Text40", year3.Substring(3, 1));


                pdfFormFields.SetField("Receipt  No", RNo);
                pdfFormFields.SetFieldProperty("Receipt  No", "textsize", fontSize, null);

                pdfStamper.FormFlattening = true;

                // close the pdf  
                pdfStamper.Close();
                bytes = System.IO.File.ReadAllBytes(path);

                BlobStorageService objBlobService = new BlobStorageService();

                string filepath = objBlobService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);


                string pdfnameServer = filepdfpath + @"\Files\LEVY_CLEARANCE_CERTIFICATE_" + regNoName + ".pdf";
                wwwrootpath = Path.Combine(this.Environment.WebRootPath, tempPath) + regNoName + ".pdf";
                string CertName = "LEVY_CLEARANCE_CERTIFICATE_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
                model.FormStatus = "Finished";

                string jsonData1;
                string tablename1 = "cicform8rcptdetails";

                AzureTablesData.GetEntity(StorageName, StorageKey, tablename1, rowkey, out jsonData1);
                JObject myJObject1 = JObject.Parse(jsonData1);
                int cntJson1 = myJObject1["value"].Count();

                for (int k = 0; k < cntJson1; k++)
                {
                    var tempReceiptNo = (string)myJObject1["value"][k]["ReceiptNo"];
                    if (tempReceiptNo == "1")
                    {
                        var PartiKey = (string)myJObject1["value"][k]["PartitionKey"];
                        var RowKey = (string)myJObject1["value"][k]["RowKey"];
                        ReceiptNoDetails ReceiptNoDetailsmodel = new ReceiptNoDetails();
                        ReceiptNoDetailsmodel.ReceiptNo = RNo;
                        ReceiptNoDetailsmodel.CertificateNo = model.CertificateNo;
                        ReceiptNoDetailsmodel.Amount = (decimal)myJObject1["value"][k]["Amount"];
                        var response = AzureTablesData.UpdateEntity(StorageName, StorageKey, tablename1, JsonConvert.SerializeObject(ReceiptNoDetailsmodel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), PartiKey, RowKey);

                        updateReceiptNoDetails(ReceiptNoDetailsmodel.ReceiptNo, PartiKey, RowKey, ReceiptNoDetailsmodel.Amount, model.CertificateNo);
                    }
                }
            }

            memoryCache.Set("CertFiles", files);
            model.CertificateNo1 = regNoName;                    
            memoryCache.Set("Form8Model", model);

            return RedirectToAction("Index", "GenerateCertificate");
        }

        private string updateReceiptNoDetails(string Rno, string PartiKey, string rowkey,decimal amount,string CertificateNo)
        {
            string istr = "";
            GetAccessToken();
            try
            {
                var data1 = JObject.FromObject(new
                {
                    invoiceNo = PartiKey,
                    prnno = rowkey,
                    certificatenumber = CertificateNo,
                    recieptnumber = Rno,
                    amount = amount
                });

                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string u = _azureConfig.BCURL + "/cicprojectCertificates";
                    HttpResponseMessage response = httpClient.PostAsync(@u, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string str = response.Content.ReadAsStringAsync().Result;

                        JObject myProjJObject = JObject.Parse(str);
                        istr = (string)myProjJObject["id"];
                    }
                }
                return istr;
            }
            catch
            { return ""; }
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



        public string GetCertificateData(string name)
        {

            string jsonData;
            string tCert = "";
            List<String> tempCertList = new List<String>();
            String[] clist;
            int count = 0;
            AzureTablesData.GetEntitybyCertificate(StorageName, StorageKey, "cicform1", out jsonData);
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();
            string Grade = "NA";
            for (int i = 0; i < cntJson; i++)
            {

                tCert = (string)myJObject["value"][i]["CertificateNo"];
                if (tCert.Contains(','))
                {
                    clist = tCert.Split(",");
                    count = clist.Count();

                    for (int j = 0; j <= count - 1; j++)
                    {
                        if (name == clist[j])
                        {
                            Grade = (string)myJObject["value"][i]["Grade"];
                            break;
                        }
                    }
                }
                else
                {
                    if (name == tCert)
                    {
                        Grade = (string)myJObject["value"][i]["Grade"];
                        break;
                    }
                }


            }

            if (Grade.Contains(","))
            {
                clist = Grade.Split(",");
                count = clist.Count();

                for (int j = 0; j <= count - 1; j++)
                {
                    if (name[0] == clist[j][0])
                    {
                        Grade = clist[j];
                        break;
                    }
                }

            }

            return Grade;
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
