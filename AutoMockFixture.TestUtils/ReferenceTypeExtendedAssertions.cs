using AutoMockFixture.Moq4;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace AutoMockFixture.TestUtils;

internal abstract class ReferenceTypeExtendedAssertions<TSubject, TAssertions>
     : ReferenceTypeAssertions<TSubject, TAssertions>
        where TAssertions : ReferenceTypeExtendedAssertions<TSubject, TAssertions>
{
    protected ReferenceTypeExtendedAssertions(TSubject subject) : base(subject)
    {
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> HaveAutoMockState(bool autoMock)
    {
        using (var scope = new AssertionScope(nameof(AutoMock.IsAutoMock)))
        {
            AutoMock.IsAutoMock(Subject).Should().Be(autoMock);
        }

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> BeAutoMock() => HaveAutoMockState(true);

    [CustomAssertion]
    public AndConstraint<TAssertions> NotBeAutoMock() => HaveAutoMockState(false);

    [CustomAssertion]
    public AndConstraint<TAssertions> Be(InternalSimpleTestClass expected,
        string because = "", params object[] becauseArgs)
    {
        ((object?)Subject).Should().Be(expected, because, becauseArgs);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> NotBe(InternalSimpleTestClass expected,
        string because = "", params object[] becauseArgs)
    {
        ((object?)Subject).Should().NotBe(expected, because, becauseArgs);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> BeEquivalentTo(InternalSimpleTestClass expected,
        string because = "", params object[] becauseArgs)
    {
        ((object?)Subject).Should().BeEquivalentTo(expected, because, becauseArgs);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> NotBeEquivalentTo(InternalSimpleTestClass expected,
    string because = "", params object[] becauseArgs)
    {
        ((object?)Subject).Should().NotBeEquivalentTo(expected, because, becauseArgs);

        return new AndConstraint<TAssertions>((TAssertions)this);
    }
}
