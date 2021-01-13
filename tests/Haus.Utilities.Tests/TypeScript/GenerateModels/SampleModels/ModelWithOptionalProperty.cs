using Haus.Core.Models;

namespace Haus.Utilities.Tests.TypeScript.GenerateModels.SampleModels
{
    public class ModelWithOptionalProperty
    {
        [OptionalGeneration]
        public long Id { get; set; }
        
        [OptionalGeneration]
        public double? Value { get; set; }
    }
}