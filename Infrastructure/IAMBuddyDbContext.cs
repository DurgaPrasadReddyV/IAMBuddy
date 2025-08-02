namespace IAMBuddy.Tools.Data;
using System.Reflection;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class IAMBuddyDbContext(DbContextOptions<IAMBuddyDbContext> options) : DbContext(options)
{
    public DbSet<AdminAuditLog> AdminAuditLogs { get; set; } = null!;
    public DbSet<HumanIdentity> HumanIdentities { get; set; } = null!;
    public DbSet<BusinessApplication> BusinessApplications { get; set; } = null!;
    public DbSet<BusinessAppEnvironment> BusinessAppEnvironments { get; set; } = null!;
    public DbSet<BusinessAppUser> BusinessAppUsers { get; set; } = null!;
    public DbSet<BusinessAppActiveDirectoryGroup> BusinessAppActiveDirectoryGroups { get; set; } = null!;
    public DbSet<BusinessAppActiveDirectoryInstance> BusinessAppActiveDirectoryInstances { get; set; } = null!;
    public DbSet<BusinessAppHumanActiveDirectoryAccount> BusinessAppHumanActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<BusinessAppHumanActiveDirectoryGroupMembership> BusinessAppHumanActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<BusinessAppServiceActiveDirectoryAccount> BusinessAppServiceActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<BusinessAppServiceActiveDirectoryGroupMembership> BusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabase> BusinessAppMSSQLDatabases { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADGroupUser> BusinessAppMSSQLDatabaseADGroupUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADGroupUserRole> BusinessAppMSSQLDatabaseADGroupUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADHumanUser> BusinessAppMSSQLDatabaseADHumanUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADHumanUserRole> BusinessAppMSSQLDatabaseADHumanUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADServiceUser> BusinessAppMSSQLDatabaseADServiceUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADServiceUserRole> BusinessAppMSSQLDatabaseADServiceUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabasePermission> BusinessAppMSSQLDatabasePermissions { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseRole> BusinessAppMSSQLDatabaseRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseSQLAccountUser> BusinessAppMSSQLDatabaseSQLAccountUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseSQLAccountUserRole> BusinessAppMSSQLDatabaseSQLAccountUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServer> BusinessAppMSSQLServers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADGroupLogin> BusinessAppMSSQLServerADGroupLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADGroupLoginRole> BusinessAppMSSQLServerADGroupLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADHumanLogin> BusinessAppMSSQLServerADHumanLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADHumanLoginRole> BusinessAppMSSQLServerADHumanLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADServiceLogin> BusinessAppMSSQLServerADServiceLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADServiceLoginRole> BusinessAppMSSQLServerADServiceLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerInstance> BusinessAppMSSQLServerInstances { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerListener> BusinessAppMSSQLServerListeners { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerSQLAccountLogin> BusinessAppMSSQLServerSQLAccountLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerSQLAccountLoginRole> BusinessAppMSSQLServerSQLAccountLoginRoles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure enum to string conversions globally or per entity if needed
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                    var converterInstance = Activator.CreateInstance(converterType, null);
                    property.SetValueConverter(converterInstance as ValueConverter);
                }
            }
        }
    }

    public override int SaveChanges()
    {
        this.ApplyAuditableEntityChanges();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyAuditableEntityChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditableEntityChanges()
    {
        var entries = this.ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var currentDateTime = DateTime.UtcNow; // Use UTC for consistency
        var currentUser = "SystemSeeder"; // Replace with actual user context in a real application

        foreach (var entry in entries)
        {
            var auditableEntity = (AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                auditableEntity.CreatedDate = currentDateTime;
                auditableEntity.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditableEntity.ModifiedDate = currentDateTime;
                auditableEntity.ModifiedBy = currentUser;
            }
        }
    }
}

// Enum to string converter for PostgreSQL
public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
{
    public EnumToStringConverter()
        : base(
            v => v.ToString(),
            v => (TEnum)Enum.Parse(typeof(TEnum), v))
    {
    }
}
