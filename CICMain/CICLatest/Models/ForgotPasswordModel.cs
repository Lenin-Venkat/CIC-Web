﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        //[EmailAddress]
        public string EmailPhoneNumber { get; set; }

        
    }
}
