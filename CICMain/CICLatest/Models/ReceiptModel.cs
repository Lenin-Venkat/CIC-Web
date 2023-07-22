using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ReceiptModel
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }        

        public int RNo { get; set; }
    }
}
