using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class HashLiteral : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var pairs = Pairs.Select(q => q.Key.ToString() + ":" + q.Value.ToString()).ToArray();

            o.Append("{");
            o.Append(string.Join(", ", pairs));
            o.Append("}");

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public Dictionary<IExpression, IExpression> Pairs { get; set; }
    }
}