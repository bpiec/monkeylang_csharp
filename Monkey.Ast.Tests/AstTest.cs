using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using Monkey.Token;

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
    }
}