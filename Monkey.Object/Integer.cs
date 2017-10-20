namespace Monkey.Object
{
    public struct Integer : IObject, IHashable
    {
        public ObjectType Type => ObjectType.INTEGER;
        public long Value { get; set; }

        public string Inspect()
        {
            return Value.ToString("d");
        }

        public HashKey HashKey()
        {
            return new HashKey
            {
                Type = Type,
                Value = Value
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Integer integer && integer.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public object Clone()
        {
            return new Integer
            {
                Value = Value
            };
        }
    }
}