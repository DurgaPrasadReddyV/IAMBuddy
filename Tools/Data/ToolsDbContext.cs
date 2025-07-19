namespace IAMBuddy.Tools.Data;
using System.Reflection;
using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ToolsDbContext(DbContextOptions<ToolsDbContext> options) : DbContext(options)
{

    // DbSets for all entities
    public DbSet<HumanIdentity> HumanIdentities { get; set; } = null!;
    public DbSet<BusinessApplication> BusinessApplications { get; set; } = null!;
    public DbSet<BusinessApplicationEnvironment> BusinessApplicationEnvironments { get; set; } = null!;
    public DbSet<BusinessApplicationTeamMember> BusinessApplicationTeamMembers { get; set; } = null!;
    public DbSet<NonHumanIdentity> NonHumanIdentities { get; set; } = null!;
    public DbSet<ActiveDirectoryAccount> ActiveDirectoryAccounts { get; set; } = null!;
    public DbSet<ActiveDirectoryGroup> ActiveDirectoryGroups { get; set; } = null!;
    public DbSet<ActiveDirectoryGroupMembership> ActiveDirectoryGroupMemberships { get; set; } = null!;
    public DbSet<BusinessApplicationResource> BusinessApplicationResources { get; set; } = null!;
    public DbSet<SqlServerListener> SqlServerListeners { get; set; } = null!;
    public DbSet<SqlServer> SqlServers { get; set; } = null!;
    public DbSet<SqlServerInstance> SqlServerInstances { get; set; } = null!;
    public DbSet<Database> Databases { get; set; } = null!;
    public DbSet<ServerLogin> ServerLogins { get; set; } = null!;
    public DbSet<ServerRole> ServerRoles { get; set; } = null!;
    public DbSet<DatabaseUser> DatabaseUsers { get; set; } = null!;
    public DbSet<DatabaseRole> DatabaseRoles { get; set; } = null!;
    public DbSet<DatabasePermission> Permissions { get; set; } = null!;
    public DbSet<ServerLoginRole> ServerLoginRoles { get; set; } = null!;
    public DbSet<DatabaseUserRole> DatabaseUserRoles { get; set; } = null!;
    public DbSet<AdminAuditLog> AdminAuditLogs { get; set; } = null!;


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
