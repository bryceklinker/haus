using System.Threading.Tasks;
using Haus.Identity.Core.Clients.Factories;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConsentRequest = Haus.Identity.Core.Clients.Models.ConsentRequest;

namespace Haus.Identity.Web.Consent
{
    [Authorize]
    [Route("consent")]
    public class ConsentController : Controller
    {
        private readonly IConsentRequestFactory _factory;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;

        public ConsentController(IConsentRequestFactory factory, 
            IIdentityServerInteractionService interactionService, 
            IClientStore clientStore, 
            IResourceStore resourceStore)
        {
            _factory = factory;
            _interactionService = interactionService;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] string returnUrl)
        {
            var authorizationRequest = await _interactionService.GetAuthorizationContextAsync(returnUrl);
            if (authorizationRequest == null)
                return Unauthorized();

            var client = await _clientStore.FindEnabledClientByIdAsync(authorizationRequest.ClientId);
            if (client == null)
                return Unauthorized();

            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(authorizationRequest.ScopesRequested);
            if (resources == null)
                return Unauthorized();

            var request = _factory.Create(client, returnUrl, resources.IdentityResources, resources.ApiResources);
            return View(request);
        }

        [HttpPost("")]
        public async Task<IActionResult> Index([FromForm] ConsentRequest request)
        {
            if (!request.DidConsent)
                return Unauthorized();
            
            var authorizationRequest = await _interactionService.GetAuthorizationContextAsync(request.ReturnUrl);
            if (authorizationRequest == null)
                return Unauthorized();
            
            var response = new ConsentResponse
            {
                RememberConsent = true,
                ScopesConsented = request.ConsentedScopes
            };
            await _interactionService.GrantConsentAsync(authorizationRequest, response);
            return Redirect(request.ReturnUrl);
        }
    }
}