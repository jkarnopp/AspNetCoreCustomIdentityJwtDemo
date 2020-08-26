using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AspNetCoreCustomIdentyJwtDemo.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser() : base()
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? CompanyId { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}