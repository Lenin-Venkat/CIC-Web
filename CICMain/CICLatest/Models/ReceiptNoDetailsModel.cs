using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ReceiptNoDetails
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }        
        public string ReceiptNo { get; set; }
        public decimal Amount { get; set; }
        public string CertificateNo { get; set; }
    }
}
