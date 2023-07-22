using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class UserModel: IdentityUser
    {
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string CustNo { get; set; }
    }
}
