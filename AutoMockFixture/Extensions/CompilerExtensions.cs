using System.ComponentModel;

namespace System.Runtime.CompilerServices;

#if !NET7_0_OR_GREATER

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class RequiredMemberAttribute : Attribute { }

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public const string RefStructs = "RefStructs";

    public const string RequiredMembers = "RequiredMembers";

    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}

#endif

#if !NET5_0_OR_GREATER

[EditorBrowsable(EditorBrowsableState.Never)]
public static class IsExternalInit
{
}

namespace System.Diagnostics.CodeAnalysis
{

    #if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER
    public sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }
    }
#endif
}