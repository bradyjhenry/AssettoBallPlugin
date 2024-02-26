
namespace AssettoBallPlugin
{
    public class GameManager
    {

        public List<Team> Teams { get; set; } = new List<Team>();
        public int WinningScore { get; set; }

        public GameManager(AssettoBallConfiguration configration)
        {
            WinningScore = configration.GameState.WinningScore;
        }

        // Initialize game setup
        public void InitializeGame()
        {
            var team1 = new Team("Team 1");
            var team2 = new Team("Team 2");

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
