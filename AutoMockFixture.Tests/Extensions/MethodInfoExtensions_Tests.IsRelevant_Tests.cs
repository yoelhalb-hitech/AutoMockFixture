using DotNetPowerExtensions.Reflection;
using System.Reflection.Emit;

public class NoNamespace
{
    public void PublicMethod() { }
    internal void InternalMethod() { }
    public class Inner
    {
        public void PublicMethod() { }
        internal void InternalMethod() { }
    }
}

namespace System
{
    public class WithSystemNamespace
    {
        public void PublicMethod() { }
        internal void InternalMethod() { }
        public class Inner
        {
            public void PublicMethod() { }
            internal void InternalMethod() { }
        }
    }

    namespace InnerNamespace
    {
        public class WithSystemPrefix
        {
            public void PublicMethod() { }
            internal void InternalMethod() { }
            public class Inner
            {
                public void PublicMethod() { }
                internal void InternalMethod() { }
            }
        }
    }
}

namespace Microsoft
{
    public class WithMicrosoftNamespace
    {
        public void PublicMethod() { }
        internal void InternalMethod() { }
        public class Inner
        {
            public void PublicMethod() { }
            internal void InternalMethod() { }
        }
    }

    namespace InnerNamespace
    {
        public class WithMicrosoftPrefix
        {
            public void PublicMethod() { }
            internal void InternalMethod() { }
            public class Inner
            {
                public void PublicMethod() { }
                internal void InternalMethod() { }
            }
        }
    }
}

namespace AutoMockFixture.Tests.Extensions
{
    public class WithNamespace
    {
        public void PublicMethod() { }
        internal void InternalMethod() { }
        public class Inner
        {
            public void PublicMethod() { }
            internal void InternalMethod() { }
        }
    }
    public class MethodInfoExtensions_Tests__IsRelevant_Tests
    {
        [TestCase(typeof(NoNamespace), ExpectedResult = true)]
        [TestCase(typeof(NoNamespace.Inner), ExpectedResult = true)]
        [TestCase(typeof(System.WithSystemNamespace), ExpectedResult = false)]
        [TestCase(typeof(System.WithSystemNamespace.Inner), ExpectedResult = false)]
        [TestCase(typeof(System.InnerNamespace.WithSystemPrefix), ExpectedResult = false)]
        [TestCase(typeof(System.InnerNamespace.WithSystemPrefix.Inner), ExpectedResult = false)]
        [TestCase(typeof(Microsoft.WithMicrosoftNamespace), ExpectedResult = false)]
        [TestCase(typeof(Microsoft.WithMicrosoftNamespace.Inner), ExpectedResult = false)]
        [TestCase(typeof(Microsoft.InnerNamespace.WithMicrosoftPrefix), ExpectedResult = false)]
        [TestCase(typeof(Microsoft.InnerNamespace.WithMicrosoftPrefix.Inner), ExpectedResult = false)]
        [TestCase(typeof(WithNamespace), ExpectedResult = true)]
        [TestCase(typeof(WithNamespace.Inner), ExpectedResult = true)]
        public bool Test_IsRelevantWorks_ForInternal(Type type)
        {
            var method = type.GetMethod(nameof(WithNamespace.InternalMethod), BindingFlagsExtensions.AllBindings);
            return method!.IsRelevant();
        }

        [TestCase(typeof(NoNamespace))]
        [TestCase(typeof(NoNamespace.Inner))]
        [TestCase(typeof(System.WithSystemNamespace))]
        [TestCase(typeof(System.WithSystemNamespace.Inner))]
        [TestCase(typeof(System.InnerNamespace.WithSystemPrefix))]
        [TestCase(typeof(System.InnerNamespace.WithSystemPrefix.Inner))]
        [TestCase(typeof(Microsoft.WithMicrosoftNamespace))]
        [TestCase(typeof(Microsoft.WithMicrosoftNamespace.Inner))]
        [TestCase(typeof(Microsoft.InnerNamespace.WithMicrosoftPrefix))]
        [TestCase(typeof(Microsoft.InnerNamespace.WithMicrosoftPrefix.Inner))]
        [TestCase(typeof(WithNamespace))]
        [TestCase(typeof(WithNamespace.Inner))]
        public void Test_IsRelevantWorks_ForPublic(Type type)
        {
            var method = type.GetMethod(nameof(WithNamespace.PublicMethod), BindingFlagsExtensions.AllBindings);
            method!.IsRelevant().Should().BeTrue();
        }

        [Test]
        public void Test_IsRelevantWorks_ForDynamicMethod_WithNoClass()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(
                "EmptyDynamicMethod",
                typeof(void),
                new Type[] {},
                typeof(WithNamespace).Module);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ret);


            var methodInfo = dynamicMethod.GetBaseDefinition();

            methodInfo!.IsRelevant().Should().BeTrue();
        }

        [Test]
        public void Test_IsRelevantWorks_ForLambda()
        {
            Action myAction = () => {};

            myAction.Method!.IsRelevant().Should().BeTrue();
        }
    }
}
