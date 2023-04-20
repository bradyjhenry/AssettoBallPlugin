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

    private BodyHandle _sphereBodyHandle;

    public AssettoBall(EntryCarManager entryCarManager, Func<EntryCar, EntryCarAssettoBall> entryCarFactory, AssettoBallConfiguration configuration, ACServerConfiguration serverConfiguration, CSPServerScriptProvider scriptProvider, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _entryCarManager = entryCarManager;
        _entryCarFactory = entryCarFactory;
        _serverConfiguration = serverConfiguration;
        _configuration = configuration;

        var simulation = Simulation.Create(new BufferPool(), new DemoNarrowPhaseCallbacks(new SpringSettings(30, 1)), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));
        // Add a sphere to the simulation.
        var sphere = new Sphere(1f); // 0.5 is the radius of the sphere
        var sphereIndex = simulation.Shapes.Add(sphere);
        var spherePose = new RigidPose(new Vector3(0, 100, 0)); // Position the sphere 200 units above the floor

        float sphereMass = 1.0f;

        _sphereBodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(spherePose, sphere.ComputeInertia(sphereMass), new CollidableDescription(sphereIndex, 0.1f), new BodyActivityDescription(0.01f)));

        // Add a floor to the simulation.
        var floor = new Box(200, 1, 200);
        var floorIndex = simulation.Shapes.Add(floor);
        var floorPose = new RigidPose(new Vector3(0, 0, 0)); // Position the floor at 0
        simulation.Statics.Add(new StaticDescription(floorPose, floorIndex));

        _simulation = simulation;

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


    private void SendPacket(ACTcpClient client, Vector3 position)
    {
        client.SendPacket(new AssettoBallPosition { Position = position});
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var entryCar in _entryCarManager.EntryCars)
        {
            _instances.Add(entryCar.SessionId, _entryCarFactory(entryCar));
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var sphereBodyReference = _simulation.Bodies.GetBodyReference(_sphereBodyHandle);
                var spherePosition = sphereBodyReference.Pose.Position;

                foreach (var instance in _instances)
                {
                    var client = instance.Value.EntryCar.Client;
                    if (client == null || !client.HasSentFirstUpdate)
                        continue;


                    client.SendPacket(new AssettoBallPosition { Position = spherePosition });

                }

                _simulation.Timestep(1.0f / 33f);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during assettoball update");
            }
            finally
            {
                await Task.Delay(33, stoppingToken);
            }
        }
    }
}
