using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class MainViewModel
    {
        public string err { get; set; }
        public int Tab3First { get; set; }
        public int Tab3Sec { get; set; }
        public int Tab3Third { get; set; }
        public string WorkDiscipline { get; set; }
        public string OwnerCategory { get; set; }
        public string SelectedCategory { get; set; }
        public List<Category> ListCategory { get; set; }
        public List<Category> ListCategory1 { get; set; }
        public List<categoryType> ListCategory2 { get; set; }
        public string WorkDisciplineSubCategorry { get; set; }
        public Tab1 tabb1 { get; set; }
        public Tab2 tab2 { get; set; }
        public List<Tab3FirstSection> Tab3MainSection { get; set; }
        public List<Tab3SecSection> tab3SecSection { get; set; }
        public List<Tab3ThirdSection> tab3ThirdSection { get; set; }
        public Tab4 tab4 { get; set; }
        public Tab5 tab5 { get; set; }
        public int selectedsubcategory { get; set; }//For subcategory
        public List<subCategory> subCategoryModel { get; set; }

        //  public List<Tab3FirstSection> tab3FirstSection { get; set; }
        public int FirmRegistrationNo { get; set; }

        public string FormStatus { get; set; }
        public string Reviewer { get; set; }
        public string ImagePath { get; set; }
        //added for update 
        public string formval { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string CreatedBy { get; set; }
        public int FormRegistrationNo { get; set; }
        public string path { get; set; }
        public string FormName { get; set; }
        //adding for file 
        public string Signature1 { get; set; }
        public string WitnessSignature { get; set; }
        public string WitnessSignature1 { get; set; }
        public string SummaryPage { get; set; }
        public string LetterOfContract { get; set; }
        public string Letterindicating { get; set; }
        public string Signedletterupload { get; set; }
        public string CreatedDate { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public int PartialInvoiceCount { get; set;}
        public int CreateClearenceCertificate { get; set; }
        public int projectCertificateCreated { get; set; }
        public int NoOfPartialCertificateCreated { get; set; }
        public List<CertificatesList> ListOfCertificates { get; set; }
        public List<SubContractorsList> ListOfSubContractors { get; set; }
        public List<SuppliersList> ListOfSuppliers { get; set; }

    }
    public class Tab1
    {
        [Required(ErrorMessage = "Please Enter Organization")]
        public string Oraganization { get; set; }

        [Required(ErrorMessage = "Please Enter Certificate No")]
        public string CertificateNo { get; set; }
        [Required(ErrorMessage = "Please Enter Grade")]

        public string Grade { get; set; }

        [Required(ErrorMessage = "Please Enter Name type")]
        public string TypeGender { get; set; }
        [Required(ErrorMessage = "Please Enter First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please Enter Surname")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }
        [Required(ErrorMessage = "Please Enter Telephone No")]
        public string Telephone { get; set; }
        //[Required(ErrorMessage = "Please Enter Fax")]
        public string FaxNo { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile No")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please Enter Postal Address")]
        public string Form1PostalAddress { get; set; }

    }
    public class Tab2
    {
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        // [Required(ErrorMessage = "Please Enter ID No.")]
        public string IDNO { get; set; }

        //  [Required(ErrorMessage = "Please Enter Passport No")]
        public string PassportNo { get; set; }
        [Required(ErrorMessage = "Please Enter Postal Address")]
        public string PostalAddress { get; set; }
        [Required(ErrorMessage = "Please Enter Physical Address")]
        public string PhysicalAddress { get; set; }


        [Required(ErrorMessage = "Please Enter Telephone No")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Please Enter Fax No")]
        public string FaxNo { get; set; }


        [Required(ErrorMessage = "Please Enter Email Address")]
        public string EmailAdress { get; set; }
        //[Required(ErrorMessage = "Please Enter CLientCategory")]
        //public string CLientCategory { get; set; }

        [Required(ErrorMessage = "Please Enter Name Type")]
        public string AuthoriseGende { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        public string AuthrisedFirstName { get; set; }
        [Required(ErrorMessage = "Please Enter Surname")]
        public string AuthorisedSurname { get; set; }
        [Required(ErrorMessage = "Please Enter Designation")]
        public string Designation { get; set; }
        //[Required(ErrorMessage = "Please Enter Telephone")]
        // public string TelephoneTab2 { get; set; }

        //[Required(ErrorMessage = "Please Enter Fax")]
        public string AuthorisedFaxNo { get; set; }
        [Required(ErrorMessage = "Please Enter Telephone No ")]
        public string TelePhoneNo { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile")]
        public string AuthorisedMobile { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        public string AuthorisedEmail { get; set; }
        //[Required(ErrorMessage = "Please Select Workdiscipline")]
        //public string Workdisciplie { get; set; }
        //[Required(ErrorMessage = "Please Select Category")]
        //public string WorktypeCategory { get; set; }
        [Required(ErrorMessage = "Please Enter Other")]
        public string Other { get; set; }
    }
    public class Tab3FirstSection
    {
        //[Required(ErrorMessage = "Please Enter Profession", AllowEmptyStrings = false)]
        public string Profession { get; set; }
        //[Required(ErrorMessage = "Please Enter Name of Consultancy Firm", AllowEmptyStrings = false)]
        public string NameofConsultancyFirm { get; set; }
        //[Required(ErrorMessage = "Please Enter Country of Origin", AllowEmptyStrings = false)]
        public string CountryofOrigin { get; set; }
        //[Required(ErrorMessage = "Please Enter Contact", AllowEmptyStrings = false)]
        public string Contact { get; set; }
        //[Required(ErrorMessage = "Please Enter CicRegistration No",AllowEmptyStrings = false)]
        public string CicRegistrationNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }

    }
    public class Tab3SecSection
    {

        [Required(ErrorMessage = "Please Enter Name of SubContractors", AllowEmptyStrings = false)]
        public string NameofSubContractors { get; set; }
        [Required(ErrorMessage = "Please Enter Country of Origin", AllowEmptyStrings = false)]
        public string CountryofOrigin { get; set; }
        [Required(ErrorMessage = "Please Enter Scope of Work", AllowEmptyStrings = false)]
        public string ScopeofWork { get; set; }
        [Required(ErrorMessage = "Please Enter Contact Details", AllowEmptyStrings = false)]
        public string ContactDetails { get; set; }
        [Required(ErrorMessage = "Please Enter Registration No", AllowEmptyStrings = false)]
        public string RegistrationNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }

    }

    public class Tab3ThirdSection
    {
        [Required(ErrorMessage = "Please Enter Supplier", AllowEmptyStrings = false)]
        public string Supplier { get; set; }
        [Required(ErrorMessage = "Please Enter Country of Origin", AllowEmptyStrings = false)]
        public string CountryofOrigin { get; set; }
        [Required(ErrorMessage = "Please Enter Scope of Work", AllowEmptyStrings = false)]
        public string ScopeofWork { get; set; }

        [Required(ErrorMessage = "Please Enter Contact Details", AllowEmptyStrings = false)]
        public string ContactDetails { get; set; }
        [Required(ErrorMessage = "Please Enter Registration No", AllowEmptyStrings = false)]
        public string RegistrationNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }

    }
    public class Tab4
    {
        [Required(ErrorMessage = "Please Select Project Type")]

        public string ProjectType { get; set; }
        [Required(ErrorMessage = "Please Enter Bid Reference")]
        public string BidReference { get; set; }
        [Required(ErrorMessage = "Please Enter Project Tite")]
        public string ProjectTite { get; set; }
        [Required(ErrorMessage = "Please Enter Project Funder")]
        public string ProjectorFunder { get; set; }
        [Required(ErrorMessage = "Please Enter Project Location")]
        public string ProjectLocation { get; set; }
        [Required(ErrorMessage = "Please Enter Town/Inkhundla")]
        public string TownInkhundla { get; set; }
        [Required(ErrorMessage = "Please Enter Region")]
        public string Region { get; set; }
        [Required(ErrorMessage = "Please Enter GPSCo")]
        public string GPSCo { get; set; }
        [Required(ErrorMessage = "Please Enter Date of Award")]
        public DateTime DateofAward { get; set; }
        [Required(ErrorMessage = "Please Enter Brief Description of Project")]
        public string BriefDescriptionofProject { get; set; }
        [Required(ErrorMessage = "Please Enter Proposed Commencment Date")]
        public DateTime ProposedCommencmentDate { get; set; }
        [Required(ErrorMessage = "Please Enter Proposed Complete Date")]
        public DateTime ProposedCompleteDate { get; set; }
        [Required(ErrorMessage = "Please Enter  Revised Date ")]
        public DateTime RevisedDate { get; set; }
        [Required(ErrorMessage = "Please Enter  Contract Value ")]
        [Range(0, 9999999999999999.99)]
        public decimal ContractVAlue { get; set; }
        [Required(ErrorMessage = "Please Enter  Levy Paybale ")]

        public decimal LevyPaybale { get; set; }
        [Required(ErrorMessage = "Please Enter  Total Project Cost")]
        public decimal TotalProjectCost { get; set; }
        [Required(ErrorMessage = "Please Enter  Total Project Cost IncludingLevy")]
        public decimal TotalProjectCostIncludingLevy { get; set; }
        [Required(ErrorMessage = "Please Enter Levy Payment Options")]
        public string LevyPaymentOptions { get; set; }
        [Required(ErrorMessage = "Please Enter Time Frame option")]
        public string TimeFrameoption { get; set; }
    }

    public class Tab5
    {
        [Required(ErrorMessage = "Please Upload File")]
        public IFormFile SummaryPage { get; set; }
        [Required(ErrorMessage = "Please Upload File")]
        public IFormFile LetterOfContract { get; set; }
        [Required(ErrorMessage = "Please Upload File")]
        public IFormFile Letterindicating { get; set; }
        [Required(ErrorMessage = "Please Upload File")]

        public IFormFile Signedletterupload { get; set; }

        //Declaration Section
        [Required(ErrorMessage = "Name is required")]
        public string RepresentativeName { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        public string CompName { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Name of place is required")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Day is required")]
        [Range(1, 31)]
        public int Day { get; set; }
        [Range(1, 12)]
        [Required(ErrorMessage = "Month is required")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(22, 99)]
        public int Year { get; set; }

        //[Required(ErrorMessage = "Please upload your signature")]
        public IFormFile Signature { get; set; }
        //Witness part
        [Required(ErrorMessage = "Please add Name")]
        public string WitnessName { get; set; }
        [Required(ErrorMessage = "Please add Signature")]
        public IFormFile WitnessSignature { get; set; }
        [Required(ErrorMessage = "Please add TitleDesignation")]
        public string WitnessTitleDesignation { get; set; }


        public string WitnessName1 { get; set; }

        public IFormFile WitnessSignature1 { get; set; }

        public string WitnessTitleDesignation1 { get; set; }
    }

    public class SuppliersList
    {
        public string SupplierValue { get; set; }

        public string SupplierText { get; set; }
    }

    public class SubContractorsList
    {
        public string SubContractorValue { get; set; }

        public string SubContractorText { get; set; }
    }

    public class CertificatesList
    {
        public string CertificateNoValue { get; set; }

        public string CertificateNoText { get; set; }
    }
}
