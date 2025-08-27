namespace IAMBuddy.Application.ActiveDirectory;

using System.Threading.Tasks;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using MediatR;

public record ProvisionBusinessAppHumanActiveDirectoryAccountCommand : IRequest<int>
{
}

public class ProvisionBusinessAppHumanActiveDirectoryAccountCommandHandler(IIAMBuddyDbContext context) :
    IRequestHandler<ProvisionBusinessAppHumanActiveDirectoryAccountCommand, int>
{
    private readonly IIAMBuddyDbContext context = context;

    public async Task<int> Handle(ProvisionBusinessAppHumanActiveDirectoryAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = new ProvisionBusinessAppHumanActiveDirectoryAccount
        {
        };

        this.context.ProvisionBusinessAppHumanActiveDirectoryAccounts.Add(entity);

        await this.context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
