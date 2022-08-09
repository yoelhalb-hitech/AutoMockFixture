using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class AutoMockAutoPropertiesCommand : CustomAutoPropertiesCommand
{
    public AutoMockAutoPropertiesCommand(AutoMockFixture fixture) : base(fixture) { }
    public AutoMockAutoPropertiesCommand(IRequestSpecification specification, AutoMockFixture fixture) : base(specification, fixture) { }


    protected override void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi, ITracker tracker)
    {
        try
        {
            Logger.LogInfo("Before Resolved ");
            var request = new AutoMockPropertyRequest(GetSpecimenType(specimen), pi,tracker);
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
            base.HandleProperty(specimen, context, pi, tracker);
        }
    }

    protected override void HandleField(object specimen, ISpecimenContext context, FieldInfo fi, ITracker tracker)
    {
        try
        {
            Logger.LogInfo("Before field Resolved ");
            var request = new AutoMockFieldRequest(GetSpecimenType(specimen), fi, tracker);
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
            base.HandleField(specimen, context, fi, tracker);
        }
    }
}