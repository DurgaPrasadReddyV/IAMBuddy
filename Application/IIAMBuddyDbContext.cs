namespace IAMBuddy.Application;

using System.Threading.Tasks;
using IAMBuddy.Domain.BusinessApp;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;

public interface IIAMBuddyDbContext
{
    public DbSet<BusinessAppEnvironment> BusinessAppEnvironments { get; }
    public DbSet<BusinessApplication> BusinessApplications { get; }
    public DbSet<BusinessAppResourceIdentity> BusinessAppResourceIdentities { get; }
    public DbSet<BusinessAppUser> BusinessAppUsers { get; }

    // Business Application -> Active Directory entities
    public DbSet<BusinessAppActiveDirectoryDirectoryDomain> BusinessAppActiveDirectoryDirectoryDomains { get; }
    public DbSet<BusinessAppActiveDirectoryDirectoryForest> BusinessAppActiveDirectoryDirectoryForests { get; }
    public DbSet<BusinessAppActiveDirectoryOrganizationalUnit> BusinessAppActiveDirectoryOrganizationalUnits { get; }
    public DbSet<BusinessAppHumanActiveDirectoryAccount> BusinessAppHumanActiveDirectoryAccounts { get; }
    public DbSet<BusinessAppServiceActiveDirectoryAccount> BusinessAppServiceActiveDirectoryAccounts { get; }
    public DbSet<DeprovisionBusinessAppHumanActiveDirectoryAccount> DeprovisionBusinessAppHumanActiveDirectoryAccounts { get; }
    public DbSet<DeprovisionBusinessAppServiceActiveDirectoryAccount> DeprovisionBusinessAppServiceActiveDirectoryAccounts { get; }
    public DbSet<ProvisionBusinessAppHumanActiveDirectoryAccount> ProvisionBusinessAppHumanActiveDirectoryAccounts { get; }
    public DbSet<ProvisionBusinessAppServiceActiveDirectoryAccount> ProvisionBusinessAppServiceActiveDirectoryAccounts { get; }
    public DbSet<UpdateBusinessAppHumanActiveDirectoryAccount> UpdateBusinessAppHumanActiveDirectoryAccounts { get; }
    public DbSet<UpdateBusinessAppServiceActiveDirectoryAccount> UpdateBusinessAppServiceActiveDirectoryAccounts { get; }
    public DbSet<BusinessAppActiveDirectoryGroup> BusinessAppActiveDirectoryGroups { get; }
    public DbSet<DeprovisionBusinessAppActiveDirectoryGroup> DeprovisionBusinessAppActiveDirectoryGroups { get; }
    public DbSet<ProvisionBusinessAppActiveDirectoryGroup> ProvisionBusinessAppActiveDirectoryGroups { get; }
    public DbSet<UpdateBusinessAppActiveDirectoryGroup> UpdateBusinessAppActiveDirectoryGroups { get; }
    public DbSet<AddBusinessAppHumanActiveDirectoryGroupMembership> AddBusinessAppHumanActiveDirectoryGroupMemberships { get; }
    public DbSet<AddBusinessAppServiceActiveDirectoryGroupMembership> AddBusinessAppServiceActiveDirectoryGroupMemberships { get; }
    public DbSet<BusinessAppHumanActiveDirectoryGroupMembership> BusinessAppHumanActiveDirectoryGroupMemberships { get; }
    public DbSet<BusinessAppServiceActiveDirectoryGroupMembership> BusinessAppServiceActiveDirectoryGroupMemberships { get; }
    public DbSet<RemoveBusinessAppHumanActiveDirectoryGroupMembership> RemoveBusinessAppHumanActiveDirectoryGroupMemberships { get; }
    public DbSet<RemoveBusinessAppServiceActiveDirectoryGroupMembership> RemoveBusinessAppServiceActiveDirectoryGroupMemberships { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
