using AutoMockFixture.Moq4;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoMockFixture.TestUtils;

internal static class AutoMockAssertions
{
    [CustomAssertion]
    public static AndConstraint<TAssertions> HaveAutoMockState<TAssertions>(
            this TAssertions assertions, bool autoMock)
        where TAssertions : ObjectAssertions
    {
        using (var scope = new AssertionScope(nameof(AutoMock.IsAutoMock)))
        {
            AutoMock.IsAutoMock(assertions.Subject).Should().Be(autoMock);
        }

        return new AndConstraint<TAssertions>((TAssertions)assertions);
    }

    [CustomAssertion]
    public static AndConstraint<TAssertions> BeAutoMock<TAssertions>(this TAssertions assertions)
        where TAssertions : ObjectAssertions
            => HaveAutoMockState<TAssertions>(assertions, true);

    [CustomAssertion]
    public static AndConstraint<TAssertions> NotBeAutoMock<TAssertions>(this TAssertions assertions)
        where TAssertions : ObjectAssertions
            => HaveAutoMockState<TAssertions>(assertions, false);

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllAutoMock<T>(this GenericCollectionAssertions<T> assertions)
    {
        assertions.AllNonNull();
        return CollectionAssertions.ExecuteInternal(assertions, item => AutoMock.IsAutoMock(item), "non auto mock", "not auto mock");
    }

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllNonAutoMock<T>(this GenericCollectionAssertions<T> assertions)
    {
        assertions.AllNonNull();
        return CollectionAssertions.ExecuteInternal(assertions, item => !AutoMock.IsAutoMock(item), "auto mock");
    }

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllHaveAutoMockState<T>(
            this GenericCollectionAssertions<T> assertions, bool callBase)
        => callBase ? assertions.AllAutoMock() : assertions.AllNonAutoMock();
}
