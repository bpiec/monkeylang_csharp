using Monkey.Ast.Expressions;
using Monkey.Ast.Statements;
using System.Collections.Generic;

namespace Monkey.Ast
{
    public static class Modifier
    {
        public delegate INode ModifierFunc(INode node);

        public static INode Modify(INode node, ModifierFunc modifier)
        {
            if (node is Program program)
            {
                for (var i = 0; i < program.Statements.Count; i++)
                {
                    var statement = program.Statements[i];
                    program.Statements[i] = Modify(statement, modifier) as IStatement;
                }
            }
            else if (node is ExpressionStatement expression)
            {
                expression.Expression = Modify(expression.Expression, modifier) as IExpression;
            }
            else if (node is InfixExpression infix)
            {
                infix.Left = Modify(infix.Left, modifier) as IExpression;
                infix.Right = Modify(infix.Right, modifier) as IExpression;
            }
            else if (node is PrefixExpression prefix)
            {
                prefix.Right = Modify(prefix.Right, modifier) as IExpression;
            }
            else if (node is IndexExpression index)
            {
                index.Left = Modify(index.Left, modifier) as IExpression;
                index.Index = Modify(index.Index, modifier) as IExpression;
            }
            else if (node is IfExpression ife)
            {
                ife.Condition = Modify(ife.Condition, modifier) as IExpression;
                ife.Consequence = Modify(ife.Consequence, modifier) as BlockStatement;
                if (ife.Alternative != null)
                {
                    ife.Alternative = Modify(ife.Alternative, modifier) as BlockStatement;
                }
            }
            else if (node is BlockStatement block)
            {
                for (var i = 0; i < block.Statements.Count; i++)
                {
                    var statement = block.Statements[i];
                    block.Statements[i] = Modify(statement, modifier) as IStatement;
                }
            }
            else if (node is ReturnStatement ret)
            {
                ret.ReturnValue = Modify(ret.ReturnValue, modifier) as IExpression;
            }
            else if (node is LetStatement let)
            {
                let.Value = Modify(let.Value, modifier) as IExpression;
            }
            else if (node is FunctionLiteral function)
            {
                for (var i = 0; i < function.Parameters.Count; i++)
                {
                    var statement = function.Parameters[i];
                    function.Parameters[i] = Modify(statement, modifier) as Identifier;
                }
                function.Body = Modify(function.Body, modifier) as BlockStatement;
            }
            else if (node is ArrayLiteral array)
            {
                for (var i = 0; i < array.Elements.Count; i++)
                {
                    var statement = array.Elements[i];
                    array.Elements[i] = Modify(statement, modifier) as IExpression;
                }
            }
            else if (node is HashLiteral hash)
            {
                var newPairs = new Dictionary<IExpression, IExpression>();
                foreach (var pair in hash.Pairs)
                {
                    var newKey = Modify(pair.Key, modifier) as IExpression;
                    var newVal = Modify(pair.Value, modifier) as IExpression;
                    newPairs.Add(newKey, newVal);
                }
                hash.Pairs = newPairs;
            }

            return modifier(node);
        }
    }
}