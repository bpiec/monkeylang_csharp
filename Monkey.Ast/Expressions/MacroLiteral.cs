using Monkey.Ast.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class MacroLiteral : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var parameters = Parameters.Select(q => q.ToString()).ToArray();

            o.Append(TokenLiteral);
            o.Append("(");
            o.Append(string.Join(", ", parameters));
            o.Append(") ");
            o.Append(Body);

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MacroLiteral macro))
            {
                return false;
            }

            if (Parameters == null && macro.Parameters == null)
            {
                return true;
            }

            if (Parameters.Count != macro.Parameters.Count)
            {
                return false;
            }

            for (var i = 0; i < Parameters.Count; i++)
            {
                if (!Parameters[i].Equals(macro.Parameters[i]))
                {
                    return false;
                }
            }

            return Body.Equals(macro.Body);
        }

        public override int GetHashCode()
        {
            var hashCode = -691572618;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Identifier>>.Default.GetHashCode(Parameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<BlockStatement>.Default.GetHashCode(Body);
            return hashCode;
        }

        public Token.Token Token { get; set; } // The 'macro' token
        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }
    }
}