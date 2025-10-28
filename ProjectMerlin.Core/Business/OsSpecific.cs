using System.Drawing;
using System.Runtime.InteropServices;

namespace ProjectMerlin.Core.Business;

internal static class OsSpecific
{
    internal static async Task<Color> GetPixelAsync(int x, int y, CancellationToken cancellationToken = default)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return await LinuxGetPixelAsync(x, y, cancellationToken);

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return await WindowsGetPixelAsync(x, y, cancellationToken);

        else
            throw new PlatformNotSupportedException($"'{RuntimeInformation.OSDescription}' isn't supported yet.");
    }

    private static async Task<Color> WindowsGetPixelAsync(int x, int y, CancellationToken cancellationToken = default)
    {
#if !TARGET_WINDOWS
        return await Task.Run(() =>
        {
            var nintZero = IntPtr.Zero;
            var desktopDc = GetDC(nintZero);
            var pixel = GetPixel(desktopDc, x, y);
            _ = ReleaseDC(nintZero, desktopDc);

            var pixelWithAlpha = (int)(pixel + 0xFF000000);
            return Color.FromArgb(pixelWithAlpha);

        }, cancellationToken);


        // Gets the deviceContext for the desktops. IntPtr.Zero for all desktops
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        // Getts the pixel from the desktop.
        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        // Releases the deviceContext, thus avoiding leaks.
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
#else
        await Task.Delay(500, cancellationToken);
        return Color.FromArgb(0, 255, 0);
#endif
    }

    private static async Task<Color> LinuxGetPixelAsync(int x, int y, CancellationToken cancellationToken = default)
    {
#if TARGET_LINUX && !TARGET_ANDROID
        var process = Process.Start("grim", @$"-g ""{x},{y} 1x1"" - | convert - -format ""%[pixel:p{{0,0}}]"" info:");
        var srgbText = await process.StandardOutput.ReadToEndAsync(cancellationToken);
#else
        await Task.Delay(500, cancellationToken);
        var srgbText = "srgb(0,255,0)%";
#endif

        var openPos = srgbText.IndexOf('(') + 1;
        var closePos = srgbText.IndexOf(')', openPos);

        var length = closePos - openPos;
        var result = srgbText.Substring(openPos, length);

        var rgb = result.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
        return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
    }
}

