using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class AutoMockAutoPropertiesCommand : CustomAutoPropertiesCommand
    {
        public AutoMockAutoPropertiesCommand() : base(new IgnoreProxyMembersSpecification())
        {
        }


        public override void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var mock = AutoMockHelpers.GetFromObj(specimen);
            if (mock == null)
                return;

            base.Execute(mock.GetMocked(), context);
        }

        protected override void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi)
        {
            if (!AutoMockHelpers.IsAutoMockAllowed(pi.PropertyType))
            {
                base.HandleProperty(specimen, context, pi);
                return;
            }

            try
            {
                var newProp = new AutoMockPropertyInfo(pi);
                var propertyValue = context.Resolve(newProp);

                if (propertyValue is NoSpecimen || propertyValue is OmitSpecimen
                    || propertyValue is null || propertyValue is not IAutoMock mock || mock.GetInnerType() != pi.PropertyType)
                {
                    base.HandleProperty(specimen, context, pi);
                    return;
                }

                pi.SetValue(specimen, mock.GetMocked(), null);
            }
            catch
            {

                base.HandleProperty(specimen, context, pi);
            }
        }

        protected override void HandleField(object specimen, ISpecimenContext context, FieldInfo fi)
        {
            if (!AutoMockHelpers.IsAutoMockAllowed(fi.FieldType))
            {
                base.HandleField(specimen, context, fi);
                return;
            }

            try
            {
                var newField = new AutoMockFieldInfo(fi);
                var fieldValue = context.Resolve(newField);

                if (fieldValue is NoSpecimen || fieldValue is OmitSpecimen
                    || fieldValue is null || fieldValue is not IAutoMock mock || mock.GetInnerType() != fi.FieldType)
                {
                    base.HandleField(specimen, context, fi);
                    return;
                }

                fi.SetValue(specimen, mock.GetMocked());
            }
            catch
            {

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
