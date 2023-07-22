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
    public class CICForm4Model
    {
       
        public ApplicationTypeModel4 App { get; set; }

        public BusinessDetailsModel4 businessModel { get; set; }

        public List<DirectorshipShareDividends4> Sharelist { get; set; }

        public Documents4 docs { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public int c { get; set; }
        public string CreatedDate { get; set; }
        public int FormRegistrationNo { get; set; }
        public string FormStatus { get; set; }
        public string err { get; set; }
        public string Reviewer { get; set; }
        public string CreatedBy { get; set; }
        public string formval { get; set; }
        public string FormName { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }

    }

    public class ApplicationTypeModel4
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }

        [Required(ErrorMessage = "Please enter Association Name")]
        public string AssociationName { get; set; }

        [Required(ErrorMessage = "Please enter Authorized Officer Name")]
        public string AuthorisedOfficerName { get; set; }

        //[Required(ErrorMessage = "Please add Signature")]
        //[DataType(DataType.Upload)]
        public IFormFile Filesignature { get; set; }

        //[Column(TypeName = "varchar(250)")]
        public string ImagePath { get; set; }

        public string signaturefilename { get; set; }

    }

    public class ApplicationTypeList4
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public Boolean Selected { get; set; }
    }

    public class BusinessDetailsModel4
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

        //[Required(ErrorMessage = "Select at least 1 category")]
        public List<categoryType> Category { get; set; }
        public List<subCategory> subCategoryModel { get; set; }
       // public string subCategoryName { get; set; }
        public int selectedsubcategory { get; set; }
        
        
        public string SelectedCategoryValue { get; set; }

        //[CategoryValidation(ErrorMessage = "Please choose level of registration")]
        //public List<SubCategoryType> subCategory { get; set; }

      

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

        //[Required(ErrorMessage = "Please upload Business Representative signature")]
        public IFormFile Businesssignature { get; set; }

        //[Column(TypeName = "varchar(250)")]
        //public string ImagePath { get; set; }
        public string BusinessFileSignatureName { get; set; }

    }
    public class subCategory
    {
        [Key]
        public int SubCategoryID { get; set; }

        public int CategoryID { get; set; }

        public string SubCategoryName { get; set; }

        public Boolean selected { get; set; }
    }
    public class DirectorshipShareDividends4
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "Please enter Director Name",AllowEmptyStrings =false)]
        public string DirectorName { get; set; }

        [Required(ErrorMessage = "Please select Nationnality", AllowEmptyStrings = false)]
        public string Nationnality { get; set; }

        [Required(ErrorMessage = "Please enter Qualifications", AllowEmptyStrings = false)]
        public string Qualifications { get; set; }

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
        
        //[Required(ErrorMessage = "Please upload File", AllowEmptyStrings = false)]
        public IFormFile ShareFile { get; set; }

       


    }

    public class BusinessTypeCheckboxList4
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public Boolean Selected { get; set; }
    }

   

    public class associationType4
    {
        [Key]
        public int AssociationId { get; set; }
        public string AssociationName { get; set; }
        public Boolean Selected { get; set; }
    }

    public class SubCategoryType4
    {
        [Key]
        public int SubCategoryID { get; set; }

        public int CategoryID { get; set; }

        public string SubCategoryName { get; set; }

        [NotMapped]public Boolean selected { get; set; }

    }

    public class Documents4
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

        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile BusinessFile7 { get; set; }

        //Company Shareholders/Directors/Technical Staff Particulars Files
        //[Required(ErrorMessage = "Please upload File")]
        public IFormFile ShareholdersFile1 { get; set; }

        
        //Foreign Files
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

       // [Required(ErrorMessage = "Please upload File")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public IFormFile Signature2 { get; set; }

        //[Required]
        public string WitnessedTitle { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]        
        public bool TermsAndConditions { get; set; }

        //[Column(TypeName = "varchar(250)")]
        //public string ImagePath { get; set; }

        //FileUploadShow                
        public string BusinessFile1Name { get; set; }

        public string BusinessFile2Name { get; set; }

        public string BusinessFile3Name { get; set; }

        public string BusinessFile4Name { get; set; }

        public string BusinessFile5Name { get; set; }

        public string BusinessFile6Name { get; set; }

        public string ShareholdersFile1Name { get; set; }

        public string BusinessFile7Name { get; set; }
        public string TaxLawName { get; set; }

        public string EvidenceName { get; set; }

        public string ComplianceName { get; set; }

        public string Signature1Name { get; set; }

        public string Signature2Name { get; set; }
    }
}
