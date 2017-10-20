using System.Linq;

namespace Monkey.Object
{
    public struct String : IObject, IHashable
    {
        public ObjectType Type => ObjectType.STRING;
        public string Value { get; set; }

        public string Inspect()
        {
            return Value;
        }

        public HashKey HashKey()
        {
            return new HashKey
            {
                Type = Type,
                Value = FNVConstants.CreateHash(Value)
            };
        }

        public override bool Equals(object obj)
        {
            return obj is String str && str.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public object Clone()
        {
            return new String
            {
                Value = Value
            };
        }

        private class FNVConstants
        {
            private const int OffsetBasis = unchecked((int)2166136261);
            private const int Prime = 16777619;

            public static int CreateHash(params object[] objs)
            {
                return objs.Aggregate(OffsetBasis, (r, o) => (r ^ o.GetHashCode()) * Prime);
            }
        }
    }
}