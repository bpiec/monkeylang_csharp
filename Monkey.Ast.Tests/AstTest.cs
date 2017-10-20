using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using Monkey.Token;
using System.Collections.Generic;

namespace Monkey.Ast.Tests
{
    [TestClass]
    public class AstTest
    {
        [TestMethod]
        public void TestString()
        {
            var program = new Program
            {
                Statements =
                {
                    new LetStatement
                    {
                        Token = new Token.Token(TokenType.LET, "let"),
                        Name = new Identifier
                        {
                            Token = new Token.Token(TokenType.IDENT, "myVar"),
                            Value = "myVar"
                        },
                        Value = new Identifier
                        {
                            Token = new Token.Token(TokenType.IDENT, "anotherVar"),
                            Value = "anotherVar"
                        }
                    }
                }
            };
            Assert.AreEqual("let myVar = anotherVar;", program.ToString(), $"program.String wrong. got={program}");
        }

        [TestMethod]
        public void TestModify()
        {
            IExpression One() => new IntegerLiteral { Value = 1, Token = new Token.Token() };
            IExpression Two() => new IntegerLiteral { Value = 2, Token = new Token.Token() };

            INode TurnOneIntoTwo(INode node)
            {
                if (!(node is IntegerLiteral integer))
                {
                    return node;
                }

                if (integer.Value != 1)
                {
                    return node;
                }

                integer.Value = 2;
                return integer;
            }

            var tests = new Dictionary<INode, INode>
            {
                {
                    One(),
                    Two()
                },
                {
                    new Program
                    {
                        Statements = {new ExpressionStatement {Expression = One()}}
                    },
                    new Program
                    {
                        Statements = {new ExpressionStatement {Expression = Two()}}
                    }
                },
                {
                    new InfixExpression
                    {
                        Left = One(),
                        Operator = "+",
                        Right = Two()
                    },
                    new InfixExpression
                    {
                        Left = Two(),
                        Operator = "+",
                        Right = Two()
                    }
                },
                {
                    new InfixExpression
                    {
                        Left = Two(),
                        Operator = "+",
                        Right = One()
                    },
                    new InfixExpression
                    {
                        Left = Two(),
                        Operator = "+",
                        Right = Two()
                    }
                },
                {
                    new PrefixExpression
                    {
                        Operator = "-",
                        Right = One()
                    },
                    new PrefixExpression
                    {
                        Operator = "-",
                        Right = Two()
                    }
                },
                {
                    new IndexExpression
                    {
                        Left = One(),
                        Index = One()
                    },
                    new IndexExpression
                    {
                        Left = Two(),
                        Index = Two()
                    }
                },
                {
                    new IfExpression
                    {
                        Condition = One(),
                        Consequence = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = One()
                                }
                            }
                        },
                        Alternative = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = One()
                                }
                            }
                        }
                    },
                    new IfExpression
                    {
                        Condition = Two(),
                        Consequence = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = Two()
                                }
                            }
                        },
                        Alternative = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = Two()
                                }
                            }
                        }
                    }
                },
                {
                    new ReturnStatement
                    {
                        ReturnValue = One()
                    },
                    new ReturnStatement
                    {
                        ReturnValue = Two()
                    }
                },
                {
                    new LetStatement
                    {
                        Value = One()
                    },
                    new LetStatement
                    {
                        Value = Two()
                    }
                },
                {
                    new FunctionLiteral
                    {
                        Parameters = new List<Identifier>(),
                        Body = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = One()
                                }
                            }
                        }
                    },
                    new FunctionLiteral
                    {
                        Parameters = new List<Identifier>(),
                        Body = new BlockStatement
                        {
                            Statements = new List<IStatement>
                            {
                                new ExpressionStatement
                                {
                                    Expression = Two()
                                }
                            }
                        }
                    }
                },
                {
                    new ArrayLiteral
                    {
                        Elements = new List<IExpression>
                        {
                            One(),
                            Two()
                        }
                    },
                    new ArrayLiteral
                    {
                        Elements = new List<IExpression>
                        {
                            Two(),
                            Two()
                        }
                    }
                },
                {
                    new HashLiteral
                    {
                        Pairs = new Dictionary<IExpression, IExpression>
                        {
                            {
                                One(), One()
                            }
                        }
                    },
                    new HashLiteral
                    {
                        Pairs = new Dictionary<IExpression, IExpression>
                        {
                            {
                                Two(), Two()
                            }
                        }
                    }
                }
            };

            foreach (var tt in tests)
            {
                var modified = Modifier.Modify(tt.Key, TurnOneIntoTwo);

                Assert.AreEqual(tt.Value, modified, $"not equal. got={modified}, want={tt.Value}");
            }
        }
    }
}