using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Haus.Site.Host;

public static class RootComponentMappingCollectionExtensions
{
    public static RootComponentMappingCollection AddHausSiteComponents(this RootComponentMappingCollection components)
    {
        components.Add<App>("#app");
        components.Add<HeadOutlet>("head::after");
        return components;
    }
}
