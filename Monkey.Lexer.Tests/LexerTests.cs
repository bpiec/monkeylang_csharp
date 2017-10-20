using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monkey.Token;
using System.Collections.Generic;

namespace Monkey.Lexer.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestNextToken1()
        {
            const string input = "=+(){},;";
            var tests = new List<Test>(new[]
            {
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.PLUS, "+"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.EOF, ""),
            });

            DoAsserts(input, tests);
        }

        [TestMethod]
        public void TestNextToken2()
        {
            const string input = @"let five = 5;
                                   let ten = 10;
                                   let add = fn(x, y) {
                                       x + y;
                                   };
                                   let result = add(five, ten); ";
            var tests = new List<Test>(new[]
            {
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.FUNCTION, "fn"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.PLUS, "+"),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "result"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.EOF, "")
            });

            DoAsserts(input, tests);
        }

        [TestMethod]
        public void TestNextToken3()
        {
            const string input = @"let five = 5;
                                   let ten = 10;

                                   let add = fn(x, y) {
                                     x + y;
                                   };

                                   let result = add(five, ten);
                                   !-/*5;
                                   5 < 10 > 5;
";
            var tests = new List<Test>(new[]
            {
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.FUNCTION, "fn"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.PLUS, "+"),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "result"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.BANG, "!"),
                new Test(TokenType.MINUS, "-"),
                new Test(TokenType.SLASH, "/"),
                new Test(TokenType.ASTERISK, "*"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.LT, "<"),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.GT, ">"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";")
            });

            DoAsserts(input, tests);
        }

        [TestMethod]
        public void TestNextToken4()
        {
            const string input = @" let five = 5;
                                    let ten = 10;
                                    let add = fn(x, y) {
                                        x + y;
                                    };
                                    let result = add(five, ten);
                                    !-/*5;
                                    5 < 10 > 5;
                                    if (5 < 10) {
                                        return true;
                                    } else {
                                        return false;
                                    }
                                    10 == 10;
                                    10 != 9;
                                    ""foobar""
                                    ""foo bar""
                                    [1, 2];
                                    {""foo"": ""bar""}
                                    macro(x, y) { x + y; };";

            var tests = new List<Test>(new[]
            {
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.FUNCTION, "fn"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.PLUS, "+"),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LET, "let"),
                new Test(TokenType.IDENT, "result"),
                new Test(TokenType.ASSIGN, "="),
                new Test(TokenType.IDENT, "add"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "five"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "ten"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.BANG, "!"),
                new Test(TokenType.MINUS, "-"),
                new Test(TokenType.SLASH, "/"),
                new Test(TokenType.ASTERISK, "*"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.LT, "<"),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.GT, ">"),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.IF, "if"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.INT, "5"),
                new Test(TokenType.LT, "<"),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.RETURN, "return"),
                new Test(TokenType.TRUE, "true"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.ELSE, "else"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.RETURN, "return"),
                new Test(TokenType.FALSE, "false"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.EQ, "=="),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.INT, "10"),
                new Test(TokenType.NOT_EQ, "!="),
                new Test(TokenType.INT, "9"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.STRING, "foobar"),
                new Test(TokenType.STRING, "foo bar"),
                new Test(TokenType.LBRACKET, "["),
                new Test(TokenType.INT, "1"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.INT, "2"),
                new Test(TokenType.RBRACKET, "]"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.STRING, "foo"),
                new Test(TokenType.COLON, ":"),
                new Test(TokenType.STRING, "bar"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.MACRO, "macro"),
                new Test(TokenType.LPAREN, "("),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.COMMA, ","),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.RPAREN, ")"),
                new Test(TokenType.LBRACE, "{"),
                new Test(TokenType.IDENT, "x"),
                new Test(TokenType.PLUS, "+"),
                new Test(TokenType.IDENT, "y"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.RBRACE, "}"),
                new Test(TokenType.SEMICOLON, ";"),
                new Test(TokenType.EOF, "")
            });

            DoAsserts(input, tests);
        }

        private void DoAsserts(string input, List<Test> tests)
        {
            var l = new Lexer(input);
            for (var i = 0; i < tests.Count; i++)
            {
                var tt = tests[i];
                var tok = l.NextToken();
                Assert.AreEqual(tt.ExpectedType, tok.Type, $"tests[{i}] - tokentype wrong. expected={tt.ExpectedType}, got={tok.Type}");
                Assert.AreEqual(tt.ExpectedLiteral, tok.Literal, $"tests[{i}] - literal wrong. expected={tt.ExpectedLiteral}, got={tok.Literal}");
            }
        }

        private struct Test
        {
            public TokenType ExpectedType { get; }
            public string ExpectedLiteral { get; }

            public Test(TokenType expectedType, string expectedLiteral)
            {
                ExpectedType = expectedType;
                ExpectedLiteral = expectedLiteral;
            }

            public override string ToString()
            {
                return $"{ExpectedType} {ExpectedLiteral}";
            }
        }
    }
}