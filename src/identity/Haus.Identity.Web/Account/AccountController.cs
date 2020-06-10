using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Identity.Web.Account
{
    [Route("api/account")]
    public class AccountsController : Controller
    {
        private readonly IClientSecretValidator _clientSecretValidator;
        private readonly ITokenRequestValidator _tokenRequestValidator;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        
        public AccountsController(
            ITokenRequestValidator tokenRequestValidator, 
            IClientSecretValidator clientSecretValidator, 
            ITokenResponseGenerator tokenResponseGenerator)
        {
            _tokenRequestValidator = tokenRequestValidator;
            _clientSecretValidator = clientSecretValidator;
            _tokenResponseGenerator = tokenResponseGenerator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(IFormCollection form)
        {
            var clientResult = await _clientSecretValidator.ValidateAsync(HttpContext);
            var tokenResult = await _tokenRequestValidator.ValidateRequestAsync(form.AsNameValueCollection(), clientResult);
            if (tokenResult.IsError)
                return Unauthorized(tokenResult);

            var response = await _tokenResponseGenerator.ProcessAsync(tokenResult);
            return Ok(response);
        }
    }
}