namespace IAMBuddy.Domain.Common;
using IAMBuddy.Domain.BusinessApp;

internal interface IHasPrimaryOwner
{
    public int PrimaryOwnerId { get; set; }
    public BusinessAppUser PrimaryOwner { get; set; }
}
