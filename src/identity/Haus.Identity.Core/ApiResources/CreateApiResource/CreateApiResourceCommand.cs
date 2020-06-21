using Haus.Identity.Core.Common.Messaging;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateApiResourceCommand : ICommand<CreateApiResourceResult>
    {
        public string Name { get; }
        public string DisplayName { get; }

        public CreateApiResourceCommand(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}