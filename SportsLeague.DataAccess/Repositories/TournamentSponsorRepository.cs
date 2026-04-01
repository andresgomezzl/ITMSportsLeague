using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsLeague.DataAccess.Repositories
{
    // Repositorio para gestionar TournamentSponsor
    public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        // Obtiene todos los sponsors (vínculos) de un torneo e incluye la entidad Sponsor
        public async Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId)
        {
            return await _dbSet
                .Where(ts => ts.TournamentId == tournamentId)
                .Include(ts => ts.Sponsor)
                .Include(ts => ts.Tournament)
                .ToListAsync();
        }

        // Obtiene el vínculo específico entre torneo y sponsor
        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ts => ts.TournamentId == tournamentId && ts.SponsorId == sponsorId);
        }

        // Obtiene todos los vínculos para un sponsor e incluye la entidad Tournament
        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .Include(ts => ts.Sponsor)
                .ToListAsync();
        }
    }
}
