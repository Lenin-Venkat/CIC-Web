using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Cicf6Model
    {

        public ApplicationTypeModell App { get; set; }
        public PersonelDetailsModel personelDetails { get; set; }
        public List<EducationBackGroundetails> educationBackground { get; set; }
        public List<BackGroundetails> backGroundetails { get; set; }

        public DocumentUpload documentUpload { get; set; }
        public Declaration declaration { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string ImagePath { get; set; }
        public int c { get; set; }

        public int d { get; set; }
        public string FormStatus { get; set; }

        public AddNewRegistrationNo AddNewRegistrationNo { get; set; }
        public string CreatedBy { get; set; }
        public string Reviewer { get; set; }
        public int FormRegistrationNo { get; set; }
        public string FormName { get; set; }
        public string formval { get; set; }
        //variable for file populate
        public string File { get; set; }
        public string NatureofTradeUpload { get; set; }
        public string fileupload1 { get; set; }
        public string fileupload2 { get; set; }
        public string fileupload3 { get; set; }
        public string fileupload4 { get; set; }
        public string Signature { get; set; }
        public string fileupload6 { get; set; }
        public string CreatedDate { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }

    }
    public class ApplicationTypeModell
    {
        [Required(ErrorMessage = "Application type is required.")]
        public string AppType { get; set; }
        public string Categories { get; set; }
        public string AssociationName { get; set; }

        [Required(ErrorMessage = "Please enter Authorised Officer Name")]
        public string AuthorisedOfficerName { get; set; }


        //[Required(ErrorMessage = "Please add AssociationAttachment")]
        //public string AssociationCertificateAttachment { get; set; }
        //[Required(ErrorMessage = "Please add Association Attachment")]
        public IFormFile Filee { get; set; }
         public  string File { get; set; }
        //[Column(TypeName = "varchar(250)")]
        //public string ImagePath { get; set; }
        //public string formval { get; set; }

    }
    public class PersonelDetailsModel
    {
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Surname")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Please enter IDNo")]
        public string IDNo { get; set; }
        [Required(ErrorMessage = "Please enter Nationality")]
        public string Nationality { get; set; }
        [Required(ErrorMessage = "Please enter Home Area")]
        public string HomeArea { get; set; }
        [Required(ErrorMessage = "Please enter Chief")]
        public string Chief { get; set; }
        [Required(ErrorMessage = "Please enter Indvuna")]
        public string Indvuna { get; set; }
        //[Required(ErrorMessage = "Please enter Temporal Residence Permit No")]
        public string TemporalResidencePermitNo { get; set; }
        //[Required(ErrorMessage = "Please enter Work Permit No")]
        public string WorkPermitNo { get; set; }
        [Required(ErrorMessage = "Please enter CellphoneNo")]
        public string CellphoneNo { get; set; }
        
        [Required(ErrorMessage = "Please enter Email address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The email address is not valid")]
        public string Emailaddress { get; set; }
        //[Required(ErrorMessage = "Please enter Fax No")]
        //public string FaxNo { get; set; }
        [Required(ErrorMessage = "Please enter Residential Address")]
        public string ResidentialAddress { get; set; }
        [Required(ErrorMessage = "Please enter Next of Kin")]
        public string NextofKin { get; set; }
        [Required(ErrorMessage = "Please enter Relationship")]
        public string Relationship { get; set; }
        [Required(ErrorMessage = "Please enter Contact No")]
        public string ContactNo { get; set; }
        //[Required(ErrorMessage = "Please enter Nature of Trade/Expertise")]
        [Required(ErrorMessage = "Please enter Nature of Trade/Expertise")]
        public string NatureofTradeExpertise { get; set; }
        //[Required(ErrorMessage = "Please add Nature of Trade Upload")]
        public IFormFile NatureofTradeUpload { get; set; }

    }
    public class EducationBackGroundetails
    {
        [Required(ErrorMessage = "Please enter Highest Education  Level")]
        public string HighestEducationLevel { get; set; }
        [Required(ErrorMessage = "Please enter Qualifications Attained")]
        public string QualificationsAttained { get; set; }
        [Required(ErrorMessage = "Please enter Education Institution")]
        public string EducationInstitution { get; set; }
       
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public class BackGroundetails
    {
        [Required(ErrorMessage = "Please enter Names and Surname")]
        public string NamesandSurname { get; set; }
        [Required(ErrorMessage = "Please enter Type of Work Performed")]
        public string TypeofWorkPerformed { get; set; }
        [Required(ErrorMessage = "Please enter TelephoneNo")]
        public string TelephoneNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
    public class DocumentUpload
    {
        [NotMapped]
        public IFormFile fileupload1 { get; set; }


        public IFormFile fileupload2 { get; set; }


        public IFormFile fileupload3 { get; set; }


        public IFormFile fileupload4 { get; set; }


        public IFormFile fileupload5 { get; set; }


        public IFormFile fileupload6 { get; set; }
        public int customerCheck { get; set; }
        //declaration part


        public string WitnessedName { get; set; }
        public string WitnessedSignatureimage { get; set; }
        public string WitnessedSignatureTitleDesignation { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile Filee { get; set; }
        public string FaxNo { get; set; }
    }
    public class Declaration
    {

        [Required(ErrorMessage = "Please add Name")]
        public string Namee { get; set; }
       // [Required(ErrorMessage = "Please add Signature")]
        public IFormFile Signature { get; set; }
        [Required(ErrorMessage = "Please add TitleDesignation")]
        public string TitleDesignation { get; set; }
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept Terms and conditions")]
        public bool TermsAndConditions { get; set; }
    }
    public class AddNewRegistrationNo
    {
        public string ProjectRegistrationNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

    }
}
