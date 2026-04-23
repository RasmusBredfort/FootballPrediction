using FootballPrediction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FootballPrediction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballController : ControllerBase
    {
        private readonly FootballDataService _footballDataService;
        private readonly PredictionService _predictionService;

        public FootballController(FootballDataService footballDataService, PredictionService predictionService)
        {
            _footballDataService = footballDataService;
            _predictionService = predictionService;
        }

        [HttpGet("import-premier-league-matches")]
        public async Task<IActionResult> ImportPremierLeagueMatches()
        {
            var importedCount = await _footballDataService.ImportPremierLeagueMatchesAsync();
            return Ok($"{importedCount} kampe blev gemt i databasen.");
        }

        [HttpGet("preview-premier-league-matches")]
        public async Task<IActionResult> PreviewPremierLeagueMatches()
        {
            var result = await _footballDataService.GetPremierLeagueMatchesPreviewAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("matches")]
        public IActionResult GetMatchesFromDatabase()
        {
            var matches = _footballDataService.GetMatchesFromDatabase();
            return Ok(matches);
        }

        [HttpGet("matches/team/{teamName}")]
        public IActionResult GetMatchesByTeam(string teamName)
        {
            var matches = _footballDataService.GetMatchesByTeam(teamName);
            return Ok(matches);
        }

        [HttpGet("matches/five/{teamName}")]
        public IActionResult GetLastFiveMatchesByTeam(string teamName)
        {
            var matches = _predictionService.GetLastFiveMatchesByTeam(teamName);
            return Ok(matches);
        }

        [HttpGet("delete-matches")]
        public async Task<IActionResult> DeleteAllMatches()
        {
            var deletedCount = await _footballDataService.DeleteAllMatchesAsync();
            return Ok($"{deletedCount} kampe blev slettet fra databasen.");
        }

        [HttpGet("matches/team/statistics")]
        public IActionResult GetTeamStatistics([FromQuery] string teamName)
        {
            var statistics = _predictionService.GetTeamStatistics(teamName);
            return Ok(statistics);
        }

        [HttpGet("Match-Prediction")]
        public IActionResult PredictMatch([FromQuery] string homeTeam, [FromQuery] string awayTeam)
        {
            var prediction = _predictionService.PredictMatch(homeTeam, awayTeam);
            return Ok(prediction);
        }
    }
}
