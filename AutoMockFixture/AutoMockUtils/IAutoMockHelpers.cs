﻿
namespace AutoMockFixture.AutoMockUtils;

internal interface IAutoMockHelpers
{
    bool IsAutoMock<T>();
    bool IsAutoMock(Type? t);
    Type? GetMockedType(Type? t);
    /// <summary>
    /// Returns the non generic base interface, for use when it might be already an AutoMock but we don't know the generic type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    IAutoMock? GetFromObj(object? obj);
    Type GetAutoMockType(Type inner);

    bool IsAutoMockAllowed(Type t, bool force = false);

    bool IsAllowed(Type t, bool force = false);

    SetupServiceFactoryBase GetSetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc);

    ISpecimenCommand GetStubAllPropertiesCommand();
    ISpecimenCommand GetClearInvocationsCommand();

    ISpecimenCommand GetAutoMockInitCommand();

    Type InterfaceProxyBase { get; }
    IRequestSpecification MockRequestSpecification { get; }

    bool CanMock(Type t);
}
