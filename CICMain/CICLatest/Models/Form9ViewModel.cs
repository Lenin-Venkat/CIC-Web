using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace CICLatest.Models
{
    public class Form9ViewModel
    {
        [Key]
        public int Id { get; set; }
        public Form9View App { get; set; }
        public List<BuildingWorkForProject> buildingWorkForProject { get; set; }
        public List<CivilsWorksProjects> civilsWorksProjects { get; set; }
        public List<MechanicalWorksProjects> mechanicalWorksProjects { get; set; }
        public int FirstGrid { get; set; }
        public int SecondGrid { get; set; }
        public int ThirdGrid { get; set; }
        public string ImagePath { get; set; }
        public string FormStatus { get; set; }
        public string Reviewer { get; set; }
        //Adding New field for edit 
        public string formval { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string CreatedBy { get; set; }
        public int FormRegistrationNo { get; set; }
        public string path { get; set; }
        public string FormName { get; set; }

        public string CreatedDate { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
    }
    public class Form9View
    {

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Institution Focal Person name is required")]
        public string InstitutionFocalPerson { get; set; }

        [Required(ErrorMessage = "Postal address is required")]
        public string PostalAddress { get; set; }

        [Required(ErrorMessage = "Physical Address is required")]
        public string PhysicalAddress { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Telephone Number is required")]
        public string TelephoneNumber { get; set; }

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
        public Nullable<int> Day { get; set; }
        [Range(1, 12)]
        [Required(ErrorMessage = "Month is required")]
        public Nullable<int> Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(22, 99)]
        public  Nullable<int> Year { get; set; }
       // [NotMapped]
        [Required(ErrorMessage = "Please upload your signature")]
        public IFormFile Signature { get; set; }
        [Required(ErrorMessage = "Please upload file")]
        public IFormFile PurchaseordersFile { get; set; }
        [Required(ErrorMessage = "Please upload file")]
        public IFormFile InvoicesFile  { get; set; }
        [Required(ErrorMessage = "Please upload file")]
        public IFormFile SummarybillofquantitiesFile { get; set; }
        public string FaxNo { get; set; }
        //new for file 
        public string SignatureImage { get; set; }

        public string PurchaseordersFilee { get; set; }

        public string InvoicesFilee { get; set; }

        public string SummarybillofquantitiesFilee { get; set; }
    }
    public class BuildingWorkForProject
    {
        //[Required(ErrorMessage = "Please enter Project Name")]
        public string ProjectName { get; set; }
        //[Required(ErrorMessage = "Please enter Name Of Responsible Contractor")]
        public string NameOfResponsibleContractor { get; set; }
        //[Required(ErrorMessage = "Please enter Est Project Cost")]
        public int EstProjectCost { get; set; }
        //[Required(ErrorMessage = "Please enter ProposedCommencementDate")]
        public DateTime ? ProposedCommencementDate { get; set; }
        //[Required(ErrorMessage = "Please enter Proposed Completion Date")]
        public DateTime ? ProposedCompletionDate { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }

    }
    public class CivilsWorksProjects
    {

        //[Required(ErrorMessage = "Please enter Project Name")]
        public string ProjectName { get; set; }
        //[Required(ErrorMessage = "Please enter Name Of Responsible Contractor")]
        public string NameOfResponsibleContractor { get; set; }
        //[Required(ErrorMessage = "Please enter Est Project Cost")]
        public int EstProjectCost { get; set; }
        //[Required(ErrorMessage = "Please enter ProposedCommencementDate")]
        public DateTime ? ProposedCommencementDate { get; set; }
        //[Required(ErrorMessage = "Please enter Proposed Completion Date")]
        public DateTime ? ProposedCompletionDate { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }
    }
    public class MechanicalWorksProjects

    {
        //[Required(ErrorMessage = "Please enter Project Name")]
        public string ProjectName { get; set; }
        //[Required(ErrorMessage = "Please enter Name Of Responsible Contractor")]
        public string NameOfResponsibleContractor { get; set; }
        //[Required(ErrorMessage = "Please enter Est Project Cost")]
        public int EstProjectCost { get; set; }
        //[Required(ErrorMessage = "Please enter ProposedCommencementDate")]
        public DateTime ? ProposedCommencementDate { get; set; }
        //[Required(ErrorMessage = "Please enter Proposed Completion Date")]
        public DateTime ? ProposedCompletionDate { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int FormRegistrationNo { get; set; }
    }
   
}
