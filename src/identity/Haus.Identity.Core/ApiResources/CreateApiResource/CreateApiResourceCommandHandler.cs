using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateApiResourceCommandHandler : ICommandHandler<CreateApiResourceCommand, CreateApiResourceResult>
    {
        private readonly ConfigurationDbContext _context;

        public CreateApiResourceCommandHandler(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateApiResourceResult> Handle(CreateApiResourceCommand request,
            CancellationToken cancellationToken)
        {
            if (await DoesApiResourceExist(request.Name))
            {
                return CreateApiResourceResult.Failed($"Api resource '{request.Name}` already exists");
            }
            var resource = new ApiResource(request.Name, request.DisplayName);
            _context.Add(resource.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return CreateApiResourceResult.Success();
        }

        private async Task<bool> DoesApiResourceExist(string name)
        {
            return await _context.ApiResources.AnyAsync(r => r.Name == name);
        }
    }
}