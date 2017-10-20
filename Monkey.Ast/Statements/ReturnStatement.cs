using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Statements
{
    public class ReturnStatement : IStatement
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();
            o.Append(TokenLiteral + " ");
            if (ReturnValue != null)
            {
                o.Append(ReturnValue.ToString());
            }
            o.Append(";");
            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is ReturnStatement statement && EqualityComparer<IExpression>.Default.Equals(ReturnValue, statement.ReturnValue);
        }

        public override int GetHashCode()
        {
            return -1579408684 + EqualityComparer<IExpression>.Default.GetHashCode(ReturnValue);
        }

        public Token.Token Token { get; set; } // the 'return' token
        public IExpression ReturnValue { get; set; }
    }
}