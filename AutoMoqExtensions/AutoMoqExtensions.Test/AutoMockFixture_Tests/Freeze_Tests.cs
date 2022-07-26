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
        public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AndCallBase()
        {
            var fixture = new AutoMockFixture();

            var obj1 = fixture.CreateAutoMock<SingletonUserClass>(true);
            var obj2 = fixture.CreateAutoMock<SingletonUserClass>(true);
            
            var singletonMock1 = fixture.CreateAutoMock<SingletonClass>(true);
            var singletonMock2 = fixture.CreateAutoMock<SingletonClass>(true);

            var mock1 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(true);
            var mock2 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(true);

            obj1.Class1.Should().NotBeNull();
            obj1.Class1.Should().BeAssignableTo<SingletonClass>();
            obj1.Class2.Should().Be(obj1.Class1);
            obj1.SingletonProp.Should().Be(obj1.Class1);
            obj1.SingletonField.Should().Be(obj1.Class1);

            obj2.Class1.Should().Be(obj1.Class1);
            obj2.Class2.Should().Be(obj1.Class1);
            obj2.SingletonProp.Should().Be(obj1.Class1);
            obj2.SingletonField.Should().Be(obj1.Class1);

            singletonMock1.Should().Be(obj1.Class1);

            singletonMock2.Should().Be(obj1.Class1);

            mock1.GetMocked().Should().Be(obj1.Class1);

            mock2.GetMocked().Should().Be(obj1.Class1);
        }

        [Test]
        public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AndNonCallBase()
        {
            var fixture = new AutoMockFixture();
            var obj1 = fixture.CreateAutoMock<SingletonUserClass>(false);
            var obj2 = fixture.CreateAutoMock<SingletonUserClass>(false);

            var singletonMock1 = fixture.CreateAutoMock<SingletonClass>(false);
            var singletonMock2 = fixture.CreateAutoMock<SingletonClass>(false);

            var mock1 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(false);
            var mock2 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(false);

            obj1.SingletonProp.Should().NotBeNull();
            obj1.SingletonField.Should().Be(obj1.SingletonProp);

            obj2.SingletonProp.Should().Be(obj1.SingletonProp);
            obj2.SingletonField.Should().Be(obj1.SingletonProp);

            singletonMock1.Should().Be(obj1.SingletonProp);
            singletonMock2.Should().Be(obj1.SingletonProp);

            mock1.GetMocked().Should().Be(obj1.SingletonProp);

            mock2.GetMocked().Should().Be(obj1.SingletonProp);
        }

        [Test]
        public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase()
        {
            var fixture = new AutoMockFixture();
            var obj1 = fixture.CreateAutoMock<SingletonClass>(false);
            var obj2 = fixture.CreateAutoMock<SingletonClass>(true);

            obj1.Should().NotBeNull();
            obj2.Should().NotBeNull();

            obj2.Should().NotBe(obj1);
        }
    }
}
