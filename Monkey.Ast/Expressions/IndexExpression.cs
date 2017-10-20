using System.Text;

namespace Monkey.Ast.Expressions
{
    public class IndexExpression : IExpression
    {
        public string TokenLiteral => Token.Literal;

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

        public Token.Token Token { get; set; }
        public IExpression Left { get; set; }
        public IExpression Index { get; set; }
    }
}