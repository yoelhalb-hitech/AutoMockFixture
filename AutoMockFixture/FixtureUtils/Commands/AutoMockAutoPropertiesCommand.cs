using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using DotNetPowerExtensions.Reflection;
using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockAutoPropertiesCommand : CustomAutoPropertiesCommand
{
    public AutoMockAutoPropertiesCommand(IAutoMockFixture fixture) : base(fixture) { }
    public AutoMockAutoPropertiesCommand(IRequestSpecification specification, IAutoMockFixture fixture) : base(specification, fixture) { }

    // In mock it might have been setup lazily so don't eveluate it now rather check if it's
    protected override bool NeedsSetup(object specimen, PropertyDetail pd)
    {
        var mock = Fixture.AutoMockHelpers.GetFromObj(specimen);
        // We base on the name and not on the property itself since it might be a sub property, but the names should match in general (TODO... need to check the case of exlicit implemented in the base)
        // Not checking on the value since for single method props it might actually save the method instead of the prop
        // TODO... we need to handle the explicit interface better
        if (mock is not null && mock.MethodsSetup.Any(ms => ms.Key is not null && ms.Key == pd.ReflectionInfo.Name)) return false;

        var prop = pd;

        while (prop.GetMethod is null && prop.BasePrivateGetMethod is null && prop.OverridenProperty is not null)
        {
            prop = prop.OverridenProperty;
        }

        return base.NeedsSetup(specimen, prop);
    }

    protected override void HandleProperty(object specimen, ISpecimenContext context, PropertyDetail pd, ITracker tracker)
    {
        var prop = pd;

        while(prop.SetMethod is null && prop.BasePrivateSetMethod is null && prop.OverridenProperty is not null)
        {
            prop = prop.OverridenProperty;
        }

        base.HandleProperty(specimen, context, prop, tracker);
    }

    protected override object? GetPropertyValue(object specimen, ISpecimenContext context, PropertyInfo pi, ITracker tracker)
    {
        try
        {
            Logger.LogInfo("Before Resolved ");
            var request = new AutoMockPropertyRequest(GetSpecimenType(specimen), pi,tracker);
            var propertyValue = context.Resolve(request);

            Logger.LogInfo("Resolved property: " + propertyValue.GetType().Name);

            if (propertyValue is IAutoMock mock && mock.GetInnerType() == pi.PropertyType) return mock.GetMocked();

             return propertyValue;
        }
        catch
        {
            Logger.LogInfo("In catch");
            return base.GetPropertyValue(specimen, context, pi, tracker);
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