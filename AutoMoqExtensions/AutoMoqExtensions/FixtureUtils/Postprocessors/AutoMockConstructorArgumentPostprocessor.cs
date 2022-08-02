using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Customizations;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;
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
            if (request is not ConstructorArgumentRequest ctorArgsRequest) return new NoSpecimen();

            var type = ctorArgsRequest.ParameterInfo.ParameterType;

            // Caution: Cannot just use FirstOrDefault and check for nullability as the custom value itself can be null
            var hasCustomValue = ConstructorArgumentValues.Any(v => IsValidArgumentValue(type, v, ctorArgsRequest.Path));
            if(hasCustomValue)
            {
                var customValue = ConstructorArgumentValues.First(v => IsValidArgumentValue(type, v, ctorArgsRequest.Path));
                ctorArgsRequest.SetResult(customValue);
                return customValue;
            }

            if (!autoMockableSpecification.IsSatisfiedBy(type) || !ctorArgsRequest.ShouldAutoMock)
            {
                object newRequest = ctorArgsRequest.IsInAutoMockChain || ctorArgsRequest.IsInAutoMockDepnedencyChain
                                        ? new AutoMockDependenciesRequest(type, ctorArgsRequest)
                                        : new NonAutoMockRequest(type, ctorArgsRequest);
                var result = context.Resolve(newRequest);
                ctorArgsRequest.SetResult(result);
                return result;
            }

            var specimen = context.Resolve(new AutoMockRequest(type, ctorArgsRequest));

            if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            {
                ctorArgsRequest.SetResult(specimen);
                return specimen;
            }

            ctorArgsRequest.SetCompleted();
            return specimen;
        }

        private bool IsValidArgumentValue(Type type, ConstructorArgumentValue argumentValue, string path)
        {
            if (argumentValue.Path is not null && argumentValue.Path != path) return false;

            if (argumentValue.Value is not null) return type.IsInstanceOfType(argumentValue.Value) 
                    || (argumentValue.Value is IAutoMock mock && type.IsInstanceOfType(mock.GetMocked()));

            return type.IsNullAllowed();
        }
    }
}
