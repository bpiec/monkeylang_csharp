namespace Monkey.Ast.Expressions
{
    public class Boolean : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString() => Token.Literal;

        public Token.Token Token { get; set; }

        public bool Value { get; set; }
    }
}