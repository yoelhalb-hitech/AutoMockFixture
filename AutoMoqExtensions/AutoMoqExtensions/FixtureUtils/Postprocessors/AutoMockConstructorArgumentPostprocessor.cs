using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Customizations;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class AutoMockConstructorArgumentPostprocessor : ISpecimenBuilder
    {
        public AutoMockConstructorArgumentPostprocessor(List<ConstructorArgumentValue> constructorArgumentValues)
        {
            ConstructorArgumentValues = constructorArgumentValues;
        }

        private static readonly AutoMockableSpecification autoMockableSpecification = new();

        public List<ConstructorArgumentValue> ConstructorArgumentValues { get; }

        public object? Create(object request, ISpecimenContext context)
        {
            if (request is not AutoMockConstructorArgumentRequest mockRequest) return new NoSpecimen();

            var type = mockRequest.ParameterInfo.ParameterType;

            // Caution: Cannot just use FirstOrDefault and check for nullability as the custom value itself can be null
            var hasCustomValue = ConstructorArgumentValues.Any(v => IsValidArgument(type, v, mockRequest.Path));
            if(hasCustomValue)
            {
                var customValue = ConstructorArgumentValues.Any(v => IsValidArgument(type, v, mockRequest.Path));
                mockRequest.SetResult(customValue);
                return customValue;
            }

            if (!autoMockableSpecification.IsSatisfiedBy(type))
            {
                var result = context.Resolve(mockRequest.ParameterInfo);
                mockRequest.SetResult(result);
                return result;
            }

            var specimen = context.Resolve(new AutoMockRequest(type, mockRequest));

            if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            {
                mockRequest.SetResult(specimen);
                return specimen;
            }                

            mockRequest.SetCompleted();
            return specimen;
        }

        private bool IsValidArgument(Type type, ConstructorArgumentValue argumentValue, string path)
        {
            if (argumentValue.Path is not null && argumentValue.Path != path) return false;

            if (argumentValue.Value is not null) return type.IsInstanceOfType(argumentValue.Value);

            return type.IsNullAllowed();
        }
    }
}
