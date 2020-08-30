namespace Haus.Identity.Models.Clients
{
    public class CreateClientPayload
    {
        public string Name { get; set; }

        public CreateClientPayload(string name)
        {
            Name = name;
        }
    }
}