using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monkey.Ast;
using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using System;
using System.Collections.Generic;
using Boolean = Monkey.Ast.Expressions.Boolean;

namespace Monkey.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestLetStatements()
        {
            var tests = new[]
            {
                new
                {
                    Input = "let x = 5;",
                    ExpectedIdentifier = "x",
                    ExpectedValue = (object)5
                },
                new
                {
                    Input = "let y = true;",
                    ExpectedIdentifier = "y",
                    ExpectedValue = (object)true
                },
                new
                {
                    Input = "let foobar = y",
                    ExpectedIdentifier = "foobar",
                    ExpectedValue = (object)"y"
                }
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Input);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                Assert.AreEqual(1, program.Statements.Count, $"program.Statements does not contain 1 statement. got={program.Statements.Count}");

                var stmt = program.Statements[0];
                TestLetStatement(stmt, tt.ExpectedIdentifier);

                var val = (stmt as LetStatement).Value;
                TestLiteralExpression(val, tt.ExpectedValue);
            }
        }

        [TestMethod]
        public void TestLetForErrors()
        {
            const string input = @"
                                   let x 5;
                                   let y = 10;
                                   let 838383;
                                   ";
            var l = new Lexer.Lexer(input);
            var p = new Parser(l);

            p.ParseProgram();
            var errors = p.Errors;
            Assert.AreEqual(2, errors.Count, $"Parser should return 2 errors, got {errors.Count}");
        }

        [TestMethod]
        public void TestReturnStatements()
        {
            var tests = new Dictionary<string, object>
            {
                {"return 5;", 5},
                { "return true;", true},
                { "return foobar;", "foobar"}
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Key);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                Assert.AreEqual(1, program.Statements.Count, $"program.Statements does not contain 1 statement. got={program.Statements.Count}");

                var stmt = program.Statements[0];
                var returnStmt = stmt as ReturnStatement;
                Assert.IsNotNull(returnStmt, $"stmt not ReturnStatement. got={stmt.GetType().Name}");
                Assert.AreEqual("return", returnStmt.TokenLiteral, $"returnStmt.TokenLiteral not 'return', got {returnStmt.TokenLiteral}");
                TestLiteralExpression(returnStmt.ReturnValue, tt.Value);
            }
        }

        [TestMethod]
        public void TestIdentifierExpression()
        {
            const string input = "foobar;";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program has not enough statements. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statement[0] is not ExpressionStatement. got={program.Statements[0].GetType().Name}");
            Assert.IsNotNull(stmt.Expression, "stmt.Expression is null");

            var ident = stmt.Expression as Identifier;
            Assert.IsNotNull(ident, $"exp is not Identifier. got={stmt.Expression.GetType().Name}");
            Assert.AreEqual("foobar", ident.Value, $"ident.Value not foobar. got={ident.Value}");
            Assert.AreEqual("foobar", ident.TokenLiteral, $"ident.TokenLiteral not foobar. got={ident.TokenLiteral}");
        }

        [TestMethod]
        public void TestIntegerLiteralExpression()
        {
            const string input = "5;";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program has not enough statements. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statement[0] is not IntegerLiteral. got={program.Statements[0].GetType().Name}");
            Assert.IsNotNull(stmt.Expression, "stmt.Expression is null");

            var ident = stmt.Expression as IntegerLiteral;
            Assert.IsNotNull(ident, $"exp is not Identifier. got={stmt.Expression.GetType().Name}");
            Assert.AreEqual(5, ident.Value, $"ident.Value not 5. got={ident.Value}");
            Assert.AreEqual("5", ident.TokenLiteral, $"ident.TokenLiteral not 5. got={ident.TokenLiteral}");
        }

        [TestMethod]
        public void TestBooleanExpression()
        {
            var tests = new Dictionary<string, bool>
            {
                {"true;", true},
                { "false;", false}
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Key);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                Assert.AreEqual(1, program.Statements.Count, $"program has not enough statements. got={program.Statements.Count}");

                var stmt = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(stmt, $"program.Statement[0] is not IntegerLiteral. got={program.Statements[0].GetType().Name}");
                Assert.IsNotNull(stmt.Expression, "stmt.Expression is null");

                var ident = stmt.Expression as Boolean;
                Assert.IsNotNull(ident, $"exp is not Boolean. got={stmt.Expression.GetType().Name}");
                Assert.AreEqual(tt.Value, ident.Value, $"ident.Value not {tt.Value}. got={ident.Value}");
                Assert.AreEqual(tt.Value.ToString().ToLower(), ident.TokenLiteral, $"ident.TokenLiteral not {tt.Value.ToString().ToLower()}. got={ident.TokenLiteral}");
            }
        }

        [TestMethod]
        public void TestParsingPrefixExpressions()
        {
            var prefixTests = new[]
            {
                new
                {
                    Input = "!5;",
                    Operator = "!",
                    Value = (object)5L
                },
                new
                {
                    Input = "-15;",
                    Operator = "-",
                    Value = (object)15L
                },
                new
                {
                    Input = "!true;",
                    Operator = "!",
                    Value = (object)true
                },
                new
                {
                    Input = "!false;",
                    Operator = "!",
                    Value = (object)false
                }
            };

            foreach (var tt in prefixTests)
            {
                var l = new Lexer.Lexer(tt.Input);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                Assert.AreEqual(1, program.Statements.Count, $"program.Statements does not contain 1 statement. got={program.Statements.Count}");

                var stmt = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(stmt, $"program.Statements[0] is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

                var exp = stmt.Expression as PrefixExpression;
                Assert.IsNotNull(exp, $"stmt is not ast.PrefixExpression. got={stmt.Expression.GetType().Name}");

                Assert.AreEqual(tt.Operator, exp.Operator, $"exp.Operator is not '{tt.Operator}'. got={exp.Operator}");

                TestLiteralExpression(exp.Right, tt.Value);
            }
        }

        [TestMethod]
        public void TestParsingInfixExpressions()
        {
            var infixTests = new[]
            {
                new
                {
                    Input = "5 + 5;",
                    LeftValue = (object)5L,
                    Operator = "+",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 - 5;",
                    LeftValue = (object)5L,
                    Operator = "-",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 * 5;",
                    LeftValue = (object)5L,
                    Operator = "*",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 / 5;",
                    LeftValue = (object)5L,
                    Operator = "/",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 > 5;",
                    LeftValue = (object)5L,
                    Operator = ">",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 < 5;",
                    LeftValue = (object)5L,
                    Operator = "<",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 == 5;",
                    LeftValue = (object)5L,
                    Operator = "==",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "5 != 5;",
                    LeftValue = (object)5L,
                    Operator = "!=",
                    RightValue = (object)5L
                },
                new
                {
                    Input = "true == true",
                    LeftValue = (object)true,
                    Operator = "==",
                    RightValue = (object)true
                },
                new
                {
                    Input = "true != false",
                    LeftValue = (object)true,
                    Operator = "!=",
                    RightValue = (object)false
                },
                new
                {
                    Input = "false == false",
                    LeftValue = (object)false,
                    Operator = "==",
                    RightValue = (object)false
                }
            };

            foreach (var tt in infixTests)
            {
                var l = new Lexer.Lexer(tt.Input);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                Assert.AreEqual(1, program.Statements.Count, $"program.Statements does not contain 1 statement. got={program.Statements.Count}");

                var stmt = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(stmt, $"program.Statements[0] is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

                TestInfixExpression(stmt.Expression, tt.LeftValue, tt.Operator, tt.RightValue);
            }
        }

        [TestMethod]
        public void TestOperatorPrecedenceParsing()
        {
            var tests = new Dictionary<string, string>
            {
                {"-a * b", "((-a) * b)"},
                { "!-a", "(!(-a))"},
                { "a + b + c", "((a + b) + c)"},
                { "a + b - c", "((a + b) - c)"},
                { "a * b * c", "((a * b) * c)"},
                { "a * b / c", "((a * b) / c)"},
                { "a + b / c", "(a + (b / c))"},
                { "a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"},
                { "3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"},
                { "5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"},
                { "5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"},
                { "3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"},
                {"true", "true"},
                {"false", "false"},
                {"3 > 5 == false", "((3 > 5) == false)"},
                {"3 < 5 == true", "((3 < 5) == true)"},
                {"1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"},
                {"(5 + 5) * 2", "((5 + 5) * 2)"},
                {"2 / (5 + 5)", "(2 / (5 + 5))"},
                {"-(5 + 5)", "(-(5 + 5))"},
                {"!(true == true)", "(!(true == true))"},
                {"a + add(b * c) + d", "((a + add((b * c))) + d)"},
                {"add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"},
                {"add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))"},
                {"a * [1, 2, 3, 4][b * c] * d", "((a * ([1, 2, 3, 4][(b * c)])) * d)"},
                {"add(a * b[2], b[1], 2 * [1, 2][1])", "add((a * (b[2])), (b[1]), (2 * ([1, 2][1])))"}
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Key);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                var actual = program.ToString();
                Assert.AreEqual(tt.Value, actual, $"expected={tt.Value}, got={actual}");
            }
        }

        [TestMethod]
        public void TestIfExpression()
        {
            const string input = "if (x < y) { x }";
            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program.Body does not contain 1 statement. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statements[0] is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

            var exp = stmt.Expression as IfExpression;
            Assert.IsNotNull(exp, $"stmt.Expression is not ast.IfExpression. got={stmt.Expression.GetType().Name}");

            TestInfixExpression(exp.Condition, "x", "<", "y");

            Assert.AreEqual(1, exp.Consequence.Statements.Count, $"consequence is not 1 statement. got={exp.Consequence.Statements.Count}");

            var consequence = exp.Consequence.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequence, $"Statements[0] is not ast.ExpressionStatement. got={exp.Consequence.Statements[0].GetType().Name}");

            TestIdentifier(consequence.Expression, "x");

            Assert.IsNull(exp.Alternative, $"exp.Alternative.Statements was not nil. got={exp.Alternative}");
        }

        [TestMethod]
        public void TestIfElseExpression()
        {
            const string input = "if (x < y) { x } else { y }";
            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program.Body does not contain 1 statement. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statements[0] is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

            var exp = stmt.Expression as IfExpression;
            Assert.IsNotNull(exp, $"stmt.Expression is not ast.IfExpression. got={stmt.Expression.GetType().Name}");

            TestInfixExpression(exp.Condition, "x", "<", "y");

            Assert.AreEqual(1, exp.Consequence.Statements.Count, $"consequence is not 1 statement. got={exp.Consequence.Statements.Count}");

            var consequence = exp.Consequence.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequence, $"Statements[0] is not ast.ExpressionStatement. got={exp.Consequence.Statements[0].GetType().Name}");

            TestIdentifier(consequence.Expression, "x");

            Assert.AreEqual(1, exp.Alternative.Statements.Count, $"exp.Alternative.Statements does not contain 1 statement. got={exp.Alternative.Statements.Count}");

            var alternative = exp.Alternative.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(alternative, $"Statements[0] is not ast.ExpressionStatement. got={exp.Alternative.Statements[0].GetType().Name}");

            TestIdentifier(alternative.Expression, "y");
        }

        [TestMethod]
        public void TestFunctionLiteralParsing()
        {
            const string input = "fn(x, y) { x + y; }";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program.Body does not contain 1 statement. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statements[0] is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

            var function = stmt.Expression as FunctionLiteral;
            Assert.IsNotNull(function, $"stmt.Expression is not ast.FunctionLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual(2, function.Parameters.Count, $"function literal parameters wrong. want 2, got={function.Parameters.Count}");

            TestLiteralExpression(function.Parameters[0], "x");
            TestLiteralExpression(function.Parameters[1], "y");

            Assert.AreEqual(1, function.Body.Statements.Count, $"function.Body.Statements has not 1 statement. got={function.Body.Statements.Count}");

            var bodyStmt = function.Body.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(bodyStmt, $"function body stmt is not ast.ExpressionStatement. got={function.Body.Statements[0].GetType().Name}");

            TestInfixExpression(bodyStmt.Expression, "x", "+", "y");
        }

        [TestMethod]
        public void TestFunctionParameterParsing()
        {
            var tests = new[]
            {
                new
                {
                    Input = "fn() {};",
                    ExpectedParams = new string[0]
                },
                new
                {
                    Input = "fn(x) {};",
                    ExpectedParams = new [] {"x"}
                },
                new
                {
                    Input = "fn(x, y, z) {};",
                    ExpectedParams = new [] {"x", "y", "z"}
                }
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Input);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                var stmt = program.Statements[0] as ExpressionStatement;
                var function = stmt.Expression as FunctionLiteral;

                Assert.AreEqual(tt.ExpectedParams.Length, function.Parameters.Count, $"length parameters wrong. want {tt.ExpectedParams.Length}, got={function.Parameters.Count}");

                for (var i = 0; i < tt.ExpectedParams.Length; i++)
                {
                    var ident = tt.ExpectedParams[i];
                    TestLiteralExpression(function.Parameters[i], ident);
                }
            }
        }

        [TestMethod]
        public void TestCallExpressionParsing()
        {
            const string input = "add(1, 2 * 3, 4 + 5);";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            Assert.AreEqual(1, program.Statements.Count, $"program.Statements does not contain 1 statement. got={program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"stmt is not ast.ExpressionStatement. got={program.Statements[0].GetType().Name}");

            var exp = stmt.Expression as CallExpression;
            Assert.IsNotNull(exp, $"stmt.Expression is not ast.CallExpression. got={stmt.Expression.GetType().Name}");

            TestIdentifier(exp.Function, "add");

            Assert.AreEqual(3, exp.Arguments.Count, $"wrong length of arguments. got={exp.Arguments.Count}");

            TestLiteralExpression(exp.Arguments[0], 1);
            TestInfixExpression(exp.Arguments[1], 2, "*", 3);
            TestInfixExpression(exp.Arguments[2], 4, "+", 5);
        }

        [TestMethod]
        public void TestCallExpressionParameterParsing()
        {
            var tests = new[]
            {
                new
                {
                    Input = "add();",
                    ExpectedIdent = "add",
                    ExpectedArgs = new string[0]
                },
                new
                {
                    Input = "add(1);",
                    ExpectedIdent = "add",
                    ExpectedArgs = new [] {"1"}
                },
                new
                {
                    Input = "add(1, 2 * 3, 4 + 5);",
                    ExpectedIdent = "add",
                    ExpectedArgs = new [] {"1", "(2 * 3)", "(4 + 5)"}
                }
            };

            foreach (var tt in tests)
            {
                var l = new Lexer.Lexer(tt.Input);
                var p = new Parser(l);
                var program = p.ParseProgram();
                CheckParserErrors(p);

                var stmt = program.Statements[0] as ExpressionStatement;
                var exp = stmt.Expression as CallExpression;

                Assert.IsNotNull(exp, $"stmt.Expression is not ast.CallExpression. got={stmt.Expression.GetType().Name}");

                TestIdentifier(exp.Function, tt.ExpectedIdent);

                Assert.AreEqual(tt.ExpectedArgs.Length, exp.Arguments.Count, $"wrong number of arguments. want {tt.ExpectedArgs.Length}, got={exp.Arguments.Count}");

                for (var i = 0; i < tt.ExpectedArgs.Length; i++)
                {
                    var arg = tt.ExpectedArgs[i];
                    Assert.AreEqual(arg, exp.Arguments[i].ToString(), $"argument {i} wrong. want={arg}, got={exp.Arguments[i].ToString()}");
                }
            }
        }

        [TestMethod]
        public void TestStringLiteralExpression()
        {
            const string input = "\"hello world\";";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var literal = stmt.Expression as StringLiteral;
            Assert.IsNotNull(literal, $"exp not StringLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual("hello world", literal.Value, $"literal.Value no \"hello world\". got={literal.Value}");
        }

        [TestMethod]
        public void TestParsingEmptyArrayLiterals()
        {
            const string input = "[]";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var array = stmt.Expression as ArrayLiteral;
            Assert.IsNotNull(array, $"exp not ArrayLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual(0, array.Elements.Count, $"len(array.Elements) not 0. got={array.Elements.Count}");
        }

        [TestMethod]
        public void TestParsingArrayLiterals()
        {
            const string input = "[1, 2 * 2, 3 + 3]";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var array = stmt.Expression as ArrayLiteral;
            Assert.IsNotNull(array, $"exp not ArrayLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual(3, array.Elements.Count, $"len(array.Elements) not 3. got={array.Elements.Count}");

            TestIntegerLiteral(array.Elements[0], 1);
            TestInfixExpression(array.Elements[1], 2, "*", 2);
            TestInfixExpression(array.Elements[2], 3, "+", 3);
        }

        [TestMethod]
        public void TestParsingIndexExpressions()
        {
            const string input = "myArray[1 + 1]";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var indexExp = stmt.Expression as IndexExpression;
            Assert.IsNotNull(indexExp, $"exp not IndexExpression. got={stmt.Expression.GetType().Name}");

            TestIdentifier(indexExp.Left, "myArray");

            TestInfixExpression(indexExp.Index, 1, "+", 1);
        }

        [TestMethod]
        public void TestParsingEmptyHashLiteral()
        {
            const string input = "{}";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, $"exp is not HashLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual(0, hash.Pairs.Count, $"hash.Pairs has wrong length. got={hash.Pairs.Count}");
        }

        [TestMethod]
        public void TestParsingHashLiteralsStringKeys()
        {
            const string input = "{\"one\": 1, \"two\": 2, \"three\": 3}";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, $"exp is not HashLiteral. got={stmt.Expression.GetType().Name}");

            var expected = new Dictionary<string, long>
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as StringLiteral;
                Assert.IsNotNull(literal, $"key is not ast.StringLiteral. got={pair.Key.GetType().Name}");

                var expectedValue = expected[literal.ToString()];

                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [TestMethod]
        public void TestParsingHashLiteralsBooleanKeys()
        {
            const string input = "{true: 1, false: 2}";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, $"exp is not HashLiteral. got={stmt.Expression.GetType().Name}");

            var expected = new Dictionary<string, long>
            {
                { "true", 1 },
                { "false", 2 }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as Boolean;
                Assert.IsNotNull(literal, $"key is not ast.Boolean. got={pair.Key.GetType().Name}");

                var expectedValue = expected[literal.ToString()];

                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [TestMethod]
        public void TestParsingHashLiteralsIntegerKeys()
        {
            const string input = "{1: 1, 2: 2, 3: 3}";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, $"exp is not HashLiteral. got={stmt.Expression.GetType().Name}");

            var expected = new Dictionary<string, long>
            {
                { "1", 1 },
                { "2", 2 },
                { "3", 3 }
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as IntegerLiteral;
                Assert.IsNotNull(literal, $"key is not ast.IntegerLiteral. got={pair.Key.GetType().Name}");

                var expectedValue = expected[literal.ToString()];

                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [TestMethod]
        public void TestParsingHashLiteralsWithExpressions()
        {
            const string input = "{\"one\": 0 + 1, \"two\": 10 - 8, \"three\": 15 / 5}";

            var l = new Lexer.Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            CheckParserErrors(p);

            var stmt = program.Statements[0] as ExpressionStatement;
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, $"exp is not HashLiteral. got={stmt.Expression.GetType().Name}");

            Assert.AreEqual(3, hash.Pairs.Count, $"hash.Pairs has wrong length. got={hash.Pairs.Count}");

            var tests = new Dictionary<string, Action<IExpression>>
            {
                { "one", e => TestInfixExpression(e, 0, "+", 1) },
                { "two", e => TestInfixExpression(e, 10, "-", 8) },
                { "three", e => TestInfixExpression(e, 15, "/", 5) },
            };

            foreach (var pair in hash.Pairs)
            {
                var literal = pair.Key as StringLiteral;
                Assert.IsNotNull(literal, $"key is not ast.StringLiteral. got={pair.Key.GetType().Name}");

                Assert.IsTrue(tests.ContainsKey(literal.ToString()));

                var testFunc = tests[literal.ToString()];
                testFunc(pair.Value);
            }
        }

        private void TestIntegerLiteral(IExpression il, long value)
        {
            var integ = il as IntegerLiteral;
            Assert.IsNotNull(integ, $"il not IntegerLiteral. got={il.GetType().Name}");
            Assert.AreEqual(value.ToString(), integ.TokenLiteral, $"integ.TokenLiteral not {value}. got={integ.TokenLiteral}");
        }

        private void TestIdentifier(IExpression exp, string value)
        {
            var ident = exp as Identifier;
            Assert.IsNotNull(ident, $"exp not *ast.Identifier. got={exp.GetType().Name}");

            Assert.AreEqual(value, ident.Value, $"ident.Value not {value}. got={ident.Value}");
            Assert.AreEqual(value, ident.TokenLiteral, $"ident.TokenLiteral not {value}. got={ident.TokenLiteral}");
        }

        private void TestBooleanLiteral(IExpression exp, bool value)
        {
            var bo = exp as Boolean;
            Assert.IsNotNull(bo, $"exp not *ast.Boolean. got={exp.GetType().Name}");

            Assert.AreEqual(value, bo.Value, $"ident.Value not {value}. got={bo.Value}");
            Assert.AreEqual(value.ToString().ToLower(), bo.TokenLiteral, $"ident.TokenLiteral not {value.ToString().ToLower()}. got={bo.TokenLiteral}");
        }

        private void TestLiteralExpression<T>(IExpression exp, T expected)
        {
            if (expected is int)
            {
                TestIntegerLiteral(exp, Convert.ToInt32(expected));
            }
            else if (expected is long)
            {
                TestIntegerLiteral(exp, Convert.ToInt64(expected));
            }
            else if (expected is string)
            {
                TestIdentifier(exp, Convert.ToString(expected));
            }
            else if (expected is bool)
            {
                TestBooleanLiteral(exp, Convert.ToBoolean(expected));
            }
            else
            {
                Assert.Fail($"type of exp not handled. got={expected.GetType().Name}");
            }
        }

        private void TestInfixExpression<T, TU>(IExpression exp, T left, string @operator, TU right)
        {
            var opExp = exp as InfixExpression;
            Assert.IsNotNull(opExp, $"exp is not ast.OperatorExpression. got={exp.GetType().Name}({exp})");

            TestLiteralExpression(opExp.Left, left);
            Assert.AreEqual(@operator, opExp.Operator, $"exp.Operator is not '{@operator}'. got={opExp.Operator}");
            TestLiteralExpression(opExp.Right, right);
        }

        private void TestLetStatement(IStatement s, string name)
        {
            Assert.AreEqual("let", s.TokenLiteral, $"s.TokenLiteral is not 'let'. got={s.TokenLiteral}");

            var letStmnt = s as LetStatement;
            Assert.IsNotNull(letStmnt, $"s is not LetStatement. got={s.GetType().Name}");

            Assert.AreEqual(name, letStmnt.Name.Value, $"letStmt.Name.Value not '{name}'. got={letStmnt.Name.Value}");

            Assert.AreEqual(name, letStmnt.Name.TokenLiteral, $"s.Name not '{name}'. got={letStmnt.Name}");
        }

        private void CheckParserErrors(Parser parser)
        {
            var errors = parser.Errors;
            Assert.AreEqual(0, errors.Count, $"Parser has {errors.Count} errors: {string.Join("; ", errors)}");
        }
    }
}