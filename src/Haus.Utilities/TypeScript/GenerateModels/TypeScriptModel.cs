using System;

namespace Haus.Utilities.TypeScript.GenerateModels
{
    public record TypeScriptModel(Type ModelType, string FileName, string Contents)
    {
        public string ModelName => ModelType.ToTypescriptTypeName();
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}