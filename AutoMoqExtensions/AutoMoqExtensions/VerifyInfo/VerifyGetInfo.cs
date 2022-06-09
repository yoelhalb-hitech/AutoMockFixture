using Moq;
using System;
using System.Linq.Expressions;

namespace AutoMoqExtensions
{
    public class VerifyGetInfo<T, TProperty> : IVerifyInfo<T> where T : class
    {
        private readonly Expression<Func<T, TProperty>> expression;
        private readonly Times times;

        public VerifyGetInfo(Expression<Func<T, TProperty>> expression, Times times)
        {
            this.expression = expression;
            this.times = times;
        }

        public void Verify(Mock<T> obj)
        {
            obj.VerifyGet(expression, times);
        }
    }
}
