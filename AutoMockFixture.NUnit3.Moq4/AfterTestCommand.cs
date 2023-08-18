using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal;
using System;

namespace AutoMockFixture.NUnit3.Moq4;

#if NETFRAMEWORK

public abstract class AfterTestCommand : DelegatingTestCommand
{
    /// <summary>
    /// Construct an AfterCommand
    /// </summary>
    public AfterTestCommand(TestCommand innerCommand) : base(innerCommand) { }

    /// <summary>
    /// Execute the command
    /// </summary>
    public override TestResult Execute(TestExecutionContext context)
    {
       context.CurrentResult = innerCommand.Execute(context);

        AfterTest?.Invoke(context);

        return context.CurrentResult;
    }

    /// <summary>
    /// Set this to perform action after the inner command.
    /// </summary>
    protected Action<TestExecutionContext>? AfterTest;
}

#endif