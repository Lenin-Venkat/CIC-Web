using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static CICLatest.Helper.CustomValidations;

namespace CICLatest.Models
{
    public class CICForm1Model
    {
        public string FormName { get; set; }
        public ApplicationTypeModel App { get; set; }

        public BusinessDetailsModel businessModel { get; set; }

        public List<DirectorshipShareDividends> Sharelist { get; set; }

        public FinancialCapability financialCapabilityModel { get; set; }

        [ApplicantValidation(ErrorMessage ="Applicant Bank details are mandatory")]
        public List<ApplicantBank> applicantBank { get; set; }

        public List<WorksCapability> worksCapability { get; set; }

        public Documents docs { get; set; }

        public string formval { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public int c { get; set; }
        public int cFin { get; set; }

        public int cWork { get; set; }

        public Int64 FirmRegistrationNo { get; set; }

        public string FormStatus { get; set; }

        public string err { get; set; }

        public string Grade { get; set; }

        public string Reviewer { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public string GradeStr { get; set; }
        public string ScoreStr { get; set; }

        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public string newGradecomment { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public string RegistrationID { get; set; }

        public Boolean ReadOnlyField1 { get; set; }
    }

    public class ApplicationTypeModel
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }

        //[Required(ErrorMessage = "Please enter Association Name")]
        public string AssociationName { get; set; }

        [Required(ErrorMessage = "Please enter Authorized Officer Name")]
        public string AuthorisedOfficerName { get; set; }

       
        [DataType(DataType.Upload)]
        //[Required(ErrorMessage = "Please add Signature")]
        public IFormFile Filesignature { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string ImagePath { get; set; }

        //[Required(ErrorMessage = "Please add Signature")]
        public string signaturefilename { get; set; }
    }

    public class ApplicationTypeList
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public Boolean Selected { get; set; }
    }

    public class BusinessDetailsModel
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

        public string FaxNo { get; set; }

        [Required(ErrorMessage = "Please enter Email")]
        [EmailAddress]
        public string Email { get; set; }

        [CategoryValidation(ErrorMessage = "Select at least 1 category")]
        public List<categoryType> Category { get; set; }

        public List<subCategory> subCategoryBuilding { get; set; }
        public List<subCategory> subCategoryCivil { get; set; }
        public List<subCategory> subCategoryElect { get; set; }
        public List<subCategory> subCategoryMech { get; set; }
        public int selectedBuildingSubcategory { get; set; }
        public int selectedCivilSubcategory { get; set; }
        public int selectedMechSubcategory { get; set; }
        public int selectedElectSubcategory { get; set; }

        public List<SubCategoryType> subCategory { get; set; }

        [Required(ErrorMessage = "Please enter Present Grade Registered for")]
        public string PresentGrade { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Name")]
        public string BusinessRepresentativeName { get; set; }

        [Required(ErrorMessage = "Please enter Position")]
        public string BusinessRepresentativePositionNumber { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Cell Number")]
        public string BusinessRepresentativeCellNo { get; set; }

        public string BusinessRepresentativeFax { get; set; }

        [Required(ErrorMessage = "Please enter Business Representative Email")]
        [EmailAddress]
        public string BusinessRepresentativeEmail { get; set; }

       // [Required(ErrorMessage = "Please upload Business Representative signature")]
        public IFormFile Businesssignature { get; set; }

        public string BusinessFileSignatureName { get; set; }

        // public List<DirectorshipShareDividends> DirectorShareDividends { get; set; }

        //public string formval { get; set; }

    }

    public class DirectorshipShareDividends
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "Please enter Director Name",AllowEmptyStrings =false)]
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

        public string ShareFileName { get; set; }
    }

    public class BusinessTypeCheckboxList
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public Boolean Selected { get; set; }
    }

    public class categoryType
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public Boolean Selected { get; set; }
    }

    public class SubCategoryType
    {
        [Key]
        public int SubCategoryID { get; set; }

        public int CategoryID { get; set; }

        public string SubCategoryName { get; set; }

        [NotMapped]public Boolean selected { get; set; }

       


    }

    public class FinancialCapability
    {
        [Required(ErrorMessage = "Please enter Annual Turnover for Year 1")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999999999.99)]
        public decimal AnnualTurnoverYear1 { get; set; }

        [Required(ErrorMessage = "Please enter Annual Turnover for Year 2")]
        [Range(0, 9999999999999999.99)]
        public decimal AnnualTurnoverYear2 { get; set; }

        [Required(ErrorMessage = "Please enter Annual Turnover for Year 3")]
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

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile StatmentFile { get; set; }

        public string statementFilename { get; set; }
    }

    public class ApplicantBank
    {
        //[Required (AllowEmptyStrings =false)]
        public string BankName { get; set; }

        //[Required(AllowEmptyStrings = false)]        
        public string BranchName { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string BranchCode { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string AccountHoulderName { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public int AccountNo { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string AccountTYpe { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string TelephoneNo { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class WorksCapability
    {
        [Required(ErrorMessage = "Please enter Project Name", AllowEmptyStrings = false)]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Please enter Location", AllowEmptyStrings = false)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Please enter Completion Date", AllowEmptyStrings = false)]
        public DateTime? CompletionDate { get; set; }

        [Required(ErrorMessage = "Please enter Contract Sum", AllowEmptyStrings = false)]
        [Range(0, 9999999999999999.99)]
        public decimal ContractSum { get; set; }

        [Required(ErrorMessage = "Please enter Telephone No", AllowEmptyStrings = false)]
        public string TelephoneNo { get; set; }

        [Required(ErrorMessage = "Please enter Type of involvement", AllowEmptyStrings = false)]
        public string TypeofInvolvement { get; set; }

        [Required(ErrorMessage = "Please enter Registation No", AllowEmptyStrings = false)]
        public string RegistationNo { get; set; }
        // public string formval { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Documents
    {
        //Business Particulars Files
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile1 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile2 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile3 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile4 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile5 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile6 { get; set; }

        //Company Shareholders/Directors/Technical Staff Particulars Files
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile ShareholdersFile1 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile ShareholdersFile2 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile ShareholdersFile3 { get; set; }

        //Financial Requirements Files
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile FinancialFile1 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile FinancialFile2 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile FinancialFile3 { get; set; }

        //Track record
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile TrackRecordFile1 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile TrackRecordFile2 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile TrackRecordFile3 { get; set; }

        //Joint venture files
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile JointVentureFile1 { get; set; }


        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile JointVentureFile2 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile JointVentureFile3 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile JointVentureFile4 { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile TaxLaw { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile Evidence { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile Compliance { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile Signature1 { get; set; }

        [Required]
        public string Title { get; set; }

        //[Required]
        public string WitnessedName { get; set; }
       
        public IFormFile Signature2 { get; set; }

        //[Required]
        public string WitnessedTitle { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]
        [Required]
        public bool TermsAndConditions { get; set; }


        //FileUploadShow                
        public string BusinessFile1Name { get; set; }

        public string BusinessFile2Name { get; set; }

        public string BusinessFile3Name { get; set; }

        public string BusinessFile4Name { get; set; }

        public string BusinessFile5Name { get; set; }

        public string BusinessFile6Name { get; set; }

        public string ShareholdersFile1Name { get; set; }

        public string ShareholdersFile2Name { get; set; }

        
        public string ShareholdersFile3Name { get; set; }

        public string FinancialFile1Name { get; set; }

        public string FinancialFile2Name { get; set; }

        public string FinancialFile3Name { get; set; }

        public string TrackRecordFile1Name { get; set; }

        public string TrackRecordFile2Name { get; set; }

        public string TrackRecordFile3Name { get; set; }

        public string JointVentureFile1Name { get; set; }


        public string JointVentureFile2Name { get; set; }

        public string JointVentureFile3Name { get; set; }

        public string JointVentureFile4Name { get; set; }

        public string TaxLawName { get; set; }

        public string EvidenceName { get; set; }

        public string ComplianceName { get; set; }

        public string Signature1Name { get; set; }

        public string Signature2Name { get; set; }
    }

    public class FileList
    {
        public string FileKey { get; set; }
        public string FileValue { get; set; }
    }

    public class AssociationList
    {
       
        public string AssociationName { get; set; }

        public string AssociationName1 { get; set; }



    }
}
