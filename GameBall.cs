using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using System.Numerics;
using BepuUtilities;
using BepuPhysics.Constraints;



namespace AssettoBallPlugin;
public class GameBall
{
    // Sphere properties
    public float SphereRadius { get; }
    public Vector3 SphereInitialPosition { get; }
    public SimpleMaterial Material { get; set; }
    public BodyHandle BodyHandle { get; set; }

    public GameBall(float sphereRadius, Vector3 sphereInitialPosition)
    {
        Material = new SimpleMaterial
        {
            FrictionCoefficient = 0.2f,
            MaximumRecoveryVelocity = float.MaxValue,
            SpringSettings = new SpringSettings(10f, 0.001f)
        };
        SphereRadius = sphereRadius;
        SphereInitialPosition = sphereInitialPosition;
    }

    public void KeepAwake(Simulation simulation)
    {
        var bodyReference = simulation.Bodies.GetBodyReference(BodyHandle);
        if (!bodyReference.Awake)
        {
            bodyReference.Awake = true;
        }
    }

    public void AddToSimulation(Simulation simulation, CollidableProperty<SimpleMaterial> collidableMaterials)
    {
        var sphere = new Sphere(SphereRadius);
        var sphereIndex = simulation.Shapes.Add(sphere);
        var spherePose = new RigidPose(SphereInitialPosition);

        float sphereMass = 100.0f;


        BodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(spherePose, sphere.ComputeInertia(sphereMass), new CollidableDescription(sphereIndex, 0.1f), new BodyActivityDescription(sleepThreshold: 1e-9f, minimumTimestepCountUnderThreshold: 255)));
        collidableMaterials.Allocate(BodyHandle) = Material;
        Console.WriteLine($"Ball handle: {BodyHandle}");
    }
}
