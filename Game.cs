using Qmmands.Delegates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssettoBallPlugin
{
    public class Game
    {

        private StateManager _stateManager;
        private GameContext _gameContext;
        private GameManager _gameManager;
        private Dictionary<int, GameEntryCar>? _instances;

        public Game(AssettoBallConfiguration configuration)  {
            _gameContext = new GameContext(configuration, _instances);

            _gameManager = new GameManager(_gameContext);

            _stateManager = new StateManager(_gameContext, _gameManager);
        }

        public void SetInstances(Dictionary<int, GameEntryCar> instances)
        {
            _instances = instances;
            _gameContext.UpdateInstances(_instances);
        }

        public void StateChangeRequest(State newState)
        {
            _stateManager.SetState(newState);
        }

        public void Update()
        {
            _stateManager.Update();
        }
    }
}
