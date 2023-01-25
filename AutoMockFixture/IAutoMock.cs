using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests;
using System.Reflection;

namespace AutoMockFixture;

// TODO... add analyzer for:
// 1) TAnon to ensure the properties are actually parameters of the method and the correct type
// 2) method name as string that it is correct and there is only one overload
// 3) return tyhpe is correct for the method provided
// TODO... maybe split it in partial classes for readability

// TODO... hanlde out and ref methods
public interface IAutoMock
{
    bool CallBase { get; set; }
    AutoMockFixture Fixture { get; }
    void EnsureMocked();
    Type GetInnerType();
    object GetMocked();
    ITracker? Tracker { get; set; }
    object? Target { get; }

    // We need it on the interface, since in Mock it is only on the generic version which we don't always have access to
    void Verify();
    // We need it on the interface, since in Mock it is only on the generic version which we don't always have access to
    void VerifyNoOtherCalls();

    Dictionary<string, MemberInfo> MethodsSetup { get; }
    Dictionary<string, CannotSetupMethodException> MethodsNotSetup { get; }
}
