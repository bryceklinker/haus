using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Common;

public interface IHausCommandCreator<T>
{
    HausCommand<T> AsHausCommand();
}