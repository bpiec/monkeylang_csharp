namespace Monkey.Ast.Expressions
{
    public class IntegerLiteral : IExpression
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString() => Token.Literal;

        public Token.Token Token { get; set; }
        public long Value { get; set; }
    }
}