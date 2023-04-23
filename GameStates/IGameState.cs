namespace AssettoBallPlugin;

public interface IGameState
{
    void Enter(GameContext context);
    void Exit(GameContext context);
    void Update(GameContext context);
}
