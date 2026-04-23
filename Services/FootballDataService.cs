using FootballPrediction.Data;
using FootballPrediction.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Text.Json;

namespace FootballPrediction.Services
{
    public class FootballDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public FootballDataService(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        private async Task<FootballDataResponse?> FetchPremierLeagueMatchesAsync()
        {
            var response = await _httpClient.GetAsync("competitions/PL/matches");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<FootballDataResponse>(json, options);
        }

        public async Task<int> ImportPremierLeagueMatchesAsync()
        {
            var footballDataResponse = await FetchPremierLeagueMatchesAsync();

            if (footballDataResponse == null)
            {
                return 0;
            }

            int importedCount = 0;

            foreach (var apiMatch in footballDataResponse.Matches)
            {
                var existingMatch = _context.Matches.FirstOrDefault(m => m.FootballDataMatchId == apiMatch.Id);

                if (existingMatch != null)
                {
                    continue;
                }

                var match = new Match
                {
                    FootballDataMatchId = apiMatch.Id,
                    HomeTeam = apiMatch.HomeTeam.Name,
                    AwayTeam = apiMatch.AwayTeam.Name,
                    MatchDate = apiMatch.UtcDate,
                    HomeTeamScore = apiMatch.Score.FullTime.Home,
                    AwayTeamScore = apiMatch.Score.FullTime.Away,
                    Status = apiMatch.Status,
                    MatchDay = apiMatch.Matchday,
                    SeasonStartYear = apiMatch.Season.StartDate.Year
                };

                _context.Matches.Add(match);
                importedCount++;
            }

            await _context.SaveChangesAsync();

            return importedCount;
        }

        public async Task<FootballDataResponse?> GetPremierLeagueMatchesPreviewAsync()
        {
            return await FetchPremierLeagueMatchesAsync();
        }

        public List<Match> GetMatchesFromDatabase()
        {
            return _context.Matches.ToList();
        }

        public List<Match> GetMatchesByTeam(string teamName)
        {
            return _context.Matches
                .Where(m => m.HomeTeam == teamName || m.AwayTeam == teamName)
                .OrderBy(m => m.MatchDay)
                .ToList();
        }

        public async Task<int> DeleteAllMatchesAsync()
        {
            var matches = _context.Matches.ToList();

            _context.Matches.RemoveRange(matches);

            await _context.SaveChangesAsync();

            return matches.Count;
        }

    }
}
