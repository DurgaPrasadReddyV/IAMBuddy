using IAMBuddy.SqlServerManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.SqlServerManagementService;

public class SqlServerManagementDbContext : DbContext
{
    public SqlServerManagementDbContext(DbContextOptions<SqlServerManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<SqlServerLogin> SqlServerLogins { get; set; }
    public DbSet<SqlServerRole> SqlServerRoles { get; set; }
    public DbSet<SqlServerUser> SqlServerUsers { get; set; }
    public DbSet<SqlServerRoleAssignment> SqlServerRoleAssignments { get; set; }
    public DbSet<SqlServerOperation> SqlServerOperations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure SqlServerLogin
        modelBuilder.Entity<SqlServerLogin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.LoginName, e.ServerInstance }).IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasMany(e => e.Users)
                .WithOne(e => e.Login)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasMany(e => e.RoleAssignments)
                .WithOne(e => e.Login)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Operations)
                .WithOne(e => e.Login)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure SqlServerRole
        modelBuilder.Entity<SqlServerRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleName, e.ServerInstance, e.DatabaseName }).IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasMany(e => e.RoleAssignments)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Operations)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure SqlServerUser
        modelBuilder.Entity<SqlServerUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserName, e.DatabaseName, e.ServerInstance }).IsUnique();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Login)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasMany(e => e.RoleAssignments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Operations)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure SqlServerRoleAssignment
        modelBuilder.Entity<SqlServerRoleAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Role)
                .WithMany(e => e.RoleAssignments)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Login)
                .WithMany(e => e.RoleAssignments)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(e => e.RoleAssignments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Operations)
                .WithOne(e => e.RoleAssignment)
                .HasForeignKey(e => e.RoleAssignmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ensure either LoginId or UserId is set, but not both
            entity.HasCheckConstraint("CK_RoleAssignment_LoginOrUser", 
                "(LoginId IS NOT NULL AND UserId IS NULL) OR (LoginId IS NULL AND UserId IS NOT NULL)");
        });

        // Configure SqlServerOperation
        modelBuilder.Entity<SqlServerOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.RequestId);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Login)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.LoginId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.User)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Role)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.RoleAssignment)
                .WithMany(e => e.Operations)
                .HasForeignKey(e => e.RoleAssignmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Seed default SQL Server roles
        SeedDefaultRoles(modelBuilder);
    }
    
    private void SeedDefaultRoles(ModelBuilder modelBuilder)
    {
        var defaultServerRoles = new[]
        {
            new SqlServerRole { Id = 1, RoleName = "sysadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can perform any activity in the server", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 2, RoleName = "serveradmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can change server-wide configuration options and shut down the server", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 3, RoleName = "securityadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can manage logins and their properties", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 4, RoleName = "processadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can end processes that are running in an instance of SQL Server", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 5, RoleName = "setupadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can add and remove linked servers", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 6, RoleName = "bulkadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can run the BULK INSERT statement", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 7, RoleName = "diskadmin", RoleType = "Server", ServerInstance = "Default", Description = "Members can manage disk files", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 8, RoleName = "dbcreator", RoleType = "Server", ServerInstance = "Default", Description = "Members can create, alter, drop, and restore any database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 9, RoleName = "public", RoleType = "Server", ServerInstance = "Default", Description = "Every SQL Server login belongs to the public server role", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" }
        };
        
        var defaultDatabaseRoles = new[]
        {
            new SqlServerRole { Id = 10, RoleName = "db_owner", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can perform all configuration and maintenance activities on the database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 11, RoleName = "db_accessadmin", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can add or remove access to the database for Windows logins, Windows groups, and SQL Server logins", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 12, RoleName = "db_securityadmin", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can modify role membership and manage permissions", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 13, RoleName = "db_ddladmin", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can run any Data Definition Language (DDL) command in a database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 14, RoleName = "db_backupoperator", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can back up the database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 15, RoleName = "db_datareader", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can read all data from all user tables", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 16, RoleName = "db_datawriter", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members can add, change, or delete data from all user tables", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 17, RoleName = "db_denydatareader", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members cannot read any data in the user tables within a database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" },
            new SqlServerRole { Id = 18, RoleName = "db_denydatawriter", RoleType = "Database", ServerInstance = "Default", DatabaseName = "Default", Description = "Members cannot add, modify, or delete any data in the user tables within a database", IsBuiltIn = true, CreatedBy = "System", ModifiedBy = "System" }
        };
        
        modelBuilder.Entity<SqlServerRole>().HasData(defaultServerRoles);
        modelBuilder.Entity<SqlServerRole>().HasData(defaultDatabaseRoles);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            
            if (entry.State == EntityState.Added)
            {
                if (entry.Property("CreatedDate").CurrentValue == null)
                    entry.Property("CreatedDate").CurrentValue = now;
            }
            
            if (entry.Property("ModifiedDate") != null)
                entry.Property("ModifiedDate").CurrentValue = now;
        }
    }
}