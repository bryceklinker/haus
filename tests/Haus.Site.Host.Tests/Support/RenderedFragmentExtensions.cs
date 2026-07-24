using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Diffing.Extensions;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;

namespace Haus.Site.Host.Tests.Support;

public static class RenderedFragmentExtensions
{
    public static IElement[] FindAllByTag(
        this IRenderedComponent<IComponent> fragment,
        string tag,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
    {
        return fragment.FindAll(tag).FindByOptions(configureOptions).ToArray();
    }

    public static IElement FindByTag(
        this IRenderedComponent<IComponent> fragment,
        string tag,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
    {
        return fragment.FindAll(tag).FindByOptions(configureOptions).First();
    }

    public static IElement FindByClass(
        this IRenderedComponent<IComponent> fragment,
        string className,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
    {
        return fragment.FindAllByClass(className, configureOptions).First();
    }

    public static IElement[] FindAllByClass(
        this IRenderedComponent<IComponent> fragment,
        string className,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
    {
        return fragment.FindAll($".{className}").FindByOptions(configureOptions).ToArray();
    }

    public static IElement FindByRole(
        this IRenderedComponent<IComponent> fragment,
        string role,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
    {
        return fragment.FindAll(CreateRoleSelector(role)).FindByOptions(configureOptions).First();
    }

    public static IRenderedComponent<MudButton> FindMudButtonByText(
        this IRenderedComponent<IComponent> fragment,
        string text
    )
    {
        return fragment.FindByComponent<MudButton>(opts => opts.WithText(text));
    }

    public static IRenderedComponent<MudTextField<T>> FindMudTextFieldById<T>(
        this IRenderedComponent<IComponent> fragment,
        string id
    )
    {
        return fragment.FindByComponent<MudTextField<T>>(opts => opts.WithId(id));
    }

    public static IRenderedComponent<T> FindByComponent<T>(
        this IRenderedComponent<IComponent> fragment,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
        where T : IComponent
    {
        return fragment.FindAllByComponent<T>(configureOptions).First();
    }

    public static IEnumerable<IRenderedComponent<T>> FindAllByComponent<T>(
        this IRenderedComponent<IComponent> fragment,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
        where T : IComponent
    {
        return fragment.FindComponents<T>().FindByOptions(configureOptions);
    }

    public static async Task ClickAsync(this IRenderedComponent<MudButton> button)
    {
        await button.InvokeAsync(async () =>
        {
            await button.Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });
    }

    public static T? GetValue<T>(this IRenderedComponent<MudTextField<T>> field)
    {
        return field.Instance.GetState(x => x.Value);
    }

    private static string CreateRoleSelector(string role)
    {
        return role switch
        {
            "button" => "button",
            _ => $"[role={role}]",
        };
    }

    private static IEnumerable<IRenderedComponent<T>> FindByOptions<T>(
        this IEnumerable<IRenderedComponent<T>> components,
        Func<FindOptions, FindOptions>? configureOptions = null
    )
        where T : IComponent
    {
        return components.Where(c => c.FindAll("*").FindByOptions(configureOptions).Any());
    }

    private static IEnumerable<IElement> FindByOptions(
        this IEnumerable<IElement> elements,
        Func<FindOptions, FindOptions>? configureOptions
    )
    {
        var options = ConfigureFindOptions(configureOptions);
        var query = elements;

        if (!string.IsNullOrWhiteSpace(options.Text))
            query = query.Where(e => e.Text().Contains(options.Text, StringComparison.OrdinalIgnoreCase));

        if (options.TextRegex != null)
            query = query.Where(e => options.TextRegex.IsMatch(e.Text()));

        if (!string.IsNullOrWhiteSpace(options.Name))
            query = query.Where(e => e.AttributeContains("name", options.Name));

        if (options.NameRegex != null)
            query = query.Where(e => e.AttributeContains("name", options.NameRegex));

        if (!string.IsNullOrWhiteSpace(options.Id))
            query = query.Where(e => e.AttributeContains("id", options.Id));

        if (options.IdRegex != null)
            query = query.Where(e => e.AttributeContains("id", options.IdRegex));

        if (options.ClassName != null)
            query = query.Where(e => e.ClassList.Contains(options.ClassName));

        if (options.ClassNameRegex != null)
            query = query.Where(e => e.ClassList.Any(c => options.ClassNameRegex.IsMatch(c)));

        return query;
    }

    private static FindOptions ConfigureFindOptions(Func<FindOptions, FindOptions>? configureOptions = null)
    {
        var configure = configureOptions ?? (opts => opts);
        return configure(new FindOptions());
    }

    private static bool AttributeContains(this IElement element, string attributeName, string value)
    {
        return element.TryGetAttrValue(attributeName, out string attributeValue)
            && attributeValue.Contains(value, StringComparison.OrdinalIgnoreCase);
    }

    private static bool AttributeContains(this IElement element, string attributeName, Regex regex)
    {
        return element.TryGetAttrValue(attributeName, out string attributeValue) && regex.IsMatch(attributeValue);
    }
}
