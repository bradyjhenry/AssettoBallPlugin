using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using Serilog;
using System.Globalization;
using System.Numerics;
using Mesh = BepuPhysics.Collidables.Mesh;

namespace AssettoBallPlugin;
public class GameStage
{
    // Stage properties
    private string _meshfilepath {  get; set; }

    public StaticHandle StaticHandle { get; set; }

    public SimpleMaterial Material { get; set; }

    public GameStage(string meshfilename)
    {
        string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string basePath = Path.GetDirectoryName(assemblyLocation);
        string relativePath = Path.Combine(basePath, "Res", meshfilename);

        if (File.Exists(relativePath))
        {
            _meshfilepath = relativePath;
        }
        else
        {
            Log.Debug(relativePath);
            throw new Exception("Stage file not found!");
        }


        Material = new SimpleMaterial
        {
            FrictionCoefficient = 1f,
            MaximumRecoveryVelocity = 2,
            SpringSettings = new SpringSettings(30, 1)
        };
    }

    public void AddToSimulation(Simulation simulation, CollidableProperty<SimpleMaterial> collidableMaterials)
    {
        BufferPool pool = new BufferPool();
        Mesh meshContent = LoadMeshFromObj(_meshfilepath, pool);
        var staticShapeIndex = simulation.Shapes.Add(meshContent);

        var staticDescription = new StaticDescription
        {
            Shape = staticShapeIndex,
            Pose = new RigidPose
            {
                Position = new Vector3(0, 0, 0),
            }
        };
        StaticHandle = simulation.Statics.Add(staticDescription);
    }

    public static Mesh LoadMeshFromObj(string filePath, BufferPool pool)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<Triangle>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith("v "))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vertex = new Vector3(
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                    float.Parse(parts[3], CultureInfo.InvariantCulture));
                vertices.Add(vertex);
            }
            else if (line.StartsWith("f "))
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var index0 = int.Parse(parts[1].Split('/')[0]) - 1;
                var index1 = int.Parse(parts[2].Split('/')[0]) - 1;
                var index2 = int.Parse(parts[3].Split('/')[0]) - 1;

                var triangle = new Triangle(vertices[index0], vertices[index1], vertices[index2]);
                triangles.Add(triangle);
            }
        }

        // Convert list to buffer for BepuPhysics
        pool.Take(triangles.Count, out Buffer<Triangle> triangleBuffer);
        for (int i = 0; i < triangles.Count; i++)
        {
            triangleBuffer[i] = triangles[i];
        }

        var mesh = new Mesh(triangleBuffer, new Vector3(1, 1, 1), pool);

        // Remember to return the buffer when done with the mesh if you're not storing it in a simulation.
        // This example assumes you'll add the mesh to a simulation and manage its lifetime accordingly.

        return mesh;
    }
}
