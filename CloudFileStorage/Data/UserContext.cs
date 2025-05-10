using CloudFileStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudFileStorage.Data
{
    public class UserContext : DbContext
    { 
        public UserContext(DbContextOptions options) :base(options){ }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.username)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.password)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v)
                );
            modelBuilder.Entity<User>()
                .Property(u => u.createdAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>()
                .Property(u => u.updatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
