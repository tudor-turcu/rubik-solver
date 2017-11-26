using Microsoft.Extensions.Logging;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddConsole(LogLevel.Debug).AddDebug(LogLevel.Debug);

        public static ILogger CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();
    }
}
