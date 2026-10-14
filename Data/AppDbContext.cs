using Microsoft.EntityFrameworkCore;
using AuthDemo.Api.Models;

namespace AuthDemo.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet cho các entity
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ 1-N: Category -> TaskItem (CategoryId có thể null)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed Data: 3 danh mục mặc định
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Work",     Description = "Công việc hàng ngày" },
                new Category { Id = 2, Name = "Study",    Description = "Học tập và nghiên cứu" },
                new Category { Id = 3, Name = "Personal", Description = "Việc cá nhân" }
            );
        }
    }
}