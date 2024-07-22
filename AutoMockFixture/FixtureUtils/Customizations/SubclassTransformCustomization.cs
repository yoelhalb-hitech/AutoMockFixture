using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Customizations;

/// <summary>
/// Customization to use a given subclass instead of the original baseclass
/// </summary>
public class SubclassTransformCustomization : ICustomization
{
    public SubclassTransformCustomization(Type originalType, Type subclassType)
    {
        if (originalType.IsGenericType && originalType.GenericTypeArguments.Any(a => a.IsGenericParameter)) throw new ArgumentException("Type has a stub parmater which is not allowed", nameof(originalType));

        if (subclassType.IsGenericType && subclassType.GenericTypeArguments.Any(a => a.IsGenericParameter)) throw new ArgumentException("Type has a stub parmater which is not allowed", nameof(subclassType));

        if((!originalType.IsGenericType || !originalType.IsGenericTypeDefinition) && !originalType.IsAssignableFrom(subclassType)) throw new ArgumentException("subclassType does not inherit originalType", nameof(subclassType));
        // An open generic subtype does not have the original open as base or interface but rather a closed with stubs
        else if(originalType.IsGenericType && originalType.IsGenericTypeDefinition && !subclassType.GetBasesAndInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == originalType )) throw new ArgumentException("subclassType does not inherit originalType", nameof(subclassType));

        if(originalType.IsGenericType && originalType.IsGenericTypeDefinition && (!subclassType.IsGenericType || !subclassType.IsGenericTypeDefinition)) throw new ArgumentException("subclassType has to be an open generic when originalType is", nameof(subclassType));

        if(subclassType.IsGenericType && subclassType.IsGenericTypeDefinition && (!originalType.IsGenericType || !originalType.IsGenericTypeDefinition)) throw new ArgumentException("subclassType cannot be an open generic when originalType is not", nameof(subclassType));

        if(subclassType.IsGenericType && subclassType.IsGenericTypeDefinition && originalType.GenericTypeArguments.Length != subclassType.GenericTypeArguments.Length) throw new ArgumentException("subclassType generic argument does not match base class", nameof(subclassType));

        if(subclassType.IsGenericType && subclassType.IsGenericTypeDefinition && originalType.GenericTypeArguments.Any(a => !subclassType.GenericTypeArguments.Contains(a)
            || originalType.GenericTypeArguments.ToList().IndexOf(a) != subclassType.GenericTypeArguments.ToList().IndexOf(a))) throw new ArgumentException("subclassType generic argument does not match base class", nameof(subclassType));

        OriginalType = originalType;
        SubclassType = subclassType;
    }

    public Type OriginalType { get; }
    public Type SubclassType { get; }


    public void Customize(IFixture fixture)
    {
        if (fixture is not IAutoMockFixture mockFixture) throw new ArgumentException("fixture is not an AutoMockFixture", nameof(fixture));

        fixture.Customizations.Insert(0, new SubClassTransformBuilder(OriginalType, SubclassType, mockFixture.AutoMockHelpers));
    }


    internal class SubClassTransformBuilder : ISpecimenBuilder
    {
        public Type OriginalType { get; }
        public Type SubclassType { get; }
        private IAutoMockHelpers AutoMockHelpers { get; }

        public SubClassTransformBuilder(Type originalType, Type subclassType, IAutoMockHelpers autoMockHelpers)
        {
            this.OriginalType = originalType;
            this.SubclassType = subclassType;
            this.AutoMockHelpers = autoMockHelpers;
        }

        private IRequestSpecification GenericSpecification => new GenericTypeSpecification(OriginalType);
        private IRequestSpecification RequestSpecification
                => new TypeOrRequestSpecification(new OrRequestSpecification(new ExactTypeSpecification(OriginalType), GenericSpecification), AutoMockHelpers!);

        private Type GetNewType(Type requestType)
        {
            var isMock = AutoMockHelpers!.IsAutoMock(requestType);
            var originalType = isMock ? AutoMockHelpers.GetMockedType(requestType) : requestType;

            var newType = OriginalType.IsGenericType && OriginalType.IsGenericTypeDefinition ? SubclassType.MakeGenericType(requestType.GenericTypeArguments) : SubclassType;

            return isMock ? AutoMockHelpers.GetAutoMockType(newType) : newType;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (!RequestSpecification.IsSatisfiedBy(request)) return new NoSpecimen();

            var requestType = request is Type type ? type : request is IRequestWithType withType ? withType.Request : null;
            if (requestType is null) return new NoSpecimen(); // TODO... should not arrive here

            var newType = GetNewType(requestType);

            object? newRequest = request switch
            {
                Type _ => newType,
                AutoMockDependenciesRequest dependenciesRequest => dependenciesRequest with { Request = newType },
                AutoMockDirectRequest directRequest => directRequest with { Request = newType },
                AutoMockRequest mockRequest => mockRequest with { Request = newType },
                NonAutoMockRequest nonRequest => nonRequest with { Request = newType },
                _ => null // TODO... should not happen
            };

            if (newRequest is null) return new NoSpecimen();

            var result = context.Resolve(newRequest);
            (request as ITracker)?.SetResult(result, this);

            return result;

        }
    }
}
