namespace AssettoBallPlugin;

public interface IGameState
{
    event Action<State> RequestStateChange;

    void Enter();
    void Exit();
    void Update();
}
