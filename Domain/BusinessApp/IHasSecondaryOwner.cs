namespace IAMBuddy.Domain.Common;
using IAMBuddy.Domain.BusinessApp;

internal interface IHasSecondaryOwner
{
    public int? SecondaryOwnerId { get; set; }
    public BusinessAppUser? SecondaryOwner { get; set; }
}
