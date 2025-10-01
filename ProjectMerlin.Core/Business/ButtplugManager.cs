using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buttplug.Client;
using Buttplug.Core;
using Buttplug.Core.Messages;
using Serilog.Events;

namespace ProjectMerlin.Core.Business;
public sealed class ButtplugManager
{
    private readonly ButtplugClient _buttplugClient = new(nameof(ProjectMerlin));

    /// <summary>
    /// Connectts to the server, which manages the devices.
    /// </summary>
    /// <param name="domain">The address to connect to.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ButtplugClientDevice"/>.</returns>
    /// <remarks>https://intiface.com/central/</remarks>
    public async Task<ObservableCollection<ButtplugClientDevice>?> ConnectToServerAsync(string domain = "127.0.0.1:12345")
    {
        try
        {
            var url = $"ws://{domain}";
            var connector = new ButtplugWebsocketConnector(new(url));

            Helper.Logger.Verbose("Connecting to server at '{address}'.", url);
            await _buttplugClient.ConnectAsync(connector);
            Helper.Logger.Information("Connecting to server at '{address}'.", url);

            ObservableCollection<ButtplugClientDevice> devices = new(_buttplugClient.Devices);
            Helper.Logger.Verbose("Inital connected devices {connected}.", devices.Count);
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                Helper.Logger.Verbose("Device: '{deviceName}' '{deviceInedx}'.", device.Name, device.Index);
            }

            _buttplugClient.DeviceAdded += (_, e) =>
            {
                Helper.Logger.Verbose("Adding device '{deviceName}' '{deviceInedx}'.", e.Device.Name, e.Device.Index);
                devices.Add(e.Device);
            };
            _buttplugClient.DeviceRemoved += (_, e) =>
            {
                Helper.Logger.Verbose("Adding devices '{deviceName}' '{deviceInedx}'.", e.Device.Name, e.Device.Index);
                devices.Remove(e.Device);
            };

            return devices;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error connecting to server.");
            return null;
        }
    }

    /// <summary>
    /// Starts scanning for devices.
    /// </summary>
    /// <param name="cancellationToken">An optinal <see cref="CancellationToken"/> to force stop scanning.</param>
    /// <returns>A <see cref="Task"/> representing this Methods work.</returns>
    public async Task StartScaanningAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            Helper.Logger.Verbose("Start scanning for devices.");
            await _buttplugClient.StartScanningAsync(cancellationToken);
            Helper.Logger.Verbose("Finished scanning for devices.");
        }
        catch (TaskCanceledException ex)
        {
            Helper.Logger.Error(ex, "Canceled task.");
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Unexpected error while scanning for devices.");
        }
    }
}
