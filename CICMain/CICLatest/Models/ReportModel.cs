using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ReportModel
    {
        public int filterColumn { get; set; }
        
        [Required(ErrorMessage ="Please select criteria")]
        public string filterColumnName { get; set; }

        public string AppTypeValue { get; set; }

        public DateTime Fromdate { get; set; }
        public DateTime Enddate { get; set; }

        public string CatValue { get; set; }
        public List<Category> CategoryModel { get; set; }
        public int selectedcat { get; set; }
        public string Formname { get; set; }
        public string err { get; set; }
        public OperationalReports OperationalReports { get; set; }
        public string OperationalReportType { get; set; }
    }
}
