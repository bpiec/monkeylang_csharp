using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class ArrayLiteral : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var elements = Elements.Select(q => q.ToString()).ToArray();

            o.Append("[");
            o.Append(string.Join(", ", elements));
            o.Append("]");

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ArrayLiteral array))
            {
                return false;
            }

            if (Elements == null && array.Elements == null)
            {
                return true;
            }

            if (Elements.Count != array.Elements.Count)
            {
                return false;
            }

            for (var i = 0; i < Elements.Count; i++)
            {
                if (!Elements[i].Equals(array.Elements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 1573927372 + EqualityComparer<List<IExpression>>.Default.GetHashCode(Elements);
        }

        public Token.Token Token { get; set; }
        public List<IExpression> Elements { get; set; }
    }
}