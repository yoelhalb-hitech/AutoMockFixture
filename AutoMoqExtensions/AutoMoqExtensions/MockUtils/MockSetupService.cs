using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AutoMoqExtensions.MockUtils.CannotSetupMethodException;

namespace AutoMoqExtensions.MockUtils
{
    internal class MockSetupService
    {
        private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

        private readonly IAutoMock mock;
        private readonly ISpecimenContext context;
        private readonly Type mockedType;

        public MockSetupService(IAutoMock mock, ISpecimenContext context)
        {
            this.mock = mock;
            this.context = context;
            this.mockedType = mock.GetInnerType();
        }

        public void Setup()
        {
            var tracker = mock.Tracker;
            var properties = GetConfigurableAutoProperties();
            foreach (var prop in properties) // We setup here all virtual properties in case the constructor needs them
            {
                try
                {
                    var propValue = context.Resolve(new AutoMockPropertyRequest(mockedType, prop, tracker));
                    SetupHelpers.SetupAutoProperty(mockedType, prop.PropertyType, mock, prop, propValue);

                    mock.MethodsSetup.Add(prop.GetTrackingPath(), prop);
                }
                catch (Exception ex)
                {
                    HandleCannotSetup(prop.GetTrackingPath(), ex);
                }
            }

            var singleMethodProperties = GetConfigurableSingleMethodProperties();
            foreach (var prop in singleMethodProperties)
            {
                try
                {
                    var method = prop.GetAccessors(true).First();

                    new MethodSetupService(mock, mockedType, method, context).Setup();
                    mock.MethodsSetup.Add(prop.GetTrackingPath(), method);
                }
                catch (Exception ex)
                {
                    HandleCannotSetup(prop.GetTrackingPath(), ex);
                }
            }

            var methods = GetConfigurableMethods();
            foreach (var method in methods)
            {
                try
                {
                    new MethodSetupService(mock, mockedType, method, context).Setup();
                    mock.MethodsSetup.Add(method.GetTrackingPath(), method);
                }
                catch (Exception ex)
                {
                    HandleCannotSetup(method.GetTrackingPath(), ex);
                }
            }
        }

        private IEnumerable<MethodInfo> GetConfigurableMethods()
        {
            // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
            var methods = delegateSpecification.IsSatisfiedBy(mockedType)
                ? new[] { mockedType.GetTypeInfo().GetMethod("Invoke") }
                : mockedType.GetAllMethods();

            var propMethods = mockedType.GetAllProperties().SelectMany(p => p.GetAccessors(true));
            methods = methods.Except(propMethods);

            return methods.Where(m => CanBeConfigured(m));
        }

        private IEnumerable<PropertyInfo> GetConfigurableAutoProperties() 
            => mockedType.GetAllProperties().Where(p => p.HasGetAndSet()).Where(p => CanBeConfigured(p));

        private IEnumerable<PropertyInfo> GetConfigurableSingleMethodProperties()
            => mockedType.GetAllProperties().Where(p => !p.HasGetAndSet()).Where(m => CanBeConfigured(m));

        private bool CanBeConfigured(PropertyInfo property)
              => CanBeConfiguredInternal(property.GetAccessors(true), property.GetTrackingPath());        

        private bool CanBeConfigured(MethodInfo method)
            => CanBeConfiguredInternal(new[] { method }, method.GetTrackingPath());

        private void HandleCannotSetup(string trackingPath, CannotSetupReason reason) 
            => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

        private void HandleCannotSetup(string trackingPath, Exception ex)
            => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));


        private bool CanBeConfiguredInternal(MethodInfo[] methods, string trackingPath)
        {
            if (!mockedType.IsInterface && methods.Any(m => !m.IsOverridable()))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.NonVirtual);
                return false;
            }

            if (methods.Any(m => m.IsPrivate))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.Private);
                return false;
            }

            if (methods.Any(m => !m.IsPublicOrInternal()))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.Protected);
                return false;
            }

            return true;
        }
    }
}
