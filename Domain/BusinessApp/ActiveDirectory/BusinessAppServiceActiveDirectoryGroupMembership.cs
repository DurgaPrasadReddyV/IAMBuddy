namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp;

public class BusinessAppServiceActiveDirectoryGroupMembership : AppOwnedBusinessAppResource
{
    public DateTime? MemberSince { get; set; }
    public string? AddedBy { get; set; }
    public int GroupId { get; set; }
    public virtual BusinessAppActiveDirectoryGroup Group { get; set; } = null!;
    public int? BusinessAppServiceActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppServiceActiveDirectoryAccount? BusinessAppServiceActiveDirectoryAccount { get; set; }
}
