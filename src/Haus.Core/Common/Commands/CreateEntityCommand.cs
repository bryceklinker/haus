namespace Haus.Core.Common.Commands
{
    public abstract class CreateEntityCommand<TModel> : ICommand<TModel>
    {
        public TModel Model { get; }

        public CreateEntityCommand(TModel model)
        {
            Model = model;
        }
    }
}