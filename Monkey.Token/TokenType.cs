namespace Monkey.Token
{
    public enum TokenType
    {
        ILLEGAL,
        EOF,

        // Identifiers + literals
        IDENT, // add, foobar, x, y, ...

        INT, // 1343456
        STRING, // "foobar"

        // Operators
        ASSIGN,

        PLUS,
        MINUS,
        BANG,
        ASTERISK,
        SLASH,

        LT,
        GT,

        EQ,
        NOT_EQ,

        // Delimiters
        COMMA,

        SEMICOLON,
        COLON,

        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        LBRACKET,
        RBRACKET,

        // Keywords
        FUNCTION,

        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
        MACRO
    }
}