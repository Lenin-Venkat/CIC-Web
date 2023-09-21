using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class SaveModelForm5
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

        //Business  details 
       
        public string NameOFJoinVenture { get; set; }
       
        public string TypeofJoointVenture { get; set; }
      
        public string Telephone { get; set; }
       
        public string Fax { get; set; }
      
        public string Email { get; set; }
       
        public string Phyaddress { get; set; }
       
        public string FirstName { get; set; }
      
        public string NameType { get; set; }
       
        public string SurName { get; set; }
       
        public string Designation { get; set; }
        
        public string BusinessTelephone { get; set; }
       
        public string FaxNo { get; set; }
      
        public string MobileNo { get; set; }
       
        public string BusinessEmail { get; set; }
       
        public DateTime DateofRegistration { get; set; }
       
        public string TaxIdentityNo { get; set; }
        
        public string  Category { get; set; }
       // public int CategoryId { get; set; }
       public int Subcatogory { get; set; }
        public string SubcatogoryId  { get; set; }

        //Details Of Projects
        //First Section
       
        public string NameofApplicant { get; set; }
        
        public string CountryOfOrigin { get; set; }
       
        public string ContactDetails { get; set; }
       
        public string CICRegistrationNo { get; set; }
       
        public int Shareholding { get; set; }
       
        public IFormFile AttchedDoc { get; set; }
        //Second Section
       
        public string NameofConsultant { get; set; }
       
        public string CountryyofOrigin { get; set; }
       
        public string CICRegistrationNoConsultant { get; set; }
       
        public string DescriptionOfWork { get; set; }
      
        public string ContractValueOfWork { get; set; }

        //Declaration Documentupload section  
        public IFormFile fileupload1 { get; set; }
        public IFormFile fileupload2 { get; set; }
       
        public int customerCheck { get; set; }

        public string WitnessedName { get; set; }
        public string WitnessedSignatureimage { get; set; }
        public string WitnessedTitleDesignation { get; set; }

        public string ImagePath { get; set; }

        public IFormFile File { get; set; }

        public string Namee { get; set; }
        public IFormFile Signature { get; set; }
        public string TitleDesignation { get; set; }
        public List<DetailOfProjects> detailOfProjects { get; set; }
        public List<SubConsultantDetail> subConsultantDetail { get; set; }
        public string path { get; set; }
        public string Reviewer { get; set; }
        public string comment { get; set; }
        public string CreatedBy { get; set; }

        public string SignatureName { get; set; }
        public string BusineesParticularsfile1Name { get; set; }
        public string BusineesParticularsfile2Name { get; set; }
        public string Signature2Name { get; set; }
        public string CreatedDate { get; set; }

        public string CertificateNo { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public string RegistrationID { get; set; }
        public string Grade { get; set; }
    }
}
