namespace Haus.Identity.Core.Accounts.Models
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Username { get; set; }
        public bool RememberMe { get; set; }
        public string RedirectUrl { get; set; }
    }
}