using Serilog;

namespace ECommerceSharedLibrary.Logs;

public static class LogException
{
    public static void LogExceptions(Exception ex)
    {
        LogToFile(ex.Message);
        LogToConsole(ex.Message);
        LogToDebugger(ex.Message);
    }

    public static void LogToDebugger(string exMessage)
        => Log.Debug(exMessage);

    public static void LogToConsole(string exMessage)
        => Log.Warning(exMessage);

    public static void LogToFile(string exMessage)
        => Log.Information(exMessage);

}