

namespace AssettoBallPlugin.GameStates;

public class InitializingState : IGameState
{
    private GameContext _gameContext;
    private GameManager _gameManager;

    public event Action<State> RequestStateChange;
    public InitializingState(GameContext gameContext, GameManager gameManager)
    {
        _gameContext = gameContext;
        _gameManager = gameManager;
    }

    public void Enter()
    {
        _gameManager.InitializeGame();
    }

    public void Update()
    {
        foreach (var instance in _gameContext.Instances)
        {
            var client = instance.Value.EntryCar.Client;
            if (client == null || !client.HasSentFirstUpdate)
                continue;

            if (instance.Value.EntryCar.Status.EngineRpm > 2000)
            {
                RequestStateChange?.Invoke(State.Playing);
            }
        }
    }

    public void Exit()
    {
        // Logic to execute when exiting the Initializing state
    }

}
