using System.Text;

namespace Monkey.Ast.Expressions
{
    public class InfixExpression : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            o.Append("(");
            o.Append(Left.ToString());
            o.Append(" " + Operator + " ");
            o.Append(Right.ToString());
            o.Append(")");

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public IExpression Left { get; set; }
        public string Operator { get; set; }
        public IExpression Right { get; set; }
    }
}