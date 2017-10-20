using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Statements
{
    public class BlockStatement : IStatement
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            foreach (var statement in Statements)
            {
                o.Append(statement.ToString());
            }

            return o.ToString();
        }

        public Token.Token Token { get; set; }
        public List<IStatement> Statements { get; set; }
    }
}