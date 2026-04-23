namespace FootballPrediction.Models
{
    public class FootballDataResponse
    {
        public List<FootballMatchDto> Matches { get; set; } = new();
    }

    public class FootballMatchDto 
    {
        public int Id { get; set; }
        public DateTime UtcDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Matchday { get; set; }
        public FootballSeasonDto Season { get; set; } = new();
        public FootballTeamDto HomeTeam { get; set; } = new();
        public FootballTeamDto AwayTeam { get; set; } = new();
        public FootballScoreDto Score { get; set; } = new();
    }

    public class FootballSeasonDto
    {
        public DateTime StartDate { get; set; }
    }

    public class FootballTeamDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class FootballScoreDto
    {
        public FootballFullTimeScoreDto FullTime { get; set; } = new();
    }

    public class FootballFullTimeScoreDto
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }
}
