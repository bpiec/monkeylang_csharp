using Monkey.Token;

namespace Monkey.Lexer
{
    public class Lexer
    {
        private readonly string _input;
        private int _position; // current position in input (points to current char)
        private int _readPosition; // current reading position in input (after current char)
        private char _ch; // current char under examination

        public Lexer(string input)
        {
            _input = input;

            ReadChar();
        }

        public Token.Token NextToken()
        {
            SkipWhitespace();

            Token.Token tok;

            switch (_ch)
            {
                case '=':
                    if (PeekChar() == '=')
                    {
                        var ch = _ch.ToString();
                        ReadChar();
                        tok = new Token.Token(TokenType.EQ, ch + _ch);
                    }
                    else
                    {
                        tok = new Token.Token(TokenType.ASSIGN, _ch);
                    }
                    break;

                case '+':
                    tok = new Token.Token(TokenType.PLUS, _ch);
                    break;

                case '-':
                    tok = new Token.Token(TokenType.MINUS, _ch);
                    break;

                case '!':
                    if (PeekChar() == '=')
                    {
                        var ch = _ch.ToString();
                        ReadChar();
                        tok = new Token.Token(TokenType.NOT_EQ, ch + _ch);
                    }
                    else
                    {
                        tok = new Token.Token(TokenType.BANG, _ch);
                    }
                    break;

                case '/':
                    tok = new Token.Token(TokenType.SLASH, _ch);
                    break;

                case '*':
                    tok = new Token.Token(TokenType.ASTERISK, _ch);
                    break;

                case '<':
                    tok = new Token.Token(TokenType.LT, _ch);
                    break;

                case '>':
                    tok = new Token.Token(TokenType.GT, _ch);
                    break;

                case ';':
                    tok = new Token.Token(TokenType.SEMICOLON, _ch);
                    break;

                case ',':
                    tok = new Token.Token(TokenType.COMMA, _ch);
                    break;

                case '(':
                    tok = new Token.Token(TokenType.LPAREN, _ch);
                    break;

                case ')':
                    tok = new Token.Token(TokenType.RPAREN, _ch);
                    break;

                case '{':
                    tok = new Token.Token(TokenType.LBRACE, _ch);
                    break;

                case '}':
                    tok = new Token.Token(TokenType.RBRACE, _ch);
                    break;

                case '"':
                    tok = new Token.Token(TokenType.STRING, ReadString());
                    break;

                case '[':
                    tok = new Token.Token(TokenType.LBRACKET, _ch);
                    break;

                case ']':
                    tok = new Token.Token(TokenType.RBRACKET, _ch);
                    break;

                case ':':
                    tok = new Token.Token(TokenType.COLON, _ch);
                    break;

                case (char)0:
                    tok = new Token.Token(TokenType.EOF, "");
                    break;

                default:
                    if (IsLetter(_ch))
                    {
                        tok = new Token.Token
                        {
                            Literal = ReadIdentifier()
                        };
                        tok.Type = tok.LookupIdentifier(tok.Literal);
                        return tok;
                    }
                    if (char.IsDigit(_ch))
                    {
                        tok = new Token.Token(TokenType.INT, ReadNumber());
                        return tok;
                    }

                    tok = new Token.Token(TokenType.ILLEGAL, _ch);
                    break;
            }

            ReadChar();

            return tok;
        }

        private void ReadChar()
        {
            if (_readPosition >= _input.Length)
            {
                _ch = (char)0;
            }
            else
            {
                _ch = _input[_readPosition];
            }

            _position = _readPosition;
            _readPosition++;
        }

        private char PeekChar()
        {
            if (_readPosition >= _input.Length)
            {
                return (char)0;
            }

            return _input[_readPosition];
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(_ch))
            {
                ReadChar();
            }
        }

        private string ReadIdentifier()
        {
            var position = _position;
            while (IsLetter(_ch))
            {
                ReadChar();
            }
            return _input.Substring(position, _position - position);
        }

        private string ReadNumber()
        {
            var position = _position;
            while (char.IsDigit(_ch))
            {
                ReadChar();
            }
            return _input.Substring(position, _position - position);
        }

        private string ReadString()
        {
            var position = _position + 1;
            while (true)
            {
                ReadChar();
                if (_ch == '"')
                {
                    break;
                }
            }
            return _input.Substring(position, _position - position);
        }

        private bool IsLetter(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }
    }
}