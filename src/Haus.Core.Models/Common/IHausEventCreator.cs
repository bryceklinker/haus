using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Common
{
    public interface IHausEventCreator<T>
    {
        HausEvent<T> AsHausEvent();
    }
}