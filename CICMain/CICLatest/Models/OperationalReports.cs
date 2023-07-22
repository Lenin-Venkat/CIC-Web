using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;

namespace CICLatest.Models
{
    public class OperationalReports
    {
        public string[] ContractorColumns = { "REG NO", "CATEGORY", "WORKS DISCIPLINE", "GRADE", "CONTRACTOR NAME",	"TELEPHONE No", "MOBILE No", "EMAIL", "POSTAL/PHYSICAL ADDRESS", "REG date" };
        public string[] ProjectColumns = { "Registration Date", "DATE AWARDED", "DATE STARTED", "COMPLETION DATE", "PROJECT OWNER", "CLASSIFICATION", "CONTRACTOR", "CONTRACTOR GRADE", "Missing Column", "CELL NO", "TELEPHONE NO", "CONTACT PERSON", "PROJECT DETAILS", "CONTRACT SUM", "LEVY %", "LEVY AMOUNT" };
        public string[] ProjectColumns1 = { "Registration Date", "DATE AWARDED", "DATE STARTED", "COMPLETION DATE", "PROJECT OWNER", "CLASSIFICATION", "CONTRACTOR", "CONTRACTOR GRADE", "CELL NO", "TELEPHONE NO", "CONTACT PERSON", "PROJECT DETAILS", "CONTRACT SUM", "LEVY %", "LEVY AMOUNT" };
        public List<Project> ProjectsData { get; set; }
        public List<Contractor> ContractorsData { get; set; }
        public string SelectedSearchType = "";
        public string SearchValue = "";
        public string SelectedReportType = "    ";
        public List<ContractorFormAlias> ContractorFormAlias
        {
            get
            {
                return GetContractorFormAlias();
            }
        }

        private List<ContractorFormAlias> GetContractorFormAlias()
        {
            List<ContractorFormAlias> result = new List<ContractorFormAlias>();
            string[] CiCForms = { "cicform1()",  "CicForm3()", "cicform4()", "CicForm5()", "CicForm6()", "CicForm7()" };
            foreach (string form in CiCForms)
            {
                ContractorFormAlias eachItem = new ContractorFormAlias();
                eachItem.ActualColumnNameOf = new Contractor();
                eachItem.FormName = form;
                eachItem.ActualColumnNameOf.RegNo = "CertificateNo";
                eachItem.ActualColumnNameOf.Category = (form == "CicForm3()") ? "CategoryId" :
                   (form == "CicForm7()") ? "WorkDisciplineType" : "Category";
                eachItem.ActualColumnNameOf.WorkDiscipline = (form == "cicform1()" || form == "CicForm2()") ? "BuildingSubCategory" :
                    (form == "CicForm3()") ? "Subcatogory" :
                    (form == "cicform4()") ? "SubCategoryName" :
                    (form == "CicForm5()") ? "Subcatogory" :
                    (form == "CicForm6()") ? "Categories" : "WorkDisciplineType";
                eachItem.ActualColumnNameOf.Grade = (form == "cicform1()" || form == "CicForm2()") ? "Grade" :
                    (form == "CicForm3()") ? "JVGrade" : "Categories";
                eachItem.ActualColumnNameOf.ContractorName = (form == "cicform1()" || form == "CicForm2()" || form == "cicform4()" || form == "CicForm7()") ? "BusinessName" :
                    (form == "CicForm3()" || form == "CicForm5()") ? "NameOFJoinVenture" : "Name";
                eachItem.ActualColumnNameOf.TelephoneNo = (form == "cicform1()" || form == "CicForm2()" || form == "cicform4()" || form == "CicForm7()") ? "TelephoneNumber" :
                    "BusinessTelephone";

                eachItem.ActualColumnNameOf.MobileNo = (form == "cicform1()" || form == "CicForm2()" || form == "cicform4()" || form == "CicForm7()") ? "BusinessRepresentativeCellNo" :
                    (form == "CicForm3()" || form == "CicForm5()") ? "MobileNo" : "CellphoneNo";
                eachItem.ActualColumnNameOf.Email = (form == "cicform1()" || form == "CicForm2()" || form == "cicform4()" || form == "CicForm7()") ? "BusinessRepresentativeEmail" :
                    (form == "CicForm3()" || form == "CicForm5()") ? "BusinessEmail" : "Emailaddress";
                eachItem.ActualColumnNameOf.PostalPhysicalAddress = (form == "cicform1()" || form == "CicForm2()" || form == "cicform4()" || form == "CicForm7()") ? "PostalAddress" :
                    (form == "CicForm3()" || form == "CicForm5()") ? "Phyaddress" : "ResidentialAddress";
                eachItem.ActualColumnNameOf.RegDate = "CreatedDate";
                result.Add(eachItem);
            }

            return result;
        }

    }
    public class Project
    {
        public string CreatedDate { get; set; }
        public string DateofAward { get; set; }
        public string Proposedcommencedate { get; set; }
        public string Proposedcompleteddate { get; set; }
        public string Name { get; set; }
        //public string ClientSubcategory { get; set; }
        public string OwnerCategoryId { get; set; }
        public string Organization { get; set; }
        public string Grade { get; set; }
        public string missingCol { get; set; }
        public string MobileNo { get; set; }
        public string Telephone { get; set; }
        public string FirstNameSurnamefields { get; set; }
        public string BriefDescriptionofProject { get; set; }
        public string ContractVAlue { get; set; }
        public string LevyPaybale { get; set; }
        public string TotalProjectCost { get; set; }
    }
    public class Contractor
    {
        public string RegNo { get; set; }
        public string Category { get; set; }
        public string WorkDiscipline { get; set; }
        public string Grade { get; set; }
        public string ContractorName { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PostalPhysicalAddress { get; set; }
        public string RegDate { get; set; }

    }
    public class ContractorFormAlias
    {
        public string FormName { get; set; }
        public Contractor ActualColumnNameOf { get; set; }
    }
}
