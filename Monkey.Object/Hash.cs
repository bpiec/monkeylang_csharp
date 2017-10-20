using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Object
{
    public struct Hash : IObject
    {
        public ObjectType Type => ObjectType.HASH;
        public Dictionary<HashKey, HashPair> Pairs { get; set; }

        public string Inspect()
        {
            var o = new StringBuilder();

            var pairs = Pairs.Select(q => $"{q.Value.Key.Inspect()}: {q.Value.Value.Inspect()}");

            o.Append("{");
            o.Append(string.Join(", ", pairs));
            o.Append("}");

            return o.ToString();
        }

        public object Clone()
        {
            return new Hash
            {
                Pairs = Pairs.ToDictionary(q => (HashKey)q.Key.Clone(), q => (HashPair)q.Value.Clone())
            };
        }
    }
}