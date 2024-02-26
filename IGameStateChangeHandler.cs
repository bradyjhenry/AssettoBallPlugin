namespace AssettoBallPlugin;

public interface IGameStateChangeHandler
{
    void OnStateChangeRequest(State newState);
}
