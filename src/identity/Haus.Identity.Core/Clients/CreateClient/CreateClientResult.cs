using Haus.Cqrs.Commands;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateClientResult : CommandResult
    {
        public string Id { get; private set; }

        private CreateClientResult(params string[] errors)
            : base(errors)
        {
            
        }        
        public static CreateClientResult Success(string id)
        {
            return new CreateClientResult
            {
                Id = id
            };
        }

        public static CreateClientResult Failed(params string[] errors)
        {
            return new CreateClientResult(errors);
        }
    }
}