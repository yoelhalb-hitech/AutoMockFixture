using AutoMoqExtensions.AutoMockUtils;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class Recursion_Tests
    {
        public class Test1
        {
            public Test1(Test2 test){ Test = test; }
            public Test2 Test;
        }

        public class Test2
        {
            public Test2(Test1 test){ Test = test; }
            public Test1 Test;
        }

        [Test]
        public void Test_Can_Create_AutoMock_When_NotCallBase()
        {
            var fixture = new AutoMockFixture();
            var mock = fixture.Create<AutoMock<Test1>>();

            AutoMockHelpers.GetFromObj(mock).Should().NotBeNull();

            Test1? obj = null;
            Assert.DoesNotThrow(() => obj = mock.GetMocked());

            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<Test1>();

            obj!.Test.Should().NotBeNull();
            obj!.Test.Should().BeAssignableTo<Test2>();

            AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
        }

        [Test]
        public void Test_Can_CreateAutoMock_AutoMock_When_NotCallBase()
        {
            var fixture = new AutoMockFixture();
            Test2? obj = null;
            Assert.DoesNotThrow(() => obj = fixture.CreateAutoMock<Test2>());

            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<Test2>();
            AutoMockHelpers.GetFromObj(obj!).Should().NotBeNull();

            obj!.Test.Should().NotBeNull();
            obj!.Test.Should().BeAssignableTo<Test1>();

            AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
        }

        [Test]
        public void Test_Can_Create()
        {
            var fixture = new AutoMockFixture();
            Test2? obj = null;
            Assert.DoesNotThrow(() => obj = fixture.Create<Test2>());

            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<Test2>();
            AutoMockHelpers.GetFromObj(obj!).Should().BeNull();

            obj!.Test.Should().NotBeNull();
            obj!.Test.Should().BeAssignableTo<Test1>();

            AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
        }
    }
}
