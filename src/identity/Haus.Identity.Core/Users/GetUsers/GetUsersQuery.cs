using Haus.Cqrs.Queries;
using Haus.Identity.Core.Users.Models;
using Haus.Models;

namespace Haus.Identity.Core.Users.GetUsers
{
    public class GetUsersQuery : IQuery<ListModel<HausUserModel>>
    {
        public string SearchTerm { get; }

        public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);
        
        public GetUsersQuery(string searchTerm = null)
        {
            SearchTerm = searchTerm;
        }
    }
}