using FootballPrediction.Data;
using FootballPrediction.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FootballPrediction.Services
{
    public class PredictionService
    {
        
        private readonly ApplicationDbContext _context;
        public PredictionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Match> GetLastFiveMatchesByTeam(string teamName)
        {
            return _context.Matches.Where(m => (m.HomeTeam == teamName || m.AwayTeam == teamName) && m.Status == "FINISHED")
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

        public PredictionResult PredictMatch(string homeTeam, string awayTeam)
        {
            var homeStats = GetTeamStatistics(homeTeam);
            var awayStats = GetTeamStatistics(awayTeam);

            double winsWeight = 3;
            double drawsWeight = 1;
            double lossesWeight = -2;
            double goalsScoredWeight = 0.5;
            double goalsConcededWeight = -0.5;
            double homeAdvantageWeight = 1.5;

            double homeScore =
                (homeStats.Wins * winsWeight) +
                (homeStats.Draws * drawsWeight) +
                (homeStats.Losses * lossesWeight) +
                (homeStats.GoalsScored * goalsScoredWeight) +
                (homeStats.GoalsConceded * goalsConcededWeight) +
                homeAdvantageWeight;

            double awayScore =
                (awayStats.Wins * winsWeight) +
                (awayStats.Draws * drawsWeight) +
                (awayStats.Losses * lossesWeight) +
                (awayStats.GoalsScored * goalsScoredWeight) +
                (awayStats.GoalsConceded * goalsConcededWeight);

            string predictedOutcome;
            string reason;

            double difference = Math.Abs(homeScore - awayScore);

            if(difference < 1.0)
            {
                predictedOutcome = "Draw";
                reason = "The teams are very close in form";
            }
            else if (homeScore >  awayScore) 
            {
                predictedOutcome = "HomeWin";
                reason = "The home team is in better form";
            }
            else
            {
                predictedOutcome = "AwayWin";
                reason = "The away team is in better form";
            }

            var predictionResult = new PredictionResult
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeTeamScore = homeScore,
                AwayTeamScore = awayScore,
                PredictedOutcome = predictedOutcome,
                Reason = reason
            };

            return predictionResult;
        }
    }
}
