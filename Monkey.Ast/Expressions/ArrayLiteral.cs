using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class ArrayLiteral : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var elements = Elements.Select(q => q.ToString()).ToArray();

            o.Append("[");
            o.Append(string.Join(", ", elements));
            o.Append("]");

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public List<IExpression> Elements { get; set; }
    }
}