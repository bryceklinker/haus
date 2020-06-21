using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Models;
using MediatR;

namespace Haus.Identity.Core.Accounts.GetAccounts
{
    public class GetAccountsQuery : IQuery<ListModel<HausUserModel>>
    {
        public string SearchTerm { get; }

        public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);
        
        public GetAccountsQuery(string searchTerm = null)
        {
            SearchTerm = searchTerm;
        }
    }
}