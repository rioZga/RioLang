using Antlr4.Runtime.Misc;
using RioLang.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RioLang
{
    public class RioVisitor : RioBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; } = new();

        public RioVisitor()
        {
            Variables["Write"] = new Func<object?[], object?>(Write);
        }

        private object? Write(object?[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            return null;
        }

        public override object? VisitFunctionCall([NotNull] RioParser.FunctionCallContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var args = context.expression().Select(e => Visit(e)).ToArray();

            if (!Variables.ContainsKey(name))
                throw new Exception($"Function {name} is not defined");

            if (Variables[name] is not Func<object?[], object?> func)
                throw new Exception($"Variable {name} is not a function");

            return func(args);

        }

        public override object? VisitAssignement([NotNull] RioParser.AssignementContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            var value = Visit(context.expression());
            Variables[varName] = value;

            return null;
        }

        public override object? VisitConstant([NotNull] RioParser.ConstantContext context)
        {
            if (context.INTEGER() is { } i)
                return int.Parse(i.GetText());

            if (context.FLOAT() is { } f)
                return float.Parse(f.GetText());

            if (context.STRING() is { } s)
                return s.GetText()[1..^1];

            if (context.BOOL() is { } b)
                return b.GetText() == "true";

            return null;
        }

        public override object? VisitIdentifierExpression([NotNull] RioParser.IdentifierExpressionContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            if (!Variables.ContainsKey(varName))
            {
                throw new Exception($"Variable {varName} is not defined.");
            }

            return Variables[varName];
        }

        public override object? VisitAdditiveExpression([NotNull] RioParser.AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.addOp().GetText();

            return op switch
            {
                "+" => Add(left, right),
                "-" => Subtract(left, right),
                _ => throw new NotImplementedException()
            };

        }

        private object? Add(object? left, object? right)
        {
            if (left is int l && right is int r)
                return l + r;

            if (left is float lf && right is float rf)
                return lf + rf;

            if (left is int lInt && right is float rFloat)
                return lInt + rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat + rInt;

            if (left is string || right is string)
                return $"{left}{right}";

            throw new Exception($"Cannot add values.");
        }

        private object? Subtract(object? left, object? right)
        {
            if (left is int l && right is int r)
                return l - r;

            if (left is float lf && right is float rf)
                return lf - rf;

            if (left is int lInt && right is float rFloat)
                return lInt - rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat - rInt;

            throw new Exception($"Cannot add values.");
        }

        public override object? VisitWhileBlock([NotNull] RioParser.WhileBlockContext context)
        {
            if (IsTrue(Visit(context.expression())))
            {
                do
                {
                    Visit(context.block());
                } while (IsTrue(Visit(context.expression())));
            }
            else
            {
                Visit(context.elseIfBlock());
            }

            return null;
        }

        public override object VisitComparisonExpression([NotNull] RioParser.ComparisonExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.compareOp().GetText();

            return op switch
            {
                "<" => LessThan(left, right),
                _ => throw new Exception("Cannot compare values")
            };
        }

        private bool IsTrue(object? value)
        {
            if (value is bool b)
                return b;

            throw new Exception("Value is not boolean.");
        }

        private bool LessThan(object? left, object? right)
        {
            if (left is int l && right is int r)
                return l < r;

            if (left is float lf && right is float rf)
                return lf < rf;

            if (left is int lInt && right is float rFloat)
                return lInt < rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat < rInt;

            throw new Exception("Cannot compare values");
        }
    }
}
