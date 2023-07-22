using CICLatest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.MappingConfigurations
{
    public class Form3Mapper
    {
        public Form3Model mapData(CICForm3Model p3, Form3Model ModelForm3, int registrationNo)
        {
            ModelForm3.FormName = "Form3";
            ModelForm3.FormRegistrationNo = registrationNo;
            if(p3.App.NameOFJoinVenture!=null)
            {
                ModelForm3.PartitionKey = p3.App.NameOFJoinVenture;
            }
            else
            {
                ModelForm3.PartitionKey = "Form3PK";
            }
            
            ModelForm3.RowKey = "Form" + registrationNo.ToString();
            ModelForm3.AppType = p3.App.AppType;
            ModelForm3.CustNo = p3.CustNo;
            ModelForm3.NameOFJoinVenture = p3.App.NameOFJoinVenture;
            ModelForm3.TypeofJoointVenture = p3.App.TypeofJoointVenture;
            ModelForm3.Telephone = p3.App.Telephone;
            ModelForm3.Fax = p3.App.Fax;
            ModelForm3.Email = p3.App.Email;
            ModelForm3.Phyaddress = p3.App.Phyaddress;
            ModelForm3.FirstName = p3.App.FirstName;
            ModelForm3.NameType = p3.App.NameType;
            ModelForm3.SurName = p3.App.SurName;
            ModelForm3.Designation = p3.App.Designation;
            ModelForm3.BusinessTelephone = p3.App.BusinessTelephone;
            ModelForm3.FaxNo = p3.App.FaxNo;
            ModelForm3.MobileNo = p3.App.MobileNo;
            ModelForm3.BusinessEmail = p3.App.BusinessEmail;
            ModelForm3.CategoryId = p3.businessModel.SelectedCategoryValue;
            ModelForm3.Subcatogory = p3.businessModel.selectedsubcategory;
            //------------- Project Details Section
            ModelForm3.BidReferenceNo = p3.projectDetailsModel.BidReferenceNo;
            ModelForm3.ProjectTitle = p3.projectDetailsModel.ProjectTitle;
            ModelForm3.DateofAward = p3.projectDetailsModel.DateofAward;
            ModelForm3.CommencementDate = p3.projectDetailsModel.CommencementDate;
            ModelForm3.CompletionDate = p3.projectDetailsModel.CompletionDate;
            ModelForm3.DescriptionofProject = p3.projectDetailsModel.DescriptionofProject;
            ModelForm3.ClientName = p3.projectDetailsModel.ClientName;
            ModelForm3.ContractValue = p3.projectDetailsModel.ContractValue;

            ModelForm3.FormStatus = p3.FormStatus;
            ModelForm3.Reviewer = p3.Reviewer.Trim();
            ModelForm3.CreatedBy = p3.CreatedBy;
            ModelForm3.CreatedDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
            if (p3.doc3.TermsAndConditions == true)
            {
                ModelForm3.customerCheck = 0;
            }
            else
            {
                ModelForm3.customerCheck = 1;
            }
            ModelForm3.Name = p3.doc3.Name;
            ModelForm3.ImagePath= p3.ImagePath;
            ModelForm3.TitleDesignation = p3.doc3.TitleDesignation;
            ModelForm3.WitnessedTitleDesignation = p3.declaration3.TitleDesignation;
            ModelForm3.WitnessedName = p3.declaration3.Name;

            return ModelForm3;
        }

        public ParticularsofJointVentureParties mapJointDetails(ParticularsofJointVentureParties m, ParticularsofJointVentureParties p, int registrationNo)
        {

            m.PartitionKey = p.NameofApplicant;
            m.RowKey = "Form" + registrationNo.ToString();
            m.NameofApplicant = p.NameofApplicant;
            m.CountryOfOrigin = p.CountryOfOrigin;
            m.ContactDetails = p.ContactDetails;
            m.CICRegistrationNo = p.CICRegistrationNo;
            m.Shareholding = p.Shareholding;
            return m;
        }

        public TechnicalAdministrativeStaff mapTechStaffDetails(TechnicalAdministrativeStaff m, TechnicalAdministrativeStaff p, int registrationNo)
        {
            m.PartitionKey = p.Category;
            m.RowKey = "Form" + registrationNo.ToString();
            m.Category = p.Category;
            m.Number = p.Number;
            m.YearsofExperience = p.YearsofExperience;
            m.FormRegistrationNo = registrationNo;
            return m;
        }

        public ProjectStaff mapProjectStaffDetails(ProjectStaff m, ProjectStaff p, int registrationNo)
        {
            m.PartitionKey = p.StaffName;
            m.RowKey = "Form" + registrationNo.ToString();
            m.StaffName = p.StaffName;
            m.StaffPosition = p.StaffPosition;
            m.StaffQualification = p.StaffQualification;
            m.StaffExperience = p.StaffExperience;
            m.StaffNationality = p.StaffNationality;
            m.IdNO = p.IdNO;
            m.StaffActivity = p.StaffActivity;
            return m;
        }
        public LabourForce mapLabourDetails(LabourForce m, LabourForce p, int registrationNo)
        {
            m.PartitionKey = p.Gender;
            m.RowKey = "Form" + registrationNo.ToString();
            m.Gender = p.Gender;
            m.Swazi1 = p.Swazi1;
            m.Foreign1 = p.Foreign1;
            m.Swazi2 = p.Swazi2;
            m.Foreign2 = p.Foreign2;
            m.Total = p.Total;
            m.FormRegistrationNo = registrationNo;

            return m;
        }

        public SubContractors mapSubContractDetails(SubContractors m, SubContractors p, int registrationNo)
        {
            m.PartitionKey = p.NameofContractor;
            m.RowKey = "Form" + registrationNo.ToString();
            m.NameofContractor = p.NameofContractor;
            m.CountryyofOrigin = p.CountryyofOrigin;
            m.CICRegistrationNo = p.CICRegistrationNo;
            m.DescriptionOfWork = p.DescriptionOfWork;
            m.ContractValueOfWork = p.ContractValueOfWork;
            return m;
        }
    }
}
