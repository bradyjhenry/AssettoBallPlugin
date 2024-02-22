using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssettoBallPlugin
{
    public class Player
    {
        public string Name { get; set; }
        public int Id { get; set; } // SteamID, I think
        public Team? Team { get; set; } // The team this player belongs to, if any

        public Player(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

    public class Team
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public int Score { get; set; } = 0; // Initial score

        public Team(string name)
        {
            Name = name;
        }

        // Add player to team
        public void AddPlayer(Player player)
        {
            player.Team = this; // Set player's team reference
            Players.Add(player);
        }

        // Increment team's score
        public void AddScore(int points)
        {
            Score += points;
        }
    }
}
