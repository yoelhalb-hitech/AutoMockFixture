using System.Globalization;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class MethodWrapper : MethodInfo
{
    private readonly MethodInfo original;

    public MethodWrapper(MethodInfo original)
    {
        this.original = original;
        this.MethodName = original.Name;
        this.MethodAttributes = original.Attributes;
        this.ParentType = original.DeclaringType;
    }

    public override Type ReturnType => original.ReturnType;

    public override CallingConventions CallingConvention => original.CallingConvention;

    public override bool ContainsGenericParameters => original.ContainsGenericParameters;

    public override IEnumerable<CustomAttributeData> CustomAttributes => original.CustomAttributes;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    public override bool IsConstructedGenericMethod => original.IsConstructedGenericMethod;
#endif
    public override bool IsGenericMethod => original.IsGenericMethod;
    public override bool IsGenericMethodDefinition => original.IsGenericMethodDefinition;
    public override bool IsSecurityCritical => original.IsSecurityCritical;
    public override bool IsSecuritySafeCritical => original.IsSecuritySafeCritical;
    public override bool IsSecurityTransparent => original.IsSecurityTransparent;
    public override MemberTypes MemberType => original.MemberType;
    public override int MetadataToken => original.MetadataToken;
    public override MethodImplAttributes MethodImplementationFlags => original.MethodImplementationFlags;
    public override Module Module => original.Module;
    public override ParameterInfo ReturnParameter => original.ReturnParameter;
    public override Delegate CreateDelegate(Type delegateType) => original.CreateDelegate(delegateType);
    public override Delegate CreateDelegate(Type delegateType, object? target) => original.CreateDelegate(delegateType, target);
    public override IList<CustomAttributeData> GetCustomAttributesData() => original.GetCustomAttributesData();
    public override Type[] GetGenericArguments() => original.GetGenericArguments();
    public override MethodInfo GetGenericMethodDefinition() => original.GetGenericMethodDefinition();
    public override MethodBody? GetMethodBody() => original.GetMethodBody();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    public override bool HasSameMetadataDefinitionAs(MemberInfo other) => original.HasSameMetadataDefinitionAs(other);
#endif
    public override MethodInfo MakeGenericMethod(params Type[] typeArguments) => original.MakeGenericMethod(typeArguments);
    public override string? ToString() => original.ToString();

    public override ICustomAttributeProvider ReturnTypeCustomAttributes => original.ReturnTypeCustomAttributes;

    public MethodAttributes MethodAttributes { get; set; }
    public override MethodAttributes Attributes => MethodAttributes;

    public override RuntimeMethodHandle MethodHandle => original.MethodHandle;

    public Type? ParentType { get; set; }
    public override Type? DeclaringType => ParentType;

    public string MethodName { get; set; }
    public override string Name => MethodName;

    public override Type? ReflectedType => original.ReflectedType;

    public override MethodInfo GetBaseDefinition() => original.GetBaseDefinition();

    public override object[] GetCustomAttributes(bool inherit) => original.GetCustomAttributes(inherit);

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => original.GetCustomAttributes(attributeType, inherit);

    public override MethodImplAttributes GetMethodImplementationFlags() => original.GetMethodImplementationFlags();
    public override ParameterInfo[] GetParameters() => original.GetParameters();
    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
        => original.Invoke(obj, invokeAttr, binder, parameters, culture);

    public override bool IsDefined(Type attributeType, bool inherit)  => original.IsDefined(attributeType, inherit);

    public override bool Equals(object? obj)
    {
        if (obj is not null && ReferenceEquals(obj, original)) return true;

        return base.Equals(obj);
    }

    public override int GetHashCode() => original.GetHashCode();
}
