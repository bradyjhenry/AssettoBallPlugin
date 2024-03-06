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
        private Dictionary<int, GameEntryCar> _instances;
        private AssettoBallConfiguration _configuration;

        public Game(AssettoBallConfiguration configuration, Dictionary<int, GameEntryCar> instances )  {

            _instances = instances;
            _configuration = configuration; 

            _gameContext = new GameContext(_configuration, _instances);

            _gameManager = new GameManager(_gameContext);

            _stateManager = new StateManager(_gameContext, _gameManager);
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
