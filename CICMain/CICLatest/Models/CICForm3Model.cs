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
    public class CICForm3Model
    {
        public ApplicationTypeModel3 App { get; set; }

        public BusinessDetailsModel3 businessModel { get; set; }
        public int JVGridCnt { get; set; }
        public int CategoryGridCnt { get; set; }
        public int ProjectGrid { get; set; }
        public int forthGrid { get; set; }
        public int SubContractorGridCnt { get; set; }

        public string FormStatus { get; set; }
        public string ImagePath { get; set; }
        public string Reviewer { get; set; }
        public List<ParticularsofJointVentureParties> JointVenturePartiesModel { get; set; }
        public List<TechnicalAdministrativeStaff> TechnicalAdministrativeStaffModel { get; set; }
        public List<ProjectStaff> projectStaffModel { get; set; }
        public List<LabourForce> LabourForceModel { get; set; }
        public List<SubContractors> SubContractorModel { get; set; }
        public ProjectDetails projectDetailsModel { get; set; }
        public Doc3 doc3 { get; set; }
        public Declaration3 declaration3 { get; set; }

        public string formval { get; set; }
        public int FormRegistrationNo { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
    }
    public class ApplicationTypeModel3
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }
        [Required(ErrorMessage = "Please Enter Name of Joint Venture.")]
        public string NameOFJoinVenture { get; set; }

        [Required(ErrorMessage = "Please select Type of Joint Venture.")]
        public string TypeofJoointVenture { get; set; }

        [Required(ErrorMessage = "Please Enter Telephone.")]
        public string Telephone { get; set; }

        //[Required(ErrorMessage = "Please Enter Fax.")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "Please Enter Email.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Physical address.")]
        public string Phyaddress { get; set; }

        [Required(ErrorMessage = "Please Enter First Name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Title")]
        public string NameType { get; set; }

        [Required(ErrorMessage = "Please Enter Surname")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Please Enter Business Telephone")]
        public string BusinessTelephone { get; set; }

        //[Required(ErrorMessage = "Please Enter FaxNo")]
        public string FaxNo { get; set; }

        [Required(ErrorMessage = "Please Enter MobileNo")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please Enter Business Email")]
        [EmailAddress]
        public string BusinessEmail { get; set; }
    }

    public class BusinessDetailsModel3
    {

        //[CategoryValidation(ErrorMessage = "Select at least 1 category")]
        public List<categoryType> Category { get; set; }

        public int SelectedCategoryValue { get; set; }
        [CategoryValidation(ErrorMessage = "Please choose specialist category")]
        public List<SubCategoryType> subCategory { get; set; }

        public List<subCategory> subCategoryModel { get; set; }
        // public string subCategoryName { get; set; }
        public int selectedsubcategory { get; set; }

        public List<Business> Businesses { get; set; }
    }

    public class ParticularsofJointVentureParties
    {
        [Required(ErrorMessage = "Please Enter Name of Contractor.", AllowEmptyStrings = false)]
        public string NameofApplicant { get; set; }

        [Required(ErrorMessage = "Please Enter Business Name", AllowEmptyStrings = false)]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Please Enter Country Of Origin.", AllowEmptyStrings = false)]
        public string CountryOfOrigin { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Details.", AllowEmptyStrings = false)]
        public string ContactDetails { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a number")]
        [RegularExpression(@"^\d+$")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]

        [Range(1, 100, ErrorMessage = "Please use values between 1 to 100 for % Shares")]
        public int Shareholding { get; set; }

        [Required(ErrorMessage = "Please Enter CIC Registration No", AllowEmptyStrings = false)]
        public string CICRegistrationNo { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }


    }

    public class TechnicalAdministrativeStaff
    {
        public string Category { get; set; }
        public int Number { get; set; }

        public int YearsofExperience { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public int FormRegistrationNo { get; set; }
    }
    public class ProjectStaff
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public string StaffName { get; set; }

        public string StaffPosition { get; set; }

        public string StaffQualification { get; set; }

        public int StaffExperience { get; set; }

        public string StaffNationality { get; set; }

        public string IdNO { get; set; }

        public string StaffActivity { get; set; }

    }

    public class LabourForce
    {

        public string Gender { get; set; }

        public int Swazi1 { get; set; }

        public int Foreign1 { get; set; }

        public int Swazi2 { get; set; }

        public int Foreign2 { get; set; }

        public int Total { get; set; }



        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public int FormRegistrationNo { get; set; }


    }

    public class SubContractors
    {
        public string NameofContractor { get; set; }

        public string CountryyofOrigin { get; set; }

        public string CICRegistrationNo { get; set; }

        public string DescriptionOfWork { get; set; }

        public decimal ContractValueOfWork { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public class ProjectDetails
    {
        [Required(ErrorMessage = "Please enter Bid Reference Number")]
        public string BidReferenceNo { get; set; }

        [Required(ErrorMessage = "Please enter Project Title")]
        public string ProjectTitle { get; set; }

        [Required(ErrorMessage = "Please enter Date of Award")]
        public DateTime DateofAward { get; set; }

        [Required(ErrorMessage = "Please enter Company Proposed Commencement Date")]
        public DateTime CommencementDate { get; set; }

        [Required(ErrorMessage = "Please enter Company Proposed Completion Date")]
        public DateTime CompletionDate { get; set; }

        [Required(ErrorMessage = "Please enter Brief Description of Project")]
        public string DescriptionofProject { get; set; }

        [Required(ErrorMessage = "Please enter Client Name & Contact Details")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Please enter Financial Value of surety")]
        public decimal ContractValue { get; set; }


    }
    public class Doc3
    {
        [Required(ErrorMessage = "Please add Name")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Please add Signature")]
        public IFormFile Signature { get; set; }

        public string SignatureName { get; set; }

        [Required(ErrorMessage = "Please add Title/Designation")]
        public string TitleDesignation { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]
        public bool TermsAndConditions { get; set; }

        //[Required(ErrorMessage = "Please upload file")]
        public IFormFile BusineesParticularsfile1 { get; set; }

        public string BusineesParticularsfile1Name { get; set; }

        //[Required(ErrorMessage = "Please upload file")]
        public IFormFile BusineesParticularsfile2 { get; set; }

        public string BusineesParticularsfile2Name { get; set; }

        public string ImagePath { get; set; }


    }

    public class Declaration3
    {
        public string Name { get; set; }

        public IFormFile Signature { get; set; }
        public string SignatureName { get; set; }

        public string TitleDesignation { get; set; }

        public string ImagePath { get; set; }
    }

    public class Business
    {
        public string BusinessNameValue { get; set; }

        public string BusinessNameText { get; set; }
    }
}
