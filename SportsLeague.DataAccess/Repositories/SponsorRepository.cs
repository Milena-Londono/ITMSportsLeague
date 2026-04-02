using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(s =>
                s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId)
        {
            return await _dbSet.AnyAsync(s =>
                s.Id != excludeId &&
                s.Name.ToLower() == name.ToLower());
        }
    }
}
