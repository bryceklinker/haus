using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Common
{
    public interface IHausEventConverter<T>
    {
        HausEvent<T> AsHausEvent();
    }
}