namespace Monkey.Ast.Expressions
{
    public class IntegerLiteral : IExpression
    {
        public string TokenLiteral => Token?.Literal;

        public override string ToString() => Token.Literal;

        public Token.Token Token { get; set; }
        public long Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is IntegerLiteral literal && literal.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}