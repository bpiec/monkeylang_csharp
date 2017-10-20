using Monkey.Ast;
using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using Monkey.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Array = Monkey.Object.Array;
using Boolean = Monkey.Object.Boolean;
using Environment = Monkey.Object.Environment;
using String = Monkey.Object.String;

namespace Monkey.Evaluator
{
    public class Evaluator
    {
        private readonly Null _null;
        private readonly Boolean _true;
        private readonly Boolean _false;
        private readonly Dictionary<string, Builtin> _builtins;

        public Evaluator()
        {
            _null = new Null();
            _true = new Boolean
            {
                Value = true
            };
            _false = new Boolean
            {
                Value = false
            };
            _builtins = new Dictionary<string, Builtin>
            {
                {
                    "len", new Builtin
                    {
                        Fn = args =>
                        {
                            if (args.Length != 1)
                            {
                                return NewError($"wrong number of arguments. got={args.Length}, want=1");
                            }

                            if (args[0] is Array array)
                            {
                                return new Integer
                                {
                                    Value = array.Elements.Length
                                };
                            }

                            if (args[0] is String str)
                            {
                                return new Integer
                                {
                                    Value = str.Value.Length
                                };
                            }

                            return NewError($"argument to `len` not supported, got {args[0].Type}");
                        }
                    }
                },
                {
                    "first", new Builtin
                    {
                        Fn = args =>
                        {
                            if (args.Length != 1)
                            {
                                return NewError($"wrong number of arguments. got={args.Length}, want=1");
                            }

                            if (args[0] is Array array)
                            {
                                return array.Elements.FirstOrDefault() ?? _null;
                            }

                            return NewError($"argument to `first` must be ARRAY, got {args[0].Type}");
                        }
                    }
                },
                {
                    "last", new Builtin
                    {
                        Fn = args =>
                        {
                            if (args.Length != 1)
                            {
                                return NewError($"wrong number of arguments. got={args.Length}, want=1");
                            }

                            if (args[0] is Array array)
                            {
                                return array.Elements.LastOrDefault() ?? _null;
                            }

                            return NewError($"argument to `last` must be ARRAY, got {args[0].Type}");
                        }
                    }
                },
                {
                    "rest", new Builtin
                    {
                        Fn = args =>
                        {
                            if (args.Length != 1)
                            {
                                return NewError($"wrong number of arguments. got={args.Length}, want=1");
                            }

                            if (args[0] is Array array)
                            {
                                return new Array
                                {
                                    Elements = array.Elements.Skip(1).Select(q => q.Clone()).Cast<IObject>().ToArray()
                                };
                            }

                            return NewError($"argument to `rest` must be ARRAY, got {args[0].Type}");
                        }
                    }
                },
                {
                    "push", new Builtin
                    {
                        Fn = args =>
                        {
                            if (args.Length != 2)
                            {
                                return NewError($"wrong number of arguments. got={args.Length}, want=2");
                            }

                            if (args[0] is Array array)
                            {
                                return new Array
                                {
                                    Elements = array.Elements.Select(q => q.Clone()).Cast<IObject>().Concat(new[] {args[1]}).ToArray()
                                };
                            }

                            return NewError($"argument to `push` must be ARRAY, got {args[0].Type}");
                        }
                    }
                },
                {
                    "puts", new Builtin
                    {
                        Fn = args =>
                        {
                            foreach (var arg in args)
                            {
                                Console.WriteLine(arg.Inspect());
                            }

                            return _null;
                        }
                    }
                }
            };
        }

        public IObject Eval(INode node, Environment env)
        {
            // Statements
            if (node is Program program)
            {
                return EvalProgram(program.Statements, env);
            }
            if (node is ExpressionStatement expression)
            {
                return Eval(expression.Expression, env);
            }
            if (node is BlockStatement block)
            {
                return EvalBlockStatement(block.Statements, env);
            }
            if (node is ReturnStatement rs)
            {
                var val = Eval(rs.ReturnValue, env);
                if (IsError(val))
                {
                    return val;
                }
                return new ReturnValue
                {
                    Value = val
                };
            }
            if (node is LetStatement let)
            {
                var val = Eval(let.Value, env);
                if (IsError(val))
                {
                    return val;
                }
                env.Set(let.Name.Value, val);
            }

            // Expressions
            if (node is IntegerLiteral integer)
            {
                return new Integer
                {
                    Value = integer.Value
                };
            }
            if (node is Ast.Expressions.Boolean boolean)
            {
                return NativeBoolToBooleanObject(boolean.Value);
            }
            if (node is PrefixExpression prefix)
            {
                var right = Eval(prefix.Right, env);
                if (IsError(right))
                {
                    return right;
                }
                return EvalPrefixExpression(prefix.Operator, right);
            }
            if (node is InfixExpression infix)
            {
                var left = Eval(infix.Left, env);
                if (IsError(left))
                {
                    return left;
                }

                var right = Eval(infix.Right, env);
                if (IsError(right))
                {
                    return right;
                }

                return EvalInfixExpression(infix.Operator, left, right);
            }
            if (node is IfExpression ie)
            {
                return EvalIfExpression(ie, env);
            }
            if (node is Identifier identifier)
            {
                return EvalIdentifier(identifier, env);
            }
            if (node is FunctionLiteral function)
            {
                var parameters = function.Parameters;
                var body = function.Body;
                return new Function
                {
                    Parameters = parameters,
                    Env = env,
                    Body = body
                };
            }
            if (node is CallExpression call)
            {
                var fn = Eval(call.Function, env);
                if (IsError(fn))
                {
                    return fn;
                }
                var args = EvalExpressions(call.Arguments, env);
                if (args.Length == 1 && IsError(args[0]))
                {
                    return args[0];
                }

                return ApplyFunction(fn, args);
            }
            if (node is StringLiteral str)
            {
                return new String
                {
                    Value = str.Value
                };
            }
            if (node is ArrayLiteral array)
            {
                var elements = EvalExpressions(array.Elements, env);
                if (elements.Length == 1 && IsError(elements[0]))
                {
                    return elements[0];
                }
                return new Array
                {
                    Elements = elements
                };
            }
            if (node is IndexExpression indexExp)
            {
                var left = Eval(indexExp.Left, env);
                if (IsError(left))
                {
                    return left;
                }
                var index = Eval(indexExp.Index, env);
                if (IsError(index))
                {
                    return index;
                }
                return EvalIndexExpression(left, index);
            }
            if (node is HashLiteral hash)
            {
                return EvalHashLiteral(hash, env);
            }

            return null;
        }

        private IObject EvalProgram(IEnumerable<IStatement> stmts, Environment env)
        {
            IObject result = _null;

            foreach (var statement in stmts)
            {
                result = Eval(statement, env);

                if (result is ReturnValue returnValue)
                {
                    return returnValue.Value;
                }
                if (result is Error)
                {
                    return result;
                }
            }

            return result;
        }

        private IObject EvalBlockStatement(IEnumerable<IStatement> stmts, Environment env)
        {
            IObject result = _null;

            foreach (var statement in stmts)
            {
                result = Eval(statement, env);

                if (result is ReturnValue || result is Error)
                {
                    return result;
                }
            }

            return result;
        }

        private IObject EvalPrefixExpression(string op, IObject right)
        {
            switch (op)
            {
                case "!":
                    return EvalBangOperatorExpression(right);

                case "-":
                    return EvalMinusPrefixOperatorExpression(right);

                default:
                    return NewError($"unknown operator: {op}{right.Type}");
            }
        }

        private IObject EvalInfixExpression(string op, IObject left, IObject right)
        {
            if (left is Integer leftInteger && right is Integer rightInteger)
            {
                return EvalIntegerInfixExpression(op, leftInteger, rightInteger);
            }

            if (left is String leftString && right is String rightString)
            {
                return EvalStringInfixExpression(op, leftString, rightString);
            }

            switch (op)
            {
                case "==":
                    return NativeBoolToBooleanObject(left.Equals(right));

                case "!=":
                    return NativeBoolToBooleanObject(!left.Equals(right));
            }

            if (left.Type != right.Type)
            {
                return NewError($"type mismatch: {left.Type} {op} {right.Type}");
            }

            return NewError($"unknown operator: {left.Type} {op} {right.Type}");
        }

        private IObject EvalIfExpression(IfExpression ie, Environment env)
        {
            var condition = Eval(ie.Condition, env);
            if (IsError(condition))
            {
                return condition;
            }

            if (IsTruthy(condition))
            {
                return Eval(ie.Consequence, env);
            }
            if (ie.Alternative != null)
            {
                return Eval(ie.Alternative, env);
            }
            return _null;
        }

        private IObject EvalIdentifier(Identifier node, Environment env)
        {
            var val = env.Get(node.Value);
            if (val != null)
            {
                return val;
            }

            if (_builtins.ContainsKey(node.Value))
            {
                return _builtins[node.Value];
            }

            return NewError($"identifier not found: {node.Value}");
        }

        private IObject[] EvalExpressions(IEnumerable<IExpression> exps, Environment env)
        {
            var result = new List<IObject>();

            foreach (var e in exps)
            {
                var evaluated = Eval(e, env);
                if (IsError(evaluated))
                {
                    return new[] { evaluated };
                }
                result.Add(evaluated);
            }

            return result.ToArray();
        }

        private IObject EvalBangOperatorExpression(IObject right)
        {
            if (right is Boolean boolean)
            {
                return boolean.Equals(_true) ? _false : _true;
            }
            if (right is Null)
            {
                return _true;
            }
            return _false;
        }

        private IObject EvalMinusPrefixOperatorExpression(IObject right)
        {
            if (right is Integer integer)
            {
                return new Integer
                {
                    Value = -integer.Value
                };
            }

            return NewError($"unknown operator: -{right.Type}");
        }

        private IObject EvalIntegerInfixExpression(string op, Integer left, Integer right)
        {
            var leftVal = left.Value;
            var rightVal = right.Value;

            switch (op)
            {
                case "+":
                    return new Integer
                    {
                        Value = leftVal + rightVal
                    };

                case "-":
                    return new Integer
                    {
                        Value = leftVal - rightVal
                    };

                case "*":
                    return new Integer
                    {
                        Value = leftVal * rightVal
                    };

                case "/":
                    return new Integer
                    {
                        Value = leftVal / rightVal
                    };

                case "<":
                    return NativeBoolToBooleanObject(leftVal < rightVal);

                case ">":
                    return NativeBoolToBooleanObject(leftVal > rightVal);

                case "==":
                    return NativeBoolToBooleanObject(leftVal == rightVal);

                case "!=":
                    return NativeBoolToBooleanObject(leftVal != rightVal);

                default:
                    return NewError($"unknown operator: {left.Type} {op} {right.Type}");
            }
        }

        private IObject EvalStringInfixExpression(string op, String left, String right)
        {
            if (op != "+")
            {
                return NewError($"unknown operator: {left.Type} {op} {right.Type}");
            }

            var leftVal = left.Value;
            var rightVal = right.Value;
            return new String
            {
                Value = leftVal + rightVal
            };
        }

        private IObject ApplyFunction(IObject fn, IEnumerable<IObject> args)
        {
            if (fn is Function function)
            {
                var extendedEnv = ExtendFunctionEnv(function, args.ToArray());
                var evaluated = Eval(function.Body, extendedEnv);
                return UnwrapReturnValue(evaluated);
            }

            if (fn is Builtin builtin)
            {
                return builtin.Fn(args.ToArray());
            }

            return NewError($"not a function: {fn.Type}");
        }

        private IObject EvalIndexExpression(IObject left, IObject index)
        {
            if (left is Array && index is Integer)
            {
                return EvalArrayIndexExpression(left, index);
            }
            if (left is Hash)
            {
                return EvalHashIndexExpression(left, index);
            }

            return NewError($"index operator nto supported: {left.Type}");
        }

        private IObject EvalArrayIndexExpression(IObject array, IObject index)
        {
            var arrayObject = (Array)array;
            var idx = ((Integer)index).Value;
            var max = arrayObject.Elements.Length - 1;

            if (idx < 0 || idx > max)
            {
                return _null;
            }

            return arrayObject.Elements[idx];
        }

        private IObject EvalHashIndexExpression(IObject hash, IObject index)
        {
            var hashObject = (Hash)hash;

            if (!(index is IHashable key))
            {
                return NewError($"unusable as hash key: {index.Type}");
            }

            if (!hashObject.Pairs.ContainsKey(key.HashKey()))
            {
                return _null;
            }

            var pair = hashObject.Pairs[key.HashKey()];
            return pair.Value;
        }

        private IObject EvalHashLiteral(HashLiteral node, Environment env)
        {
            var pairs = new Dictionary<HashKey, HashPair>();

            foreach (var pair in node.Pairs)
            {
                var key = Eval(pair.Key, env);
                if (IsError(key))
                {
                    return key;
                }

                if (!(key is IHashable hashKey))
                {
                    return NewError($"unusable as hash key: {key.Type}");
                }

                var value = Eval(pair.Value, env);
                if (IsError(value))
                {
                    return value;
                }

                var hashed = hashKey.HashKey();
                pairs.Add(hashed, new HashPair
                {
                    Key = key,
                    Value = value
                });
            }

            return new Hash
            {
                Pairs = pairs
            };
        }

        private IObject UnwrapReturnValue(IObject obj)
        {
            if (obj is ReturnValue returnValue)
            {
                return returnValue.Value;
            }

            return obj;
        }

        private Boolean NativeBoolToBooleanObject(bool input)
        {
            return input ? _true : _false;
        }

        private Error NewError(string message)
        {
            return new Error
            {
                Message = message
            };
        }

        private Environment ExtendFunctionEnv(Function fn, IObject[] args)
        {
            var env = Environment.NewEnclosedEnvironment(fn.Env);

            for (var i = 0; i < fn.Parameters.Count; i++)
            {
                env.Set(fn.Parameters[i].Value, args[i]);
            }

            return env;
        }

        private bool IsTruthy(IObject obj)
        {
            if (obj.Equals(_null))
            {
                return false;
            }
            if (obj.Equals(_true))
            {
                return true;
            }
            if (obj.Equals(_false))
            {
                return false;
            }
            return true;
        }

        private bool IsError(IObject obj)
        {
            return obj is Error;
        }
    }
}