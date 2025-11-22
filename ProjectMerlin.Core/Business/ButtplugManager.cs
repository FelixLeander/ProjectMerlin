using Buttplug.Client;
using ProjectMerlin.Core.Models;
using Serilog;
using System.Collections.ObjectModel;

namespace ProjectMerlin.Core.Business;

/// <summary>
/// This class handles all communication with the IntiFace-Central-Server.
/// </summary>
/// <param name="clientName">The Name this instance will use to communicate with IntiFace-Central.</param>
/// <param name="addRandomPostfix">Controls if a bas64 encoded <see cref="Guid"/> should be added as postfix (seperated by a dash).
/// Example.: MyAppName-6a7960ce21a44dafb079c018bf557da5</param>
public sealed class ButtplugManager(string clientName = nameof(ProjectMerlin), bool addRandomPostfix = true)
{
    /// <summary>
    /// Gets the Name used to connect to IntiFace-Central-Server.
    /// </summary>
    /// <remarks>Can only be set via. The <see cref="ButtplugManager"/> constructor.</remarks>
    public string ClientName => _buttplugClient.Name;

    /// <summary>
    /// The true heart of this project.
    /// Thanks to you guys, I don't have to deal with this.
    /// Thanks to that, I can concentrate on the truly important stuff; developing this.
    /// </summary>
    private readonly ButtplugClient _buttplugClient = new
        (
            addRandomPostfix
            ? clientName
            : $"{clientName}-{Guid.NewGuid():n}"
        );


    /// <summary>
    /// Connects to the server, which manages the devices.
    /// </summary>
    /// <param name="uri">The address where IntiFace-Central is served.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ButtplugClientDevice"/>.</returns>
    /// <remarks>https://intiface.com/central/</remarks>
    /// <exception cref="Exception">Check the inner exception.</exception>
    public async Task<IReadOnlyCollection<ButtplugClientDevice>?> ConnectToServerAsync(string uri = "ws://127.0.0.1:12345")
    {
        try
        {
            var connector = new ButtplugWebsocketConnector(new(uri));

            Helper.Logger.Verbose("Connecting to server at '{address}'.", uri);
            await _buttplugClient.ConnectAsync(connector);
            Helper.Logger.Information("Connecting to server at '{address}'.", uri);

            var observableDevices = new ObservableCollection<ButtplugClientDevice>(_buttplugClient.Devices);
            Helper.Logger.Verbose("Initial connected devices {connected}.", observableDevices.Count);
            foreach (var device in observableDevices)
                Helper.Logger.Verbose("Device: '{deviceName}' '{deviceIndex}'.", device.Name, device.Index);

            _buttplugClient.DeviceAdded += (_, e) =>
            {
                Helper.Logger.Verbose("Adding device '{deviceName}' '{deviceIndex}'.", e.Device.Name, e.Device.Index);
                observableDevices.Add(e.Device);
            };
            _buttplugClient.DeviceRemoved += (_, e) =>
            {
                Helper.Logger.Verbose("Adding devices '{deviceName}' '{deviceIndex}'.", e.Device.Name, e.Device.Index);
                observableDevices.Remove(e.Device);
            };

            return observableDevices;
        }
        catch (Exception ex)
        {
            Helper.Logger.Error(ex, "Error connecting to server.");
            throw new Exception("Connecting to server failed.", ex);
        }
    }

    /// <summary>
    /// Starts scanning for devices.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to force stop scanning.</param>
    /// <returns>A <see cref="Task"/> representing this Methods work.</returns>
    public async Task StartScanningAsync(CancellationToken cancellationToken = default)
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

    public ButtplugClientDevice? GetMatchingDevice(TriggerAction triggerAction)
    {
        var device = _buttplugClient.Devices.FirstOrDefault(f => f.Name.Equals(triggerAction.Name, StringComparison.OrdinalIgnoreCase));
        if (device == null)
            Helper.Logger.Warning("Found no device for {trigerAction} '{name}'.", nameof(TriggerAction), triggerAction.Name);
        else
            Helper.Logger.Verbose("Found device {index} '{name}'.", device.Index, triggerAction.Name);
        return device;
    }

    public ButtplugClientDevice[] GetCurrentDevices() => _buttplugClient.Devices;
}
