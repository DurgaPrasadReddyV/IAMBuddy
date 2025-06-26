using IAMBuddy.MSSQLAccountManager.Models;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.MSSQLAccountManager.Data
{
    public class MSSQLAccountManagerContext : DbContext
    {
        public MSSQLAccountManagerContext(DbContextOptions<MSSQLAccountManagerContext> options)
            : base(options)
        {
        }

        public DbSet<SqlLogin> SqlLogins { get; set; }
        public DbSet<ServerRole> ServerRoles { get; set; }
        public DbSet<ServerRoleAssignment> ServerRoleAssignments { get; set; }
        public DbSet<DatabaseUser> DatabaseUsers { get; set; }
        public DbSet<DatabaseRole> DatabaseRoles { get; set; }
        public DbSet<DatabaseRoleAssignment> DatabaseRoleAssignments { get; set; }
        public DbSet<ServerInstance> ServerInstances { get; set; }
        public DbSet<OperationResult> OperationResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure SqlLogin
            modelBuilder.Entity<SqlLogin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ServerName, e.InstanceName, e.LoginName }).IsUnique();
                entity.Property(e => e.LoginType).HasConversion<string>();
            });

            // Configure ServerRole
            modelBuilder.Entity<ServerRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ServerName, e.InstanceName, e.RoleName }).IsUnique();
            });

            // Configure ServerRoleAssignment
            modelBuilder.Entity<ServerRoleAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.SqlLoginId, e.ServerRoleId }).IsUnique();
                
                entity.HasOne(e => e.SqlLogin)
                    .WithMany(e => e.ServerRoleAssignments)
                    .HasForeignKey(e => e.SqlLoginId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.ServerRole)
                    .WithMany(e => e.ServerRoleAssignments)
                    .HasForeignKey(e => e.ServerRoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DatabaseUser
            modelBuilder.Entity<DatabaseUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ServerName, e.InstanceName, e.DatabaseName, e.UserName }).IsUnique();
                entity.Property(e => e.UserType).HasConversion<string>();
                
                entity.HasOne(e => e.SqlLogin)
                    .WithMany(e => e.DatabaseUsers)
                    .HasForeignKey(e => e.SqlLoginId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure DatabaseRole
            modelBuilder.Entity<DatabaseRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ServerName, e.InstanceName, e.DatabaseName, e.RoleName }).IsUnique();
            });

            // Configure DatabaseRoleAssignment
            modelBuilder.Entity<DatabaseRoleAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DatabaseUserId, e.DatabaseRoleId }).IsUnique();
                
                entity.HasOne(e => e.DatabaseUser)
                    .WithMany(e => e.DatabaseRoleAssignments)
                    .HasForeignKey(e => e.DatabaseUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.DatabaseRole)
                    .WithMany(e => e.DatabaseRoleAssignments)
                    .HasForeignKey(e => e.DatabaseRoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ServerInstance
            modelBuilder.Entity<ServerInstance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ServerName, e.InstanceName }).IsUnique();
            });

            // Configure OperationResult
            modelBuilder.Entity<OperationResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OperationType).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.HasIndex(e => new { e.ResourceType, e.ServerName, e.CreatedAt });
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}