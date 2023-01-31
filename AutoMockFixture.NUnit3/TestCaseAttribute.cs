
namespace AutoMockFixture.NUnit3;

// From https://stackoverflow.com/a/43339950/640195

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object?[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T) };
}

// For exactly two type arguments. See the base implementation above.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object?[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T1), typeof(T2) };
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2, T3> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object?[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T1), typeof(T2), typeof(T3) };
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2, T3, T4> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object?[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2, T3, T4, T5> : TestCaseGenericAttribute
{
    public TestCaseAttribute(params object?[] arguments)
        : base(arguments) => TypeArguments = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
}

