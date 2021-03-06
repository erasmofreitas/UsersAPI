using Microsoft.AspNetCore.Identity;

namespace UserAPI.Domain.Identity
{
    public class UserRole : IdentityUserRole<int>
    {
        public User user { get; set; }
        public Role Roles { get; set; }
    }
}