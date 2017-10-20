namespace Monkey.Object
{
    public struct Boolean : IObject, IHashable
    {
        public ObjectType Type => ObjectType.BOOLEAN;
        public bool Value { get; set; }

        public string Inspect()
        {
            return Value.ToString().ToLower();
        }

        public HashKey HashKey()
        {
            var value = Value ? 1 : 0;
            return new HashKey
            {
                Type = Type,
                Value = value
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Boolean boolean && boolean.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public object Clone()
        {
            return new Boolean
            {
                Value = Value
            };
        }
    }
}