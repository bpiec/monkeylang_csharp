using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class PrefixExpression : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            o.Append("(");
            o.Append(Operator);
            o.Append(Right.ToString());
            o.Append(")");

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is PrefixExpression expression && Operator == expression.Operator && EqualityComparer<IExpression>.Default.Equals(Right, expression.Right);
        }

        public override int GetHashCode()
        {
            var hashCode = -1522770646;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Operator);
            hashCode = hashCode * -1521134295 + EqualityComparer<IExpression>.Default.GetHashCode(Right);
            return hashCode;
        }

        public Token.Token Token { get; set; } // The prefix token, e.g. !
        public string Operator { get; set; }
        public IExpression Right { get; set; }
    }
}