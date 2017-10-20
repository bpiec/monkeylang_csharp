using Monkey.Ast.Expressions;
using System.Text;

namespace Monkey.Ast.Statements
{
    public class LetStatement : IStatement
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();
            o.Append(TokenLiteral + " ");
            o.Append(Name.ToString());
            o.Append(" = ");
            if (Value != null)
            {
                o.Append(Value.ToString());
            }
            o.Append(";");
            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public Identifier Name { get; set; }
        public IExpression Value { get; set; }
    }
}