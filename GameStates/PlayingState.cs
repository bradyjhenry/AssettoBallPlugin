using AssettoBallPlugin.Packets;
using AssettoServer.Network.Tcp;
using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Numerics;

namespace AssettoBallPlugin.GameStates;

public class PlayingState : IGameState
{
    private BufferPool? _bufferPool;

    private Simulation? _simulation;

    private GameStage? _stage;

    private GameBall? _ball;

    public void Enter(GameContext context)
    {

        Log.Debug("We're Playing now");
        _bufferPool = new BufferPool();
        var collidableMaterials = new CollidableProperty<SimpleMaterial>();
        var basicCallbacks = new BasicNarrowPhaseCallbacks { CollidableMaterials = collidableMaterials };

        _simulation = Simulation.Create(_bufferPool, basicCallbacks, new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));

        _stage = new GameStage(context.Configuration.GameStage.Width, context.Configuration.GameStage.Length, context.Configuration.GameStage.Height);

        _ball = new GameBall(context.Configuration.GameBall.Radius, context.Configuration.GameBall.StartingPosition);

        _ball.AddToSimulation(_simulation, basicCallbacks.CollidableMaterials);
        _stage.AddToSimulation(_simulation, basicCallbacks.CollidableMaterials);

        foreach (var instance in context.Instances)
        {
            instance.Value.Initialize(_simulation);
        }

    }

    public void Update(GameContext context)
    {
        var sphereBodyReference = _simulation.Bodies.GetBodyReference(_ball.BodyHandle);
        var spherePosition = sphereBodyReference.Pose.Position;
        var sphereVelocity = sphereBodyReference.Velocity.Linear;

        foreach (var instance in context.Instances)
        {
            var client = instance.Value.EntryCar.Client;
            if (client == null || !client.HasSentFirstUpdate)
                continue;

            instance.Value.UpdateHitbox(_simulation);

            client.SendPacket(new AssettoBallPosition { Position = spherePosition, Velocity = sphereVelocity });
        }

        _ball.KeepAwake(_simulation);

        _simulation.Timestep(1.0f / 60f);

    }

    public void Exit(GameContext context)
    {
        // Logic to execute when exiting the Initializing state
    }

}
