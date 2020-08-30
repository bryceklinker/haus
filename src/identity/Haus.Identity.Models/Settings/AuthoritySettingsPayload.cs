namespace Haus.Identity.Models.Settings
{
    public class AuthoritySettingsPayload
    {
        public string Authority { get; set; }

        public AuthoritySettingsPayload(string authority)
        {
            Authority = authority;
        }
    }
}