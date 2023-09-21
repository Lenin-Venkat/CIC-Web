using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class SaveForm7Model 
    {

        public string FormName { get; set; }
        public string FormStatus { get; set; }
        //Application type
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public int FormRegistrationNo { get; set; }
        //Type of Application details
        public string AppType { get; set; }
       // public string Categories { get; set; }
        public string AssociationName { get; set; }
        public string AuthorisedOfficerName { get; set; }
        public IFormFile Filee { get; set; }
        //Business Details.

      
        public string BusinessName { get; set; }


        
        public string TradingStyle { get; set; }

       
        public string BusinessType { get; set; }


       
        public string Other { get; set; }

       
        public DateTime CompanyRegistrationDate { get; set; }

       
        public string CompanyRegistrationPlace { get; set; }

       
        public string CompanyRegistrationNumber { get; set; }

        
        public string PhysicalAddress { get; set; }

       
        public string CompanyHOPhysicalAddress { get; set; }

       
        public string PostalAddress { get; set; }

       
        public string TelephoneNumber { get; set; }

      
        public string FaxNo { get; set; }

     
        public string Email { get; set; }

        public string WorkDisciplineType { get; set; }

        public string BusinessRepresentativeName { get; set; }

       
        public string BusinessRepresentativePositionNumber { get; set; }

       
        public string BusinessRepresentativeCellNo { get; set; }

        
        public string BusinessRepresentativeFax { get; set; }

       
        public string BusinessRepresentativeEmail { get; set; }

       
        public IFormFile Signature { get; set; }

        //

      
        public string DirectorName { get; set; }
  
        public string Nationnality { get; set; }

     
        public string IdNO { get; set; }

        
        public string Country { get; set; }

       
        public string CellphoneNo { get; set; }

        public int SharePercent { get; set; }

        //3 Tab
        public string StateDetailsOfMaterials { get; set; }


       
        public string StateOfAttainent { get; set; }

        //List of ListOf Client
        public string NameofClient { get; set; }

        public string ServiceProvided { get; set; }

        public string DateOrPeriod { get; set; }

        public string ContractValue { get; set; }
        //Tab4 Finiancial capability

      
        public decimal AnnualTurnoverYear1 { get; set; }

      
        public decimal AnnualTurnoverYear2 { get; set; }

      
        public decimal AnnualTurnoverYear3 { get; set; }

      
        public decimal FinancialValue { get; set; }

     
        public string FinancialInstitutionName { get; set; }

      
        public decimal AvailableCapital { get; set; }
        //grid details
        public string BankName { get; set; }

        //[Required]
        public string BranchName { get; set; }

        //[Required]
        public string BranchCode { get; set; }

        //[Required]
        public string AccountHoulderName { get; set; }

        // [Required]
        public int AccountNo { get; set; }

        // [Required]
        public string AccountTYpe { get; set; }

        //[Required]
        public string TelephoneNo { get; set; }

        //Document upload 
      
        public IFormFile BusinessFile1 { get; set; }

        public IFormFile BusinessFile2 { get; set; }

       
        public IFormFile BusinessFile3 { get; set; }

       
        public IFormFile BusinessFile4 { get; set; }

       
        public IFormFile BusinessFile5 { get; set; }

       
        public IFormFile BusinessFile6 { get; set; }

        
        public IFormFile ShareholdersFile1 { get; set; }

       
        public IFormFile ShareholdersFile2 { get; set; }

      
        public IFormFile ShareholdersFile3 { get; set; }

        public IFormFile FinancialFile1 { get; set; }

       
        public IFormFile FinancialFile2 { get; set; }

      
        public IFormFile FinancialFile3 { get; set; }

        
        public IFormFile TrackRecordFile1 { get; set; }

       
        public IFormFile TrackRecordFile2 { get; set; }

       
        public IFormFile TrackRecordFile3 { get; set; }

        
        public IFormFile JointVentureFile1 { get; set; }


        
        public IFormFile JointVentureFile2 { get; set; }

       
        public IFormFile JointVentureFile3 { get; set; }

        
        public IFormFile JointVentureFile4 { get; set; }

        
        public string Name { get; set; }

        
        public IFormFile Signature1 { get; set; }

        
        public string Title { get; set; }

        
        public string WitnessedName { get; set; }

       
        public IFormFile Signature2 { get; set; }

        
        public string WitnessedTitle { get; set; }

        //Declaration section
      
        public string Namee { get; set; }
      
        public IFormFile Signaturee { get; set; }
     
        public string TitleDesignation { get; set; }
        public List<DirectorshipShareDividendsSection> Sharelist { get; set; }
        public List<ListOfPreviousClient> listOfPrevousClent { get; set; }
        public List<CompanyBank> companyBank { get; set; }
        public string Reviewer { get; set; }
        public string comment { get; set; }
        public string path { get; set; }
        public string CreatedBy { get; set; }

        public string SignatureName { get; set; }
        public string Signature2Name { get; set; }
        public string AssociationCertificateAttachmentName { get; set; }
        public string Signature3Name { get; set; }
        public string companyRegistrationName { get; set; }
        public string shraeCertificateName { get; set; }
        public string TradingLicenseName { get; set; }
        public string formjName { get; set; }
        public string FormCName { get; set; }
        public string RegistrationCertificateName { get; set; }
        public string CertifiedCompanyRegistrationName { get; set; }
        public string CertifiedIdentityDocumentsName { get; set; }
        public string RenewBusinessScopeOfworkName { get; set; }
        public string RenewCertificateName { get; set; }
        public string RenewFormJName { get; set; }
        public string ReNewFormCName { get; set; }
        public string RenewFormBMCAName { get; set; }
        public string RenewFinancialRequirmentName { get; set; }
        public string RenewIdentificationName { get; set; }
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
