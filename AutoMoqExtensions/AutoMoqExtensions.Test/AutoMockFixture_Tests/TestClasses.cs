﻿using DotNetPowerExtensions.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class InternalTestClass
    {
        internal string? InternalTest { get; set; }
    }
    internal class InternalTestClass1
    {
        internal string? InternalTest { get; set; }
    }
    internal class InternalTestFields
    {
        internal string? InternalTest;
    }

    internal abstract class InternalTestMethods
    {
        internal abstract string? InternalTestMethod();
    }
    internal abstract class InternalTestClass2
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

    internal class AutoMockTestClass
    {
        public readonly InternalTestClass TestCtorArg;// This way we will get the one that was passed
        public AutoMockTestClass(InternalTestClass testArg)
        {
            this.TestCtorArg = testArg;
        }
        public InternalTestClass1? TestClassProp { get; set; }
        public virtual InternalTestClass2? TestClassPropGet { get; }
        public InternalTestClass? TestClassField;
    }

    internal class AutoMockTestClass1 : AutoMockTestClass
    {
        public AutoMockTestClass1(InternalTestClass testArg) : base(testArg)
        {
        }
    }

    [Singleton]
    public class SingletonClass { }

    public class SingletonUserClass
    {
        public SingletonClass Class1 { get; }
        public SingletonClass Class2 { get; }
        public SingletonUserClass(SingletonClass class1, SingletonClass class2)
        {
            Class1 = class1;
            Class2 = class2;
        }
        public SingletonClass? SingletonProp { get; set; }
        public virtual SingletonClass? SingletonPropGet { get; }
        public SingletonClass? SingletonField;
    }
}