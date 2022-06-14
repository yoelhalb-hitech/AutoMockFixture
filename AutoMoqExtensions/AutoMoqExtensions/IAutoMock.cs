using AutoMoqExtensions.FixtureUtils;
using Moq;
using System;
using System.Reflection;

namespace AutoMoqExtensions
{
    // TODO... add analyzer for:
    // 1) TAnon to ensure the properties are actually parameters of the method and the correct type
    // 2) method name as string that it is correct and there is only one overload
    // 3) return tyhpe is correct for the method provided
    // TODO... maybe split it in partial classes for readability

    // TODO... hanlde out and ref methods
    public interface IAutoMock
    {
        bool CallBase { get; set; }
        DefaultValue DefaultValue { get; set; }
        void EnsureMocked();
        Type GetInnerType();
        object GetMocked();
    }
}
