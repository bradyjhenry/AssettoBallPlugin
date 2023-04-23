using BepuPhysics;

namespace AssettoBallPlugin;
public class GameContext
{
    public AssettoBallConfiguration Configuration { get; }
    public Dictionary<int, EntryCarAssettoBall> Instances { get; private set; }
    public IGameStateChangeHandler StateChangeHandler { get; }
    public GameContext(IGameStateChangeHandler stateChangeHandler, AssettoBallConfiguration configuration, Dictionary<int, EntryCarAssettoBall> instances)
    {
        StateChangeHandler = stateChangeHandler;
        Configuration = configuration;
        Instances = instances;
    }

    public void UpdateInstances(Dictionary<int, EntryCarAssettoBall> instances)
    {
        Instances = instances;
    }
}
