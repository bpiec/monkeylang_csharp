using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class CallExpression : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var args = Arguments.Select(q => q.ToString()).ToArray();

            o.Append(Function.ToString());
            o.Append("(");
            o.Append(string.Join(", ", args));
            o.Append(")");

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }
    }
}