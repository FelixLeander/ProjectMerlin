using Serilog;
using Serilog.Events;

namespace ProjectMerlin.Core.Business;
public static class Helper
{
    /// <summary>
    /// The logger used for the internaal logic.
    /// </summary>
    public static ILogger Logger { get; set; } =
#if DEBUG
        new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.File("logs/log.log", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
        .CreateLogger();
#else
        Serilog.Core.Logger.None;
#endif
}
