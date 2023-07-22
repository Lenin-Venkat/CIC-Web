using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CICLatest.Models
{
    public class tblAssociation
    {
        [Key]

        public int AssociationId { get; set; }

        public string AssociationName { get; set; }

        public string formType { get; set; }

    }

}
