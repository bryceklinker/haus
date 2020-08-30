using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Clients;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Portal.Web.Settings.Commands
{
    public class UpdateClientSettingsCommand : ICommand
    {
        public ClientCreatedPayload Payload { get; }

        public UpdateClientSettingsCommand(ClientCreatedPayload payload)
        {
            Payload = payload;
        }
    }
    
    public class UpdateClientSettingsCommandHandler : CommandHandler<UpdateClientSettingsCommand>
    {
        private readonly HausPortalDbContext _context;

        public UpdateClientSettingsCommandHandler(HausPortalDbContext context)
        {
            _context = context;
        }

        protected override async Task InnerHandle(UpdateClientSettingsCommand command, CancellationToken token = default)
        {
            if (ShouldSkipUpdatingSettings(command))
                return;
            
            var existingSettings = await _context.Set<AuthSettings>().SingleOrDefaultAsync(token);
            var authSettings = MapToAuthSettings(existingSettings, command.Payload);
            if (existingSettings == null) _context.Add(authSettings);
            await _context.SaveChangesAsync(token);
        }

        private static bool ShouldSkipUpdatingSettings(UpdateClientSettingsCommand command)
        {
            return !command.Payload.ClientName.Equals(AuthSettings.ClientName, StringComparison.OrdinalIgnoreCase);
        }

        private AuthSettings MapToAuthSettings(AuthSettings settings, ClientCreatedPayload payload)
        {
            var authSettings = settings ?? new AuthSettings();
            authSettings.ClientId = payload.ClientId;
            authSettings.ClientSecret = payload.ClientSecret;
            return authSettings;
        }
    }
}