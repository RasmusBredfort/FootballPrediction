using FootballPrediction.Data;
using FootballPrediction.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FootballPrediction.Services
{
    public class PredictionService
    {
        
        private readonly ApplicationDbContext _context;
        private readonly FootballDataService _footballDataService;
        public PredictionService(ApplicationDbContext context, FootballDataService footballDataService)
        {
            _context = context;
            _footballDataService = footballDataService;
        }

        public List<Match> GetLastFiveMatchesByTeam(string teamName)
        {
            return _context.Matches
                .Where(m => (m.HomeTeam == teamName || m.AwayTeam == teamName) && m.Status == "FINISHED")
                .OrderByDescending(m => m.MatchDay)
                .Take(5).ToList();
        }

        public TeamStatistics GetTeamStatistics(string teamName)
        {
            var lastFiveMatches = GetLastFiveMatchesByTeam(teamName);

            int wins = 0;
            int draws = 0;
            int losses = 0;
            int goalsScored = 0;
            int goalsConceded = 0;

            foreach (var match in lastFiveMatches)
            {
                bool isHomeTeam = match.HomeTeam == teamName;

                int teamScore = isHomeTeam
                    ? match.HomeTeamScore ?? 0
                    : match.AwayTeamScore ?? 0;

                int opponentScore = isHomeTeam
                    ? match.AwayTeamScore ?? 0
                    : match.HomeTeamScore ?? 0;

                goalsScored += teamScore;
                goalsConceded += opponentScore;

                if (teamScore > opponentScore)
                {
                    wins++;
                }
                else if (teamScore < opponentScore)
                {
                    losses++;
                }
                else
                {
                    draws++;
                }
            }

            var statistics = new TeamStatistics
            {
                TeamName = teamName,
                Wins = wins,
                Draws = draws,
                Losses = losses,
                GoalsScored = goalsScored,
                GoalsConceded = goalsConceded
            };

            return statistics;
        }

        public async Task<PredictionResult> PredictMatchAsync(string homeTeam, string awayTeam)
        {
            var homeStats = GetTeamStatistics(homeTeam);
            var awayStats = GetTeamStatistics(awayTeam);

            var homeStanding = await GetStandingByTeamAsync(homeTeam);
            var awayStanding = await GetStandingByTeamAsync(awayTeam);

            double homeScore = CalculateBaseScore(homeStats);
            double awayScore = CalculateBaseScore(awayStats);

            if(homeStanding != null)
            {
                homeScore += CalculateStandingsScore(homeStanding);
            }

            if(awayStanding != null)
            {
                awayScore += CalculateStandingsScore(awayStanding);
            }

            homeScore = ApplyHomeAdvantage(homeScore);

            string predictedOutcome = DetermineOutcome(homeScore, awayScore);
            string reason = BuildReason(predictedOutcome);

            return new PredictionResult
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeTeamScore = homeScore,
                AwayTeamScore = awayScore,
                PredictedOutcome = predictedOutcome,
                Reason = reason
            };
        }

        public double CalculateBaseScore(TeamStatistics stats)
        {
            double winsWeight = 3;
            double drawsWeight = 1;
            double lossesWeight = -2;
            double goalsScoredWeight = 0.5;
            double goalsConcededWeight = -0.5;

            return
                (stats.Wins * winsWeight) +
                (stats.Draws * drawsWeight) +
                (stats.Losses * lossesWeight) +
                (stats.GoalsScored * goalsScoredWeight) +
                (stats.GoalsConceded * goalsConcededWeight);
        }

        public double ApplyHomeAdvantage(double score)
        {
            double homeAdvantageWeight = 1.5;
            return score + homeAdvantageWeight;
        }

        private string DetermineOutcome(double homeScore, double awayScore)
        {
            double difference = Math.Abs(homeScore - awayScore);

            if (difference < 1.0)
            {
                return "Draw";
            }

            if (homeScore > awayScore)
            {
                return "HomeWin";
            }

            return "AwayWin";
        }

        private string BuildReason(string predictedOutcome)
        {
            if (predictedOutcome == "Draw")
            {
                return "The teams are very close based on recent form and goals.";
            }

            if (predictedOutcome == "HomeWin")
            {
                return "The home team has stronger recent form and home advantage.";
            }

            return "The away team has stronger recent form.";
        }

        public async Task<FootballStandingTableEntryDto?> GetStandingByTeamAsync(string teamName)
        {
            var standingResponse = await _footballDataService.GetFootballStandingsAsync();

            if(standingResponse == null)
            {
                return null;
            }

            var totalStanding = standingResponse.Standings.FirstOrDefault(s => s.Type == "TOTAL");

            if(totalStanding == null)
            {
                return null;
            }

            var teamStanding = totalStanding.Table.FirstOrDefault(t => t.Team.Name == teamName);

            return teamStanding;
        }

        public double CalculateStandingsScore(FootballStandingTableEntryDto standing)
        {
            double pointsWeight = 0.1;
            double goalDifferenceWeight = 0.05;

            double standingscore = (standing.Points * pointsWeight) + (standing.GoalDifference * goalDifferenceWeight);

            return standingscore;
        }
    }
}
