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
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    public class CertificateForm9Controller : Controller
    {
        static string StorageName = "";
        static string StorageKey = "";
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        private readonly EmailConfiguration _emailcofig;
        private IHostingEnvironment Environment;
        static int ManufacturersCnt = 0, SuppliersCnt = 0;
        
        public CertificateForm9Controller(IMemoryCache memoryCache, EmailConfiguration emailconfig, ApplicationContext context, AzureStorageConfiguration azureConfig, IHostingEnvironment _environment)
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
            string tablename = "cicform9";
            List<CertificateModel> files = new List<CertificateModel>();

            
            AzureTablesData.GetEntity(StorageName, StorageKey, tablename, rowkey, out jsonData);
            SaveModelForm9 model = new SaveModelForm9();
            JObject myJObject = JObject.Parse(jsonData);
            int cntJson = myJObject["value"].Count();

            string filepdfpath="", grade = "", regNoName = "";
            for (int i = 0; i < cntJson; i++)
            {
                string partitionkey = (string)myJObject["value"][i]["PartitionKey"];
                string fName = (string)myJObject["value"][i]["RowKey"];
                DateTime FDate = (DateTime)myJObject["value"][i]["Timestamp"];
                string formattedDate = FDate.ToShortDateString();
                model.PartitionKey = partitionkey;
                model.RowKey = fName;
                model.CompanyName = (string)myJObject["value"][i]["CompanyName"];
                model.InstitutionFocalPerson = (string)myJObject["value"][i]["InstitutionFocalPerson"];
                model.PostalAddress = (string)myJObject["value"][i]["PostalAddress"];
                model.PhysicalAddress = (string)myJObject["value"][i]["PhysicalAddress"];
                model.Email = (string)myJObject["value"][i]["Email"];
                model.FaxNo = (string)myJObject["value"][i]["FaxNo"];
                model.TelephoneNumber = (string)myJObject["value"][i]["TelephoneNumber"];
                model.FormName = (string)myJObject["value"][i]["FormName"];
                model.FormStatus = (string)myJObject["value"][i]["FormStatus"];
                model.RepresentativeName = (string)myJObject["value"][i]["RepresentativeName"];
                model.CompName = (string)myJObject["value"][i]["CompName"];
                model.Position = (string)myJObject["value"][i]["Position"];
                model.Place = (string)myJObject["value"][i]["Place"];
                model.Day = (int)myJObject["value"][i]["Day"];
                model.Month = (int)myJObject["value"][i]["Month"];
                model.Year = (int)myJObject["value"][i]["Year"];
                model.path = (string)myJObject["value"][i]["path"];
                model.FormRegistrationNo = (int)myJObject["value"][i]["FormRegistrationNo"];
                model.Reviewer = (string)myJObject["value"][i]["Reviewer"];
                model.CreatedBy = (string)myJObject["value"][i]["CreatedBy"];
                filepdfpath = (string)myJObject["value"][i]["path"];
                model.CreatedDate = (string)myJObject["value"][i]["CreatedDate"];
            }

            //if(model.TimeFrameoption.Trim() == "Once -Off Payment")
            //{
                AzureTablesData.GetEntity(StorageName, StorageKey, "cicform1", model.CertificateNo, out jsonData);

                JObject myJObject1 = JObject.Parse(jsonData);
                int cntJson1 = myJObject1["value"].Count();
                string certNo = "";

                for (int i = 0; i < cntJson1; i++)
                {
                    certNo = (string)myJObject1["value"][i]["CertificateNo"];
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
                tempPath1 = "Files/" + "LEVY CLEARANCE CERTIFICATE.pdf";

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

                pdfFormFields.SetField("Project Registration Number", regNoName);
                pdfFormFields.SetField("Project Name Give name of project as it appears in the contract document or letter of award", model.CompanyName.ToUpper());
                pdfFormFields.SetField("Project Location", model.Place.ToUpper());
                pdfFormFields.SetField("Project Owner s", model.CompanyName.ToUpper());
                pdfFormFields.SetField("undefined", model.PostalAddress.ToUpper());
                pdfFormFields.SetField("undefined_2", model.CompanyName.ToUpper()); //contractor name
                pdfFormFields.SetField("Contractor CIC Registration Certificate No", certNo);// cert
                pdfFormFields.SetField("Text17", date.Substring(0, 1)); //d
                pdfFormFields.SetField("Text18", date.Substring(1, 1));// d
                pdfFormFields.SetField("Text19", month.Substring(0, 1)); //d
                pdfFormFields.SetField("Text20", month.Substring(1, 1));// d
                pdfFormFields.SetField("Text21", year.Substring(0, 1)); //d
                pdfFormFields.SetField("Text22", year.Substring(1, 1));// d
                pdfFormFields.SetField("Text23", year.Substring(2, 1)); //d
                pdfFormFields.SetField("Text24", year.Substring(3, 1));// d
                pdfFormFields.SetField("Text33", date1.Substring(0, 1)); //d
                pdfFormFields.SetField("Text34", date1.Substring(1, 1));// d
                pdfFormFields.SetField("Text35", month1.Substring(0, 1)); //d
                pdfFormFields.SetField("Text36", month1.Substring(1, 1));// d
                pdfFormFields.SetField("Text37", year1.Substring(0, 1)); //d
                pdfFormFields.SetField("Text38", year1.Substring(1, 1));// d
                pdfFormFields.SetField("Text39", year1.Substring(2, 1)); //d
                pdfFormFields.SetField("Text40", year1.Substring(3, 1));// d            
                pdfFormFields.RemoveField("Receipt No");

                pdfStamper.FormFlattening = true;

                // close the pdf  
                pdfStamper.Close();
                byte[] bytes = System.IO.File.ReadAllBytes(path);

                BlobStorageService objBlobService = new BlobStorageService();

                string filepath = objBlobService.UploadFileToBlob(tempPath, bytes, "application/pdf", filepdfpath);

                string pdfnameServer = filepdfpath + @"\Files\Certificate_" + regNoName + ".pdf";
                string CertName = "Certificate_" + regNoName + ".pdf";
                files.Add(new CertificateModel { FilePath = pdfnameServer, FileName = CertName, emailTo = model.CreatedBy, grade = grade });
                memoryCache.Set("CertFiles", files);
                model.CertificateNo = regNoName;
                model.FormStatus = "Finished";
                memoryCache.Set("Form9Model", model);
            //}

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
    }
}
