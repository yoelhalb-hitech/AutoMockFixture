using AutoMoqExtensions.AutoMockUtils;
using Moq;
using System.Linq.Expressions;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class AutoMockStubAllPropertiesCommand : ISpecimenCommand
{
    public void Execute(object specimen, ISpecimenContext context)
    {
        var mock = AutoMockHelpers.GetFromObj(specimen);
        if (mock is null) return;

        // Disable generation of default values (if enabled), otherwise SetupAllProperties will hang if there's a circular dependency
        var mockDefaultValueSetting = mock.DefaultValue;
        mock.DefaultValue = DefaultValue.Empty;

        try
        {
            if (!mock.CallBase)
            {
                // stub properties
                mock.GetType()
                    .GetMethod(nameof(Mock<object>.SetupAllProperties))
                    .Invoke(mock, new object[0]);
            }
            else
            {
                // If callbase it will already have property behavior from the parent
                // and we don't want to setup as it will destroy any existing values from the ctor since the object has already been created
                // and also it might prevent to call the base

                // But for properties that are abstract in the base or interface we need to enable it
                // Note that from c# and on we can have implementations in an interface as well...
                // Also we base the interface on the mock type in case Moq decides to add a base class...

                var baseType = AutoMockHelpers.GetMockedType(mock.GetType())!;

                if (!baseType.IsAbstract && !baseType.IsInterface) return;

                var method = mock.GetType()
                        .GetMethods()
                        .First(m => m.Name == nameof(Mock<object>.SetupProperty) && m.GetParameters().Length == 1);

                // Note we do it also on protected props since the base code might be dependent on it
                foreach (var prop in baseType.GetAllProperties().Where(p => p.IsOverridable() && p.HasGetAndSet(true)))
                {
                    // Only if any if the methods isn't implemented
                    // (note that if it's private it can't be abstract, so we don't have to go to the declaring prop)
                    if (prop.GetMethod.IsAbstract && prop.SetMethod.IsAbstract)
                    {
                        var paramExpression = Expression.Parameter(baseType);
                        var expression = Expression.Lambda(Expression.Property(paramExpression, prop.Name), paramExpression);

                        method.MakeGenericMethod(prop.PropertyType).Invoke(mock, new[] { expression });
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
