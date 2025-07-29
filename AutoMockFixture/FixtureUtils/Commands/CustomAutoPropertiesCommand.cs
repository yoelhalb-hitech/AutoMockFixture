using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Commands;

// AutoFixture has special handling for `AutoPropertiesCommand`, but still calls it as `ISpecimenCommand`
internal class CustomAutoPropertiesCommand : AutoPropertiesCommand, ISpecimenCommand
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

    public IAutoMockFixture Fixture { get; }
    public bool IncludePrivateSetters { get; set; }
    public bool IncludePrivateOrMissingGetter { get; set; }

    public CustomAutoPropertiesCommand(IAutoMockFixture fixture) : this(new TrueRequestSpecification(), fixture)
    {
    }

    public CustomAutoPropertiesCommand(IRequestSpecification specification, IAutoMockFixture fixture) : base(specification)
    {
        Fixture = fixture;
    }


    // For when the system is coming via ISpecimenCommand
    void ISpecimenCommand.Execute(object specimen, ISpecimenContext context) => Execute(specimen, context);

    // For when the system is coming via the current class
    public virtual new void Execute(object specimen, ISpecimenContext context)
    {
        Logger.LogInfo("In auto properties");
        if (context is null) throw new ArgumentNullException(nameof(context));

        if (specimen is null || specimen is IAutoMock || delegateSpecification.IsSatisfiedBy(specimen)) return;

        // TODO... if we want to have `Create on demend` we have to cache here the original tracker

        var mock = Fixture.AutoMockHelpers.GetFromObj(specimen);
        var existingTracker = mock?.Tracker;
        if (existingTracker is null) Fixture.ProcessingTrackerDict.TryGetValue(specimen, out existingTracker);

        var tracker = existingTracker ?? new NonAutoMockRequest(specimen.GetType(), Fixture);

        foreach (var pd in GetPropertiesWithSet(specimen))
        {
            Logger.LogInfo("Property: " + pd.Name);
            try
            {
                // If it is already set (possibly by the constructor or if it's static) then no need to set again
                if (!NeedsSetup(specimen, pd)) continue;

                HandleProperty(specimen, context, pd, tracker);
            }
            catch { }
        }

        foreach (var fi in GetFields(specimen))
        {
            Logger.LogInfo("Field: " + fi.Name);
            try
            {
                // If it is already set (possibly by the constructor or if it's static) then no need to set again
                if (!object.Equals(fi.GetValue(specimen), fi.FieldType.GetDefault())) continue; // Use object.Equals because of primitive types

                HandleField(specimen, context, fi, tracker);
            }
            catch { }
        }

        if (existingTracker is null) tracker.SetCompleted(this);
    }

    protected virtual bool NeedsSetup(object specimen, PropertyDetail pd) =>
        (pd.GetMethod is not null || pd.BasePrivateGetMethod is not null)
            && object.Equals(pd.GetMethod is not null ? pd.ReflectionInfo.GetValue(specimen)
                            : pd.BasePrivateGetMethod!.ReflectionInfo.Invoke(specimen, new object[] { }),
                    pd.ReflectionInfo.PropertyType.GetDefault()); // Use object.Equals because of primitive types

    protected virtual void HandleProperty(object specimen, ISpecimenContext context, PropertyDetail pd, ITracker tracker)
    {
        if (pd.SetMethod is null && pd.BasePrivateSetMethod is null) return; // Avoid creating the value..

        var propertyValue = GetPropertyValue(specimen, context, pd.ReflectionInfo, tracker);

        if (propertyValue is NoSpecimen || propertyValue is OmitSpecimen) return;

        if (pd.ReflectionInfo.SetMethod is not null) pd.ReflectionInfo.SetValue(specimen, propertyValue, null);
        else if (pd.BasePrivateSetMethod is not null) pd.BasePrivateSetMethod!.ReflectionInfo.Invoke(specimen, new object?[] { propertyValue });
    }

    protected virtual object? GetPropertyValue(object specimen, ISpecimenContext context, PropertyInfo pi, ITracker tracker)
    {
        Logger.LogInfo("In base prop");

        return context.Resolve(new PropertyRequest(pi.DeclaringType!, pi, tracker));
    }

    protected virtual void HandleField(object specimen, ISpecimenContext context, FieldInfo fi, ITracker tracker)
    {
        var fieldValue = context.Resolve(new FieldRequest(fi.DeclaringType!, fi, tracker));

        if (fieldValue is not NoSpecimen && fieldValue is not OmitSpecimen)
            fi.SetValue(specimen, fieldValue);
    }

    protected override Type GetSpecimenType(object specimen)
    {
        if (specimen is null) throw new ArgumentNullException(nameof(specimen));

        if (ExplicitSpecimenType is not null) return ExplicitSpecimenType;

        return specimen.GetType();
    }

    // We need to handle internal properties as the outside code might depend on it
    // However no need for private protected properties as it is only used if callBase and if so the base should set it...
    protected IEnumerable<FieldInfo> GetFields(object specimen)
    {
        var type = GetSpecimenType(specimen);

        var detailInfo = type.GetTypeDetailInfo();

        // Remember that backing fields will get filtered out by the TypeDetailInfo
        var details = from fd in detailInfo.FieldDetails
                                // Setting it up in case a specific method is setup to `callBase` and it calls the base private field
                                .Concat(detailInfo.BasePrivateFieldDetails.Where(_ => IncludePrivateSetters && IncludePrivateOrMissingGetter))
                                .Concat(detailInfo.ShadowedFieldDetails)
                        select fd.ReflectionInfo;

        // Reflection throws when setting a static InitOnly field
        // InitOnly is meant to be set by the ctor only so it's like a private setter
        return from fi in details
                            where !fi.IsInitOnly || (!fi.IsStatic && IncludePrivateSetters)
                            where (IncludePrivateSetters && IncludePrivateOrMissingGetter) || fi.IsRelevant()
                            where Specification?.IsSatisfiedBy(fi) != false
                               select fi;
    }

    protected IEnumerable<PropertyDetail> GetPropertiesWithSet(object specimen)
    {
        var type = GetSpecimenType(specimen);

        var detailInfo = type.GetTypeDetailInfo();

        // Remember that type might be a mock and that mock usually has added interfaces, so we go in that case by the interfaces on the base type
        var validInterfaces = type.GetInterface(typeof(IAutoMock).FullName!) != typeof(IAutoMock) ? type.GetInterfaces() : type.BaseType?.GetInterfaces();

        return from pi in detailInfo.PropertyDetails
                        .Concat(detailInfo.BasePrivatePropertyDetails.Where(_ => IncludePrivateSetters))
                        .Concat(detailInfo.ShadowedPropertyDetails)
                        .Concat(detailInfo.ExplicitPropertyDetails.Where(d => validInterfaces?.Contains(d.ExplicitInterface) ?? false))
                     where pi.SetMethod is not null || pi.BasePrivateSetMethod is not null
                     where IncludePrivateSetters || pi.SetMethod?.ReflectionInfo.IsRelevant() == true
                     where IncludePrivateOrMissingGetter || pi.GetMethod?.ReflectionInfo.IsRelevant() == true
                     where pi.ReflectionInfo.GetIndexParameters().Length == 0 && Specification?.IsSatisfiedBy(pi.ReflectionInfo) != false
                     select pi;
    }
}
