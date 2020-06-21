using System.Collections.Generic;
using System.Collections.Immutable;

namespace Haus.Identity.Core.Accounts.CreateAccount
{
    public class CreateAccountResult
    {
        public string Id { get; }
        
        public bool WasSuccessful { get; }

        public ImmutableArray<string> Errors { get; }
        
        private CreateAccountResult(string id)
        {
            WasSuccessful = true;
            Id = id;
        }

        private CreateAccountResult(IEnumerable<string> errors)
        {
            WasSuccessful = false;
            Errors = errors.ToImmutableArray();
        }
        
        public static CreateAccountResult Success(string id)
        {
            return new CreateAccountResult(id);
        }

        public static CreateAccountResult Failed(IEnumerable<string> errors)
        {
            return new CreateAccountResult(errors);
        }
    }
}