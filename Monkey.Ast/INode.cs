namespace Monkey.Ast
{
    public interface INode
    {
        string TokenLiteral { get; }

        string ToString();
    }
}