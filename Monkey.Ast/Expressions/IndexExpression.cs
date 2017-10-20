using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class IndexExpression : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            o.Append("(");
            o.Append(Left.ToString());
            o.Append("[");
            o.Append(Index.ToString());
            o.Append("])");

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is IndexExpression expression && EqualityComparer<IExpression>.Default.Equals(Left, expression.Left) && EqualityComparer<IExpression>.Default.Equals(Index, expression.Index);
        }

        public override int GetHashCode()
        {
            var hashCode = 1366789467;
            hashCode = hashCode * -1521134295 + EqualityComparer<IExpression>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IExpression>.Default.GetHashCode(Index);
            return hashCode;
        }

        public Token.Token Token { get; set; }
        public IExpression Left { get; set; }
        public IExpression Index { get; set; }
    }
}