namespace Monkey.Object
{
    public struct Null : IObject
    {
        public ObjectType Type => ObjectType.NULL;

        public string Inspect()
        {
            return "null";
        }

        public override bool Equals(object obj)
        {
            return obj is Null;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public object Clone()
        {
            return new Null();
        }
    }
}