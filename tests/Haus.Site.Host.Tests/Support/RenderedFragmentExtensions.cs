using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Diffing.Extensions;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;

namespace Haus.Site.Host.Tests.Support;

public static class RenderedFragmentExtensions
{
    public static IElement[] FindAllByTag(
        this IRenderedFragment fragment,
        string tag, 
        Func<FindOptions, FindOptions>? configureOptions = null)
    {
        return fragment.FindAll(tag)
            .FindByOptions(configureOptions)
            .ToArray();
    }

    public static IElement FindByTag(
        this IRenderedFragment fragment, 
        string tag, 
        Func<FindOptions, FindOptions>? configureOptions = null)
    {
        return fragment.FindAll(tag)
            .FindByOptions(configureOptions)
            .First();
    }

    public static IElement[] FindAllByClass(
        this IRenderedFragment fragment,
        string className,
        Func<FindOptions, FindOptions>? configureOptions = null)
    {
        return fragment.FindAll($".{className}")
            .FindByOptions(configureOptions)
            .ToArray();
    }
    
    
    public static IElement FindByRole(
        this IRenderedFragment fragment,
        string role,
        Func<FindOptions, FindOptions>? configureOptions = null)
    {
        return fragment
            .FindAll(CreateRoleSelector(role))
            .FindByOptions(configureOptions)
            .First();
    }

    public static IRenderedComponent<T> FindByComponent<T>(this IRenderedFragment fragment,
        Func<FindOptions, FindOptions>? configureOptions = null) where T : IComponent
    {
        return fragment.FindAllByComponent<T>(configureOptions).First();
    }

    public static IEnumerable<IRenderedComponent<T>> FindAllByComponent<T>(
        this IRenderedFragment fragment,
        Func<FindOptions, FindOptions>? configureOptions = null) where T : IComponent
    {
        return fragment.FindComponents<T>()
            .FindByOptions(configureOptions);
    }
    
    private static string CreateRoleSelector(string role)
    {
        return role switch
        {
            "button" => "button",
            _ => $"[role={role}]"
        };
    }

    private static IEnumerable<IRenderedComponent<T>> FindByOptions<T>(
        this IEnumerable<IRenderedComponent<T>> components,
        Func<FindOptions, FindOptions>? configureOptions = null) where T : IComponent
    {
        return components
            .Where(c => c.FindAll("*").FindByOptions(configureOptions).Any());
    }
    
    private static IEnumerable<IElement> FindByOptions(
        this IEnumerable<IElement> elements,
        Func<FindOptions, FindOptions>? configureOptions)
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
        return element.TryGetAttrValue(attributeName, out string attributeValue)
            && regex.IsMatch(attributeValue);
    }
}