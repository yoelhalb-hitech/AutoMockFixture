using Castle.DynamicProxy;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class InvocationWrapper : Castle.DynamicProxy.IInvocation
{
    private readonly Castle.DynamicProxy.IInvocation original;
    private readonly MethodInfo? proceedMethod;

    public InvocationWrapper(Castle.DynamicProxy.IInvocation original, MethodInfo newMethod)
    {
        this.original = original;
        this.Method = newMethod;
    }

    public InvocationWrapper(Castle.DynamicProxy.IInvocation original, MethodInfo newMethod, MethodInfo proceedMethod):this(original, newMethod)
    {
        this.proceedMethod = proceedMethod;
    }

    public object[] Arguments => original.Arguments;

    public Type[] GenericArguments => original.GenericArguments;

    public object InvocationTarget => original.InvocationTarget;

    public MethodInfo Method { get; }

    public MethodInfo MethodInvocationTarget => original.MethodInvocationTarget;

    public object Proxy => original.Proxy;

    public object? ReturnValue { get => original.ReturnValue; set => original.ReturnValue = value; }

    public Type TargetType => original.TargetType;

    public IInvocationProceedInfo CaptureProceedInfo() => original.CaptureProceedInfo();
    public object GetArgumentValue(int index) => original.GetArgumentValue(index);

    public MethodInfo GetConcreteMethod() => original.GetConcreteMethod();

    public MethodInfo GetConcreteMethodInvocationTarget() => original.GetConcreteMethodInvocationTarget();

    public void Proceed()
    {
        if (proceedMethod is null)
        {
            original.Proceed();
            return;
        }

        try
        {
            var met = proceedMethod.IsGenericMethodDefinition ? proceedMethod.MakeGenericMethod(GenericArguments) : proceedMethod;
            ReturnValue = met.Invoke(Proxy, Arguments);
        }
        catch (TargetInvocationException ex) // Reflection rethrows any errors as `TargetInvocationException` so get the inner exception
        {
            throw ex.InnerException ?? ex;
        }
        catch
        {
            original.Proceed();
        }
    }

    public void SetArgumentValue(int index, object value) => original.SetArgumentValue(index, value);
}
