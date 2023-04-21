using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using AssettoServer.Network.Tcp;
using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using AssettoServer.Server.Plugin;
using AssettoServer.Utils;
using Microsoft.Extensions.Hosting;
using AssettoBallPlugin.Packets;
using Serilog;
using System.Reflection;
using BepuPhysics.CollisionDetection;
using System.Numerics;
using BepuUtilities;
using BepuPhysics.Constraints;

namespace AssettoBallPlugin;

public class AssettoBall : CriticalBackgroundService, IAssettoServerAutostart
{

    private readonly EntryCarManager _entryCarManager;

    private readonly ACServerConfiguration _serverConfiguration;

    private readonly Func<EntryCar, EntryCarAssettoBall> _entryCarFactory;

    private readonly Dictionary<int, EntryCarAssettoBall> _instances = new();

    private readonly AssettoBallConfiguration _configuration;

    private Simulation _simulation;

    private BufferPool _bufferPool;

    private AssettoBallStage _stage; 

    public AssettoBall(EntryCarManager entryCarManager, Func<EntryCar, EntryCarAssettoBall> entryCarFactory, AssettoBallConfiguration configuration, ACServerConfiguration serverConfiguration, CSPServerScriptProvider scriptProvider, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _entryCarManager = entryCarManager;
        _entryCarFactory = entryCarFactory;
        _serverConfiguration = serverConfiguration;
        _configuration = configuration;

        _bufferPool = new BufferPool();

        _simulation = Simulation.Create(_bufferPool, new BasicNarrowPhaseCallbacks(), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));

        _stage = new AssettoBallStage(1, new Vector3(0,10,0));

        _stage.AddToSimulation(_simulation, _bufferPool);

        Log.Debug("AssettoBall Loaded Baby");

        if (_serverConfiguration.Extra.EnableClientMessages)
        {
            using var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AssettoBallPlugin.lua.assettoballplugin.lua")!);
            scriptProvider.AddScript(streamReader.ReadToEnd(), "assettoballplugin.lua");
        }
        else
        {
            throw new ConfigurationException("AssettoBall: EnableClientMessages must be set to true in extra_cfg man!");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var entryCar in _entryCarManager.EntryCars)
        {
            var entryCarAssettoBall = _entryCarFactory(entryCar);
            _instances.Add(entryCar.SessionId, entryCarAssettoBall);
            entryCarAssettoBall.InitializeHitbox(_simulation);
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var sphereBodyReference = _simulation.Bodies.GetBodyReference(_stage.Ball);
                var spherePosition = sphereBodyReference.Pose.Position;

                foreach (var instance in _instances)
                {
                    var client = instance.Value.EntryCar.Client;
                    if (client == null || !client.HasSentFirstUpdate)
                        continue;

                    instance.Value.UpdateHitbox(_simulation);

                    var carRef = _simulation.Bodies.GetBodyReference(instance.Value.HitboxHandle);
                    var carPos = carRef.Pose.Position;

                    client.SendPacket(new AssettoBallPosition { Position = spherePosition });
                    Log.Debug(spherePosition.ToString() + "cooool" + carPos.ToString());

                }
                _simulation.Timestep(1.0f / 60f);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during assettoball update");
            }
            finally
            {
                await Task.Delay(16, stoppingToken);
            }
        }
    }
}
