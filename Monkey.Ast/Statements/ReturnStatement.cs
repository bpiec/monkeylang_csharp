using System.Text;

namespace Monkey.Ast.Statements
{
    public class ReturnStatement : IStatement
    {
        public string TokenLiteral => Token.Literal;

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

        public Token.Token Token { get; set; } // the 'return' token
        public IExpression ReturnValue { get; set; }
    }
}