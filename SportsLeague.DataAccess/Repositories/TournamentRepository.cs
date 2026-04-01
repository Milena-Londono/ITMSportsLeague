using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentRepository : GenericRepository<Tournament>, ITournamentRepository
    {
        public TournamentRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tournament>> GetByStatusAsync(TournamentStatus status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<Tournament?> GetByIdWithTeamsAsync(int id)
        {
            return await _dbSet
                .Where(t => t.Id == id)
                .Include(t => t.TournamentTeams)
                    .ThenInclude(tt => tt.Team)
                    //.ThenInclude(t => t.Players) // Si quieres incluir tambien los jugadores de cada equipo
                .FirstOrDefaultAsync();

            //Primero estoy en torneo ---> luego para recuperar los equipos de ese torneo, debo pasar 
            //obligatoriamente por la tabla intermedia TournamentTeams,
            //y luego deahí recuperar los equipos asociados a ese torneo. Por eso uso el ThenInclude.
        }
    }
}
