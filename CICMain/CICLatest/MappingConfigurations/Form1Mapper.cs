using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Models;


namespace CICLatest.MappingConfigurations
{
    public class Form1Mapper
    {
        string tempCategory = null;
        public Form1Model mapData(CICForm1Model CModel, Form1Model m, int registrationNo)
        {
            if(CModel.App.AppType == null)
            {
                CModel.App.AppType = "NewApplication";
            }
            m.PartitionKey = CModel.App.AppType;
            m.RowKey = "Form" + registrationNo;
            m.FirmRegistrationNo = registrationNo;
            m.FormStatus = CModel.FormStatus;
            m.Reviewer = CModel.Reviewer;
            m.FormName = CModel.FormName;
            m.CreatedBy = CModel.CreatedBy;
            m.CreatedDate = CModel.CreatedDate;
            //Application type
            m.AppType = CModel.App.AppType;
            m.AssociationName = CModel.App.AssociationName;
            m.AuthorisedOfficerName = CModel.App.AuthorisedOfficerName;
            m.path = CModel.App.ImagePath;
            m.Grade = CModel.Grade;
            m.ScoreStr = CModel.ScoreStr;
            m.CustNo = CModel.CustNo;
            //Business Details
            m.BusinessName = CModel.businessModel.BusinessName;
            m.TradingStyle = CModel.businessModel.TradingStyle;
            m.BusinessType = CModel.businessModel.BusinessType;
            if (m.BusinessType != "Other")
            {
                m.Other = "-";
            }
            else
            {
                m.Other = CModel.businessModel.Other;
            }
            m.CompanyRegistrationDate = CModel.businessModel.CompanyRegistrationDate;
            m.CompanyRegistrationPlace = CModel.businessModel.CompanyRegistrationPlace;
            m.CompanyRegistrationNumber = CModel.businessModel.CompanyRegistrationNumber;
            m.PhysicalAddress = CModel.businessModel.PhysicalAddress;
            m.CompanyHOPhysicalAddress = CModel.businessModel.CompanyHOPhysicalAddress;
            m.PostalAddress = CModel.businessModel.PostalAddress;
            m.TelephoneNumber = CModel.businessModel.TelephoneNumber;
            m.FaxNo = CModel.businessModel.FaxNo;
            m.Email = CModel.businessModel.Email;

            for (int i = 0; i < CModel.businessModel.Category.Count; i++)
            {
                if (CModel.businessModel.Category[i].Selected)
                {
                    if (tempCategory == null)
                    {
                        tempCategory = CModel.businessModel.Category[i].CategoryName;
                    }
                    else
                    {
                        tempCategory = tempCategory + "," + CModel.businessModel.Category[i].CategoryName;
                    }

                }
            }
            m.Category = tempCategory;
            m.BuildingSubCategory = CModel.businessModel.selectedBuildingSubcategory;
            m.CivilSubCategory = CModel.businessModel.selectedCivilSubcategory;
            m.MechanicalSubCategory = CModel.businessModel.selectedMechSubcategory;
            m.ElectricalSubCategory = CModel.businessModel.selectedElectSubcategory;
            m.PresentGrade = CModel.businessModel.PresentGrade;
            m.BusinessRepresentativeName = CModel.businessModel.BusinessRepresentativeName;
            m.BusinessRepresentativePositionNumber = CModel.businessModel.BusinessRepresentativePositionNumber;
            m.BusinessRepresentativeCellNo = CModel.businessModel.BusinessRepresentativeCellNo;
            m.BusinessRepresentativeFax = CModel.businessModel.BusinessRepresentativeFax;
            m.BusinessRepresentativeEmail = CModel.businessModel.BusinessRepresentativeEmail;
            //signature

            //Financial
            m.AnnualTurnoverYear1 = CModel.financialCapabilityModel.AnnualTurnoverYear1;
            m.AnnualTurnoverYear2 = CModel.financialCapabilityModel.AnnualTurnoverYear2;
            m.AnnualTurnoverYear3 = CModel.financialCapabilityModel.AnnualTurnoverYear3;
            m.AvailableCapital = CModel.financialCapabilityModel.AvailableCapital;
            m.FinancialInstitutionName = CModel.financialCapabilityModel.FinancialInstitutionName;
            m.FinancialValue = CModel.financialCapabilityModel.FinancialValue;

            //Work


            //docs
            m.Name = CModel.docs.Name;
            m.Title = CModel.docs.Title;
            m.WitnessedName = CModel.docs.WitnessedName;
            m.WitnessedTitle = CModel.docs.WitnessedTitle;

            return m;
        }

        public DirectorshipShareDividends mapShareDetails(DirectorshipShareDividends m, DirectorshipShareDividends p, int registrationNo)
        {
            
                m.PartitionKey = p.DirectorName;
                m.RowKey = "Form"+ Convert.ToString(registrationNo);
                m.DirectorName = p.DirectorName;
                m.CellphoneNo = p.CellphoneNo;
                m.Nationnality = p.Nationnality;
                m.IdNO = p.IdNO;
                m.Country = p.Country;
                m.SharePercent = p.SharePercent;
                return m;
        }

        public ApplicantBank mapBankDetails(ApplicantBank m, ApplicantBank p, int registrationNo)
        {

            m.PartitionKey = p.BankName;
            m.RowKey = "Form" + Convert.ToString(registrationNo);
            m.BankName = p.BankName;
            m.BranchName = p.BranchName;
            m.BranchCode = p.BranchCode;
            m.AccountHoulderName = p.AccountHoulderName;
            m.AccountNo = p.AccountNo;
            m.AccountTYpe = p.AccountTYpe;
            m.TelephoneNo = p.TelephoneNo;
            return m;
        }

        public WorksCapability mapWorkDetails(WorksCapability m, WorksCapability p, int registrationNo)
        {

            m.PartitionKey = p.ProjectName;
            m.RowKey = "Form" + Convert.ToString(registrationNo);
            m.ProjectName = p.ProjectName;
            m.RegistationNo = p.RegistationNo;
            m.Location = p.Location;
            if(p.CompletionDate == null)
            {
                m.CompletionDate = DateTime.Now;
            }
            else
            {
                m.CompletionDate = p.CompletionDate;
            }
            
            m.ContractSum = p.ContractSum;
            m.TelephoneNo = p.TelephoneNo;
            m.TypeofInvolvement = p.TypeofInvolvement;
            return m;
        }
    }
}
