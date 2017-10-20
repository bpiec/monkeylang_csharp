using System;

namespace Monkey.Object
{
    public struct HashKey : ICloneable
    {
        public ObjectType Type { get; set; }
        public long Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HashKey hashKey && hashKey.Value == Value && hashKey.Type == Type;
        }

        public override int GetHashCode()
        {
            return ((int)Type * 397) ^ Value.GetHashCode();
        }

        public object Clone()
        {
            return new HashKey
            {
                Value = Value,
                Type = Type
            };
        }
    }
}