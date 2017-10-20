namespace Monkey.Ast.Expressions
{
    public class StringLiteral : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString() => Token.Literal;

        public Token.Token Token { get; set; }
        public string Value { get; set; }
    }
}