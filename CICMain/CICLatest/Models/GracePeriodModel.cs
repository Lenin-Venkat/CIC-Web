using Microsoft.AspNetCore.Builder;
using System;
using System.ComponentModel.DataAnnotations;

namespace CICLatest.Models
{
    public class GracePeriodModel
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int? NumberOfDays { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Timestamp { get; set; }
        [Required(ErrorMessage = "Please select grace period")]
        public DateTime allowedDate { get; set; }
    }
}
