using Monkey.Ast;

namespace Monkey.Object
{
    public struct Quote : IObject
    {
        public ObjectType Type => ObjectType.QUOTE;
        public INode Node { get; set; }

        public string Inspect()
        {
            return $"QUOTE({Node.ToString()})";
        }

        public object Clone()
        {
            return new Quote
            {
                Node = Node
            };
        }
    }
}