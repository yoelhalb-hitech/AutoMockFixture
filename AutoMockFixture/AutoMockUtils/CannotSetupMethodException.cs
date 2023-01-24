
namespace AutoMockFixture.MockUtils;

public class CannotSetupMethodException
{
    public CannotSetupReason Reason { get; }
    public Exception? Exception { get; }

    public enum CannotSetupReason
    {
        NonVirtual,
        Private,
        Protected,
        CallBaseNoAbstract,
        TypeNotPublic,
        InterfaceMethodNotFound,
        Exception
    }

    public CannotSetupMethodException(CannotSetupReason reason, Exception? exception = null)
    {
        Reason = reason;
        Exception = exception;
    }
}
