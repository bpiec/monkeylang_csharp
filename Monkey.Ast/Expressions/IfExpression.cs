using Monkey.Ast.Statements;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class IfExpression : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();
            o.Append("if");
            o.Append(Condition.ToString());
            o.Append(" ");
            o.Append(Consequence.ToString());

            if (Alternative != null)
            {
                o.Append("else ");
                o.Append(Alternative);
            }

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement Alternative { get; set; }
    }
}