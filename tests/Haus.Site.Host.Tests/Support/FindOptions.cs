using System.Text.RegularExpressions;

namespace Haus.Site.Host.Tests.Support;

public record FindOptions(
    string? Text = null,
    Regex? TextRegex = null,
    string? Name = null,
    Regex? NameRegex = null,
    string? Id = null,
    Regex? IdRegex = null
)
{
    public FindOptions WithText(string text) => this with { Text = text };

    public FindOptions WithTextRegex(Regex textRegex) => this with { TextRegex = textRegex };

    public FindOptions WithName(string name) => this with { Name = name };

    public FindOptions WithNameRegex(Regex regex) => this with { NameRegex = regex };

    public FindOptions WithId(string id) => this with { Id = id };
    
    public FindOptions WithIdRegex(Regex regex) => this with { IdRegex = regex };
}