using System.Collections.Generic;

namespace Monkey.Token
{
    public class Token
    {
        private readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "fn", TokenType.FUNCTION },
            { "let", TokenType.LET },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "return", TokenType.RETURN },
            { "macro", TokenType.MACRO }
        };

        public TokenType Type { get; set; }
        public string Literal { get; set; }

        public Token()
        {
        }

        public Token(TokenType type, string literal) : this()
        {
            Type = type;
            Literal = literal;
        }

        public Token(TokenType type, char literal) : this(type, literal.ToString())
        {
        }

        public TokenType LookupIdentifier(string ident)
        {
            if (_keywords.ContainsKey(ident))
            {
                return _keywords[ident];
            }

            return TokenType.IDENT;
        }

        public override string ToString()
        {
            return $"{Type} {Literal}";
        }
    }
}