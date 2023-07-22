using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class PaymentModel
    {
       public IFormFile Paymentfile { get; set; }
        public string CustNo { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }

        public string path { get; set; }
        public string invoiceNo { get; set; }
    }
}
