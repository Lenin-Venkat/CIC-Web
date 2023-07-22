using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static CICLatest.Helper.CustomValidations;

namespace CICLatest.Models
{
    public class Cicf5Model
    {
        public string WorkDiscipline { get; set; }
        public List<categoryType> Category { get; set; }
        public Declaration5 declaration5 { get; set; }
        public Doc5 doc5 { get; set; }
        public ApplicationType5 App { get; set; }
        public Businessdetails5 businessdetails5 { get; set; }
        public int firsGrid { get; set; }

        public int secondGrid { get; set; }
        public string FormStatus { get; set; }
        //public ApplicationType5 App { get; set; }
        public List<SubCategoryType> subCategory { get; set; }
        public List<DetailOfProjects> detailOfProjects { get; set; }
        public List<SubConsultantDetail> subConsultantDetail { get; set; }
        public string Select { get; set; }
       
        public string Reviewer { get; set; }
        public string CreatedBy { get; set; }
        //for subcategory 
        public int selectedsubcategory { get; set; }//For subcategory
        public List<subCategory> subCategoryModel { get; set; }
        //For edit 
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }
        public string FormName { get; set; }
        public string formval { get; set; }
        public string ImagePath { get; set; }
        //file label
        public string BusineesParticularsfile1 { get; set; }
        public string BusineesParticularsfile2 { get; set; }
        public string Signature { get; set; }
        public string Signature1 { get; set; }
        public string CreatedDate { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }

    }
    public class ApplicationType5
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }
       
    }
    public class Businessdetails5
    {
        [Required(ErrorMessage = "Please Enter Name of Join Venture.")]
        public string NameOFJoinVenture { get; set; }
        [Required(ErrorMessage = "Please select Type of Join Venture.")]
        public string TypeofJoointVenture { get; set; }
        [Required(ErrorMessage = "Please Enter Telephone")]
        public string Telephone { get; set; }
        //[Required(ErrorMessage = "Please Enter Fax.")]
        public string Fax { get; set; }
        [Required(ErrorMessage = "Please Enter Email.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Physical address.")]
        public string Phyaddress { get; set; }
        //[Required(ErrorMessage = "Please Enter Authorized Person.")]
        //public string AuthorizedPerson { get; set; }
        [Required(ErrorMessage = "Please Enter First Name.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please Enter Name Type")]
        public string NameType { get; set; }
        [Required(ErrorMessage = "Please Enter SurName")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation  { get; set; }
        [Required(ErrorMessage = "Please Enter Business Telephone")]
        public string BusinessTelephone { get; set; }
        //[Required(ErrorMessage = "Please Enter FaxNo")]
        public string FaxNo { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile No")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Please Enter Business Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string BusinessEmail { get; set; }
        [Required(ErrorMessage = "Please Enter Date of Registration")]
        public DateTime DateofRegistration { get; set; }
        [Required(ErrorMessage = "Please Enter Tax Identity No")]
        public string TaxIdentityNo{ get; set; }
        //[Required(ErrorMessage = "Please Select Category Applying For")]
        //public string CategoryApplyingFor { get; set; }
        //[Required(ErrorMessage = "Please Select Subcategory")]
        //public string Subcategory { get; set; }
        
      // public string SelectedValue { get; set; }
        //  [CategoryValidation(ErrorMessage = "Select at least 1 category")]
       // [Required(ErrorMessage = "Please Select Category Applying For")]
      
    }
    public class DetailOfProjects
    {
        [Required(ErrorMessage = "Please Enter Name of Applicant.")]
        public string NameofApplicant{ get; set; }
        [Required(ErrorMessage = "Please Enter Country of Origin.")]
        public string CountryOfOrigin { get; set; }
        [Required(ErrorMessage = "Please Enter Contact Details.")]
        public string ContactDetails { get; set; }
        [Required(ErrorMessage = "Please Enter CIC Registration No")]
        public string CICRegistrationNo { get; set; }
        // [Required(ErrorMessage = "Please Enter Share holding")]

        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a number")]
        [RegularExpression(@"^\d+$")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [Range(1, 100, ErrorMessage = "Please use values between 1 to 100 for % Shares")]
        public int Shareholding { get; set; }
        //[Required(ErrorMessage = "Please Upload Document")]
       // public IFormFile AttchedDoc { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public class SubConsultantDetail
    {
        [Required(ErrorMessage = "Please Enter Name of Consultant")]
        public string NameofConsultant{ get; set; }
        [Required(ErrorMessage = "Please Enter Country of Origin.")]
        public string CountryyofOrigin{ get; set; }
        [Required(ErrorMessage = "Please Enter CIC Registration No")]
        public string CICRegistrationNo { get; set; }
        [Required(ErrorMessage = "Please Enter Description of Work")]
        public string DescriptionOfWork { get; set; }
        [Required(ErrorMessage = "Please Enter Contract Value of Work")]
        [Range(0, 9999999999999999.99)]
        public decimal ContractValueOfWork{ get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public class Doc5
    {
        [Required(ErrorMessage = "Please add Name")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Please add Signature")]
        public IFormFile Signature { get; set; }
        [Required(ErrorMessage = "Please add TitleDesignation")]
        public string TitleDesignation { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]
        public bool TermsAndConditions { get; set; }

        //[Required(ErrorMessage = "Please upload file")]
        public IFormFile BusineesParticularsfile1{ get; set; }
       // [Required(ErrorMessage = "Please upload file")]
        public IFormFile BusineesParticularsfile2{ get; set; }
       
    }
    public class Declaration5
    {

        public string Name { get; set; }
      
        public IFormFile Signature { get; set; }
      
        public string TitleDesignation { get; set; }
     
    }
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }

        public int CategoryID { get; set; }

        public string SubCategoryName { get; set; }
    }
}
