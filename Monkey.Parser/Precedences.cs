namespace Monkey.Parser
{
    public enum Precedences
    {
        _ = 0,
        LOWEST,
        EQUALS,      // ==
        LESSGREATER, // > or <
        SUM,         // +
        PRODUCT,     // *
        PREFIX,      // -X or !X
        CALL,        // myFunction(X)
        INDEX        // array[index]
    }
}