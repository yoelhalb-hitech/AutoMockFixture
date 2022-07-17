using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal partial class AutoMockConstructorQuery : IMethodQuery
    {
        private static readonly DelegateSpecification DelegateSpecification = new DelegateSpecification();
        
        public IEnumerable<IMethod> SelectMethods(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!AutoMockHelpers.IsAutoMock(type)) return Enumerable.Empty<IMethod>();

            var mockType = AutoMockHelpers.GetMockedType(type);
            if(mockType is null) return Enumerable.Empty<IMethod>();

            if (mockType.GetTypeInfo().IsInterface || DelegateSpecification.IsSatisfiedBy(mockType))
            {
                return new[] { new ConstructorMethod(type.GetDefaultConstructor()) };
            }

            return from ci in mockType.GetPublicAndProtectedConstructors()
                   let paramInfos = ci.GetParameters()
                   orderby paramInfos.Length ascending
                   select new CustomConstructorMethod(ci) as IMethod;
        }
    }
}
