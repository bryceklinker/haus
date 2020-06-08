using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Models;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Core.Accounts
{
    public static class SignInManagerExtensions
    {
        public static async Task<SignInResult> PasswordSignInAsync(
            this SignInManager<HausUser> signInManager,
            LoginRequest request)
        {
            return await signInManager.PasswordSignInAsync(
                request.Username,
                request.Password,
                true,
                false
            ).ConfigureAwait(false);
        }
    }
}