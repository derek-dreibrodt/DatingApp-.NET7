using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<
        AppUser, 
        AppRole, 
        int, // 
        IdentityUserClaim<int>, 
        AppUserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>
        > // Tells what to use for users/roles. It tells IdentityDbContext what to use for user id's, role id's, etc
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        //public DbSet<AppUser> Users { get; set; } // Removed because we already have it

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Not including could cause bugs

            builder.Entity<AppUser>() 
                .HasMany(ur => ur.UserRoles) // user has many roles
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired(); // foreign key not allowed to be null

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles) // role has many userroles
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired(); // foreign key not allowed to be null

            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId}); // Represents primay key in the likes table, composite primary key
            
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser) // source user can like many users
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId) // foreign key for the relationship
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser) // target user can be liked by many users
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.TargetUserId) // foreign key for the relationship
                .OnDelete(DeleteBehavior.Cascade);
            // Can't have both be DeleteBehavior.Cascade with SQL server - only other databases
            builder.Entity<Message>()
                .HasOne(m => m.Recipient) // source user can like many users
                .WithMany(l => l.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict); // Shouldn't delete message just because users have removed their profile

            builder.Entity<Message>()
                .HasOne(m => m.Sender) // target user can be liked by many users
                .WithMany(l => l.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}