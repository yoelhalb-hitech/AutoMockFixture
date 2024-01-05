using DotNetPowerExtensions.Reflection;
using AutoMockFixture.AutoMockUtils;
using Moq;
using Moq.Protected;

namespace AutoMockFixture.Tests.AutoMockUtils;

internal class SetupServiceFactoryBase_Tests
{
    internal interface ITestClass
    {
        public string? IfaceProp { get; set; }
        public string? IfaceWriteOnlyProp { set; }
    }
    internal class TestClass : ITestClass
    {
        public string? GetSet { get; set; }
        public string? WriteOnly { set => throw new NotImplementedException(); }
        public string? ReadOnly { get => throw new NotImplementedException(); }
        public string? PrivateGet { private get; set; }
        string? ITestClass.IfaceProp { get; set; }
        string? ITestClass.IfaceWriteOnlyProp { set => throw new NotImplementedException(); }
    }

    [Test]
    [TestCase(nameof(TestClass.GetSet), MethodSetupTypes.Eager, true, MethodSetupTypes.Eager)]
    [TestCase(nameof(TestClass.GetSet), MethodSetupTypes.LazySame, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.GetSet), MethodSetupTypes.LazyDifferent, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.WriteOnly), MethodSetupTypes.Eager, false, MethodSetupTypes.Eager)]
    [TestCase(nameof(TestClass.WriteOnly), MethodSetupTypes.LazySame, false, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.WriteOnly), MethodSetupTypes.LazyDifferent, false, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.ReadOnly), MethodSetupTypes.Eager, true, MethodSetupTypes.Eager)]
    [TestCase(nameof(TestClass.ReadOnly), MethodSetupTypes.LazySame, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.ReadOnly), MethodSetupTypes.LazyDifferent, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.PrivateGet), MethodSetupTypes.Eager, false, MethodSetupTypes.Eager)]
    [TestCase(nameof(TestClass.PrivateGet), MethodSetupTypes.LazySame, false, MethodSetupTypes.LazySame)]
    [TestCase(nameof(TestClass.PrivateGet), MethodSetupTypes.LazyDifferent, false, MethodSetupTypes.LazySame)]
    [TestCase(nameof(ITestClass.IfaceProp), MethodSetupTypes.Eager, true, MethodSetupTypes.Eager)]
    [TestCase(nameof(ITestClass.IfaceProp), MethodSetupTypes.LazySame, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(ITestClass.IfaceProp), MethodSetupTypes.LazyDifferent, true, MethodSetupTypes.LazySame)]
    [TestCase(nameof(ITestClass.IfaceWriteOnlyProp), MethodSetupTypes.Eager, false, MethodSetupTypes.Eager)]
    [TestCase(nameof(ITestClass.IfaceWriteOnlyProp), MethodSetupTypes.LazySame, false, MethodSetupTypes.LazySame)]
    [TestCase(nameof(ITestClass.IfaceWriteOnlyProp), MethodSetupTypes.LazyDifferent, false, MethodSetupTypes.LazySame)]

    public void Test_GetSingleMethodPropertySetup(string name, MethodSetupTypes setupType,
                                                        bool expectsGet, MethodSetupTypes expectedSetupType)
    {
        var mock = new Mock<SetupServiceFactoryBase>(() => setupType) { CallBase = true };
        var typeDetail = typeof(TestClass).GetTypeDetailInfo();
        var prop = typeDetail.PropertyDetails.Union(typeDetail.ExplicitPropertyDetails)
                                .First(pd => pd.Name == name);

        mock.Object.GetSingleMethodPropertySetup(null!, prop, null!);

        mock.Protected().Verify("GetService", Times.Once(), expectedSetupType, ItExpr.IsNull<IAutoMock>(),
                expectsGet ? prop.GetMethod : prop.SetMethod, ItExpr.IsNull<ISpecimenContext>(), prop.GetTrackingPath());
    }
}
