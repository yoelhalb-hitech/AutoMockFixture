﻿
namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutoMockDependenciesAttribute : AutoMockTypeAttribute
{
    public AutoMockDependenciesAttribute() : base(AutoMockTypes.AutoMockDependencies) { }
}
