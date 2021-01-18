using Haus.Core.Models.Common;
using Haus.Cqrs.Commands;

namespace Haus.Core.Common.Commands
{
    public abstract record UpdateEntityCommand<TModel>(TModel Model) : ICommand
        where TModel : IdentityModel
    {
        public long Id => Model.Id;
    }
}