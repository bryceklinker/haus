using System;
using System.Linq;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateApiResourceResult
    {
        public bool WasSuccessful => Errors.IsEmptyOrNull();

        public string[] Errors { get; }

        private CreateApiResourceResult(string[] errors)
        {
            Errors = errors;
        }

        public static CreateApiResourceResult Failed(params string[] errors)
        {
            return new CreateApiResourceResult(errors);
        }

        public static CreateApiResourceResult Success()
        {
            return new CreateApiResourceResult(Array.Empty<string>());
        }
    }
}