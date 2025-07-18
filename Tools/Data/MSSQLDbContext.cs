namespace IAMBuddy.Tools.Data;

using IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;
using Microsoft.EntityFrameworkCore;

public class MSSQLDbContext(DbContextOptions<MSSQLDbContext> options) : DbContext(options)
{

    // DbSets
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

        // Apply configurations
        modelBuilder.ApplyConfiguration(new SqlServerListenerConfiguration());
        modelBuilder.ApplyConfiguration(new SqlServerConfiguration());
        modelBuilder.ApplyConfiguration(new SqlServerInstanceConfiguration());
        modelBuilder.ApplyConfiguration(new DatabaseConfiguration());
        modelBuilder.ApplyConfiguration(new ServerLoginConfiguration());
        modelBuilder.ApplyConfiguration(new ServerRoleConfiguration());
        modelBuilder.ApplyConfiguration(new DatabaseUserConfiguration());
        modelBuilder.ApplyConfiguration(new DatabaseRoleConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new ServerLoginRoleConfiguration());
        modelBuilder.ApplyConfiguration(new DatabaseUserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AdminAuditLogConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        this.UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = this.ChangeTracker.Entries<AuditableEntity>();
        var currentUser = "System"; // Get from current context/user service
        var currentTime = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = currentTime;
                    entry.Entity.CreatedBy = currentUser;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = currentTime;
                    entry.Entity.ModifiedBy = currentUser;
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    break;
            }
        }
    }
}
