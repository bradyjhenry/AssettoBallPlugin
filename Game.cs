using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssettoBallPlugin
{
    public class Game
    {

        private StateManager stateManager;

        private GameContext gameContext;


        public Game(AssettoBallConfiguration configuration) {
            stateManager = new StateManager();
            gameContext = new GameContext(configuration);
        }
    }
}
