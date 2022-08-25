
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.MockUtils;
using Moq;
using System.Reflection;

namespace AutoMoqExtensions;

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
    DefaultValue DefaultValue { get; set; }
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

    #region MethodInfo
    public IAutoMock Setup(MethodInfo method, Times times);
    public IAutoMock Setup<TAnon>(MethodInfo method, TAnon paramData) where TAnon : class;
    // Doing TAnon : class to avoid overload resolution issues
    public IAutoMock Setup<TAnon>(MethodInfo method, TAnon paramData, Times times) where TAnon : class;

    public IAutoMock Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result) where TAnon : class;
    // Doing TAnon : class to avoid overload resolution issues
    public IAutoMock Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times times) where TAnon : class;

    #endregion

    #region string

    public IAutoMock Setup(string methodName);
    public IAutoMock Setup(string methodName, Times times);

    // Doing TAnon : class to avoid overload resolution issues
    public IAutoMock Setup<TAnon>(string methodName, TAnon paramData) where TAnon : class;

    // Doing TAnon : class to avoid overload resolution issues
    public IAutoMock Setup<TAnon>(string methodName, TAnon paramData, Times times) where TAnon : class;

    // Doing TAnon : class to avoid overload resolution issues


    public IAutoMock Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result) where TAnon : class;
    public IAutoMock Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result, Times times) where TAnon : class;

    #endregion
}
