using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Category
    {
        [Key]

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string FormType { get; set; }        
    }
}
