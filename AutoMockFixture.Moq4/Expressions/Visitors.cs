using DotNetPowerExtensions.Reflection;

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
        IReadOnlyCollection<Expression> args;
        public ArgsVisitor(IReadOnlyCollection<Expression> args)
        {
            this.args = args;
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
