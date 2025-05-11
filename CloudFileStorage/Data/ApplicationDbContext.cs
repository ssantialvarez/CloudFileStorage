using CloudFileStorage.Models;
using Microsoft.EntityFrameworkCore;
using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Data
{
    public class ApplicationDbContext : DbContext
    { 
        public ApplicationDbContext(DbContextOptions options) :base(options){ }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }

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

            modelBuilder.Entity<File>()
                .Property(f => f.fileName)
                .IsRequired();
            modelBuilder.Entity<File>()
                .Property(f => f.size)
                .IsRequired();
            modelBuilder.Entity<File>()
                .Property(f => f.uploadedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<File>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
