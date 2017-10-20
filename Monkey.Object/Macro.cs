using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkey.Object
{
    public struct Macro : IObject
    {
        public ObjectType Type => ObjectType.MACRO;

        public string Inspect()
        {
            var o = new StringBuilder();

            var parameters = Parameters.Select(q => q.ToString()).ToArray();

            o.Append("macro");
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
            return new Macro
            {
                Parameters = Parameters,
                Body = Body,
                Env = Env
            };
        }
    }
}