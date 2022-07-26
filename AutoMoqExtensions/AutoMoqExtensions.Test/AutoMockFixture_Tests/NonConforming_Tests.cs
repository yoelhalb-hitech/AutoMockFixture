using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class NonConforming_Tests
    {
        [Test]
        public void Test_AutoMock_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.Create<AutoMock<InternalAbstractMethodTestClass>[]>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>[]>();
            obj.Length.Should().Be(3);
            obj.First().Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.First().GetMocked().Should().NotBeNull();
            obj.First().GetMocked().InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_NonAutoMock_Tuple()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.Create<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
            obj.Item1.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.Item1.GetMocked().Should().NotBeNull();
            obj.Item1.GetMocked().InternalTest.Should().NotBeNull();
            obj.Item2.Should().BeOfType<InternalSimpleTestClass>();
            obj.Item2.Should().NotBeNull();
            obj.Item2.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_Task()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            Assert.Throws<InvalidOperationException>(() => fixture.Create<AutoMock<Task<InternalAbstractMethodTestClass>>>());
        }

        [Test]
        public void Test_NonAutoMock_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.Create<InternalSimpleTestClass[]>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalSimpleTestClass[]>();
            obj.Length.Should().Be(3);
            obj.First().Should().BeOfType<InternalSimpleTestClass>();
        }

        [Test]
        public async Task Test_NonAutoMock_Task()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<Task<InternalSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Task<InternalSimpleTestClass>>();

            obj.IsCompleted.Should().BeTrue();
            var inner = await obj;
            inner.InternalTest.Should().NotBeNull();
        }
    }
}
