namespace FootballPrediction.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int FootballDataMatchId { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public DateTime MatchDate {  get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MatchDay { get; set; }
        public int SeasonStartYear { get; set; }
    }
}
