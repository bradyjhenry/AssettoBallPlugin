using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using AssettoServer.Network.Packets.Incoming;
using AssettoServer.Network.Packets.Shared;
using AssettoServer.Server;
using BepuPhysics.Collidables;

namespace AssettoBallPlugin;

public class EntryCarAssettoBall
{

    private Simulation _simulation { set; get; }

    public EntryCar EntryCar { get; }

    public BodyHandle bodyHandle { get; set; }


    public EntryCarAssettoBall(EntryCar entryCar, Simulation simulation)
    {
        EntryCar = entryCar;
        EntryCar.PositionUpdateReceived += OnPositionUpdateReceived;
        EntryCar.ResetInvoked += OnResetInvoked;

        _simulation = simulation;

        var box = new Box(1, 1, 1);
        var boxIndex = _simulation.Shapes.Add(box);
        var boxPose = new RigidPose(EntryCar.Status.Position);
        float boxMass = 1.0f;
        bodyHandle = _simulation.Bodies.Add(BodyDescription.CreateDynamic(boxPose, box.ComputeInertia(boxMass), new CollidableDescription(boxIndex, 0.1f), new BodyActivityDescription(0.01f)));

    }

    private void OnResetInvoked(EntryCar sender, EventArgs args)
    {

    }

    private void OnPositionUpdateReceived(EntryCar sender, in PositionUpdateIn positionUpdateIn)
    {
        var velocity = EntryCar.Status.Velocity;
        var speed = (float)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y + velocity.Z * velocity.Z);
    }

    private void SendMessage(EntryCar car, string message)
    {
        car.Client?.SendPacket(new ChatMessage { SessionId = 255, Message = message });
    }
}
