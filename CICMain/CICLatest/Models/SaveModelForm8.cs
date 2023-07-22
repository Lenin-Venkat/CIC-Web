using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class SaveModelForm8
    {
        public string Reviewer { get; set; }
        public string FormName { get; set; }
        public string FormStatus { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public int FormRegistrationNo { get; set; }
        public string CategoryId { get; set; } //chnaged int to string 
        //Tab1 Data

        public string Oraganization { get; set; }


        public string CertificateNo { get; set; }

        public string Grade { get; set; }


        public string TypeGender { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Designation { get; set; }

        public string Telephone { get; set; }

        public string FaxNo { get; set; }

        public string MobileNo { get; set; }

        public string EmailAddress { get; set; }
        public string Form1PostalAddress { get; set; }
        //Tab2 Data 

        public string Name { get; set; }

        public string IDNO { get; set; }


        public string PassportNo { get; set; }

        public string PostalAddress { get; set; }

        public string PhysicalAddress { get; set; }



        public string TelephoneWorkdiscipline { get; set; }


        public string FaxNoWorkdiscipline { get; set; }



        public string EmailAdress { get; set; }

        public string CLientCategoryWorkdiscipline { get; set; }
        public int CLientSubcategory { get; set; }
       // public int SubCatId { get; set; }

        public string AuthoriseGende { get; set; }

        public string AuthrisedFirstName { get; set; }

        public string AuthorisedSurname { get; set; }

        public string DesignationWorkdiscipline { get; set; }

        public string AuthorisedFaxNo { get; set; }
        public string AuthorisedMobile { get; set; }

        public string AuthorisedTelePhone { get; set; }

        public string AuthorisedEmail { get; set; }

        public string Other { get; set; }
        public string OwnerCategoryId { get; set; }// changed int to string 
        public string WorkDisciplinefor { get; set; }
        public int Subcategory { get; set; }

        //Tab4 
        public string ProjectType { get; set; }
        public string BidReference { get; set; }
        public string ProjectTite { get; set; }

        public string ProjectorFunder { get; set; }
        public string ProjectLocation { get; set; }
        public string TownInkhundla { get; set; }
        public string Region { get; set; }
        public string GPSCo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateofAward { get; set; }
        public string BriefDescriptionofProject { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProposedCommencmentDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProposedCompleteDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime RevisedDate { get; set; }
        public decimal ContractVAlue { get; set; }
        public decimal LevyPaybale { get; set; }
        public decimal TotalProjectCost { get; set; }
        public decimal TotalProjectCostIncludingLevy { get; set; }
        public string LevyPaymentOptions { get; set; }
        public string TimeFrameoption { get; set; }
        //Tab5 

        //Declaration Section

        public string RepresentativeName { get; set; }
        public string CompName { get; set; }
        public string Position { get; set; }
        public string Place { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        //Witness part

        public string WitnessName { get; set; }

        public string WitnessTitleDesignation { get; set; }

        public string WitnessName1 { get; set; }

        public string WitnessTitleDesignation1 { get; set; }
        public string CreatedBy { get; set; }
        public string path { get; set; }
        public List<Tab3FirstSection> Tab3MainSection { get; set; }
        public List<Tab3SecSection> tab3SecSection { get; set; }
        public List<Tab3ThirdSection> tab3ThirdSection { get; set; }
        public string comment { get; set; }

        public string Signature1 { get;set;}
        public string WitnessSignature { get; set; }
        public string WitnessSignature1 { get; set; }
        public string SummaryPage { get; set; }
        public string LetterOfContract { get; set; }
        public string Letterindicating { get; set; }
        public string Signedletterupload { get; set; }
        public string CreatedDate { get; set; }
        public string CertificateNo1 { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public int PartialInvoiceCount { get; set; }
        public int CreateClearenceCertificate { get; set; }
        public int projectCertificateCreated { get; set; }
        public int NoOfPartialCertificateCreated { get; set; }
    }
}
