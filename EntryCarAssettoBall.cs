using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using AssettoServer.Network.Packets.Incoming;
using AssettoServer.Network.Packets.Shared;
using AssettoServer.Server;
using System.Numerics;
using Serilog;

namespace AssettoBallPlugin;


public class EntryCarAssettoBall
{

    public EntryCar EntryCar { get; }

    public BodyHandle HitboxHandle { get; private set; }

    public Quaternion Rotation { get; private set; }


    public EntryCarAssettoBall(EntryCar entryCar)
    {
        EntryCar = entryCar;
        EntryCar.PositionUpdateReceived += OnPositionUpdateReceived;
        EntryCar.ResetInvoked += OnResetInvoked;
    }

    public void InitializeHitbox(Simulation simulation)
    {
        // Define the hitbox shape (e.g., a box) and add it to the simulation
        var hitbox = new Box(2, 0.5f, 1); // Adjust the dimensions as needed
        var hitboxIndex = simulation.Shapes.Add(hitbox);

        // Set the initial position and orientation of the hitbox
        var hitboxPose = new RigidPose(EntryCar.Status.Position);

        // Set the initial mass and inertia of the hitbox
        float hitboxMass = 10.0f;
        var hitboxInertia = hitbox.ComputeInertia(hitboxMass);

        // Create a dynamic body for the hitbox and add it to the simulation
        HitboxHandle = simulation.Bodies.Add(BodyDescription.CreateKinematic(hitboxPose, new CollidableDescription(hitboxIndex, 0.1f), new BodyActivityDescription(0.01f)));
        Console.WriteLine($"Car hitbox handle: {HitboxHandle}");
    }

    public void UpdateHitbox(Simulation simulation)
    {
        var hitbox = simulation.Bodies.GetBodyReference(HitboxHandle);

        // Update the hitbox position
        hitbox.Pose.Position = EntryCar.Status.Position + new Vector3(0,1.5f,0);
        //Log.Debug($"Updated hitbox position: {hitbox.Pose.Position}");

        // Update the hitbox velocity
        hitbox.Velocity.Linear = EntryCar.Status.Velocity;
        //Log.Debug($"Updated hitbox velocity: {hitbox.Velocity.Linear}");
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
