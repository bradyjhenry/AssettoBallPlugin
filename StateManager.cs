using AssettoBallPlugin.GameStates;
using System.Collections.Generic;

namespace AssettoBallPlugin;

public class StateManager
{
    private Dictionary<State, IGameState> _states;
    private IGameState _currentState;
    private GameContext _gameContext;
    private GameManager _gameManager;

    public StateManager(GameContext gameContext, GameManager gameManager)
    {

        StateChangeHandler = stateChangeHandler;

        _gameContext = gameContext;
        _gameManager = gameManager;

        _states = new Dictionary<State, IGameState>
        {
            { State.Initializing, new InitializingState(_gameContext, _gameManager ) },
            { State.Playing, new PlayingState(_gameContext, _gameManager) },
        };

        _currentState = _states[State.Initializing];
        _currentState.Enter();
    }

    public void SetState(State newState)
    {
        _currentState.Exit();
        _currentState = _states[newState];
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState.Update();
    }

    public State GetCurrentState()
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
