using System.Threading.Tasks;
using Haus.Identity.Core.Users.Entities;
using Haus.Identity.Web.Users.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Web.Users
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