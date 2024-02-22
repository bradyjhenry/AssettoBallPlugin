
namespace AssettoBallPlugin
{
    public class GameManager
    {

        public List<Team> Teams { get; set; } = new List<Team>();
        public int WinningScore { get; set; }

        public GameManager() 
        {
        }

        // Initialize game setup
        public void InitializeGame()
        {
            var team1 = new Team("Team 1");
            var team2 = new Team("Team 2");

            // Example players
            team1.AddPlayer(new Player("Player 1A", 1));
            team1.AddPlayer(new Player("Player 1B", 2));
            team2.AddPlayer(new Player("Player 2A", 3));
            team2.AddPlayer(new Player("Player 2B", 4));

            Teams.Add(team1);
            Teams.Add(team2);

        }

        public void GoalScored(Team scoringTeam)
        {
            scoringTeam.AddScore(1);

            if (scoringTeam.Score >= WinningScore)
            {
                Console.WriteLine($"{scoringTeam.Name} wins!");
                // TODO: Handle game win scenario...
            }
        }
    }

}
