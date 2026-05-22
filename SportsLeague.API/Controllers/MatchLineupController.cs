using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [Route("api/match/{matchId}/lineup")]
    [ApiController]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService matchLineupService,
            IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        // POST: api/match/{matchId}/lineup
        [HttpPost]
        public async Task<IActionResult> AddPlayerToLineup(
            int matchId,
            [FromBody] MatchLineupRequestDTO request)
        {
            try
            {
                var lineup = _mapper.Map<MatchLineup>(request);

                var createdLineup = await _matchLineupService.AddPlayerToLineupAsync(
                    matchId,
                    lineup);

                var response = _mapper.Map<MatchLineupResponseDTO>(createdLineup);

                return CreatedAtAction(
                    nameof(GetMatchLineup),
                    new { matchId = matchId },
                    response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/match/{matchId}/lineup
        [HttpGet]
        public async Task<IActionResult> GetMatchLineup(int matchId)
        {
            try
            {
                var lineup = await _matchLineupService.GetMatchLineupAsync(matchId);

                var response = _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/match/{matchId}/lineup/team/{teamId}
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetMatchLineupByTeam(
            int matchId,
            int teamId)
        {
            try
            {
                var lineup = await _matchLineupService.GetMatchLineupByTeamAsync(
                    matchId,
                    teamId);

                var response = _mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineup);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/match/{matchId}/lineup/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePlayerFromLineup(
            int matchId,
            int id)
        {
            try
            {
                await _matchLineupService.RemovePlayerFromLineupAsync(matchId, id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
