using Haus.Cqrs.Commands;

namespace Haus.Identity.Core.Users.CreateUser
{
    public class CreateUserResult : CommandResult
    {
        public string Id { get; }
        
        private CreateUserResult(string id)
        {
            Id = id;
        }

        private CreateUserResult(params string[] errors)
            : base(errors)
        {
        }
        
        public static CreateUserResult Success(string id)
        {
            return new CreateUserResult(id);
        }

        public static CreateUserResult Failed(string[] errors)
        {
            return new CreateUserResult(errors);
        }
    }
}