using ProjectMerlin.Core.Abstraction;
using ProjectMerlin.Core.Models;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ProjectMerlin.Core.Business;

public sealed class OsSpecific : IPixelProvider
{
    private enum Os
    {
        None,
        Windows,
        LinuxGrep
    }

    private readonly Os os;
    public OsSpecific()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            os = Os.LinuxGrep;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            os = Os.Windows;
        else
            throw new Exception($"Unsupported OS: {RuntimeInformation.OSDescription}");
    }

    public Color? GetPixelColor(MonitorConfig monitorConfig)
    {
        if (os == Os.LinuxGrep)
            return LinuxGetPixel(monitorConfig.PosX, monitorConfig.PosY);

        if (os == Os.Windows)
            return WindowsGetPixel(monitorConfig.PosX, monitorConfig.PosY);

        throw new PlatformNotSupportedException($"'{RuntimeInformation.OSDescription}' isn't supported yet.");
    }

    private static Color WindowsGetPixel(int x, int y)
    {
        var nintZero = IntPtr.Zero;
        var desktopDc = GetDC(nintZero);
        var pixel = GetPixel(desktopDc, x, y);
        _ = ReleaseDC(nintZero, desktopDc);

        var pixelWithAlpha = (int)(pixel + 0xFF000000);
        return Color.FromArgb(pixelWithAlpha);

        // Gets the deviceContext for the desktops. IntPtr.Zero for all desktops
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        // Gets the pixel from the desktop.
        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        // Releases the deviceContext, thus avoiding leaks.
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }

    private static Color LinuxGetPixel(int x, int y)
    {
        var process = Process.Start("grim", @$"-g ""{x},{y} 1x1"" - | convert - -format ""%[pixel:p{{0,0}}]"" info:");
        var srgbText = process.StandardOutput.ReadToEnd();

        var openPos = srgbText.IndexOf('(') + 1;
        var closePos = srgbText.IndexOf(')', openPos);

        var length = closePos - openPos;
        var result = srgbText.Substring(openPos, length);

        var rgb = result.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
        return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
    }
}
