using AssettoBallPlugin.GameStates;
using System.Collections.Generic;

namespace AssettoBallPlugin;

public class GameStateManager
{
    private Dictionary<GameState, IGameState> _states;
    private IGameState _currentState;
    public GameContext Context { get; }

    public GameStateManager(GameContext context)
    {
        Context = context;

        _states = new Dictionary<GameState, IGameState>
        {
            { GameState.Initializing, new InitializingState() },
            { GameState.Playing, new PlayingState() },
        };

        _currentState = _states[GameState.Initializing];
        _currentState.Enter(Context);
    }

    public void SetState(GameState newState)
    {
        _currentState.Exit(Context);
        _currentState = _states[newState];
        _currentState.Enter(Context);
    }

    public void Update()
    {
        _currentState.Update(Context);
    }

    public GameState GetCurrentState()
    {
        foreach (var state in _states)
        {
            if (state.Value == _currentState)
            {
                return state.Key;
            }
        }

        throw new InvalidOperationException("Current state not found in the state dictionary.");
    }
}
