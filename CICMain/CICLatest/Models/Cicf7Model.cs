using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Cicf7Model
    {
        //section First Tab
        public ApplicationType App { get; set; }
        //section 2second tab
        public BusinessDetails businessModel { get; set; }
        public List<DirectorshipShareDividendsSection> Sharelist { get; set; }


        //Technical Data and list of Previous project third tab

        //section Finiancial Capability 4 th tab
        //5 Tab Document upload  and Declration section

        public TechnicalData technicalData { get; set; }
        public List<ListOfPreviousClient> listOfPrevousClent { get; set; }
        //public List<ListOfPreviousClient> listOfPrevousClentt { get; set; }
        public FinancialCapabilityForm7 financialCapabilityForm7 { get; set; }
        public List<CompanyBank> companyBank { get; set; }
        public DeclarationForm7 declarationForm7 { get; set; }
        public DocumentsUpload documentsUpload { get; set; }

        
        public string FormStatus { get; set; }
        public string formval { get; set; }
        public int c { get; set; }
       
        public int cFin { get; set; }

        public int cWork { get; set; }
        public string err { get; set; }
        public string ImagePath { get; set; }
        public string Reviewer { get; set; }
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        //form update section 
        public string FormName { get; set; }
        public int FirmRegistrationNo { get; set; }
        public string path { get; set; }
       
       // public string AssociationCertificateAttachment { get; set; }
        public string Signature3 { get; set; }
        public string shraeCertificate { get; set; }
        public string companyRegistration { get; set; }
        public string TradingLicense { get; set; }
        public string formj { get; set; }
        public string FormC { get; set; }
        public string RegistrationCertificate { get; set; }
        public string CertifiedCompanyRegistration { get; set; }
        public string CertifiedIdentityDocuments { get; set; }
        public string RenewBusinessScopeOfwork { get; set; }
        public string RenewCertificate { get; set; }
        public string RenewFormJ { get; set; }
        public string ReNewFormC { get; set; }
        public string RenewFormBMCA { get; set; }
        public string RenewFinancialRequirment { get; set; }
        public string RenewIdentification { get; set; }
        public string Signature { get; set; }
        public string Signature2 { get; set; }
        public string statementFilename { get; set; } //adding 
        public string ShareFileName { get; set; }
        public string CreatedDate { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }

    }
    public class ApplicationType
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }
        [Required(ErrorMessage = "Please enter AssociationName.")]
        public string AssociationName { get; set; }

        [Required(ErrorMessage = "Please enter Authorised Officer Name")]
        public string AuthorisedOfficerName { get; set; }
       
        [DataType(DataType.Upload)]
        public IFormFile AssociationCertificateAttachment { get; set; }
        public string AssociationCertificateName { get; set; }

    }
    public class BusinessDetails
    {
        [Required(ErrorMessage = "Please enter Business Name")]
        public string BusinessName { get; set; }


        [Required(ErrorMessage = "Please enter Trading style")]
        public string TradingStyle { get; set; }

        [Required(ErrorMessage = "Please select Business Type")]
        public string BusinessType { get; set; }


        [Required(ErrorMessage = "Other is mandatory")]
        public string Other { get; set; }

        [Required(ErrorMessage = "Please enter Company Registration Date")]
        public DateTime CompanyRegistrationDate { get; set; }

        [Required(ErrorMessage = "Please enter Place of Registration of Company")]
        public string CompanyRegistrationPlace { get; set; }

        [Required(ErrorMessage = "Please enter Company Registration Number")]
        public string CompanyRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please enter Physical Address")]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage = "Please enter Company Head Office Physical Address")]
        public string CompanyHOPhysicalAddress { get; set; }

        [Required(ErrorMessage = "Please enter Postal Address")]
        public string PostalAddress { get; set; }

        [Required(ErrorMessage = "Please enter Telephone Number")]
        public string TelephoneNumber { get; set; }

       // [Required]
        public string FaxNo { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }

      //  [SkillValidation(ErrorMessage = "Select at least 1 category")]
        //public List<categoryType> Category { get; set; }

        // public string CategoryId { get; set; }
        [Required(ErrorMessage = "Please select Work Discipline Applying for")]
        public string WorkDisciplineType { get; set; }

        //[Required(ErrorMessage = "Please enter Present Grade Registered for")]
        //public string PresentGrade { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Name")]
        public string BusinessRepresentativeName { get; set; }

        [Required(ErrorMessage = "Please enter Position")]
        public string BusinessRepresentativePositionNumber { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Cell Number")]
        public string BusinessRepresentativeCellNo { get; set; }

       // [Required]
        public string BusinessRepresentativeFax { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string BusinessRepresentativeEmail { get; set; }

       // [Required(ErrorMessage = "Please add Signature")]
         public IFormFile Signature { get; set; }
       
    }

    public class DirectorshipShareDividendsSection
    {
       
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "Please enter Director Name", AllowEmptyStrings = false)]
        public string DirectorName { get; set; }

        [Required(ErrorMessage = "Please select Nationnality", AllowEmptyStrings = false)]
        public string Nationnality { get; set; }

        [Required(ErrorMessage = "Please enter Id /Passport No", AllowEmptyStrings = false)]
        //[RegularExpression(@"(.{13})")]
        public string IdNO { get; set; }

        [Required(ErrorMessage = "Please select Country", AllowEmptyStrings = false)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter Cellphone number", AllowEmptyStrings = false)]
        public string CellphoneNo { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a number")]
        [RegularExpression(@"^\d+$")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [Range(1, 100, ErrorMessage = "Please use values between 1 to 100 for % Shares")]
        public int SharePercent { get; set; }

        [Required(ErrorMessage = "Please upload File", AllowEmptyStrings = false)]
        public IFormFile ShareFile { get; set; }
    }
    public class TechnicalData
    {
        
        [Required(ErrorMessage = "Please enter State Details Of Materials")]
        public string StateDetailsOfMaterials { get; set; }


        [Required(ErrorMessage = "Please enter State of Any Attainment")]
        public string StateOfAttainent { get; set; }

    }
    public class ListOfPreviousClient
    {
       
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        [Required(ErrorMessage = "Please enter Name of Client", AllowEmptyStrings = false)]
        public string NameofClient { get; set; }
        [Required(ErrorMessage = "Please enter Service Provided", AllowEmptyStrings = false)]
        public string ServiceProvided { get; set; }
        [Required(ErrorMessage = "Please enter Date or Period", AllowEmptyStrings = false)]
        public DateTime? DateOrPeriod { get; set; }
        [Required(ErrorMessage = "Please enter Contract Value", AllowEmptyStrings = false)]
        [Range(0, 9999999999999999.99)]
        public decimal ContractValue { get; set; }
       
    }
    public class FinancialCapabilityForm7
    {
        [Required(ErrorMessage = "Please enter Annual Turnover for Year 1")]
        [Range(0, 9999999999999999.99)]
        public decimal AnnualTurnoverYear1 { get; set; }

        // [Required(ErrorMessage = "Please enter Annual Turnover for Year 2")]
        [Range(0, 9999999999999999.99)]
        public decimal AnnualTurnoverYear2 { get; set; }

        // [Required(ErrorMessage = "Please enter Annual Turnover for Year 3")]
        [Range(0, 9999999999999999.99)]
        public decimal AnnualTurnoverYear3 { get; set; }

        [Required(ErrorMessage = "Please enter Financial Value of surety")]
        [Range(0, 9999999999999999.99)]
        public decimal FinancialValue { get; set; }

        [Required(ErrorMessage = "Please enter Financial Institution Name")]
        public string FinancialInstitutionName { get; set; }

        [Required(ErrorMessage = "Please enter Available Capital")]
        [Range(0, 9999999999999999.99)]
        public decimal AvailableCapital { get; set; }
        [Required(ErrorMessage = "Please upload File")]
        public IFormFile StatmentFile { get; set; }

       public string statementFilename { get; set; }

    }
    public class CompanyBank
    {

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
       // [Required(ErrorMessage = "Please enter Bank Name")]
        public string BankName { get; set; }

        //[Required(ErrorMessage = "Please enter Branch Name")]
        public string BranchName { get; set; }

        //[Required(ErrorMessage = "Please enter Branch Code")]
        public string BranchCode { get; set; }

        //[Required(ErrorMessage = "Please enter Account Holder Name")]
        public string AccountHoulderName { get; set; }

        //[Required(ErrorMessage = "Please enter Account No")]
        public int AccountNo { get; set; }

        //[Required(ErrorMessage = "Please enter Account Type")]
        public string AccountTYpe { get; set; }

        //[Required(ErrorMessage = "Please enter Telephone No")]
        public string TelephoneNo { get; set; }
    }
    public class DocumentsUpload
    {
     
       // // [Required]
        public string Name { get; set; }

       // [Required]
        public IFormFile Signature1 { get; set; }

       // [Required]
        public string Title { get; set; }

       //[Required]
        public string WitnessedName { get; set; }

       // [Required]
        public IFormFile Signature2 { get; set; }

        //[Required]
        public string WitnessedTitle { get; set; }
      

    }
    public class DeclarationForm7
    {

       
        //[Required(ErrorMessage = "Please add Name")]
        public string Namee { get; set; }
       // [Required(ErrorMessage = "Please add Signature")]
        public IFormFile Signature { get; set; }
       // [Required(ErrorMessage = "Please add TitleDesignation")]
        public string TitleDesignation { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]
        public bool TermsAndConditions { get; set; }
       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile companyRegistration { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile shraeCertificate { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile TradingLicense { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile formj { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile FormC { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile RegistrationCertificate { get; set; }


       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile CertifiedCompanyRegistration { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile CertifiedIdentityDocuments { get; set; }


        // Upload section in case renew Application

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewBusinessScopeOfwork { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewCertificate { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewFormJ { get; set; }

       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile ReNewFormC { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewFormBMCA { get; set; }



       // [Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewFinancialRequirment { get; set; }

      //  [Required(ErrorMessage = "Please upload File")]
        public IFormFile RenewIdentification { get; set; }
        public IFormFile StatmentFile { get; set; }

    }

}
