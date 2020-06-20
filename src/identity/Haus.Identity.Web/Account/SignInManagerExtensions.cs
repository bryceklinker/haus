using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Web.Account.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Web.Account
{
    public static class SignInManagerExtensions
    {
        public static async Task<SignInResult> PasswordSignInAsync(
            this SignInManager<HausUser> signInManager,
            LoginViewModel viewModel)
        {
            return await signInManager.PasswordSignInAsync(
                viewModel.Username,
                viewModel.Password,
                true,
                false
            ).ConfigureAwait(false);
        }
    }
}