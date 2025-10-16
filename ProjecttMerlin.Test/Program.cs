using ProjectMerlin.Core.Business;

var bpm = new ButtplugManager("TestName", true);
Console.WriteLine($"Starting as '{bpm.ClientName}'");

var devices = await bpm.ConnectToServerAsync();
if (devices == null)
{
    Console.WriteLine("Connecting to server failed.");
    return 1;
}


var monitorManager = new MonitorManager();
var config = monitorManager.LoadMonitorConfigFromDatabase();


return 0;
