using SportsLeague.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    // Repositorio específico para gestionar la relación TournamentSponsor
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        // Devuelve los vínculos (con información del Sponsor incluida) para un torneo
        Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId);

        // Devuelve un vínculo específico entre torneo y sponsor
        Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);

        // Devuelve los vínculos (con información del Tournament incluida) para un sponsor
        Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
    }
}
