using System;
using Serilog;

namespace Haus.Hosting
{
    public static class HausLogger
    {
        public static void Fatal(Exception exception, string message)
        {
            Log.Fatal(exception, message);
        }
        
        public static void EnsureFlushed()
        {
            Log.CloseAndFlush();
        }
    }
}