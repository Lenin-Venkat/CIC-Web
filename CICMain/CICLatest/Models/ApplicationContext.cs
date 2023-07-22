using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class ApplicationContext : IdentityDbContext<UserModel>
    {
        public ApplicationContext(DbContextOptions options)
         : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategoryType> SubCategory { get; set; }

        public DbSet<tblAssociation> tblAssociation { get; set; }

        public DbSet<CICFees> cicFees { get; set; }

       // public DbSet<SubContractors> SubContractors { get; set; }
    }
}
