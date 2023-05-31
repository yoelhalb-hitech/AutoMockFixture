
using AutoMockFixture.Moq4.VerifyInfo;
using Moq;
using Moq.Language.Flow;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMockFixture.Moq4.UnitTests.AutoMockUtils;

internal class AutoMockSetups_Tests
{
    public class TestActionClass
    {
        public virtual void Test(int i, decimal d, bool b, string? s, List<string>? l) { }
    }

    public class TestFuncClass
    {
        public virtual int Test(int i, decimal d, bool b, string? s, List<string>? l) => 10;
    }

    private void Verify(ISetup setup)
    {
        setup.OriginalExpression.Should().BeAssignableTo<LambdaExpression>();

        Verify((setup.OriginalExpression as LambdaExpression)!);
    }

    private void Verify(LambdaExpression lambda)
    {
        lambda!.Body.Should().BeAssignableTo<MethodCallExpression>();

        var call = lambda!.Body as MethodCallExpression;

        call!.Arguments.OfType<MethodCallExpression>().Count().Should().Be(5);

        var method = typeof(It).GetMethod(nameof(It.IsAny));
        call!.Arguments.OfType<MethodCallExpression>().Where(m => m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == method).Count().Should().Be(5);
    }

    [Test]
    public void Test_SetupAction()
    {
        var autoMock = new AutoMock<TestActionClass>();
        var ret = autoMock.Setup(a => a.Test(default, default, default, default, default)) as SetupPhrase;

        Verify(ret!.Setup);
    }

    private void Verify(List<IVerifyInfo<TestActionClass>> verifyList)
    {

        verifyList.Count().Should().Be(1);
        var first = verifyList.First();

        first.Should().BeOfType<VerifyActionInfo<TestActionClass>>();

        var verifyAction = (first as VerifyActionInfo<TestActionClass>)!;
        Verify(verifyAction.Expression);

        verifyAction.Times.Should().Equals(Times.Once());
    }

    [Test]
    public void Test_SetupAction_WithTimes()
    {
        var autoMock = new AutoMock<TestActionClass>();
        autoMock.Setup(a => a.Test(default, default, default, default, default), Times.Once());

        Verify(autoMock.MutableSetups[0]);
        Verify(autoMock.VerifyList);
    }

    [Test]
    public void Test_SetupAction_WithSetupActionAndTimes()
    {
        var mockAction = new Mock<Action<ISetup<TestActionClass>>>();

        var autoMock = new AutoMock<TestActionClass>();
        autoMock.Setup(a => a.Test(default, default, default, default, default), mockAction.Object, Times.Once());

        mockAction.Verify(m => m.Invoke(It.IsAny<ISetup<TestActionClass>>()), Times.Once());

        Verify(autoMock.MutableSetups[0]);
        Verify(autoMock.VerifyList);
    }

    [Test]
    public void Test_SetupFunc()
    {
        var autoMock = new AutoMock<TestFuncClass>();
        var ret = autoMock.Setup(a => a.Test(default, default, default, default, default)) as SetupPhrase;

        Verify(ret!.Setup);
    }

    private void Verify(List<IVerifyInfo<TestFuncClass>> verifyList)
    {

        verifyList.Count().Should().Be(1);
        var first = verifyList.First();

        first.Should().BeOfType<VerifyFuncInfo<TestFuncClass, int>>();

        var verifyAction = (first as VerifyFuncInfo<TestFuncClass, int>)!;
        Verify(verifyAction.Expression);

        verifyAction.Times.Should().Equals(Times.Once());
    }

    [Test]
    public void Test_SetupFunc_WithTimes()
    {
        var autoMock = new AutoMock<TestFuncClass>();
        autoMock.Setup(a => a.Test(default, default, default, default, default), Times.Once());

        Verify(autoMock.MutableSetups[0]);
        Verify(autoMock.VerifyList);
    }

    [Test]
    public void Test_SetupFunc_WithResultAndTimes()
    {
        var autoMock = new AutoMock<TestFuncClass>();
        autoMock.Setup(a => a.Test(default, default, default, default, default), 10, Times.Once());

        var mockInvocation = new Mock<Invocation>(typeof(TestFuncClass),
            typeof(TestFuncClass).GetMethod(nameof(TestFuncClass.Test)),
            0, 0m, true, "", null).Object;

        (autoMock.MutableSetups[0] as Setup)!.Execute(mockInvocation);
        mockInvocation.ReturnValue.Should().Be(10);

        Verify(autoMock.MutableSetups[0]);
        Verify(autoMock.VerifyList);
    }

    [Test]
    public void Test_SetupFunc_WithSetupActionAndTimes()
    {
        var mockAction = new Mock<Action<ISetup<TestFuncClass, int>>>();

        var autoMock = new AutoMock<TestFuncClass>();
        autoMock.Setup(a => a.Test(default, default, default, default, default), mockAction.Object, Times.Once());

        mockAction.Verify(m => m.Invoke(It.IsAny<ISetup<TestFuncClass, int>>()), Times.Once());

        Verify(autoMock.MutableSetups[0]);
        Verify(autoMock.VerifyList);
    }
}
