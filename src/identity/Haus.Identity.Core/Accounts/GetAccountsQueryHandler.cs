using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Identity.Core.Accounts
{
    public class GetAccountsQuery : IRequest
    {
        
    }
    
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery>
    {
        public async Task<Unit> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}