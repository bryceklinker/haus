namespace Haus.Identity.Models.ApiResources
{
    public class CreateApiResourcePayload
    {
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public string[] Scopes { get; set; }

        public CreateApiResourcePayload(string identifier, string[] scopes, string displayName = null)
        {
            Identifier = identifier;
            DisplayName = displayName;
            Scopes = scopes;
        }
    }
}