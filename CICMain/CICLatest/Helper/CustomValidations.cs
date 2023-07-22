using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CICLatest.Helper
{
    public class CustomValidations
    {
        

        public class CategoryValidation : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                List<categoryType> instance = value as List<categoryType>;
                int count = instance == null ? 0 : (from p in instance
                                                    where p.Selected == true
                                                    select p).Count();
                if (count >= 1)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessage);
            }
        }

        public class ApplicantValidation:ValidationAttribute
        {
            protected readonly List<ValidationResult> validationResults = new List<ValidationResult>();
            public override bool IsValid(object value)
            {
                var list = value as List<ApplicantBank>;
                if (list == null) return true;

                foreach (var item in list)
                {
                    var results = new List<ValidationResult>();
                    var context = new ValidationContext(item, null, null);

                    Validator.TryValidateObject(item, context, results, true);

                    if (results.Count != 0)
                    {
                        var compositeResults = new CompositeValidationResult(String.Format("Validation failed!"));
                        results.ForEach(compositeResults.AddResult);

                        return false;
                    }
                }
                return true;                

            }
        }

        public class CompositeValidationResult : ValidationResult
        {
            private readonly List<ValidationResult> _results = new List<ValidationResult>();

            public IEnumerable<ValidationResult> Results
            {
                get
                {
                    return _results;
                }
            }

            public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
            public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
            protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

            public void AddResult(ValidationResult validationResult)
            {
                _results.Add(validationResult);
            }
        }

        public class IdValidation : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {

                return ValidationResult.Success;
            }
        }

        public bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
                //if (pi.PropertyType == typeof(IFormFile))
                //{
                //    IFormFile value = (IFormFile)pi.GetValue(myObject);
                //    if (value == null)
                //    {
                //        return true;
                //    }
                //}

            }
            return false;
        }
    }



}
