namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateClientResult
    {
        public bool WasSuccessful => string.IsNullOrWhiteSpace(ErrorMessage);
        public string Id { get; private set; }
        public string ErrorMessage { get; private set; }

        public static CreateClientResult Success(string id)
        {
            return new CreateClientResult
            {
                Id = id
            };
        }

        public static CreateClientResult Failed(string errorMessage)
        {
            return new CreateClientResult
            {
                ErrorMessage = errorMessage
            };
        }
    }
}