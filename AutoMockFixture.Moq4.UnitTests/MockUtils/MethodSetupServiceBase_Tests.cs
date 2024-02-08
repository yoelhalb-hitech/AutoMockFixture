
using AutoFixture;
using AutoFixture.Kernel;
using AutoMockFixture.Moq4.MockUtils;
using DotNetPowerExtensions.Reflection;
using Moq;

namespace AutoMockFixture.Moq4.UnitTests.MockUtils;

internal class MethodSetupServiceBase_Tests
{
    public class OverrideObjectMethods
    {
        public bool EqualsWasCalled { get; private set; }
        public bool GetHashCodeWasCalled { get; private set; }
        public bool ToStringWasCalled { get; private set; }

        public override bool Equals(object? obj)
        {
            EqualsWasCalled = true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            GetHashCodeWasCalled = true;
            return base.GetHashCode();
        }

        public override string ToString()
        {
            ToStringWasCalled = true;
            return base.ToString()!;
        }
    }

    [Test]
    public void Test_CallBase_OnObjectMethods_WhenOverriden_BugRepro()
    {
        var mock = new AutoMock<OverrideObjectMethods>();
        var contextMock = new AutoMock<ISpecimenContext>();

        foreach (var method in typeof(OverrideObjectMethods).GetTypeDetailInfo().MethodDetails)
        {
            var eagerSetup = new MethodEagerSetupService(mock, method, contextMock.Object, "");
            eagerSetup.Setup();
        }

        var obj = mock.Object;

        obj.EqualsWasCalled.Should().BeFalse();
        obj.GetHashCodeWasCalled.Should().BeFalse();
        obj.ToStringWasCalled.Should().BeFalse();

        _ = obj.Equals(new object());
        _ = obj.GetHashCode();
        _ = obj.ToString();

        obj.EqualsWasCalled.Should().BeTrue();
        obj.GetHashCodeWasCalled.Should().BeTrue();
        obj.ToStringWasCalled.Should().BeTrue();

        contextMock.Verify(m => m.Resolve(It.IsAny<object>()), Times.Never());
    }
}
