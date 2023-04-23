namespace AssettoBallPlugin;

public interface IGameStateChangeHandler
{
    void OnStateChangeRequest(GameState newState);
}
