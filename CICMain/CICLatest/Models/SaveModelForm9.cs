using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class SaveModelForm9
    {
        public string Reviewer { get; set; }
        public string FormName { get; set; }
        public string FormStatus { get; set; }
      
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public int FormRegistrationNo { get; set; }
        public string CompanyName { get; set; }

       
        public string InstitutionFocalPerson { get; set; }

      
        public string PostalAddress { get; set; }

       
        public string PhysicalAddress { get; set; }

        
        public string Email { get; set; }

        
        public string TelephoneNumber { get; set; }

      
        public string RepresentativeName { get; set; }

       
        public string CompName { get; set; }

      
        public string Position { get; set; }

       
        public string Place { get; set; }

       

        public int Day { get; set; }
      
       
        public int Month { get; set; }

       
        public int Year { get; set; }
       

        public IFormFile Signature { get; set; }
       
        public IFormFile PurchaseordersFile { get; set; }
       
        public IFormFile InvoicesFile { get; set; }
       
        public IFormFile SummarybillofquantitiesFile { get; set; }
        public string CreatedBy { get; set; }
        
        public List<BuildingWorkForProject> buildingWorkForProject { get; set; }
        public List<CivilsWorksProjects> civilsWorksProjects { get; set; }
        public List<MechanicalWorksProjects> mechanicalWorksProjects { get; set; }

        public string path { get; set; }
        public string comment { get; set; }
        public string FaxNo { get; set; }

        public string SignatureName { get; set; }

        public string PurchaseordersFileName { get; set; }

        public string InvoicesFileName { get; set; }

        public string SummarybillofquantitiesFileName { get; set; }
        public string CreatedDate { get; set; }

        public string CertificateNo { get; set; }
        public string CustNo { get; set; }
        public string ReceiptNo { get; set; }
    }
}
