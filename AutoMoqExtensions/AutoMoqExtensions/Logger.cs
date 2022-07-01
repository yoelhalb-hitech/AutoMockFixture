using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions
{
    internal static class Logger
    {
        public static void LogInfo(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
