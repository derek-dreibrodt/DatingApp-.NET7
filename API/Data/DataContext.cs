using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        //public DbSet<Photo> Photos {get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Not including could cause bugs

            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId}); // Represents primay key in the likes table
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
        }
    }
}