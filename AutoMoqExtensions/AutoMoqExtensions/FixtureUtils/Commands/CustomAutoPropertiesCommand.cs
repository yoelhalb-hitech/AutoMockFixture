using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class CustomAutoPropertiesCommand : ISpecimenCommand
{
    public Type? ExplicitSpecimenType { get; }
    public IRequestSpecification? Specification { get; }
    public AutoMockFixture Fixture { get; }
    public bool IncludePrivateSetters { get; set; }
    public bool IncludePrivateOrMissingGetter { get; set; }

    public CustomAutoPropertiesCommand(AutoMockFixture fixture) : this(new TrueRequestSpecification(), fixture)
    {
        ExplicitSpecimenType = null;
    }

    public CustomAutoPropertiesCommand(IRequestSpecification specification, AutoMockFixture fixture)
    {
        Specification = specification ?? throw new ArgumentNullException(nameof(specification));
        Fixture = fixture;
        ExplicitSpecimenType = null;
    }



    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        Logger.LogInfo("In auto properties");
        if (specimen == null) throw new ArgumentNullException(nameof(specimen));
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (specimen is IAutoMock) return;

        // TODO... if we want to have `Create on demend` we have to cache here the original tracker
        
        var mock = AutoMockHelpers.GetFromObj(specimen);
        var existingTracker = mock?.Tracker;
        if(existingTracker is null) Fixture.ProcessingTrackerDict.TryGetValue(specimen, out existingTracker);

        var tracker = existingTracker ?? new TrackerWithFixture(Fixture);

        foreach (var pi in GetPropertiesWithSet(specimen))
        {
            Logger.LogInfo("Property: " + pi.Name);
            try
            {
                // If it is already set (possibly by the constructor or if it's static) then no need to set again
                if (pi.GetValue(specimen) != pi.PropertyType.GetDefault()) continue;

                HandleProperty(specimen, context, pi, tracker);
            }
            catch { }
        }

        foreach (var fi in GetFields(specimen))
        {
            Logger.LogInfo("Field: " + fi.Name);
            try
            {
                // If it is already set (possibly by the constructor or if it's static) then no need to set again
                if (fi.GetValue(specimen) != fi.FieldType.GetDefault()) continue;

                HandleField(specimen, context, fi, tracker);
            }
            catch { }
        }

        if (existingTracker is null) tracker.SetCompleted();
    }

    protected virtual void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi, ITracker tracker)
    {
        Logger.LogInfo("In base prop");

        var propertyValue = context.Resolve(new PropertyRequest(pi.DeclaringType, pi, tracker));
   
        if (propertyValue is not NoSpecimen && propertyValue is not OmitSpecimen)
            pi.SetValue(specimen, propertyValue, null);
    }

    protected virtual void HandleField(object specimen, ISpecimenContext context, FieldInfo fi, ITracker tracker)
    {
        var fieldValue = context.Resolve(new FieldRequest(fi.DeclaringType, fi, tracker));

        if (fieldValue is not NoSpecimen && fieldValue is not OmitSpecimen)
            fi.SetValue(specimen, fieldValue);
    }

    protected virtual Type GetSpecimenType(object specimen)
    {
        if (specimen == null) throw new ArgumentNullException(nameof(specimen));

        if (ExplicitSpecimenType is not null) return ExplicitSpecimenType;

        return specimen.GetType();
    }
   
    // We need to handle internal properties as the outside code might depend on it
    // However no need for private protected properties as it is only used if callbase and if so the base should set it...
    protected IEnumerable<FieldInfo> GetFields(object specimen)
    {
        return from fi in GetSpecimenType(specimen).GetTypeInfo().GetAllFields()
               where !fi.IsInitOnly
               && fi.IsPublicOrInternal()
               && Specification?.IsSatisfiedBy(fi) != false
               select fi;
    }

    protected IEnumerable<PropertyInfo> GetPropertiesWithSet(object specimen)
    {
        return from pi in GetSpecimenType(specimen).GetTypeInfo().GetAllProperties()
               where pi.SetMethod != null
               && (IncludePrivateSetters || pi.SetMethod.IsPublicOrInternal())
               && (IncludePrivateOrMissingGetter || pi.GetMethod?.IsPublicOrInternal() == true)
               && pi.GetIndexParameters().Length == 0
               && Specification?.IsSatisfiedBy(pi) != false
               select pi;
    }


}
