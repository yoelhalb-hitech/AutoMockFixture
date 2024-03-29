﻿
namespace AutoMockFixture.FixtureUtils.MethodQueries;

internal class CustomConstructorMethod : IMethod
{
    internal CustomConstructorMethod(ConstructorInfo ctor, IAutoMockHelpers autoMockHelpers)
    {
        this.Constructor = ctor ?? throw new ArgumentNullException(nameof(ctor));
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public ConstructorInfo Constructor { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public IEnumerable<ParameterInfo> Parameters => Constructor.GetParameters();

    public object Invoke(IEnumerable<object> parameters)
    {
        var paramsToUse = GetParamsToUse(parameters);
        return this.Constructor.Invoke(paramsToUse.ToArray());
    }

    public object? Invoke(IEnumerable<object> parameters, object owner)
    {
        var paramsToUse = GetParamsToUse(parameters);
        return this.Constructor.Invoke(owner, paramsToUse.ToArray());
    }

    private IEnumerable<object> GetParamsToUse(IEnumerable<object> parameters)
    {
        var paramInfos = Parameters.ToArray();

        var paramToUse = parameters.Select((p, i) =>
        {
            if (p is IAutoMock a && !AutoMockHelpers.IsAutoMock(paramInfos[i].ParameterType))
            {
                return a.GetMocked();
            }

            return p;
        });

        return paramToUse;
    }
}
