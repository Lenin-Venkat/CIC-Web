using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ApplicationLockModel
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        
        public string AssignedTo { get; set; }

        public string FormName { get; set; }
    }
}
