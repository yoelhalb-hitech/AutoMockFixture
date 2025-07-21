using AutoMockFixture.Moq4.AutoMockUtils;
using SequelPay.DotNetPowerExtensions.Reflection;
using Moq.Protected;
using System.Security.AccessControl;

namespace AutoMockFixture.Moq4.FixtureUtils.Commands;

internal class AutoMockStubAllPropertiesCommand : ISpecimenCommand
{
    public AutoMockStubAllPropertiesCommand(AutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public AutoMockHelpers AutoMockHelpers { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        var m = AutoMockHelpers.GetFromObj(specimen);
        if (m is null || m is not Mock mock) return;

        // Disable generation of default values (if enabled), otherwise SetupAllProperties will hang if there's a circular dependency
        var mockDefaultValueSetting = mock.DefaultValue;
        mock.DefaultValue = DefaultValue.Empty;

        try
        {
            if (!mock.CallBase)
            {
                // Stub properties
                // Note that this will not setup explicit/default interface implementations, but they have to be setup on creation anyway so they will be setup in the init command
                mock.GetType()
                    .GetMethod(nameof(Mock<object>.SetupAllProperties))!
                    .Invoke(mock, new object[0]);
            }
            else
            {
                // If callBase it will already have property behavior from the parent
                // and we don't want to setup as it will destroy any existing values from the ctor since the object has already been created
                // and also it might prevent to call the base

                // But for properties that are abstract in the base or interface we need to enable it
                // Note that from c# 8 and on we can have implementations in an interface as well...
                // Also we base the interface on the mock type in case Moq decides to add a base class...

                var baseType = AutoMockHelpers.GetMockedType(mock.GetType())!;

                if (!baseType.IsAbstract && !baseType.IsInterface) return;

                var method = mock.GetType()
                        .GetMethods()
                        .First(m => m.Name == nameof(Mock<object>.SetupProperty) && m.GetParameters().Length == 1);

                var details = baseType.GetTypeDetailInfo();

                // Note we do it also on protected props since the base code might be dependent on it
                // We don't need to do it on set only as it anyway doesn't have to retrieve it and since it doesn't have an implementation there is no point in it
                foreach (var propInfo in details.PropertyDetails.Concat(details.ShadowedPropertyDetails)
                                                .Where(p => p.ReflectionInfo.IsOverridable() && p.ReflectionInfo.HasGetAndSet(true)))
                {
                    // Only if any if the methods isn't implemented
                    // (note that if it's private it can't be abstract, so we don't have to go to the declaring prop)
                    if (propInfo.ReflectionInfo.IsAbstract())
                    {
                        var paramExpression = Expression.Parameter(baseType);
                        var propExpression = Expression.Property(paramExpression, (propInfo.ReflectionInfo.Name));
                        var expression = Expression.Lambda(propExpression, paramExpression);

                        method.MakeGenericMethod(propInfo.ReflectionInfo.PropertyType).Invoke(mock, new[] { expression });
                    }
                }
            }
        }
        finally
        {
            // restore setting
            mock.DefaultValue = mockDefaultValueSetting;
        }
    }
}
