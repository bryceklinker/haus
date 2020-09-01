namespace Haus.Identity.Models.ApiResources
{
    public class ApiResourceCreatedPayload
    {
        public string Identifier { get; set; }

        public ApiResourceCreatedPayload(string identifier)
        {
            Identifier = identifier;
        }
    }
}