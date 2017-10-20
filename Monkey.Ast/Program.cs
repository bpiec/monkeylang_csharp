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

        public override bool Equals(object obj)
        {
            if (!(obj is Program program))
            {
                return false;
            }

            if (Statements == null && program.Statements == null)
            {
                return true;
            }

            if (Statements.Count != program.Statements.Count)
            {
                return false;
            }

            for (var i = 0; i < Statements.Count; i++)
            {
                if (!Statements[i].Equals(program.Statements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return (Statements != null ? Statements.GetHashCode() : 0);
        }
    }
}