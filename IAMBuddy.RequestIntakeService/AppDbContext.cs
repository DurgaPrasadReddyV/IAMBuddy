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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AccountRequest
            modelBuilder.Entity<MSSQLAccountRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ServerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RequestorEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BusinessJustification).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.RequestedDate).HasDefaultValueSql("GETUTCDATE()");
                
                // Performance indexes
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_AccountRequests_Status");
                entity.HasIndex(e => e.RequestedDate).HasDatabaseName("IX_AccountRequests_RequestedDate");
                entity.HasIndex(e => e.RequestorEmail).HasDatabaseName("IX_AccountRequests_RequestorEmail");
                entity.HasIndex(e => e.WorkflowId).HasDatabaseName("IX_AccountRequests_WorkflowId");
                entity.HasIndex(e => new { e.Username, e.ServerName, e.DatabaseName }).HasDatabaseName("IX_AccountRequests_Username_Server_Database");
            });
        }
    }
}
