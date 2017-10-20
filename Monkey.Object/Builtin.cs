namespace Monkey.Object
{
    public struct Builtin : IObject
    {
        public delegate IObject BuiltinFunction(params IObject[] args);

        public ObjectType Type => ObjectType.BUILTIN;

        public BuiltinFunction Fn { get; set; }

        public string Inspect()
        {
            return "builtin function";
        }

        public object Clone()
        {
            return new Builtin
            {
                Fn = Fn
            };
        }
    }
}