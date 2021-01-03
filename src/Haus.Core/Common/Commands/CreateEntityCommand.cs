using Haus.Cqrs.Commands;

namespace Haus.Core.Common.Commands
{
    public abstract record CreateEntityCommand<TModel>(TModel Model) : ICommand<TModel>;
}