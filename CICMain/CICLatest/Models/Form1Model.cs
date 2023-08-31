
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Form1Model : TableEntity
    {
        //public string PartitionKey { get; set; }
       // public string RowKey { get; set; }
       public string FormName { get; set; }
        public string FormStatus { get; set; }
        public string Reviewer { get; set; }
        public Int64 FirmRegistrationNo { get; set; }
        //public DateTime Timestamp { get; set; }
        public string AppType { get; set; }
        public string AssociationName { get; set; }
        public string AuthorisedOfficerName { get; set; }
        public string signature { get; set; }

        public string path { get; set; }
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
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        public string Category { get; set; }
        public string PresentGrade { get; set; }
        public string BusinessRepresentativeName { get; set; }
        public string BusinessRepresentativePositionNumber { get; set; }
        public string BusinessRepresentativeCellNo { get; set; }
        public string BusinessRepresentativeFax { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string BusinessRepresentativeEmail { get; set; }
        public string BusinessRepresentativeSign { get; set; }
        public decimal AnnualTurnoverYear1 { get; set; }
        public decimal AnnualTurnoverYear2 { get; set; }
        public decimal AnnualTurnoverYear3 { get; set; }
        public decimal FinancialValue { get; set; }
        public string FinancialInstitutionName { get; set; }
        public decimal AvailableCapital { get; set; }
        public string statementsign { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string WitnessedName { get; set; }
        public string WitnessedTitle { get; set; }

        public string Grade { get; set; }

        public string comment { get; set; }
        public string CreatedBy { get; set; }

        public List<DirectorshipShareDividends> Sharelist { get; set; }
        public List<ApplicantBank> applicantBank { get; set; }

        public List<WorksCapability> worksCapability { get; set; }

        public string BusinessFile1Name { get; set; }
        public string BusinessFile2Name { get; set; }
        public string BusinessFile3Name { get; set; }
        public string BusinessFile4Name { get; set; }
        public string BusinessFile5Name { get; set; }
        public string BusinessFile6Name { get; set; }
        public string BusinessFile7Name { get; set; }
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
        public string sign1Name { get; set; }

        public string sign2Name { get; set; }
        public string taxLawName { get; set; }
        public string EvidenceName { get; set; }
        public string ComplienceName { get; set; }
        public string CreatedDate { get; set; }

        public string CertificateNo { get; set; }
        public string cdate { get; set; }
        public int BuildingSubCategory { get; set; }
        public int CivilSubCategory { get; set; }
        public int ElectricalSubCategory { get; set; }
        public int MechanicalSubCategory { get; set; }        
        public string ScoreStr { get; set; }

        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
        public string newGradecomment { get; set; }

        public int AdminFee { get; set; }
        public int RegistrationFee { get; set; }
        public int RenewalFee { get; set; }

        public string InvoiceNo { get; set; }
        public string RegistrationID { get; set; }
    }
}
