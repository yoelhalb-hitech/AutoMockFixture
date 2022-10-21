using DotNetPowerExtensions.DependencyManagement;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

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
}

internal class WithCtorArgsTestClass
{
    public readonly InternalSimpleTestClass TestCtorArg;
    public InternalSimpleTestClass TestCtorArgProp { get; set; }
    public InternalSimpleTestClass TestCtorArgPrivateProp { get; private set; }
    public virtual InternalSimpleTestClass TestCtorArgVirtualProp { get; set; }
    public virtual InternalSimpleTestClass TestCtorArgVirtualPrivateProp { get; private set; }
    public WithCtorArgsTestClass(InternalSimpleTestClass testArg,
        InternalSimpleTestClass testCtorArgProp, InternalSimpleTestClass testCtorArgVirtualProp,
        InternalSimpleTestClass testCtorArgVirtualPrivateProp, InternalSimpleTestClass testCtorArgPrivateProp)
    {
        this.TestCtorArg = testArg;
        this.TestCtorArgProp = testCtorArgProp;
        this.TestCtorArgVirtualProp = testCtorArgVirtualProp;
        this.TestCtorArgVirtualPrivateProp = testCtorArgVirtualPrivateProp;
        this.TestCtorArgPrivateProp = testCtorArgPrivateProp;
    }
    public InternalSimpleTestClass? TestClassProp { get; set; }
    public InternalSimpleTestClass? TestClassPrivateNonVirtualProp { get; private set; }
    public virtual InternalSimpleTestClass? TestClassPropWithPrivateSet { get; private set; }
    public InternalSimpleTestClass? TestClassPropWithProtectedSet { get; protected set; }
    public virtual InternalAbstractMethodTestClass? TestClassPropGet { get; }
    public InternalAbstractSimpleTestClass? TestClassField;
}

internal class WithCtorNoArgsTestClass
{
    public readonly int TestCtor;
    public WithCtorNoArgsTestClass()
    {
        this.TestCtor = 25;
    }
}