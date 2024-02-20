namespace AssettoBallPlugin.GameStates;

public class InitializingState : IGameState
{

    public void Enter(GameContext context)
    {

    }

    public void Update(GameContext context)
    {
        foreach (var instance in context.Instances)
        {
            var client = instance.Value.EntryCar.Client;
            if (client == null || !client.HasSentFirstUpdate)
                continue;

            if (instance.Value.EntryCar.Status.EngineRpm > 2000)
            {
                context.StateChangeHandler.OnStateChangeRequest(GameState.Playing);
            }
        }
    }

    public void Exit(GameContext context)
    {
        // Logic to execute when exiting the Initializing state
    }

}
