using Moq;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Moq4.Expressions;

internal class Visitors
{
    public class TopVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Any())
            {
                return new ArgsVisitor(node.Arguments).Visit(node);
            }
            return base.VisitMethodCall(node);
        }
    }

    class ArgsVisitor : ExpressionVisitor
    {
        List<Expression> args;
        public ArgsVisitor(IReadOnlyCollection<Expression> args)
        {
            this.args = args.ToList();
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (args.Contains(node) && node.NodeType is ExpressionType.Convert
                && node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var inner = node.Operand;
                while (inner is UnaryExpression{ NodeType: ExpressionType.Convert }) inner = ((UnaryExpression)inner).Operand;

                var constExpr = inner as ConstantExpression;
                if (constExpr is not null)
                {
                    var defaultValue = constExpr.Type.GetDefault();
                    // Rememeber that for boxed structs we cannot use == only .Equals
                    if ((constExpr.Value is null && defaultValue is null) || (constExpr.Value is not null && defaultValue is not null && constExpr.Value.Equals(defaultValue)))
                    {
                        // We need to use the outer type for it to work correcly
                        return Expression.Call(typeof(Moq.It).GetMethod(nameof(Moq.It.IsAny))!.MakeGenericMethod(node.Type)!);
                    }
                }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (args.Contains(node))
            {
                var defaultValue = node.Type.GetDefault();

                // Rememeber that for boxed structs we cannot use == only .Equals
                if ((node.Value is null && defaultValue is null) || (node.Value is not null && defaultValue is not null && node.Value.Equals(defaultValue)))
                {
                    return Expression.Call(typeof(Moq.It).GetMethod(nameof(Moq.It.IsAny))!.MakeGenericMethod(node.Type)!);
                }
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            if (args.Contains(node))
            {
                return Expression.Call(typeof(Moq.It).GetMethod(nameof(Moq.It.IsAny))!.MakeGenericMethod(node.Type)!);
            }
            return base.VisitDefault(node);
        }
    }
}
