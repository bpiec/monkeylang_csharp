namespace Monkey.Ast.Statements
{
    public class ExpressionStatement : IStatement
    {
        public string TokenLiteral => Token.Literal;

        public override string ToString() => Expression != null ? Expression.ToString() : string.Empty;

        public Token.Token Token { get; set; } // the first token of the expression

        public IExpression Expression { get; set; }
    }
}