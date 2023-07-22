
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Form3Model 
    {
        public string FormName { get; set; }
        public int FormRegistrationNo { get; set; }
        public string FormStatus { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string CreatedDate { get; set; }
        //Type of Application details
        public string AppType { get; set; }

        //Business  details 
        public string NameOFJoinVenture { get; set; }
        public string TypeofJoointVenture { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")] 
        public string Email { get; set; }
        public string Phyaddress { get; set; }
        public string FirstName { get; set; }
        public string NameType { get; set; }
        public string SurName { get; set; }
        public string Designation { get; set; }
        public string BusinessTelephone { get; set; }
        public string FaxNo { get; set; }
        public string MobileNo { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string BusinessEmail { get; set; }
        

        public string Category { get; set; }
        public int CategoryId { get; set; }
        public int Subcatogory { get; set; }
        public string SubcatogoryId { get; set; }

        
        //------ Particulars of Joint Venture Parties
        public string NameofApplicant { get; set; }
        public string CountryOfOrigin { get; set; }
        public string ContactDetails { get; set; }
        public string CICRegistrationNo { get; set; }
        public int Shareholding { get; set; }
        //public IFormFile AttchedDoc { get; set; }


        //------ Sub-contractors to be employed on the Project
        public string NameofContractor { get; set; }

        public string CountryyofOrigin { get; set; }

        public string CICRegistrationNoConsultant { get; set; }

        public string DescriptionOfWork { get; set; }

        public decimal ContractValueOfWork { get; set; }

        public int customerCheck { get; set; }

        public string WitnessedName { get; set; }
        public string WitnessedSignatureimage { get; set; }
        public string WitnessedTitleDesignation { get; set; }


        //------ Project Details Section
        public string BidReferenceNo { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime DateofAward { get; set; }
        public DateTime CommencementDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string DescriptionofProject { get; set; }
        public string ClientName { get; set; }
        public decimal ContractValue { get; set; }


        //------- Declaration Documentupload section 
        public string Name { get; set; }
        public IFormFile Signature { get; set; }
        public string TitleDesignation { get; set; }
        public bool TermsAndConditions { get; set; }
        public IFormFile BusineesParticularsfile1 { get; set; }
        public IFormFile BusineesParticularsfile2 { get; set; }
        //public string WitnessedName { get; set; }
        //public IFormFile WitnessedSignature { get; set; }
        //public string WitnessedTitle { get; set; }
        public string ImagePath { get; set; }


        public string path { get; set; }
        public string Reviewer { get; set; }
        public string comment { get; set; }
        public string CreatedBy { get; set; }
        public List<ParticularsofJointVentureParties> JointVenturePartiesModel { get; set; }
        public List<TechnicalAdministrativeStaff> TechnicalAdministrativeStaffModel { get; set; }
        public List<ProjectStaff> projectStaffModel { get; set; }
        public List<LabourForce> LabourForceModel { get; set; }
        public List<SubContractors> SubContractorModel { get; set; }

        public string BusineesParticularsfile1Name { get; set; }
        public string BusineesParticularsfile2Name { get; set; }
        public string SignatureName { get; set; }
        public string Signature2Name { get; set; }
        public string CertificateNo { get; set; }

        public string JVGrade { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public string RegistrationID { get; set; }
    }
}
