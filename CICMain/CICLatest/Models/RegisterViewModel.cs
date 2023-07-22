using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Business Name is required")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Cell Phone Number is required")]        
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "Role is required")]
        //public string Id { get; set; }
        public List<Rolelist> Rolelist { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        //[Required]
        public bool TermsAndConditions { get; set; }

        public string err { get; set; }

        [Required(ErrorMessage = "country code is required")]
        public string CountryCode { get; set; }

    }
    public class Rolelist
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
