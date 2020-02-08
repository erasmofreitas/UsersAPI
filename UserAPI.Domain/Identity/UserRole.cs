using Microsoft.AspNetCore.Identity;

namespace UserAPI.Domain.Identity
{
    public class UserRole : IdentityUserRole<int>
    {
        public UserIdentity UserIdentity { get; set; }
        public Role Roles { get; set; }
    }
}