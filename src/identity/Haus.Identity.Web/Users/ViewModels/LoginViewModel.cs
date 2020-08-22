using System.Threading.Tasks;
using Haus.Identity.Web.Users.Entities;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Web.Users.ViewModels
{
    public class LoginViewModel
    {
        public string ReturntUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public async Task<bool> Login(SignInManager<HausUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync(Username, Password, false, false);
            return result.Succeeded;
        }
    }
}