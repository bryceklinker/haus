using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Queries;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Entities;
using IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web.Settings.Queries
{
    public class GetSettingsQuery : IQuery<SettingsModel>
    {
    }

    public class GetSettingsQueryHandler : IQueryHandler<GetSettingsQuery, SettingsModel>
    {
        private readonly HausPortalDbContext _context;
        private readonly IConfiguration _configuration;

        public GetSettingsQueryHandler(HausPortalDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<SettingsModel> Handle(GetSettingsQuery request, CancellationToken token = default)
        {
            var allScopes = AuthSettings.Scopes
                .Append(OidcConstants.StandardScopes.Profile)
                .Append(OidcConstants.StandardScopes.OpenId);

            return await _context.Set<AuthSettings>()
                .Select(s => new SettingsModel
                {
                    ClientId = s.ClientId,
                    ResponseType = OidcConstants.ResponseTypes.Code,
                    Authority = _configuration.AuthorityUrl(),
                    Scope = string.Join(" ", allScopes)
                })
                .SingleOrDefaultAsync(token);
        }
    }
}