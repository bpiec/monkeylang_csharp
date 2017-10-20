using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Ast.Expressions
{
    public class HashLiteral : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString()
        {
            var o = new StringBuilder();

            var pairs = Pairs.Select(q => q.Key.ToString() + ":" + q.Value.ToString()).ToArray();

            o.Append("{");
            o.Append(string.Join(", ", pairs));
            o.Append("}");

            return o.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HashLiteral program))
            {
                return false;
            }

            if (Pairs == null && program.Pairs == null)
            {
                return true;
            }

            if (Pairs.Count != program.Pairs.Count)
            {
                return false;
            }

            foreach (var key in Pairs.Keys)
            {
                if (!program.Pairs.ContainsKey(key))
                {
                    return false;
                }
                if (!Pairs[key].Equals(program.Pairs[key]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 1182183670 + EqualityComparer<Dictionary<IExpression, IExpression>>.Default.GetHashCode(Pairs);
        }

        public Token.Token Token { get; set; }
        public Dictionary<IExpression, IExpression> Pairs { get; set; }
    }
}