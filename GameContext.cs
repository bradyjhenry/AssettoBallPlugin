using BepuPhysics;

namespace AssettoBallPlugin;
public class GameContext
{
    public AssettoBallConfiguration Configuration { get; }
    public Dictionary<int, GameEntryCar> Instances { get; private set; }

    public GameContext(AssettoBallConfiguration configuration, Dictionary<int, GameEntryCar> instances)
    {
        Configuration = configuration;
        Instances = instances;
    }

    public void UpdateInstances(Dictionary<int, GameEntryCar> instances)
    {
        Instances = instances;
    }
}
