using System;

namespace Haus.Utilities.Packaging;

public interface IDebComposeVersionPinner
{
    string Pin(string template, string version);
}

public class DebComposeVersionPinner : IDebComposeVersionPinner
{
    private static readonly string[] HausImageNames = ["haus-web", "haus-zigbee", "haus-site"];

    public string Pin(string template, string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new InvalidOperationException("VERSION must be set to render the packaged docker-compose.yml");

        var pinned = template;
        foreach (var imageName in HausImageNames)
            pinned = pinned.Replace($"{imageName}-latest", $"{imageName}-{version}");

        return pinned;
    }
}
