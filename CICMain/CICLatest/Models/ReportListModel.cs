using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ReviewerListModel
    {
        public string FormDesc { get; set; }

        public string FormDate { get; set; }

        public string Status { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string FormName {get;set;}
        public string comment { get; set; }
        public string pdfFile { get; set; }

        public string apptype { get; set; }
    }
}
