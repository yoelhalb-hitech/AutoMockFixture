
namespace AutoMoqExtensions;

internal static class Logger
{
    public static void LogInfo(string message)
    {
#if DEBUG
        Console.WriteLine(message);
#endif
    }
}
