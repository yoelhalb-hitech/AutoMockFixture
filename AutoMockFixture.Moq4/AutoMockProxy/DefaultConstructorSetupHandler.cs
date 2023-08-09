using AutoFixture.AutoMoq;
using Moq.Protected;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class DefaultConstructorSetupHandler : IProxyTypeSetupHandler
{
    public void Setup(Type originalType, AutoMock<TypeInfo> mock)
    {
        var publicInstanceBinding = BindingFlags.Instance | BindingFlags.Public;

        var originalDefaultCtor = originalType.GetConstructor(publicInstanceBinding, null, new Type[] { }, null);

        var originalPublicInstanceCtors = originalType.GetConstructors(publicInstanceBinding);

        if (originalDefaultCtor is not null) originalPublicInstanceCtors = originalPublicInstanceCtors
                                                                                        .Except(new ConstructorInfo[] { originalDefaultCtor })
                                                                                        .ToArray();


        var emptyCtor = DefaultConstructorService.GetDefaultConstructor();

        Expression<Func<BindingFlags, bool>> bindingExpr = f => (f & (BindingFlags.Public | BindingFlags.Instance)) == publicInstanceBinding;

        mock.Setup(t => t.GetConstructors(It.Is(bindingExpr)))
                                .Returns(originalPublicInstanceCtors.Union(new[] { emptyCtor }).ToArray());

        mock.Protected()
            .Setup<ConstructorInfo>("GetConstructorImpl",
                    new object[] { ItExpr.Is(bindingExpr), ItExpr.IsAny<Binder>(),
                                        ItExpr.IsAny<CallingConventions>(), ItExpr.Is<Type[]>(t => !t.Any()),
                                        ItExpr.IsAny<ParameterModifier[]>() })
            .Returns(() => emptyCtor);
    }
}
