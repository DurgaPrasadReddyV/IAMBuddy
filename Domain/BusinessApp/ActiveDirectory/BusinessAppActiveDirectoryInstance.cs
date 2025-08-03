namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp;

public class BusinessAppActiveDirectoryInstance : BusinessAppOwnedResource
{
    public string DomainName { get; set; } = string.Empty;
    public string? ForestName { get; set; }
    public string? IPAddress { get; set; }
    public virtual ICollection<BusinessAppHumanActiveDirectoryAccount> HumanAccounts { get; set; } = [];
    public virtual ICollection<BusinessAppServiceActiveDirectoryAccount> ServiceAccounts { get; set; } = [];
    public virtual ICollection<BusinessAppActiveDirectoryGroup> Groups { get; set; } = [];
}
