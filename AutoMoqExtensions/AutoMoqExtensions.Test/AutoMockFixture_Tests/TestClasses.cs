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
    public readonly InternalSimpleTestClass TestCtorArg;// This way we will get the one that was passed
    public WithCtorArgsTestClass(InternalSimpleTestClass testArg)
    {
        this.TestCtorArg = testArg;
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

internal class AutoMockTestClass1 : WithCtorArgsTestClass
{
    public AutoMockTestClass1(InternalSimpleTestClass testArg) : base(testArg)
    {
    }
}

[Singleton]
public class SingletonClass { }

public class SingletonUserClass
{
    public SingletonClass Class1 { get; } // Non virtual to only work with the ctor
    public SingletonClass Class2 { get; } // Non virtual to only work with the ctor
    public SingletonUserClass(SingletonClass class1, SingletonClass class2)
    {
        Class1 = class1;
        Class2 = class2;
    }
    public virtual SingletonClass? SingletonProp { get; set; }
    public virtual SingletonClass? SingletonPropGet { get; }
    public SingletonClass? SingletonField;
}
