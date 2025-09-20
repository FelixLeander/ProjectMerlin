using Uno.UI.Hosting;

namespace ProjectMerlin.Linux.Platforms.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        try
        {
            App.InitializeLogging();

            var host = UnoPlatformHostBuilder.Create()
                .App(() => new App())
                .UseX11()
                .UseLinuxFrameBuffer()
                .UseMacOS()
                .UseWin32()
                .Build();

            host.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
