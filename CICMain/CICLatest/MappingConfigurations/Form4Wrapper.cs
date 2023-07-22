using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CICLatest.Models;


namespace CICLatest.MappingConfigurations
{
    public class Form4Wrapper
    {
        string tempCategory = null;
        public Form4Model mapData(CICForm4Model CModel, Form4Model m, int registrationNo)
        {
            m.PartitionKey = CModel.App.AppType;
            m.RowKey = "Form" + registrationNo;
            m.FormRegistrationNo = registrationNo;
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

            
            m.Category = CModel.businessModel.SelectedCategoryValue;
            m.subCategoryName = CModel.businessModel.selectedsubcategory;
            m.BusinessRepresentativeName = CModel.businessModel.BusinessRepresentativeName;
            m.BusinessRepresentativePositionNumber = CModel.businessModel.BusinessRepresentativePositionNumber;
            m.BusinessRepresentativeCellNo = CModel.businessModel.BusinessRepresentativeCellNo;
            m.BusinessRepresentativeFax = CModel.businessModel.BusinessRepresentativeFax;
            m.BusinessRepresentativeEmail = CModel.businessModel.BusinessRepresentativeEmail;
           
            //docs
            m.Name = CModel.docs.Name;
            m.Title = CModel.docs.Title;
            m.WitnessedName = CModel.docs.WitnessedName;
            m.WitnessedTitle = CModel.docs.WitnessedTitle;

            return m;
        }

        public DirectorshipShareDividends4 mapShareDetails(DirectorshipShareDividends4 m, DirectorshipShareDividends4 p, int registrationNo)
        {
            
                m.PartitionKey = p.DirectorName;
                m.RowKey = "Form"+ Convert.ToString(registrationNo);
                m.DirectorName = p.DirectorName;
                m.CellphoneNo = p.CellphoneNo;
                m.Nationnality = p.Nationnality;
                m.Qualifications = p.Qualifications;
                m.IdNO = p.IdNO;
                m.Country = p.Country;
                m.SharePercent = p.SharePercent;
                return m;
        }
    }
}
