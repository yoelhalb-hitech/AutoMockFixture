using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.MockUtils;
using System;
using System.Linq;


namespace AutoMoqExtensions.FixtureUtils.Commands
{
    // TODO... we need to handle internal methods
    internal class AutoMockVirtualMethodsCommand : ISpecimenCommand
    {
        public void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                var mock = AutoMockHelpers.GetFromObj(specimen);
                if (mock is null) return;

                var setupService = new MockSetupService(mock, context);
                setupService.Setup();
            }
            catch { }
        }
    }
}
