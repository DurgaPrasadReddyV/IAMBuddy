using IAMBuddy.RequestIntakeService.Models;
using IAMBuddy.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace IAMBuddy.RequestIntakeService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<MSSQLAccountRequest> AccountRequests { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AccountRequest
            modelBuilder.Entity<MSSQLAccountRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServerAccountName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DatabaseAccountName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ServerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RequestorEmail).IsRequired().HasMaxLength(200);
            });
        }
    }
}
