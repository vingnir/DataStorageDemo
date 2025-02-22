using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Decimal Precision
            modelBuilder.Entity<Project>()
                .Property(p => p.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Service>()
                .Property(s => s.HourlyPrice)
                .HasPrecision(18, 2);

            // Set ProjectNumber as Primary Key
            modelBuilder.Entity<Project>()
                .HasKey(p => p.ProjectNumber);

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectNumber)
                .IsUnique();

            // Seed Data (Ensures Only One-Time Seeding)
            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceId = 1, Name = "Consulting", HourlyPrice = 100.00m },
                new Service { ServiceId = 2, Name = "Development", HourlyPrice = 150.00m }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, Name = "New" },
                new Status { StatusId = 2, Name = "In Progress" },
                new Status { StatusId = 3, Name = "Completed" }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Project Manager" },
                new Role { Id = 2, Name = "Developer" },
                new Role { Id = 3, Name = "Designer" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
