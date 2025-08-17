namespace IAMBuddy.Infrastructure;
using System.Reflection;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Users;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class IAMBuddyDbContext(DbContextOptions<IAMBuddyDbContext> options) : DbContext(options)
{
    // Common entities
    public DbSet<AdminAuditLog> AdminAuditLogs { get; set; } = null!;
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; } = null!;
    public DbSet<AuthoritativeSource> AuthoritativeSources { get; set; } = null!;
    public DbSet<HumanIdentity> HumanIdentities { get; set; } = null!;

    // Business Application entities
    public DbSet<BusinessAppEnvironment> BusinessAppEnvironments { get; set; } = null!;
    public DbSet<BusinessApplication> BusinessApplications { get; set; } = null!;
    public DbSet<BusinessAppResourceIdentity> BusinessAppResourceIdentities { get; set; } = null!;
    public DbSet<BusinessAppUser> BusinessAppUsers { get; set; } = null!;

    // Business Application -> Active Directory entities
    public DbSet<BusinessAppActiveDirectoryDirectoryDomain> BusinessAppActiveDirectoryDirectoryDomains { get; set; } = null!;
    public DbSet<BusinessAppActiveDirectoryDirectoryForest> BusinessAppActiveDirectoryDirectoryForests { get; set; } = null!;
    public DbSet<BusinessAppActiveDirectoryOrganizationalUnit> BusinessAppActiveDirectoryOrganizationalUnits { get; set; } = null!;
    public DbSet<BusinessAppHumanActiveDirectoryAccount> BusinessAppHumanActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<BusinessAppServiceActiveDirectoryAccount> BusinessAppServiceActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppHumanActiveDirectoryAccount> DeprovisionBusinessAppHumanActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppServiceActiveDirectoryAccount> DeprovisionBusinessAppServiceActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<ProvisionBusinessAppHumanActiveDirectoryAccount> ProvisionBusinessAppHumanActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<ProvisionBusinessAppServiceActiveDirectoryAccount> ProvisionBusinessAppServiceActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<UpdateBusinessAppHumanActiveDirectoryAccount> UpdateBusinessAppHumanActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<UpdateBusinessAppServiceActiveDirectoryAccount> UpdateBusinessAppServiceActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<BusinessAppActiveDirectoryGroup> BusinessAppActiveDirectoryGroups { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppActiveDirectoryGroup> DeprovisionBusinessAppActiveDirectoryGroups { get; set; } = null!;
    public DbSet<ProvisionBusinessAppActiveDirectoryGroup> ProvisionBusinessAppActiveDirectoryGroups { get; set; } = null!;
    public DbSet<UpdateBusinessAppActiveDirectoryGroup> UpdateBusinessAppActiveDirectoryGroups { get; set; } = null!;
    public DbSet<AddBusinessAppHumanActiveDirectoryGroupMembership> AddBusinessAppHumanActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<AddBusinessAppServiceActiveDirectoryGroupMembership> AddBusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<BusinessAppHumanActiveDirectoryGroupMembership> BusinessAppHumanActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<BusinessAppServiceActiveDirectoryGroupMembership> BusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<RemoveBusinessAppHumanActiveDirectoryGroupMembership> RemoveBusinessAppHumanActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<RemoveBusinessAppServiceActiveDirectoryGroupMembership> RemoveBusinessAppServiceActiveDirectoryGroupMemberships { get; set; } = null!;

    // Business Application -> MSSQL entities
    public DbSet<BusinessAppMSSQLServerListener> BusinessAppMSSQLServerListeners { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabase> BusinessAppMSSQLDatabases { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLDatabaseADGroupUserRole> AddBusinessAppMSSQLDatabaseADGroupUserRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLDatabaseADHumanUserRole> AddBusinessAppMSSQLDatabaseADHumanUserRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLDatabaseADServiceUserRole> AddBusinessAppMSSQLDatabaseADServiceUserRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLDatabaseSQLAccountUserRole> AddBusinessAppMSSQLDatabaseSQLAccountUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADGroupUserRole> BusinessAppMSSQLDatabaseADGroupUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADHumanUserRole> BusinessAppMSSQLDatabaseADHumanUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADServiceUserRole> BusinessAppMSSQLDatabaseADServiceUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseSQLAccountUserRole> BusinessAppMSSQLDatabaseSQLAccountUserRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLDatabaseADGroupUserRole> RemoveBusinessAppMSSQLDatabaseADGroupUserRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLDatabaseADHumanUserRole> RemoveBusinessAppMSSQLDatabaseADHumanUserRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLDatabaseADServiceUserRole> RemoveBusinessAppMSSQLDatabaseADServiceUserRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLDatabaseSQLAccountUserRole> RemoveBusinessAppMSSQLDatabaseSQLAccountUserRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabasePermission> BusinessAppMSSQLDatabasePermissions { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseRole> BusinessAppMSSQLDatabaseRoles { get; set; } = null!;


    public DbSet<BusinessAppMSSQLDatabaseADGroupUser> BusinessAppMSSQLDatabaseADGroupUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADHumanUser> BusinessAppMSSQLDatabaseADHumanUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseADServiceUser> BusinessAppMSSQLDatabaseADServiceUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLDatabaseSQLAccountUser> BusinessAppMSSQLDatabaseSQLAccountUsers { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLDatabaseADGroupUser> DeprovisionBusinessAppMSSQLDatabaseADGroupUsers { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLDatabaseADHumanUser> DeprovisionBusinessAppMSSQLDatabaseADHumanUsers { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLDatabaseADServiceUser> DeprovisionBusinessAppMSSQLDatabaseADServiceUsers { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLDatabaseSQLAccountUser> DeprovisionBusinessAppMSSQLDatabaseSQLAccountUsers { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLDatabaseADGroupUser> ProvisionBusinessAppMSSQLDatabaseADGroupUsers { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLDatabaseADHumanUser> ProvisionBusinessAppMSSQLDatabaseADHumanUsers { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLDatabaseADServiceUser> ProvisionBusinessAppMSSQLDatabaseADServiceUsers { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLDatabaseSQLAccountUser> ProvisionBusinessAppMSSQLDatabaseSQLAccountUsers { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLDatabaseADGroupUser> UpdateBusinessAppMSSQLDatabaseADGroupUsers { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLDatabaseADHumanUser> UpdateBusinessAppMSSQLDatabaseADHumanUsers { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLDatabaseADServiceUser> UpdateBusinessAppMSSQLDatabaseADServiceUsers { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLDatabaseSQLAccountUser> UpdateBusinessAppMSSQLDatabaseSQLAccountUsers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServer> BusinessAppMSSQLServers { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerInstance> BusinessAppMSSQLServerInstances { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADGroupLogin> BusinessAppMSSQLServerADGroupLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADHumanLogin> BusinessAppMSSQLServerADHumanLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADServiceLogin> BusinessAppMSSQLServerADServiceLogins { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerSQLAccountLogin> BusinessAppMSSQLServerSQLAccountLogins { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLServerADGroupLogin> DeprovisionBusinessAppMSSQLServerADGroupLogins { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLServerADHumanLogin> DeprovisionBusinessAppMSSQLServerADHumanLogins { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLServerADServiceLogin> DeprovisionBusinessAppMSSQLServerADServiceLogins { get; set; } = null!;
    public DbSet<DeprovisionBusinessAppMSSQLServerSQLAccountLogin> DeprovisionBusinessAppMSSQLServerSQLAccountLogins { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLServerADGroupLogin> ProvisionBusinessAppMSSQLServerADGroupLogins { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLServerADHumanLogin> ProvisionBusinessAppMSSQLServerADHumanLogins { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLServerADServiceLogin> ProvisionBusinessAppMSSQLServerADServiceLogins { get; set; } = null!;
    public DbSet<ProvisionBusinessAppMSSQLServerSQLAccountLogin> ProvisionBusinessAppMSSQLServerSQLAccountLogins { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLServerADGroupLogin> UpdateBusinessAppMSSQLServerADGroupLogins { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLServerADHumanLogin> UpdateBusinessAppMSSQLServerADHumanLogins { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLServerADServiceLogin> UpdateBusinessAppMSSQLServerADServiceLogins { get; set; } = null!;
    public DbSet<UpdateBusinessAppMSSQLServerSQLAccountLogin> UpdateBusinessAppMSSQLServerSQLAccountLogins { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLServerADGroupLoginRole> AddBusinessAppMSSQLServerADGroupLoginRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLServerADHumanLoginRole> AddBusinessAppMSSQLServerADHumanLoginRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLServerADServiceLoginRole> AddBusinessAppMSSQLServerADServiceLoginRoles { get; set; } = null!;
    public DbSet<AddBusinessAppMSSQLServerSQLAccountLoginRole> AddBusinessAppMSSQLServerSQLAccountLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADGroupLoginRole> BusinessAppMSSQLServerADGroupLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADHumanLoginRole> BusinessAppMSSQLServerADHumanLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerADServiceLoginRole> BusinessAppMSSQLServerADServiceLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerSQLAccountLoginRole> BusinessAppMSSQLServerSQLAccountLoginRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLServerADGroupLoginRole> RemoveBusinessAppMSSQLServerADGroupLoginRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLServerADHumanLoginRole> RemoveBusinessAppMSSQLServerADHumanLoginRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLServerADServiceLoginRole> RemoveBusinessAppMSSQLServerADServiceLoginRoles { get; set; } = null!;
    public DbSet<RemoveBusinessAppMSSQLServerSQLAccountLoginRole> RemoveBusinessAppMSSQLServerSQLAccountLoginRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerRole> BusinessAppMSSQLServerRoles { get; set; } = null!;
    public DbSet<BusinessAppMSSQLServerPermission> BusinessAppMSSQLServerPermissions { get; set; } = null!;

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
            .Where(e => e.Entity is IAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var currentDateTime = DateTimeOffset.UtcNow; // Use UTC for consistency
        var currentUser = "SystemSeeder"; // Replace with actual user context in a real application

        foreach (var entry in entries)
        {
            var auditableEntity = (IAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                auditableEntity.CreatedAt = currentDateTime;
                auditableEntity.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Deleted)
            {
                auditableEntity.DeletedAt = currentDateTime;
                auditableEntity.DeletedBy = currentUser;
                auditableEntity.IsDeleted = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditableEntity.UpdatedAt = currentDateTime;
                auditableEntity.UpdatedBy = currentUser;
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
