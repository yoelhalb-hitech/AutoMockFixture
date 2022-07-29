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
            // We cannot use CreateAutoMock as it is not valid for arrays
            var obj = fixture.CreateWithAutoMockDependencies<AutoMock<InternalAbstractMethodTestClass>[]>();
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
            var obj = fixture.CreateNonAutoMock<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
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
        public void Test_NonAutoMock_WithDependencies_Tuple()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.CreateWithAutoMockDependencies<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
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
            Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<Task<InternalAbstractMethodTestClass>>());
        }

        [Test]
        public void Test_NonAutoMock_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass[]>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalSimpleTestClass[]>();
            obj.Length.Should().Be(3);
            obj.First().Should().BeOfType<InternalSimpleTestClass>();
        }

        [Test]
        public void Test_NonAutoMock_WithDependencies_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass[]>();
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
            var obj = fixture.CreateNonAutoMock<Task<InternalSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Task<InternalSimpleTestClass>>();

            obj.IsCompleted.Should().BeTrue();
            var inner = await obj;
            inner.InternalTest.Should().NotBeNull();
        }

        [Test]
        public async Task Test_NonAutoMock_WithDependencies_Task()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateWithAutoMockDependencies<Task<InternalSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Task<InternalSimpleTestClass>>();

            obj.IsCompleted.Should().BeTrue();
            var inner = await obj;
            inner.InternalTest.Should().NotBeNull();
        }
    }
}
