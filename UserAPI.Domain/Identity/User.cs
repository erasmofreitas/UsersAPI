using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace UserAPI.Domain.Identity
{
    public class User : IdentityUser<int>
    {
        public string Name {get; set;}
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public List<UserRole> UserRoles {get; set; }
    }
}