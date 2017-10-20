using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Object
{
    public struct Function : IObject
    {
        public ObjectType Type => ObjectType.FUNCTION;

        public string Inspect()
        {
            var o = new StringBuilder();

            var parameters = Parameters.Select(q => q.ToString()).ToArray();

            o.Append("fn");
            o.Append("(");
            o.Append(string.Join(", ", parameters));
            o.AppendLine(") {");
            o.AppendLine(Body.ToString());
            o.Append("}");

            return o.ToString();
        }

        public List<Identifier> Parameters { get; set; }
        public BlockStatement Body { get; set; }
        public Environment Env { get; set; }

        public object Clone()
        {
            return new Function
            {
                Parameters = Parameters,
                Body = Body,
                Env = Env
            };
        }
    }
}