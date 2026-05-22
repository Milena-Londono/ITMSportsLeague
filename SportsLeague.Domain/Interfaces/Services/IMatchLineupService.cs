using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchLineupService
    {
        Task<MatchLineup> AddPlayerToLineupAsync(int matchId, MatchLineup lineup);

        Task<IEnumerable<MatchLineup>> GetMatchLineupAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetMatchLineupByTeamAsync(int matchId, int teamId);

        Task RemovePlayerFromLineupAsync(int matchId, int lineupId);
    }
}
