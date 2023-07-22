using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    [Keyless]
    public class CICFees
    {
        public string FormName { get; set; }
        public string Grade { get; set; }
        public int AdminFees { get; set; }
        public int RegistrationFees { get; set; }
        public int RenewalFees { get; set; }

    }
}
