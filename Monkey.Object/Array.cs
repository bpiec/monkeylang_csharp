using System.Linq;
using System.Text;

namespace Monkey.Object
{
    public struct Array : IObject
    {
        public ObjectType Type => ObjectType.ARRAY;

        public IObject[] Elements { get; set; }

        public string Inspect()
        {
            var o = new StringBuilder();

            var elements = Elements.Select(q => q.Inspect()).ToArray();

            o.Append("[");
            o.Append(string.Join(", ", elements));
            o.Append("]");

            return o.ToString();
        }

        public object Clone()
        {
            return new Array
            {
                Elements = Elements.Select(q => q.Clone()).Cast<IObject>().ToArray()
            };
        }
    }
}