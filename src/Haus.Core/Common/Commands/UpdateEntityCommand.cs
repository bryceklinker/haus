using Haus.Core.Models.Common;
using Haus.Cqrs.Commands;

namespace Haus.Core.Common.Commands
{
    public class UpdateEntityCommand<TModel> : ICommand
        where TModel : IdentityModel
    {
        public long Id => Model.Id;
        public TModel Model { get; }

        public UpdateEntityCommand(TModel model)
        {
            Model = model;
        }
    }
}