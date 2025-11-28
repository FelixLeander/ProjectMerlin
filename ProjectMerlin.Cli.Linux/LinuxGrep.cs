using ProjectMerlin.Core.Abstraction;
using ProjectMerlin.Core.Models;
using System.Drawing;

namespace ProjectMerlin.Cli.Linux;

/// <summary>
/// PAIN PEKO
/// </summary>
public sealed class LinuxGrep : IPixelProvider
{
    public Color? GetPixelColor(MonitorConfig monitorConfig)
    {
        try
        {
            // DO NOT...I repeat DO NOT remove the triple backslashes...Don't ask. It's magic
            var args = new string[]
            {
                "-c",
                $"\"grim -g \\\"{monitorConfig.PosX},{monitorConfig.PosY} 1x1\\\" - | magick - -format \\\"%[pixel:p{{0,0}}]\\\" info:\""
            };

            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = string.Join(' ', args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = System.Diagnostics.Process.Start(processStartInfo);
            if (process == null)
                return null;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrWhiteSpace(output))
                return null;

            string pixelInfo = output.Trim().ToLowerInvariant();

            if (pixelInfo.StartsWith("srgb(") && pixelInfo.EndsWith(')'))
            {
                var inner = pixelInfo[5..^1]; // content inside srgb(...)
                var parts = inner.Split(',');

                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int r) &&
                    int.TryParse(parts[1], out int g) &&
                    int.TryParse(parts[2], out int b))
                {
                    return Color.FromArgb(r, g, b);
                }
            }
            else if (pixelInfo.StartsWith('#') && (pixelInfo.Length == 7 || pixelInfo.Length == 9))
            {
                try
                {
                    return ColorTranslator.FromHtml(pixelInfo);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
