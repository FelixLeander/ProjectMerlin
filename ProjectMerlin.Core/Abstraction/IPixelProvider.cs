using ProjectMerlin.Core.Models;
using System.Drawing;

namespace ProjectMerlin.Core.Abstraction;

public interface IPixelProvider
{
    public Color? MatchesMonitorColor(MonitorConfig monitorConfig);
}
