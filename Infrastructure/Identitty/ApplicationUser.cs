
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //This is the class that represents the user in the identity system, it inherits from IdentityUser which is a class provided by ASP.NET Core Identity
        public string? FullName { get; set; }

    }
}
