using System;

namespace Haus.Core.Models.Common
{
    public class MetadataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public MetadataModel(string key = null, string value = null)
        {
            Key = key;
            Value = value;
        }

        protected bool Equals(MetadataModel other)
        {
            return Key == other.Key && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MetadataModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }
    }
}