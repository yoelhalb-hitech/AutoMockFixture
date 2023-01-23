
namespace AutoMockFixture;

public static class Logger
{
    public static bool ShouldLog { get; set; }
    public static void LogInfo(string message)
    {
        if(ShouldLog)
            Console.WriteLine(message);
    }
}
