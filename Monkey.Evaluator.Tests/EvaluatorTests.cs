using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monkey.Ast;
using Monkey.Object;
using System.Collections.Generic;

namespace Monkey.Evaluator.Tests
{
    [TestClass]
    public class EvaluatorTests
    {
        [TestMethod]
        public void TestEvalIntegerExpression()
        {
            var tests = new Dictionary<string, long>
            {
                {"5", 5},
                {"10", 10},
                {"-5", -5},
                {"-10", -10},
                {"5 + 5 + 5 + 5 - 10", 10},
                {"2 * 2 * 2 * 2 * 2", 32},
                {"-50 + 100 + -50", 0},
                {"5 * 2 + 10", 20},
                {"5 + 2 * 10", 25},
                {"20 + 2 * -10", 0},
                {"50 / 2 * 2 + 10", 60},
                {"2 * (5 + 10)", 30},
                {"3 * 3 * 3 + 10", 37},
                {"3 * (3 * 3) + 10", 37},
                {"(5 + 10 * 2 + 15 / 3) * 2 + -10", 50}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                TestIntegerObject(evaluated, tt.Value);
            }
        }

        [TestMethod]
        public void TestEvalBooleanExpression()
        {
            var tests = new Dictionary<string, bool>
            {
                {"true", true},
                {"false", false},
                {"1 < 2", true},
                {"1 > 2", false},
                {"1 < 1", false},
                {"1 > 1", false},
                {"1 == 1", true},
                {"1 != 1", false},
                {"1 == 2", false},
                {"1 != 2", true},
                { "true == true", true},
                {"false == false", true},
                {"true == false", false},
                {"true != false", true},
                {"false != true", true},
                {"(1 < 2) == true", true},
                {"(1 < 2) == false", false},
                {"(1 > 2) == true", false},
                {"(1 > 2) == false", true}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                TestBooleanObject(evaluated, tt.Value);
            }
        }

        [TestMethod]
        public void TestBangOperator()
        {
            var tests = new Dictionary<string, bool>
            {
                {"!true", false},
                {"!false", true},
                {"!5", false},
                {"!!true", true},
                {"!!false", false},
                {"!!5", true}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                TestBooleanObject(evaluated, tt.Value);
            }
        }

        [TestMethod]
        public void TestIfElseExpressions()
        {
            var tests = new Dictionary<string, long?>
            {
                {"if (true) { 10 }", 10},
                {"if (false) { 10 }", null},
                {"if (1) { 10 }", 10},
                {"if (1 < 2) { 10 }", 10},
                {"if (1 > 2) { 10 }", null},
                {"if (1 > 2) { 10 } else { 20 }", 20},
                {"if (1 < 2) { 10 } else { 20 }", 10}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                if (tt.Value.HasValue)
                {
                    TestIntegerObject(evaluated, tt.Value.Value);
                }
                else
                {
                    TestNullObject(evaluated);
                }
            }
        }

        [TestMethod]
        public void TestReturnStatements()
        {
            var tests = new Dictionary<string, long>
            {
                {"return 10;", 10},
                {"return 10; 9;", 10},
                {"return 2 * 5; 9;", 10},
                {"9; return 2 * 5; 9;", 10},
                {"if (10 > 1) { return 10; }", 10},
                {@" if (10 > 1)
                    {
                        if (10 > 1)
                        {
                            return 10;
                        }

                        return 1;
                    }", 10
                },
                {@" let f = fn(x) {
                        return x;
                        x + 10;
                    };
                    f(10);", 10
                },
                {@" let f = fn(x) {
                        let result = x + 10;
                        return result;
                        return 10;
                    };
                    f(10);", 20
                }
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                TestIntegerObject(evaluated, tt.Value);
            }
        }

        [TestMethod]
        public void TestErrorHandling()
        {
            var tests = new Dictionary<string, string>
            {
                    {
                        "5 + true;",
                        "type mismatch: INTEGER + BOOLEAN"
                    },
                    {
                        "5 + true; 5;",
                        "type mismatch: INTEGER + BOOLEAN"
                    },
                    {
                        "-true",
                        "unknown operator: -BOOLEAN"
                    },
                    {
                        "true + false;",
                        "unknown operator: BOOLEAN + BOOLEAN"
                    },
                    {
                        "5; true + false; 5",
                        "unknown operator: BOOLEAN + BOOLEAN"
                    },
                    {
                        "if (10 > 1) { true + false; }",
                        "unknown operator: BOOLEAN + BOOLEAN"
                    },
                    {
                        @"
if (10 > 1) {
   if (10 > 1) {
      return true + false;
   }
   return 1;
}
                    ", "unknown operator: BOOLEAN + BOOLEAN"
                    },
                    {
                        "foobar",
                        "identifier not found: foobar"
                    },
                    {
                        "\"Hello\" - \"World\"",
                        "unknown operator: STRING - STRING"
                    },
                    {
                        "{\"name\": \"Monkey\"}[fn(x) { x }];",
                        "unusable as hash key: FUNCTION"
                    }
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);

                Assert.IsTrue(evaluated is Error, $"no error object returned. got={evaluated.GetType().Name}({evaluated})");

                var errObj = (Error)evaluated;
                Assert.AreEqual(tt.Value, errObj.Message, $"wrong error message. expected={tt.Value}, got={errObj.Message}");
            }
        }

        [TestMethod]
        public void TestLetStatements()
        {
            var tests = new Dictionary<string, long>
            {
                {"let a = 5; a;", 5},
                {"let a = 5 * 5; a;", 25},
                {"let a = 5; let b = a; b;", 5},
                {"let a = 5; let b = a; let c = a + b + 5; c;", 15}
            };

            foreach (var tt in tests)
            {
                TestIntegerObject(TestEval(tt.Key), tt.Value);
            }
        }

        [TestMethod]
        public void TestFunctionObject()
        {
            const string input = "fn(x) { x + 2; };";

            var evaluated = TestEval(input);
            Assert.IsTrue(evaluated is Function, $"object is not Function. got={evaluated.GetType().Name} ({evaluated})");

            var fn = (Function)evaluated;
            Assert.AreEqual(1, fn.Parameters.Count, $"function has wrong parameters. Parameters={fn.Parameters.Count}");

            Assert.AreEqual("x", fn.Parameters[0].ToString(), $"parameter is not 'x'. got={fn.Parameters[0]}");

            const string expectedBody = "(x + 2)";

            Assert.AreEqual(expectedBody, fn.Body.ToString(), $"body is not {expectedBody}. got={fn.Body}");
        }

        [TestMethod]
        public void TestFunctionApplication()
        {
            var tests = new Dictionary<string, long>
            {
                {"let identity = fn(x) { x; }; identity(5);", 5},
                {"let identity = fn(x) { return x; }; identity(5);", 5},
                {"let double = fn(x) { x * 2; }; double(5);", 10},
                {"let add = fn(x, y) { x + y; }; add(5, 5);", 10},
                {"let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20},
                {"fn(x) { x; }(5)", 5}
            };

            foreach (var tt in tests)
            {
                TestIntegerObject(TestEval(tt.Key), tt.Value);
            }
        }

        [TestMethod]
        public void TestStringLiteral()
        {
            const string input = "\"Hello World!\"";

            var evaluated = TestEval(input);
            Assert.IsTrue(evaluated is String, $"object is no String. got={evaluated.GetType().Name}, ({evaluated})");

            var str = (String)evaluated;
            Assert.AreEqual("Hello World!", str.Value, $"String has wrong value. got={str.Value}");
        }

        [TestMethod]
        public void TestStringConcatenation()
        {
            const string input = "\"Hello\" + \" \" + \"World!\"";

            var evaluated = TestEval(input);
            Assert.IsTrue(evaluated is String, $"object is not String, got={evaluated.GetType().Name} ({evaluated})");

            var str = (String)evaluated;
            Assert.AreEqual("Hello World!", str.Value, $"String has wrong value. got={str.Value}");
        }

        [TestMethod]
        public void TestBuiltinFunctions()
        {
            var tests = new Dictionary<string, object>
            {
                {"len(\"\")", 0L},
                {"len(\"four\")", 4L},
                {"len(\"hello world\")", 11L},
                {"len(1)", "argument to `len` not supported, got INTEGER"},
                {"len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1"},
                {"len([1, 2, 3])", 3},
                {"len([])", 0},
                {"first([1, 2, 3])", 1},
                {"first([])", null},
                {"first(1)", "argument to `first` must be ARRAY, got INTEGER"},
                {"last([1, 2, 3])", 3},
                {"last([])", null},
                {"last(1)", "argument to `last` must be ARRAY, got INTEGER"},
                {"rest([1, 2, 3])", new []{2, 3}},
                {"rest([])", null},
                {"push([], 1)", new [] {1}},
                {"push(1, 1)", "argument to `push` must be ARRAY, got INTEGER"}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);

                if (tt.Value is long expected)
                {
                    TestIntegerObject(evaluated, expected);
                }
                if (tt.Value is string)
                {
                    Assert.IsTrue(evaluated is Error, $"object is not Error. got={evaluated.GetType().Name} ({evaluated})");
                    var errObj = (Error)evaluated;
                    Assert.AreEqual(tt.Value, errObj.Message, $"wrong error message. expected={tt.Value}, got={errObj.Message}");
                }
            }
        }

        [TestMethod]
        public void TestArrayLiterals()
        {
            const string input = "[1, 2 * 2, 3 + 3]";

            var evaluated = TestEval(input);
            Assert.IsTrue(evaluated is Array, $"object is not Array, got={evaluated.GetType().Name} ({evaluated})");

            var result = (Array)evaluated;
            Assert.AreEqual(3, result.Elements.Length, $"array has wrong num of elements. got={result.Elements.Length}");
        }

        [TestMethod]
        public void TestArrayIndexExpressions()
        {
            var tests = new Dictionary<string, long?>
            {
                {
                    "[1, 2, 3][0]",
                    1
                },
                {
                    "[1, 2, 3][1]",
                    2
                },
                {
                    "[1, 2, 3][2]",
                    3
                },
                {
                    "let i = 0; [1][i];",
                    1
                },
                {
                    "[1, 2, 3][1 + 1];",
                    3
                },
                {
                    "let myArray = [1, 2, 3]; myArray[2];",
                    3
                },
                {
                    "let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2];",
                    6
                },
                {
                    "let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]",
                    2
                },
                {
                    "[1, 2, 3][3]",
                    null
                },
                {
                    "[1, 2, 3][-1]",
                    null
                }
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                if (tt.Value.HasValue)
                {
                    TestIntegerObject(evaluated, tt.Value.Value);
                }
                else
                {
                    TestNullObject(evaluated);
                }
            }
        }

        [TestMethod]
        public void TestHashLiterals()
        {
            const string input = @"let two = ""two"";
                                   {
                                       ""one"": 10 - 9,
                                       two: 1 + 1,
                                       ""thr"" + ""ee"": 6 / 2,
                                       4: 4,
                                       true: 5,
                                       false: 6
                                   }";

            var evaluated = TestEval(input);
            Assert.IsTrue(evaluated is Hash, $"Eval didn't return Hash. got={evaluated.GetType().Name} ({evaluated})");
            var result = (Hash)evaluated;

            var expected = new Dictionary<HashKey, long>
            {
                {new String { Value = "one"}.HashKey(), 1L},
                {new String { Value = "two"}.HashKey(), 2L},
                {new String { Value = "three"}.HashKey(), 3L},
                {new Integer { Value = 4}.HashKey(), 4L},
                {new Boolean { Value = true}.HashKey(), 5L},
                {new Boolean { Value = false}.HashKey(), 6L}
            };

            Assert.AreEqual(expected.Count, result.Pairs.Count, $"Hash has wrong num of pairs.got={result.Pairs.Count}");

            foreach (var e in expected)
            {
                Assert.IsTrue(result.Pairs.ContainsKey(e.Key), "no pair for given key in Pairs");
                var pair = result.Pairs[e.Key];

                TestIntegerObject(pair.Value, e.Value);
            }
        }

        [TestMethod]
        public void TestHashIndexExpressions()
        {
            var tests = new Dictionary<string, long?>
            {
                {
                    "{\"foo\": 5}[\"foo\"]",
                    5
                },
                {
                    "{\"foo\": 5}[\"bar\"]",
                    null
                },
                {
                    "let key = \"foo\"; {\"foo\": 5}[key]",
                    5
                },
                {
                    "{}[\"foo\"]",
                    null
                },
                {
                    "{5: 5}[5]",
                    5
                },
                {
                    "{true: 5}[true]",
                    5
                },
                {
                    "{false: 5}[false]",
                    5
                }
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                if (tt.Value.HasValue)
                {
                    TestIntegerObject(evaluated, tt.Value.Value);
                }
                else
                {
                    TestNullObject(evaluated);
                }
            }
        }

        [TestMethod]
        public void TestQuote()
        {
            var tests = new Dictionary<string, string>
            {
                {"quote(5)", "5"},
                {"quote(5 + 8)", "(5 + 8)"},
                {"quote(foobar)", "foobar"},
                {"quote(foobar + barfoo)", "(foobar + barfoo)"}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                Assert.IsTrue(evaluated is Quote, $"expected Quote. got={evaluated.GetType().Name} ({evaluated})");
                var quote = (Quote)evaluated;

                Assert.IsNotNull(quote.Node, "quote.Node is null");

                Assert.AreEqual(tt.Value, quote.Node.ToString(), $"not equal. got={quote.Node.ToString()}, want={tt.Value}");
            }
        }

        [TestMethod]
        public void TestQuoteUnquote()
        {
            var tests = new Dictionary<string, string>
            {
                {"quote(unquote(4))", "4"},
                {"quote(unquote(4 + 4))", "8"},
                {"quote(8 + unquote(4 + 4))", "(8 + 8)"},
                {"quote(unquote(4 + 4) + 8)", "(8 + 8)"},
                {"let foobar = 8; quote(foobar)", "foobar"},
                {"let foobar = 8; quote(unquote(foobar))", "8"},
                {"quote(unquote(true))", "true"},
                {"quote(unquote(true == false))", "false"},
                {"quote(unquote(quote(4 + 4)))", "(4 + 4)"},
                {"let quotedInfixExpression = quote(4 + 4); quote(unquote(4 + 4) + unquote(quotedInfixExpression))", "(8 + (4 + 4))"}
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Key);
                Assert.IsTrue(evaluated is Quote, $"expected Quote. got={evaluated.GetType().Name} ({evaluated})");
                var quote = (Quote)evaluated;

                Assert.IsNotNull(quote.Node, "quote.Node is null");

                Assert.AreEqual(tt.Value, quote.Node.ToString(), $"not equal. got={quote.Node.ToString()}, want={tt.Value}");
            }
        }

        [TestMethod]
        public void TestDefineMacros()
        {
            const string input = @" let number = 1;
                                    let function = fn(x, y) { x + y };
                                    let mymacro = macro(x, y) { x + y; };";

            var env = new Environment();
            var program = TestParseProgram(input);

            new Evaluator().DefineMacros(program, env);

            Assert.AreEqual(2, program.Statements.Count, $"Wrong number of statements. got={program.Statements.Count}");

            Assert.IsNull(env.Get("number"), "number should not be defined");
            Assert.IsNull(env.Get("function"), "function should not be defined");

            var obj = env.Get("mymacro");
            Assert.IsNotNull(obj, "macro not in environment.");

            Assert.IsTrue(obj is Macro, $"object is not Macro. got={obj.GetType().Name} (obj)");
            var macro = (Macro)obj;

            Assert.AreEqual(2, macro.Parameters.Count, $"Wrong number of macro parameters. got={macro.Parameters.Count}");

            Assert.AreEqual("x", macro.Parameters[0].ToString(), $"parameter is not 'x'. got={macro.Parameters[0]}");
            Assert.AreEqual("y", macro.Parameters[1].ToString(), $"parameter is not 'y'. got={macro.Parameters[1]}");

            const string expectedBody = "(x + y)";
            Assert.AreEqual(expectedBody, macro.Body.ToString(), $"body is not {expectedBody}. got={macro.Body}");
        }

        [TestMethod]
        public void TestExpandMacros()
        {
            var tests = new Dictionary<string, string>
            {
                {"let infixExpression = macro() { quote(1 + 2); }; infixExpression(); ", "(1 + 2)"},
                {"let reverse = macro(a, b) { quote(unquote(b) - unquote(a)); }; reverse(2 + 2, 10 - 5); ", "(10 - 5) - (2 + 2)"},
                {@" let unless = macro(condition, consequence, alternative) {
                        quote(if (!(unquote(condition))) {
                                unquote(consequence);
                              } else {
                                unquote(alternative);
                              });
                    };
                    unless(10 > 5, puts(""not greater""), puts(""greater""));", "if (!(10 > 5)) { puts(\"not greater\") } else { puts(\"greater\") }" }
            };

            foreach (var tt in tests)
            {
                var expected = TestParseProgram(tt.Value);
                var program = TestParseProgram(tt.Key);

                var env = new Environment();
                var evaluator = new Evaluator();
                evaluator.DefineMacros(program, env);
                var expanded = evaluator.ExpandMacros(program, env);

                Assert.AreEqual(expected.ToString(), expanded.ToString(), $"not equal. want={expected.ToString()}, got={expanded.ToString()}");
            }
        }

        private IObject TestEval(string input)
        {
            var l = new Lexer.Lexer(input);
            var p = new Parser.Parser(l);
            var program = p.ParseProgram();
            var env = new Environment();

            return new Evaluator().Eval(program, env);
        }

        private void TestIntegerObject(IObject obj, long expected)
        {
            Assert.IsNotNull(obj, "object is null");
            Assert.IsTrue(obj is Integer, $"object is not Integer. got={obj.GetType().Name} ({obj})");

            var result = (Integer)obj;
            Assert.AreEqual(expected, result.Value, $"object has wrong value.got={result.Value}, want={expected}");
        }

        private void TestBooleanObject(IObject obj, bool expected)
        {
            Assert.IsNotNull(obj, "object is null");
            Assert.IsTrue(obj is Boolean, $"object is not Boolean. got={obj.GetType().Name} ({obj})");

            var result = (Boolean)obj;
            Assert.AreEqual(expected, result.Value, $"object has wrong value.got={result.Value}, want={expected}");
        }

        private void TestNullObject(IObject obj)
        {
            Assert.IsTrue(obj is Null, $"object is not NULL. got={obj.Type}");
        }

        private Program TestParseProgram(string input)
        {
            var l = new Lexer.Lexer(input);
            var p = new Parser.Parser(l);
            return p.ParseProgram();
        }
    }
}