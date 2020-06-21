using Haus.Extensions;

namespace Haus.Cqrs.Commands
{
    public abstract class CommandResult
    {
        public bool WasSuccessful => Errors.IsEmptyOrNull();
        
        public string[] Errors { get; }

        protected CommandResult(params string[] errors)
        {
            Errors = errors;
        }
    }
}