using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using System.Numerics;


namespace AssettoBallPlugin;
public class AssettoBallStage
{
    // Stage properties
    public float FloorWidth { get; }
    public float FloorLength { get; }
    public float WallHeight { get; }
    public float WallThickness { get; }

    // Sphere properties
    public float SphereRadius { get; }
    public Vector3 SphereInitialPosition { get; }
    public BodyHandle Ball { get; set; }

    public AssettoBallStage(float sphereRadius, Vector3 sphereInitialPosition)
    {
        SphereRadius = sphereRadius;
        SphereInitialPosition = sphereInitialPosition;
    }

    public void AddToSimulation(Simulation simulation, BufferPool bufferPool)
    {

        // Add a floor to the simulation.
        var floor = new Box(200, 1, 200);
        var floorIndex = simulation.Shapes.Add(floor);
        var floorPose = new RigidPose(new Vector3(0, -1, 0)); // Position the floor at 0
        simulation.Statics.Add(new StaticDescription(floorPose, floorIndex));

        // Add walls
        float floorWidth = 200.0f;
        float floorLength = 200.0f;
        float wallHeight = 20.0f;
        float wallThickness = 2.0f;

/*        // Left wall
        AddWall(simulation, bufferPool, new Vector3(-floorWidth / 2 - wallThickness / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, floorLength));

        // Right wall
        AddWall(simulation, bufferPool, new Vector3(floorWidth / 2 + wallThickness / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, floorLength));

        // Back wall
        AddWall(simulation, bufferPool, new Vector3(0, wallHeight / 2, -floorLength / 2 - wallThickness / 2), new Vector3(floorWidth, wallHeight, wallThickness));

        // Front wall
        AddWall(simulation, bufferPool, new Vector3(0, wallHeight / 2, floorLength / 2 + wallThickness / 2), new Vector3(floorWidth, wallHeight, wallThickness));*/


        // Add sphere
        AddSphere(simulation, bufferPool, SphereInitialPosition, SphereRadius);
        Console.WriteLine($"Ball handle: {Ball}");
    }

    private void AddWall(Simulation simulation, BufferPool bufferPool, Vector3 position, Vector3 dimensions)
    {
        var wallShape = new Box(dimensions.X, dimensions.Y, dimensions.Z);
        var wallShapeIndex = simulation.Shapes.Add(wallShape);

        var wallPose = new RigidPose
        {
            Position = position,
            Orientation = Quaternion.Identity
        };

        var wallDescription = new StaticDescription
        {
            Pose = wallPose,
            Shape = wallShapeIndex,
        };

        simulation.Statics.Add(wallDescription);
    }

    private void AddSphere(Simulation simulation, BufferPool bufferPool, Vector3 position, float radius)
    {
        var sphere = new Sphere(radius); // 0.5 is the radius of the sphere
        var sphereIndex = simulation.Shapes.Add(sphere);
        var spherePose = new RigidPose(position);

        float sphereMass = 1.0f;

        Ball = simulation.Bodies.Add(BodyDescription.CreateDynamic(spherePose, sphere.ComputeInertia(sphereMass), new CollidableDescription(sphereIndex, 0.1f), new BodyActivityDescription(0.01f)));
    }
}
