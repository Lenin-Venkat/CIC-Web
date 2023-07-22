using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class SaveModelForm6
    {

        public string FormName { get; set; }
        public int FormRegistrationNo { get; set; }
        public string FormStatus { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
       // public int FormRegistrationNo { get;set }
        //Type of Application details
        public string AppType { get; set; }
        public string Categories { get; set; }
        public string AssociationName { get; set; }
        public string AuthorisedOfficerName { get; set; }
        public IFormFile Filee { get; set; }
        //Personel details 
      
        public string Name { get; set; }
      
        public string Surname { get; set; }
      
        public string IDNo { get; set; }
       
        public string Nationality { get; set; }
      
        public string HomeArea { get; set; }
      
        public string Chief { get; set; }
       
        public string Indvuna { get; set; }
      
        public string TemporalResidencePermitNo { get; set; }
      
        public string WorkPermitNo { get; set; }
      
        public string CellphoneNo { get; set; }
      
        public string Emailaddress { get; set; }
       
        public string FaxNo { get; set; }
      
        public string ResidentialAddress { get; set; }
       
        public string NextofKin { get; set; }
       
        public string Relationship { get; set; }
      
        public string ContactNo { get; set; }
        public string NatureofTradeExpertise { get; set; }
     
        public IFormFile NatureofTradeUpload { get; set; }
        //Education and Background Section
       
        public string HighestEducationLevel { get; set; }
      
        public string QualificationsAttained { get; set; }
       
        public string EducationInstitution { get; set; }
       
        public string NamesandSurname { get; set; }
       
        public string TypeofWorkPerformed { get; set; }
       
        public string TelephoneNo { get; set; }
        //Declaration Documentupload section  
        public IFormFile fileupload1 { get; set; }
        public IFormFile fileupload2 { get; set; }
        public IFormFile fileupload3 { get; set; }
        public IFormFile fileupload4 { get; set; }
        public IFormFile fileupload5 { get; set; }
        public IFormFile fileupload6 { get; set; }
        public int customerCheck { get; set; }
        
        public string WitnessedName { get; set; }
        public string WitnessedSignatureimage { get; set; }
        public string WitnessedSignatureTitleDesignation { get; set; }
      
        public string ImagePath { get; set; }
       
        public IFormFile File { get; set; }

        public string Namee { get; set; }
        public IFormFile Signature { get; set; }
        public string TitleDesignation { get; set; }
        public List<EducationBackGroundetails> educationBackground { get; set; }
        public List<BackGroundetails> backGroundetails { get; set; }

        public string Reviewer { get; set; }

        public string comment { get; set; }

        public string path { get; set; }

        public string CreatedBy { get; set; }
        public string FileeName { get; set; }

        public string NatureofTradeUploadName { get; set; }

        public string fileupload1Name { get; set; }

        public string fileupload2Name { get; set; }
        public string SignatureName { get; set; }

        public string fileupload3Name { get; set; }

        public string fileupload6Name { get; set; }
        public string CreatedDate { get; set; }
        public string CertificateNo { get; set; }

        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
    }
}
