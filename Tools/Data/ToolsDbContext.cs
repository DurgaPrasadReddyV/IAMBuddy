using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ToolsDbContext : DbContext
{
    public ToolsDbContext(DbContextOptions<ToolsDbContext> options) : base(options)
    {
    }

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
    public DbSet<Permission> Permissions { get; set; } = null!;
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
                    property.SetValueConverter((ValueConverter)converterInstance);
                }
            }
        }

        this.ConfigurePostgreSqlSpecifics(modelBuilder);
    }

    private void ConfigurePostgreSqlSpecifics(ModelBuilder modelBuilder)
    {
        // Use snake_case naming convention for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.ToSnakeCase());
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName()?.ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
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

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = System.Text.RegularExpressions.Regex.Replace(
            input,
            "([a-z0-9])([A-Z])",
            "$1_$2"
        ).ToLowerInvariant();

        return result;
    }
}
