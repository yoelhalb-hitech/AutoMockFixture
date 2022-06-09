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

        /// <summary>
        /// Selects constructors for the supplied <see cref="Moq.Mock{T}"/> type.
        /// </summary>
        /// <param name="type">The mock type.</param>
        /// <returns>
        /// Constructors for <paramref name="type"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method only returns constructors if <paramref name="type"/> is a
        /// <see cref="Moq.Mock{T}"/> type. If not, an empty sequence is returned.
        /// </para>
        /// <para>
        /// If the type is the type of a constructed <see cref="AutoMock{T}"/>, constructors are
        /// returned according to the generic type argument's constructors. If the type is an
        /// interface, the <see cref="AutoMock{T}()"/> default constructor is returned. If the type
        /// is a class, constructors are returned according to all the public and protected
        /// constructors of the underlying type. In this case, the
        /// <see cref="Moq.Mock{T}(object[])"/> constructor that takes a params array is returned
        /// for each underlying constructor, with information about the appropriate parameters for
        /// each constructor.
        /// </para>
        /// </remarks>
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
                   select new AutoMockConstructorMethod(type.GetParamsConstructor(), paramInfos) as IMethod;
        }
    }
}
