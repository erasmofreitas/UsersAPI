using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserAPI.Domain;
using UserAPI.Domain.Identity;

namespace UserAPI.Repository
{
    public class UserApiContext : IdentityDbContext<UserIdentity, Role, int,
                                                    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
                                                    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public UserApiContext(DbContextOptions<UserApiContext> options) : base (options) { }

        public DbSet<Users> users {get; set; }
    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        base.OnModelCreating(modelbuilder);

        modelbuilder.Entity<UserRole>(UserRole =>
            {

                UserRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                UserRole.HasOne(ur => ur.Roles)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                UserRole.HasOne(ur => ur.UserIdentity)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();                

            }
        );
    }

    }
}