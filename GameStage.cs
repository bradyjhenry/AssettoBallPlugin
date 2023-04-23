using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using System.Numerics;

namespace AssettoBallPlugin;
public class GameStage
{
    // Stage properties
    private float _floorWidth { get; set; }
    private float _floorLength { get; set; }
    private float _wallHeight { get; set; }

    public SimpleMaterial Material { get; set; }

    public GameStage(float floorWidth, float floorLength, float wallHeight)
    {
        _floorWidth = floorWidth;
        _floorLength = floorLength;
        _wallHeight = wallHeight;

        Material = new SimpleMaterial
        {
            FrictionCoefficient = 1,
            MaximumRecoveryVelocity = 2,
            SpringSettings = new SpringSettings(30, 1)
        };
    }

    public void AddToSimulation(Simulation simulation, CollidableProperty<SimpleMaterial> collidableMaterials)
    {

        // Add a floor to the simulation.
        var floor = new Box(_floorWidth, 1, _floorLength);
        var floorIndex = simulation.Shapes.Add(floor);
        var floorPose = new RigidPose(new Vector3(0, -1, 0)); // Position the floor at 0

        collidableMaterials.Allocate(simulation.Statics.Add(new StaticDescription(floorPose, floorIndex))) = Material;

        var wallThickness = 1;

        // Left wall
        AddWall(simulation, new Vector3(-_floorWidth / 2 - wallThickness / 2, _wallHeight / 2, 0), new Vector3(wallThickness, _wallHeight, _floorLength));

        // Right wall
        AddWall(simulation, new Vector3(_floorWidth / 2 + wallThickness / 2, _wallHeight / 2, 0), new Vector3(wallThickness, _wallHeight, _floorLength));

        // Back wall
        AddWall(simulation, new Vector3(0, _wallHeight / 2, -_floorLength / 2 - wallThickness / 2), new Vector3(_floorWidth, _wallHeight, wallThickness));

        // Front wall
        AddWall(simulation, new Vector3(0, _wallHeight / 2, _floorLength / 2 + wallThickness / 2), new Vector3(_floorWidth, _wallHeight, wallThickness));
    }

    private void AddWall(Simulation simulation, Vector3 position, Vector3 dimensions)
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
}
