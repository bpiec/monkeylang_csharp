using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    public class Program : INode
    {
        public List<IStatement> Statements { get; }

        public Program()
        {
            Statements = new List<IStatement>();
        }

        public string TokenLiteral
        {
            get
            {
                if (Statements.Count > 0)
                {
                    return Statements[0].TokenLiteral;
                }

                return string.Empty;
            }
        }

        public override string ToString()
        {
            var o = new StringBuilder();
            foreach (var s in Statements)
            {
                o.Append(s.ToString());
            }
            return o.ToString();
        }
    }
}