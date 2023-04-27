using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using AssettoServer.Network.Packets.Incoming;
using AssettoServer.Network.Packets.Shared;
using AssettoServer.Server;
using System.Numerics;
using Serilog;
using AssettoServer.Network.Tcp;
using BepuUtilities;


namespace AssettoBallPlugin;

public class EntryCarAssettoBall
{

    public EntryCar EntryCar { get; }

    public BodyHandle HitboxHandle { get; private set; } = default;

    public Quaternion Rotation { get; private set; }

    public event EventHandler? ClientFirstUpdateSent;


    public EntryCarAssettoBall(EntryCar entryCar)
    {
        EntryCar = entryCar;
        EntryCar.PositionUpdateReceived += OnPositionUpdateReceived;

        if (EntryCar.Client != null)
        {
            EntryCar.Client.FirstUpdateSent += OnClientFirstUpdateSent;
        }
    }
    private void OnClientFirstUpdateSent(ACTcpClient sender, EventArgs e)
    {
        ClientFirstUpdateSent?.Invoke(this, EventArgs.Empty);
    }

    public void Initialize(Simulation simulation)
    {
        if (EntryCar.Client != null && EntryCar.Client.HasSentFirstUpdate)
        {
            Log.Debug("Initialized smile");
            InitializeHitbox(simulation);
        }
    }

    public void InitializeHitbox(Simulation simulation)
    {
        // Define the hitbox shape (e.g., a box) and add it to the simulation
        var hitbox = new Box(4f, 1f, 2f); // Adjust the dimensions as needed
        var hitboxIndex = simulation.Shapes.Add(hitbox);

        // Set the initial position and orientation of the hitbox
        var hitboxPose = new RigidPose(EntryCar.Status.Position + new Vector3(0,1,0));

        // Create a dynamic body for the hitbox and add it to the simulation
        HitboxHandle = simulation.Bodies.Add(BodyDescription.CreateKinematic(hitboxPose, new CollidableDescription(hitboxIndex, 0.1f), new BodyActivityDescription(0.01f)));
        Console.WriteLine($"Car hitbox handle: {HitboxHandle}");
    }

    public void UpdateHitbox(Simulation simulation)
    {
        var position = EntryCar.Status.Position + new Vector3(0, 1, 0);
        var rotation = EntryCar.Status.Rotation;

        var hitbox = simulation.Bodies.GetBodyReference(HitboxHandle);

        hitbox.Awake = true;

        // Update the hitbox position
        hitbox.Pose.Position = position;

        hitbox.Velocity.Linear = EntryCar.Status.Velocity;

        float yaw = MathHelper.ToRadians(rotation.Y);
        float pitch = MathHelper.ToRadians(rotation.X);
        float roll = MathHelper.ToRadians(rotation.Z);

        Quaternion quaternionRotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

        // Update the hitbox rotation
        hitbox.Pose.Orientation = quaternionRotation;

    }

    private void OnPositionUpdateReceived(EntryCar sender, in PositionUpdateIn positionUpdateIn)
    {
    }
   
    private void SendMessage(EntryCar car, string message)
    {
        car.Client?.SendPacket(new ChatMessage { SessionId = 255, Message = message });
    }
}
