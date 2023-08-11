
namespace AutoMockFixture.TestUtils;

// CAUTION : The classes are `internal` so to test if AutoMockFixture also works correctly with internal

internal interface InternalReadOnlyTestInterface
{
    internal virtual string? InternalTest => null;
}

internal abstract class InternalAbstractSimpleTestClass
{
    internal virtual string? InternalTest { get; set; }
    internal virtual string? NonAbstractMethod() => null;
    internal virtual int? NonAbstractWithValueMethod() => 10;
}
internal abstract class InternalAbstractReadonlyPropertyClass
{
    internal abstract string? InternalTest { get; }
}

internal class InternalReadonlyPropertyClass
{
    internal virtual string? InternalTest { get; }
}
internal class InternalPrivateSetterPropertyClass
{
    internal virtual string? InternalTest { get; private set; }
}
internal class ProtectedSetterPropertyClass
{
    public virtual string? InternalTest { get; protected set; }
}
internal class InternalSimpleTestClass
{
    internal virtual string? InternalTest { get; set; }
}
internal class InternalTestFields
{
    internal string? InternalTest;
}

internal abstract class InternalTestMethods
{
    internal abstract string? InternalTestMethod();
}

internal interface ITestInterface
{
    string? TestProp { get; }
    string? TestMethod();
    InternalTestMethods? InternalTestMethodsObj { get; set; }
}
internal abstract class InternalAbstractMethodTestClass
{
    internal string? InternalTest { get; set; }
    // TODO... for setting up internal methods we need to have InternalsVisibleTo
    //     We might need to warn for that
    internal virtual string TestMethod() => "67";
    // TODO... It has a weird DynamicCastle error when the method is internal abstract, we need to simplify
    // TODO... It has an issue setting up out when the method has implementation
    internal abstract string TestOutParam(out string test);// => test = "43";
                                                           //public string TestOutParam1(out string test1) => test1 = "43";
    internal virtual InternalTestMethods? InternalTestMethodsObj { get; set; }
}

internal class WithCtorArgsTestClass
{
    public InternalSimpleTestClass TestCtorArg { get; } // non virtual readonly so it will only be set by the ctor
    public InternalSimpleTestClass TestCtorArgProp { get; set; }
    public InternalSimpleTestClass TestCtorArgPrivateProp { get; private set; }
    public virtual InternalSimpleTestClass TestCtorArgVirtualProp { get; set; }
    public virtual InternalSimpleTestClass TestCtorArgVirtualPrivateProp { get; private set; }
    public WithCtorArgsTestClass(InternalSimpleTestClass testArg,
        InternalSimpleTestClass testCtorArgProp, InternalSimpleTestClass testCtorArgVirtualProp,
        InternalSimpleTestClass testCtorArgVirtualPrivateProp, InternalSimpleTestClass testCtorArgPrivateProp)
    {
        TestCtorArg = testArg;
        TestCtorArgProp = testCtorArgProp;
        TestCtorArgVirtualProp = testCtorArgVirtualProp;
        TestCtorArgVirtualPrivateProp = testCtorArgVirtualPrivateProp;
        TestCtorArgPrivateProp = testCtorArgPrivateProp;
    }
    public InternalSimpleTestClass? TestClassProp { get; set; }
    public InternalSimpleTestClass? TestClassPrivateNonVirtualProp { get; private set; }
    public virtual InternalSimpleTestClass? TestClassPropWithPrivateSet { get; private set; }
    public InternalSimpleTestClass? TestClassPropWithProtectedSet { get; protected set; }
    public virtual InternalAbstractMethodTestClass? TestClassPropGet { get; }
    public InternalAbstractSimpleTestClass? TestClassField;
}

internal class WithComplexTestClass
{
    public WithCtorArgsTestClass? WithCtorArgs { get; set; }
}

internal class WithCtorNoArgsTestClass
{
    public readonly int TestCtor;
    public WithCtorNoArgsTestClass()
    {
        TestCtor = 25;
    }
}