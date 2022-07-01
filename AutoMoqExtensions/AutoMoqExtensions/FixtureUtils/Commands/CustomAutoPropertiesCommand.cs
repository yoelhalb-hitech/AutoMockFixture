using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class CustomAutoPropertiesCommand : ISpecimenCommand
    {
        public Type? ExplicitSpecimenType { get; }
        public IRequestSpecification? Specification { get; }
        public AutoMockFixture Fixture { get; }

        public CustomAutoPropertiesCommand(AutoMockFixture fixture)
            : this(new TrueRequestSpecification(), fixture)
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

            foreach (var pi in GetPropertiesWithSet(specimen))
            {
                Logger.LogInfo("Property: " + pi.Name);
                try
                {
                    // If it is already set (possibly by the constructor or if it's static) then no need to set again
                    if (pi.GetValue(specimen) != pi.PropertyType.GetDefault()) continue;

                    HandleProperty(specimen, context, pi);
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

                    HandleField(specimen, context, fi);
                }
                catch { }
            }
        }

        protected virtual void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi)
        {
            Logger.LogInfo("In base prop");

            // TODO... if we want to have `Create on demend` we have to cache here the original tracker
            var tracker = Fixture.ProcessingTrackerDict.ContainsKey(specimen) ? Fixture.ProcessingTrackerDict[specimen] : new TrackerWithFixture(Fixture);
            var propertyValue = context.Resolve(new PropertyRequest(pi.DeclaringType, pi, tracker));

            if(tracker is TrackerWithFixture) tracker.SetCompleted();
            if (!(propertyValue is OmitSpecimen))
                pi.SetValue(specimen, propertyValue, null);
        }

        protected virtual void HandleField(object specimen, ISpecimenContext context, FieldInfo fi)
        {
            // TODO... maybe we should do one TrackerWithFixture per object, and maybe save it
            var tracker = Fixture.ProcessingTrackerDict.ContainsKey(specimen) ? Fixture.ProcessingTrackerDict[specimen] : new TrackerWithFixture(Fixture);
            var fieldValue = context.Resolve(new FieldRequest(fi.DeclaringType, fi, tracker));
            if (tracker is TrackerWithFixture) tracker.SetCompleted();
            if (!(fieldValue is OmitSpecimen))
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
                   && pi.SetMethod.IsPublicOrInternal()
                   && pi.GetIndexParameters().Length == 0
                   && Specification?.IsSatisfiedBy(pi) != false
                   select pi;
        }


    }
}
