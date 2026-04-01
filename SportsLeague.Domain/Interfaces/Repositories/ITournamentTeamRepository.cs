using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentTeamRepository : IGenericRepository<TournamentTeam>
    {
        Task<TournamentTeam?> GetByTournamentAndTeamAsync(int tournamentId, int teamId); // Método para obtener una relación específica entre torneo y equipo
        Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(int tournamentId); // Método para obtener todas las relaciones de equipos en un torneo específico
    }
}
