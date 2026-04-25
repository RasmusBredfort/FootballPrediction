using Microsoft.AspNetCore.Routing.Constraints;

namespace FootballPrediction.Models
{
    public class FootballStandingsResponse
    {
        public List<FootballStandingDto> Standings { get; set; } = new();
    }

    public class FootballStandingDto
    {
        public string Stage { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<FootballStandingTableEntryDto> Table { get; set; } = new();
    }

    public class FootballStandingTableEntryDto
    {
        public int Position {  get; set; }
        public FootballStandingTeamDto Team { get; set; } = new();
        public int PlayedGames { get; set; }
        public int Won {  get; set; }
        public int Draw {  get; set; }
        public int Lost { get; set; }
        public int Points {  get; set; }
        public int GoalsFor {  get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference {  get; set; }
    }

    public class FootballStandingTeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Tla { get; set; } = string.Empty;
        public string Crest { get; set; } = string.Empty;
    }
}
