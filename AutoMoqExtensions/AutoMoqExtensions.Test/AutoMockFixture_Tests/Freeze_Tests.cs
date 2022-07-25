using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class Freeze_Tests
    {
        [Test]
        public void Test_ClassMarkedSingleton_IsFrozen()
        {
            var fixture = new AutoMockFixture();
            var obj1 = fixture.CreateAutoMock<SingletonUserClass>(true);
            var obj2 = fixture.CreateAutoMock<SingletonUserClass>(true);
            var obj3 = fixture.CreateAutoMock<SingletonUserClass>();
            var singletonMock = fixture.CreateAutoMock<SingletonClass>();
            var mock = fixture.CreateAutoMock<AutoMock<SingletonClass>>();

            obj1.Class1.Should().NotBeNull();
            obj1.Class1.Should().BeAssignableTo<SingletonClass>();
            obj1.Class2.Should().Be(obj1.Class1);
            obj1.SingletonProp.Should().Be(obj1.Class1);
            obj1.SingletonField.Should().Be(obj1.Class1);

            obj2.Class1.Should().Be(obj1.Class1);
            obj2.Class2.Should().Be(obj1.Class1);
            obj2.SingletonProp.Should().Be(obj1.Class1);
            obj2.SingletonField.Should().Be(obj1.Class1);

            obj3.SingletonProp.Should().Be(obj1.Class1);
            obj3.SingletonPropGet.Should().Be(obj1.Class1);
            obj3.SingletonField.Should().Be(obj1.Class1);

            singletonMock.Should().Be(obj1.Class1);
            mock.GetMocked().Should().Be(obj1.Class1);
        }
    }
}
