using Monkey.Ast.Statements;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class IfExpression : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();
            o.Append("if");
            o.Append(Condition.ToString());
            o.Append(" ");
            o.Append(Consequence);

            if (Alternative != null)
            {
                o.Append("else ");
                o.Append(Alternative);
            }

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is IfExpression expression && EqualityComparer<IExpression>.Default.Equals(Condition, expression.Condition) && EqualityComparer<BlockStatement>.Default.Equals(Consequence, expression.Consequence) && EqualityComparer<BlockStatement>.Default.Equals(Alternative, expression.Alternative);
        }

        public override int GetHashCode()
        {
            var hashCode = 1675952736;
            hashCode = hashCode * -1521134295 + EqualityComparer<IExpression>.Default.GetHashCode(Condition);
            hashCode = hashCode * -1521134295 + EqualityComparer<BlockStatement>.Default.GetHashCode(Consequence);
            hashCode = hashCode * -1521134295 + EqualityComparer<BlockStatement>.Default.GetHashCode(Alternative);
            return hashCode;
        }

        public Token.Token Token { get; set; }
        public IExpression Condition { get; set; }
        public BlockStatement Consequence { get; set; }
        public BlockStatement Alternative { get; set; }
    }
}