using Monkey.Ast.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class FunctionLiteral : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var parameters = Parameters.Select(q => q.ToString()).ToArray();

            o.Append(TokenLiteral);
            o.Append("(");
            o.Append(string.Join(", ", parameters));
            o.Append(") ");
            o.Append(Body.ToString());

            return o.ToString();
        }

        public Token.Token Token { get; set; } // The 'fn' token
        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }
    }
}