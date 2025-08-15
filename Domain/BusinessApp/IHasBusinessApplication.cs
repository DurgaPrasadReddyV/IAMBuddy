namespace IAMBuddy.Domain.Common;
using IAMBuddy.Domain.BusinessApp;

internal interface IHasBusinessApplication
{
    public int BusinessApplicationId { get; set; }
    public BusinessApplication BusinessApplication { get; set; }
}
