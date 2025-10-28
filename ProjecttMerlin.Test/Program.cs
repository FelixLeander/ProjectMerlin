using System.Runtime.InteropServices;
using ProjectMerlin.Core.Business;

DatabaseManager.Reset();

var monitorManager = new MonitorManager();
monitorManager.AddMonitorConfig

var bpm = new ButtplugManager("TestName", true);
var devices = await bpm.ConnectToServerAsync();
if (devices == null)
{
    Console.WriteLine("Connecting to server failed.");
    return 1;
}

await bpm.StartScanningAsync();

while (true)
{
    Console.ReadLine();
}

return 0;
