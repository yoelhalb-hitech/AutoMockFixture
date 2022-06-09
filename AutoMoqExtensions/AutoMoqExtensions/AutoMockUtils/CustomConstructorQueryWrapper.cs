using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal class CustomConstructorQueryWrapper : IMethodQuery
    {
        public CustomConstructorQueryWrapper(IMethodQuery query)
        {
            Query = query;
        }

        public IMethodQuery Query { get; }

        public IEnumerable<IMethod> SelectMethods(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var result = this.Query.SelectMethods(type);

            return result.Select(r => new AutoMockConstructorWrapper(r));
        }
    }
}
