namespace FootballPrediction.Models
{
    public class PredictionResult
    {
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public double HomeTeamScore { get; set; }
        public double AwayTeamScore { get; set; }
        public string PredictedOutcome { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
