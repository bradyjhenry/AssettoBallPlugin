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

    private BodyReference _ballBody;

    public void Enter(GameContext context)
    {

        Log.Debug("We're Playing now");
        _bufferPool = new BufferPool();
        var collidableMaterials = new CollidableProperty<SimpleMaterial>();
        var basicCallbacks = new BasicNarrowPhaseCallbacks { CollidableMaterials = collidableMaterials };

        _simulation = Simulation.Create(_bufferPool, basicCallbacks, new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));

        _stage = new GameStage(context.Configuration.GameStage.MeshOBJ);

        _ball = new GameBall(context.Configuration.GameBall.Radius, context.Configuration.GameBall.StartingPosition);

        _ball.AddToSimulation(_simulation, basicCallbacks.CollidableMaterials);
        _stage.AddToSimulation(_simulation, basicCallbacks.CollidableMaterials);

        _ballBody = _simulation.Bodies.GetBodyReference(_ball.BodyHandle);

        foreach (var instance in context.Instances)
        {
            if (instance.Value.EntryCar.Client != null)
            {
                instance.Value.Initialize(_simulation);
            }
        }

    }

    public void Update(GameContext context)
    {
        var spherePosition = _ballBody.Pose.Position;
        var sphereVelocity = _ballBody.Velocity.Linear;


        // Check if the sphere's position is out of bounds
        if (IsOutOfBounds(spherePosition))
        {
            // Reset the sphere's position and velocity
            _ballBody.Pose.Position = context.Configuration.GameBall.StartingPosition;
            _ballBody.Velocity.Linear = Vector3.Zero;
        }

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

    private bool IsOutOfBounds(Vector3 position)
    {
        // Define the bounds based on the field size, with (0, 0) at the center.
        float halfWidth = 110; // Half of the 50 units width
        float halfHeight = 53; // Half of the 100 units height
        // Check bounds along the x and y axes
        return position.X < -halfWidth || position.X > halfWidth ||
               position.Z < -halfHeight || position.Z > halfHeight;
    }

    public void Exit(GameContext context)
    {
        // Logic to execute when exiting the Initializing state
    }

}
