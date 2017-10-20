using Monkey.Ast.Expressions;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Statements
{
    public class LetStatement : IStatement
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();
            o.Append(TokenLiteral + " ");
            o.Append(Name);
            o.Append(" = ");
            if (Value != null)
            {
                o.Append(Value.ToString());
            }
            o.Append(";");
            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is LetStatement statement && EqualityComparer<Identifier>.Default.Equals(Name, statement.Name) && EqualityComparer<IExpression>.Default.Equals(Value, statement.Value);
        }

        public override int GetHashCode()
        {
            var hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<Identifier>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IExpression>.Default.GetHashCode(Value);
            return hashCode;
        }

        public Token.Token Token { get; set; }
        public Identifier Name { get; set; }
        public IExpression Value { get; set; }
    }
}