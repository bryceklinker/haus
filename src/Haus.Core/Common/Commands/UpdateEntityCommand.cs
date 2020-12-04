using Haus.Core.Models.Common;

namespace Haus.Core.Common.Commands
{
    public class UpdateEntityCommand<TModel> : ICommand
        where TModel : IModel
    {
        public long Id { get; }
        public TModel Model { get; }

        public UpdateEntityCommand(long id, TModel model)
        {
            Id = id;
            Model = model;
            Model.Id = id;
        }
    }
}