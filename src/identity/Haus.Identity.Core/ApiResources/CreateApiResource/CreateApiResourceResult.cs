using Haus.Cqrs.Commands;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateApiResourceResult : CommandResult
    {
        private CreateApiResourceResult(params string[] errors)
            : base(errors)
        {
        }

        public static CreateApiResourceResult Failed(params string[] errors)
        {
            return new CreateApiResourceResult(errors);
        }

        public static CreateApiResourceResult Success()
        {
            return new CreateApiResourceResult();
        }
    }
}