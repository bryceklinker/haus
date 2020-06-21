using Haus.Cqrs.Commands;

namespace Haus.Identity.Core.IdentityResources.CreateIdentityResource
{
    public class CreateIdentityResourceResult : CommandResult
    {
        private CreateIdentityResourceResult(params string[] errors) 
            : base(errors)
        {
            
        }

        public static CreateIdentityResourceResult Success()
        {
            return new CreateIdentityResourceResult();
        }

        public static CreateIdentityResourceResult Failed(params string[] errors)
        {
            return new CreateIdentityResourceResult(errors);
        }
    }
}