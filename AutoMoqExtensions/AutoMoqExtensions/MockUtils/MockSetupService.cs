﻿using AutoFixture.Kernel;
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
        private readonly ITracker? tracker;

        public MockSetupService(IAutoMock mock, ISpecimenContext context)
        {
            this.mock = mock;
            this.context = context;
            this.mockedType = mock.GetInnerType();
            this.tracker = mock.Tracker;
        }

        public void Setup()
        {
            var allProperties = mockedType.GetAllProperties();

            var propertiesWithSetAndGet = allProperties.Where(p => p.HasGetAndSet());
            foreach (var prop in propertiesWithSetAndGet) // We setup here all virtual properties in case the constructor needs them
            {
                SetupAutoProperty(prop);
            }

            var singleMethodProperties = allProperties.Where(p => !p.HasGetAndSet());
            foreach (var prop in singleMethodProperties)
            {
                SetupSingleMethodProperty(prop);
            }

            var methods = GetMethods();
            foreach (var method in methods)
            {
                SetupMethod(method);
            }
        }

        private void Setup(MemberInfo member, Action action)
        {
            var method = member as MethodInfo;
            var prop = member as PropertyInfo;

            var trackingPath = method?.GetTrackingPath() ?? prop!.GetTrackingPath();
            var methods = prop?.GetMethods() ?? new[] { method! };

            if (mock.CallBase && !mock.GetInnerType().IsInterface && !methods.Any(m => m.IsAbstract))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.CallBaseNoAbstract);
                return;
            }

            var configureInfo = CanBeConfigured(methods);
            if (!configureInfo.CanConfigure)
            {
                HandleCannotSetup(trackingPath, configureInfo.Reason!.Value);
                return;
            }

            try
            {
                action();                
                mock.MethodsSetup.Add(trackingPath, member);                               
            }
            catch (Exception ex)
            {
                mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));
            }
        }

        private void SetupMethod(MethodInfo method)
        {
            Setup(method, () => new MethodSetupService(mock, mockedType, method, context).Setup());          
        }

        private void SetupSingleMethodProperty(PropertyInfo prop)
        {
            var method = prop.GetMethods().First();

            Setup(prop, () => new MethodSetupService(mock, mockedType, method, context).Setup());
        }

        private void SetupAutoProperty(PropertyInfo prop)
        {
            Setup(prop, () =>
            {
                var propValue = context.Resolve(new AutoMockPropertyRequest(mockedType, prop, tracker));
                SetupHelpers.SetupAutoProperty(mockedType, prop.PropertyType, mock, prop, propValue);
            });
        }

        private IEnumerable<MethodInfo> GetMethods()
        {
            // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
            var methods = delegateSpecification.IsSatisfiedBy(mockedType)
                ? new[] { mockedType.GetTypeInfo().GetMethod("Invoke") }
                : mockedType.GetAllMethods();

            var propMethods = mockedType.GetAllProperties().SelectMany(p => p.GetMethods());
            return methods.Except(propMethods);
        }

        private void HandleCannotSetup(string trackingPath, CannotSetupReason reason) 
            => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

        private (bool CanConfigure, CannotSetupReason? Reason) CanBeConfigured(MethodInfo[] methods)
        {
            if (!mockedType.IsInterface && methods.Any(m => !m.IsOverridable())) return (false, CannotSetupReason.NonVirtual);

            if (methods.Any(m => m.IsPrivate)) return (false, CannotSetupReason.Private);           

            if (methods.Any(m => !m.IsPublicOrInternal())) return (false, CannotSetupReason.Protected);            

            return (true, null);
        }
    }
}