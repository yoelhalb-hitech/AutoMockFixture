using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.MockUtils
{
    internal class MethodSetupServiceFactory
    {
        private readonly Func<MethodSetupTypes> setupTypeFunc;

        public MethodSetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc)
        {
            this.setupTypeFunc = setupTypeFunc;
        }

        public MethodSetupServiceBase GetMethodSetup(IAutoMock mock, MethodInfo method,
            ISpecimenContext context, string? customTrackingPath = null)
        {
            return GetService(setupTypeFunc(), mock, method, context, customTrackingPath);           
        }

        public MethodSetupServiceBase GetPropertySetup(IAutoMock mock, MethodInfo method,
                                                        ISpecimenContext context, string? customTrackingPath = null)
        {
            var setupType = setupTypeFunc();
            // For properties we always use same
            if (setupType == MethodSetupTypes.LazyDifferent) setupType = MethodSetupTypes.LazySame;

            return GetService(setupType, mock, method, context, customTrackingPath);
        }

        private MethodSetupServiceBase GetService(MethodSetupTypes setupType,
            IAutoMock mock, MethodInfo method, ISpecimenContext context, string? customTrackingPath)
        {
            switch (setupType)
            {
                case MethodSetupTypes.Eager:
                    return new MethodEagerSetupService(mock, method, context, customTrackingPath);
                case MethodSetupTypes.LazySame:
                    return new MethodSetupServiceWithSameResult(mock, method, context, customTrackingPath);
                case MethodSetupTypes.LazyDifferent:
                    return new MethodSetupServiceWithDifferentResult(mock, method, context, customTrackingPath);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
