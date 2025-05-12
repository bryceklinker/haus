namespace Haus.Core.Models.Common;

public record MetadataModel(string Key, string Value)
{
    public static MetadataModel Simulated()
    {
        return new MetadataModel("simulated", "true");
    }
};
