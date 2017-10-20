namespace Monkey.Object
{
    public struct ReturnValue : IObject
    {
        public ObjectType Type => ObjectType.RETURN_VALUE;

        public string Inspect()
        {
            return Value.Inspect();
        }

        public IObject Value { get; set; }

        public object Clone()
        {
            return new ReturnValue
            {
                Value = Value.Clone() as IObject
            };
        }
    }
}