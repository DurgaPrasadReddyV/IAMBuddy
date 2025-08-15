namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using System;

internal interface IBusinessAppOwnedDirectoryPrincipal : IBusinessAppOwnedResource
{
    public int DomainId { get; set; }
    public BusinessAppActiveDirectoryDirectoryDomain Domain { get; set; }
    public string SamAccountName { get; set; }
    public string? UserPrincipalName { get; set; }
    public string DistinguishedName { get; set; }
    public string Sid { get; set; }
    public int? OrganizationalUnitId { get; set; }
    public BusinessAppActiveDirectoryOrganizationalUnit? OrganizationalUnit { get; set; }
    public bool Enabled { get; set; }
    public DateTimeOffset? WhenCreated { get; set; }
    public DateTimeOffset? WhenChanged { get; set; }
}
