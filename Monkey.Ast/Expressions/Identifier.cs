namespace Monkey.Ast.Expressions
{
    public class Identifier : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString() => Value;

        public Token.Token Token { get; set; }
        public string Value { get; set; }
    }
}