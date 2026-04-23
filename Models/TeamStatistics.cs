namespace FootballPrediction.Models
{
    public class TeamStatistics
    {
        public string TeamName { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
    }
}
