using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast.Statements
{
    public class BlockStatement : IStatement
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            foreach (var statement in Statements)
            {
                o.Append(statement.ToString());
            }

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BlockStatement block))
            {
                return false;
            }

            if (Statements == null && block.Statements == null)
            {
                return true;
            }

            if (Statements.Count != block.Statements.Count)
            {
                return false;
            }

            for (var i = 0; i < Statements.Count; i++)
            {
                if (!Statements[i].Equals(block.Statements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 271944183 + EqualityComparer<List<IStatement>>.Default.GetHashCode(Statements);
        }

        public Token.Token Token { get; set; }
        public List<IStatement> Statements { get; set; }
    }
}