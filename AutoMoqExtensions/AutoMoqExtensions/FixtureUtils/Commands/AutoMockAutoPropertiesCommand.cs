using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class AutoMockAutoPropertiesCommand : ISpecimenCommand
    {
        public virtual void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var mock = AutoMockHelpers.GetFromObj(specimen);
            if (mock == null) return;

            new AutoMockAutoPropertiesInternalCommand(mock).Execute(mock.GetMocked(), context);
        }
        private class AutoMockAutoPropertiesInternalCommand : CustomAutoPropertiesCommand
        {
            public IAutoMock Mock { get; }

            public AutoMockAutoPropertiesInternalCommand(IAutoMock mock) 
                    : base(new IgnoreProxyMembersSpecification(), mock.Fixture)
            {
                if (mock == null) throw new ArgumentNullException(nameof(mock));
                Mock = mock;
            }

            public override void Execute(object _, ISpecimenContext context)
            {
                if (context == null) throw new ArgumentNullException(nameof(context));

                Logger.LogInfo("In catch");
                base.Execute(Mock.GetMocked(), context);
            }

            protected override void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi)
            {
                try
                {
                    Logger.LogInfo("Before Resolved ");
                    var request = new AutoMockPropertyRequest(GetSpecimenType(specimen), pi, Mock.Tracker);
                    var propertyValue = context.Resolve(request);

                    Logger.LogInfo("Resolved property: " + propertyValue.GetType().Name);

                    if (propertyValue is NoSpecimen || propertyValue is OmitSpecimen) { return; }
                    else if (propertyValue is null || propertyValue is not IAutoMock mock)
                    {
                        pi.SetValue(specimen, propertyValue, null);
                        return;
                    }
                    else if (mock.GetInnerType() == pi.PropertyType) pi.SetValue(specimen, mock.GetMocked(), null);
                }
                catch
                {
                    Logger.LogInfo("In catch");
                    base.HandleProperty(specimen, context, pi);
                }
            }

            protected override void HandleField(object specimen, ISpecimenContext context, FieldInfo fi)
            {
                try
                {
                    Logger.LogInfo("Before field Resolved ");
                    var request = new AutoMockFieldRequest(GetSpecimenType(specimen), fi, Mock.Tracker);
                    var fieldValue = context.Resolve(request);
                    Logger.LogInfo("Resolved field: " + fieldValue.GetType().Name);

                    if (fieldValue is NoSpecimen || fieldValue is OmitSpecimen) { return; }
                    else if (fieldValue is null || fieldValue is not IAutoMock mock)
                    {
                        fi.SetValue(specimen, fieldValue);
                        return;
                    }
                    else if (mock.GetInnerType() == fi.FieldType) fi.SetValue(specimen, mock.GetMocked());
                }
                catch
                {
                    Logger.LogInfo("In field catch");
                    base.HandleField(specimen, context, fi);
                }
            }

            private class IgnoreProxyMembersSpecification : IRequestSpecification
            {
                public bool IsSatisfiedBy(object request)
                {
                    switch (request)
                    {
                        case FieldInfo fi:
                            return !IsProxyMember(fi);

                        case PropertyInfo _:
                            return true;

                        default:
                            return false;
                    }
                }

                private static bool IsProxyMember(FieldInfo fi)
                {
                    return string.Equals(fi.Name, "__interceptors", StringComparison.Ordinal) ||
                           string.Equals(fi.Name, "__target", StringComparison.Ordinal);
                }
            }
        }
    }
}
