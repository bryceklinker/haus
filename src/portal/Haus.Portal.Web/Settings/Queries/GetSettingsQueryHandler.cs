using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Queries;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Entities;
using IdentityModel;
using Microsoft.EntityFrameworkCore;

namespace Haus.Portal.Web.Settings.Queries
{
    public class GetSettingsQuery : IQuery<SettingsModel>
    {
    }

    public class GetSettingsQueryHandler : IQueryHandler<GetSettingsQuery, SettingsModel>
    {
        private readonly HausPortalDbContext _context;

        public GetSettingsQueryHandler(HausPortalDbContext context)
        {
            _context = context;
        }

        public async Task<SettingsModel> Handle(GetSettingsQuery request, CancellationToken token = default)
        {
            return await _context.Set<AuthSettings>()
                .Select(s => new SettingsModel
                {
                    Authority = s.Authority,
                    ClientId = s.ClientId,
                    ResponseType = OidcConstants.ResponseTypes.Code
                })
                .SingleOrDefaultAsync(token);
        }
    }
}