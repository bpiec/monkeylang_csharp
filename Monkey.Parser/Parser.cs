using Monkey.Ast;
using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using Monkey.Token;
using System.Collections.Generic;

namespace Monkey.Parser
{
    public class Parser
    {
        private delegate IExpression PrefixParseFn();

        private delegate IExpression InfixParseFn(IExpression expression);

        private readonly Lexer.Lexer _lexer;

        private Token.Token _currentToken;
        private Token.Token _peekToken;

        private readonly Dictionary<TokenType, PrefixParseFn> _prefixParseFns;
        private readonly Dictionary<TokenType, InfixParseFn> _infixParseFns;
        private readonly Dictionary<TokenType, Precedences> _precedences;

        public List<string> Errors { get; }

        public Parser(Lexer.Lexer lexer)
        {
            _lexer = lexer;
            Errors = new List<string>();

            _prefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
            RegisterPrefix(TokenType.IDENT, ParseIdentifier);
            RegisterPrefix(TokenType.INT, ParseIntegerLiteral);
            RegisterPrefix(TokenType.BANG, ParsePrefixExpression);
            RegisterPrefix(TokenType.MINUS, ParsePrefixExpression);
            RegisterPrefix(TokenType.TRUE, ParseBoolean);
            RegisterPrefix(TokenType.FALSE, ParseBoolean);
            RegisterPrefix(TokenType.LPAREN, ParseGroupedExpression);
            RegisterPrefix(TokenType.IF, ParseIfExpression);
            RegisterPrefix(TokenType.FUNCTION, ParseFunctionLiteral);
            RegisterPrefix(TokenType.STRING, ParseStringLiteral);
            RegisterPrefix(TokenType.LBRACKET, ParseArrayLiteral);
            RegisterPrefix(TokenType.LBRACE, ParseHashLiteral);
            RegisterPrefix(TokenType.MACRO, ParseMacroLiteral);

            _infixParseFns = new Dictionary<TokenType, InfixParseFn>();
            RegisterInfix(TokenType.PLUS, ParseInfixExpression);
            RegisterInfix(TokenType.MINUS, ParseInfixExpression);
            RegisterInfix(TokenType.SLASH, ParseInfixExpression);
            RegisterInfix(TokenType.ASTERISK, ParseInfixExpression);
            RegisterInfix(TokenType.EQ, ParseInfixExpression);
            RegisterInfix(TokenType.NOT_EQ, ParseInfixExpression);
            RegisterInfix(TokenType.LT, ParseInfixExpression);
            RegisterInfix(TokenType.GT, ParseInfixExpression);
            RegisterInfix(TokenType.LPAREN, ParseCallExpression);
            RegisterInfix(TokenType.LBRACKET, ParseIndexExpression);

            _precedences = new Dictionary<TokenType, Precedences>
            {
                { TokenType.EQ, Precedences.EQUALS },
                { TokenType.NOT_EQ, Precedences.EQUALS },
                { TokenType.LT, Precedences.LESSGREATER },
                { TokenType.GT, Precedences.LESSGREATER },
                { TokenType.PLUS, Precedences.SUM },
                { TokenType.MINUS, Precedences.SUM },
                { TokenType.SLASH, Precedences.PRODUCT },
                { TokenType.ASTERISK, Precedences.PRODUCT },
                { TokenType.LPAREN, Precedences.CALL },
                { TokenType.LBRACKET, Precedences.INDEX }
            };

            // Read two tokens, so _currentToken and _peekToken are both set
            NextToken();
            NextToken();
        }

        public Program ParseProgram()
        {
            var program = new Program();

            while (!CurrentTokenIs(TokenType.EOF))
            {
                var stmt = ParseStatement();
                if (stmt != null)
                {
                    program.Statements.Add(stmt);
                }
                NextToken();
            }

            return program;
        }

        private IStatement ParseStatement()
        {
            switch (_currentToken.Type)
            {
                case TokenType.LET:
                    return ParseLetStatement();

                case TokenType.RETURN:
                    return ParseReturnStatement();

                default:
                    return ParseExpressionStatement();
            }
        }

        private LetStatement ParseLetStatement()
        {
            var stmt = new LetStatement
            {
                Token = _currentToken
            };

            if (!ExpectPeek(TokenType.IDENT))
            {
                return null;
            }

            stmt.Name = new Identifier
            {
                Token = _currentToken,
                Value = _currentToken.Literal
            };

            if (!ExpectPeek(TokenType.ASSIGN))
            {
                return null;
            }

            NextToken();

            stmt.Value = ParseExpression(Precedences.LOWEST);

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }

            return stmt;
        }

        private ReturnStatement ParseReturnStatement()
        {
            var stmt = new ReturnStatement
            {
                Token = _currentToken
            };

            NextToken();

            stmt.ReturnValue = ParseExpression(Precedences.LOWEST);

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }

            return stmt;
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var stmt = new ExpressionStatement
            {
                Token = _currentToken,
                Expression = ParseExpression(Precedences.LOWEST)
            };

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }

            return stmt;
        }

        private IExpression ParseExpression(Precedences precendece)
        {
            if (!_prefixParseFns.ContainsKey(_currentToken.Type))
            {
                NoPrefixParseFnError(_currentToken.Type);
                return null;
            }

            var prefix = _prefixParseFns[_currentToken.Type];
            var leftExp = prefix();

            while (!PeekTokenIs(TokenType.SEMICOLON) && precendece < PeekPrecedence())
            {
                if (!_infixParseFns.ContainsKey(_peekToken.Type))
                {
                    return leftExp;
                }

                var infix = _infixParseFns[_peekToken.Type];
                NextToken();
                leftExp = infix(leftExp);
            }

            return leftExp;
        }

        private IExpression ParseIdentifier()
        {
            return new Identifier
            {
                Token = _currentToken,
                Value = _currentToken.Literal
            };
        }

        private IExpression ParseIntegerLiteral()
        {
            var lit = new IntegerLiteral
            {
                Token = _currentToken
            };

            if (long.TryParse(_currentToken.Literal, out var val))
            {
                lit.Value = val;
                return lit;
            }

            var msg = $"could not parse {_currentToken.Literal} as integer";
            Errors.Add(msg);
            return null;
        }

        private IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression
            {
                Token = _currentToken,
                Operator = _currentToken.Literal
            };

            NextToken();

            expression.Right = ParseExpression(Precedences.PREFIX);

            return expression;
        }

        private IExpression ParseBoolean()
        {
            return new Boolean
            {
                Token = _currentToken,
                Value = CurrentTokenIs(TokenType.TRUE)
            };
        }

        private IExpression ParseGroupedExpression()
        {
            NextToken();

            var exp = ParseExpression(Precedences.LOWEST);

            if (!ExpectPeek(TokenType.RPAREN))
            {
                return null;
            }

            return exp;
        }

        private IExpression ParseIfExpression()
        {
            var expression = new IfExpression
            {
                Token = _currentToken
            };

            if (!ExpectPeek(TokenType.LPAREN))
            {
                return null;
            }

            NextToken();
            expression.Condition = ParseExpression(Precedences.LOWEST);

            if (!ExpectPeek(TokenType.RPAREN))
            {
                return null;
            }

            if (!ExpectPeek(TokenType.LBRACE))
            {
                return null;
            }

            expression.Consequence = ParseBlockStatement();

            if (PeekTokenIs(TokenType.ELSE))
            {
                NextToken();

                if (!ExpectPeek(TokenType.LBRACE))
                {
                    return null;
                }

                expression.Alternative = ParseBlockStatement();
            }

            return expression;
        }

        private IExpression ParseFunctionLiteral()
        {
            var lit = new FunctionLiteral
            {
                Token = _currentToken
            };

            if (!ExpectPeek(TokenType.LPAREN))
            {
                return null;
            }

            lit.Parameters = ParseFunctionParameters();

            if (!ExpectPeek(TokenType.LBRACE))
            {
                return null;
            }

            lit.Body = ParseBlockStatement();

            return lit;
        }

        private List<Identifier> ParseFunctionParameters()
        {
            var identifiers = new List<Identifier>();

            if (PeekTokenIs(TokenType.RPAREN))
            {
                NextToken();
                return identifiers;
            }

            NextToken();

            var ident = new Identifier
            {
                Token = _currentToken,
                Value = _currentToken.Literal
            };
            identifiers.Add(ident);

            while (PeekTokenIs(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                ident = new Identifier
                {
                    Token = _currentToken,
                    Value = _currentToken.Literal
                };
                identifiers.Add(ident);
            }

            if (!ExpectPeek(TokenType.RPAREN))
            {
                return null;
            }

            return identifiers;
        }

        private IExpression ParseStringLiteral()
        {
            return new StringLiteral
            {
                Token = _currentToken,
                Value = _currentToken.Literal
            };
        }

        private IExpression ParseArrayLiteral()
        {
            var array = new ArrayLiteral
            {
                Token = _currentToken
            };

            array.Elements = ParseExpressionList(TokenType.RBRACKET);

            return array;
        }

        private List<IExpression> ParseExpressionList(TokenType end)
        {
            var list = new List<IExpression>();

            if (PeekTokenIs(end))
            {
                NextToken();
                return list;
            }

            NextToken();
            list.Add(ParseExpression(Precedences.LOWEST));

            while (PeekTokenIs(TokenType.COMMA))
            {
                NextToken();
                NextToken();
                list.Add(ParseExpression(Precedences.LOWEST));
            }

            if (!ExpectPeek(end))
            {
                return null;
            }

            return list;
        }

        private IExpression ParseHashLiteral()
        {
            var hash = new HashLiteral
            {
                Token = _currentToken,
                Pairs = new Dictionary<IExpression, IExpression>()
            };

            while (!PeekTokenIs(TokenType.RBRACE))
            {
                NextToken();
                var key = ParseExpression(Precedences.LOWEST);

                if (!ExpectPeek(TokenType.COLON))
                {
                    return null;
                }

                NextToken();
                var value = ParseExpression(Precedences.LOWEST);

                hash.Pairs.Add(key, value);

                if (!PeekTokenIs(TokenType.RBRACE) && !ExpectPeek(TokenType.COMMA))
                {
                    return null;
                }
            }

            if (!ExpectPeek(TokenType.RBRACE))
            {
                return null;
            }

            return hash;
        }

        private IExpression ParseMacroLiteral()
        {
            var lit = new MacroLiteral
            {
                Token = _currentToken
            };

            if (!ExpectPeek(TokenType.LPAREN))
            {
                return null;
            }

            lit.Parameters = ParseFunctionParameters();

            if (!ExpectPeek(TokenType.LBRACE))
            {
                return null;
            }

            lit.Body = ParseBlockStatement();

            return lit;
        }

        private BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement
            {
                Token = _currentToken,
                Statements = new List<IStatement>()
            };

            NextToken();

            while (!CurrentTokenIs(TokenType.RBRACE))
            {
                var stmt = ParseStatement();
                if (stmt != null)
                {
                    block.Statements.Add(stmt);
                }
                NextToken();
            }

            return block;
        }

        private IExpression ParseInfixExpression(IExpression left)
        {
            var expression = new InfixExpression
            {
                Token = _currentToken,
                Operator = _currentToken.Literal,
                Left = left
            };

            var precedense = CurPrecedence();
            NextToken();
            expression.Right = ParseExpression(precedense);

            return expression;
        }

        private IExpression ParseCallExpression(IExpression function)
        {
            var exp = new CallExpression
            {
                Token = _currentToken,
                Function = function
            };
            exp.Arguments = ParseExpressionList(TokenType.RPAREN);
            return exp;
        }

        private IExpression ParseIndexExpression(IExpression left)
        {
            var exp = new IndexExpression
            {
                Token = _currentToken,
                Left = left
            };

            NextToken();
            exp.Index = ParseExpression(Precedences.LOWEST);

            if (!ExpectPeek(TokenType.RBRACKET))
            {
                return null;
            }

            return exp;
        }

        private void NoPrefixParseFnError(TokenType t)
        {
            var msg = $"no prefix parse function for {t} found";
            Errors.Add(msg);
        }

        private bool CurrentTokenIs(TokenType tokenType)
        {
            return _currentToken.Type == tokenType;
        }

        private bool PeekTokenIs(TokenType tokenType)
        {
            return _peekToken.Type == tokenType;
        }

        private bool ExpectPeek(TokenType tokenType)
        {
            if (PeekTokenIs(tokenType))
            {
                NextToken();
                return true;
            }

            PeekError(tokenType);
            return false;
        }

        private void PeekError(TokenType tokenType)
        {
            var msg = $"Expected next token to be {tokenType}, got {_peekToken.Type} instead";
            Errors.Add(msg);
        }

        private Precedences PeekPrecedence()
        {
            return _precedences.ContainsKey(_peekToken.Type) ? _precedences[_peekToken.Type] : Precedences.LOWEST;
        }

        private Precedences CurPrecedence()
        {
            return _precedences.ContainsKey(_currentToken.Type) ? _precedences[_currentToken.Type] : Precedences.LOWEST;
        }

        private void NextToken()
        {
            _currentToken = _peekToken;
            _peekToken = _lexer.NextToken();
        }

        private void RegisterPrefix(TokenType tokenType, PrefixParseFn fn)
        {
            _prefixParseFns.Add(tokenType, fn);
        }

        private void RegisterInfix(TokenType tokenType, InfixParseFn fn)
        {
            _infixParseFns.Add(tokenType, fn);
        }
    }
}