using AutoFixture.Kernel;
using Castle.DynamicProxy;
using Moq;

namespace AutoMockFixture.Moq4.FixtureUtils.Commands;

internal class AutoMockInitCommand : ISpecimenCommand
{
    public void Execute(object specimen, ISpecimenContext context)
    {
        if (specimen is null || specimen is not IAutoMock mock || specimen is not Mock m) return;

        m.DefaultValue = DefaultValue.Empty; // When we want a value we will set it up ourselves with AutoMock

        var asMethod = typeof(Mock).GetMethod(nameof(Mock.As));
        foreach (var iface in mock.GetInnerType().GetInterfaces())
        {
            try
            {
                if (!ProxyUtil.IsAccessible(iface)) continue; // Otherwise it will prevent it from creating the mocked object later

                asMethod.MakeGenericMethod(iface).Invoke(mock, new Type[] { }); // This has to be done before creating the mocked object, otherwise it won't work
            }
            catch { } // TODO...
        }
    }
}
