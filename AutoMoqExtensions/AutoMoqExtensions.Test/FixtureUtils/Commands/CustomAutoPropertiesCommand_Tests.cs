﻿using AutoMoqExtensions.FixtureUtils.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Test.FixtureUtils.Commands;

file class TestBase
{
    public string? TestProp { get; private set; }
}
file class TestSub : TestBase { }
file class TestSubOfSub : TestSub { }

file class CustomAutoPropertiesCommandSub : CustomAutoPropertiesCommand
{
    public CustomAutoPropertiesCommandSub() : base(new AbstractAutoMockFixture())
    {
    }

    public IEnumerable<PropertyInfo> GetAllProperties<T>() where T : new()
    {
        var specimen = new T();

        return GetPropertiesWithSet(specimen);
    }
}

internal class CustomAutoPropertiesCommand_Tests
{
    [Test]
    public void Test_GetPropertiesWithSet_Returns_BaseProperty_WhenPrivate()
    {
        var props = new CustomAutoPropertiesCommandSub { IncludePrivateSetters = true }.GetAllProperties<TestSub>();

        props.Should().Contain(p => p.Name == nameof(TestBase.TestProp));
    }

    [Test]
    public void Test_GetPropertiesWithSet_Returns_BaseOfBaseProperty_WhenPrivate()
    {
        var props = new CustomAutoPropertiesCommandSub { IncludePrivateSetters = true }.GetAllProperties<TestSubOfSub>();

        props.Should().Contain(p => p.Name == nameof(TestBase.TestProp));
    }
}
