using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockAutoPropertiesCommand : CustomAutoPropertiesCommand
{
    public AutoMockAutoPropertiesCommand(IAutoMockFixture fixture) : base(fixture) { }
    public AutoMockAutoPropertiesCommand(IRequestSpecification specification, IAutoMockFixture fixture) : base(specification, fixture) { }

    // In mock it might have been setup lazily so don't eveluate it now rather check if it's
    protected override bool NeedsSetup(object specimen, PropertyInfo pi)
    {
        var mock = Fixture.AutoMockHelpers.GetFromObj(specimen);
        // We base on the name and not on the property itself since it might be a sub property, but the names should match in general (TODO... need to check the case of exlicit implemented in the base)
        // Not checking on the value since for single method props it might actually save the method instead of the prop
        if (mock is not null && mock.MethodsSetup.Any(ms => ms.Key is not null && ms.Key == pi.Name)) return false;

        return base.NeedsSetup(specimen, pi);
    }

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