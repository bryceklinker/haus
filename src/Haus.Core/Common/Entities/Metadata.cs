using System;
using Haus.Core.Models.Common;

namespace Haus.Core.Common.Entities
{
    public class Metadata
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Metadata(string key = null, string value = null)
        {
            Key = key;
            Value = value;
        }

        public virtual MetadataModel ToModel()
        {
            return new(Key, Value);
        }

        public static Metadata FromModel(MetadataModel model)
        {
            return new(model.Key, model.Value);
        }
    }
}