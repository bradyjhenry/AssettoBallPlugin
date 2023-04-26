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
            FrictionCoefficient = 1f,
            MaximumRecoveryVelocity = 2,
            SpringSettings = new SpringSettings(30, 1)
        };
    }

    public void AddToSimulation(Simulation simulation, CollidableProperty<SimpleMaterial> collidableMaterials)
    {

        // Add a floor to the simulation.
        var floor = new Box(_floorWidth, 1, _floorLength);
        var floorIndex = simulation.Shapes.Add(floor);
        var floorPose = new RigidPose(new Vector3(0, -0.5f, 0)); // Position the floor at 0

        collidableMaterials.Allocate(simulation.Statics.Add(new StaticDescription(floorPose, floorIndex))) = Material;

        var wallThickness = 1;

        var cornerRadius = 10;

        // Left wall
        AddWall(simulation, new Vector3(-_floorWidth / 2 - wallThickness / 2, _wallHeight / 2, 0), new Vector3(wallThickness, _wallHeight, _floorLength));

        // Right wall
        AddWall(simulation, new Vector3(_floorWidth / 2 + wallThickness / 2, _wallHeight / 2, 0), new Vector3(wallThickness, _wallHeight, _floorLength));

        // Back wall
        AddWall(simulation, new Vector3(0, _wallHeight / 2, -_floorLength / 2 - wallThickness / 2), new Vector3(_floorWidth, _wallHeight, wallThickness));

        // Front wall
        AddWall(simulation, new Vector3(0, _wallHeight / 2, _floorLength / 2 + wallThickness / 2), new Vector3(_floorWidth, _wallHeight, wallThickness));

        AddInnerRoundedCorner(simulation, new Vector3(-_floorWidth / 2 + cornerRadius, _wallHeight / 2, -_floorLength / 2 + cornerRadius), cornerRadius, _wallHeight, Math.PI);
        AddInnerRoundedCorner(simulation, new Vector3(-_floorWidth / 2 + cornerRadius, _wallHeight / 2, _floorLength / 2 - cornerRadius), cornerRadius, _wallHeight, -Math.PI / 2);
        AddInnerRoundedCorner(simulation, new Vector3(_floorWidth / 2 - cornerRadius, _wallHeight / 2, -_floorLength / 2 + cornerRadius), cornerRadius, _wallHeight, Math.PI / 2);
        AddInnerRoundedCorner(simulation, new Vector3(_floorWidth / 2 - cornerRadius, _wallHeight / 2, _floorLength / 2 - cornerRadius), cornerRadius, _wallHeight, 0);
    }

    private void AddWall(Simulation simulation, Vector3 position, Vector3 dimensions, Quaternion? rotation = null)
    {
        var wallShape = new Box(dimensions.X, dimensions.Y, dimensions.Z);
        var wallShapeIndex = simulation.Shapes.Add(wallShape);

        var wallPose = new RigidPose
        {
            Position = position,
            Orientation = rotation ?? Quaternion.Identity
        };

        var wallDescription = new StaticDescription
        {
            Pose = wallPose,
            Shape = wallShapeIndex,
        };

        simulation.Statics.Add(wallDescription);
    }


    private void AddInnerRoundedCorner(Simulation simulation, Vector3 cornerCenter, float cornerRadius, float wallHeight, double angleOffset)
    {
        int numSegments = 10;
        double angleIncrement = Math.PI / 2 / numSegments;

        for (int i = 0; i < numSegments; i++)
        {
            double angle = angleOffset + i * angleIncrement;
            Vector3 segmentPosition = cornerCenter + new Vector3((cornerRadius - 0.5f) * (float)Math.Cos(angle), 0, (cornerRadius - 0.5f) * (float)Math.Sin(angle));
            Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -(float)angle + (float)Math.PI / 2);

            AddWall(simulation, segmentPosition, new Vector3(cornerRadius * (float)angleIncrement, wallHeight, 1), rotation);
        }
    }

}
