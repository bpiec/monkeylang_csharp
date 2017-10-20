using System.Text;

namespace Monkey.Ast.Expressions
{
    public class InfixExpression : IExpression
    {
        public string TokenLiteral => Token?.Literal;

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

        public override bool Equals(object obj)
        {
            return obj is InfixExpression infix && infix.Left.Equals(Left) && infix.Right.Equals(Right) && infix.Operator.Equals(Operator);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Left != null ? Left.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Operator != null ? Operator.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Right != null ? Right.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}