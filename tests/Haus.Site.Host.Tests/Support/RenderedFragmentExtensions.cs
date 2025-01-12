using System.Linq;
using AngleSharp.Dom;

namespace Haus.Site.Host.Tests.Support;

public static class RenderedFragmentExtensions
{
    public static IElement[] FindAllByTag(
        this IRenderedFragment fragment, string tag)
    {
        return fragment.FindAll(tag).ToArray();
    }

    public static IElement FindByTag(this IRenderedFragment fragment, string tag)
    {
        return fragment.Find(tag);
    }

    public static IElement[] FindAllByClass(this IRenderedFragment fragment,
        string className)
    {
        return fragment.FindAll($".{className}").ToArray();
    }
    
    public static IElement FindByClass(this IRenderedFragment fragment,
        string className)
    {
        return fragment.Find($".{className}");
    }

    public static IElement[] FindAllByTag(
        this IRenderedFragment fragment, 
        string tag,
        string text)
    {
        var matches = fragment.FindAllByTag(tag);
        return matches.Where(m => m.Text().Trim().Contains(text)).ToArray();
    }
}