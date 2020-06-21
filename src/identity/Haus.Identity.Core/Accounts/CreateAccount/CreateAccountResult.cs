using Haus.Identity.Core.Common.Messaging.Commands;

namespace Haus.Identity.Core.Accounts.CreateAccount
{
    public class CreateAccountResult : CommandResult
    {
        public string Id { get; }
        
        private CreateAccountResult(string id)
        {
            Id = id;
        }

        private CreateAccountResult(params string[] errors)
            : base(errors)
        {
        }
        
        public static CreateAccountResult Success(string id)
        {
            return new CreateAccountResult(id);
        }

        public static CreateAccountResult Failed(string[] errors)
        {
            return new CreateAccountResult(errors);
        }
    }
}