using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task<bool> ExistsByNameAsync(string name); //lo añadí para validar nombre único
        // Vincular un sponsor a un torneo con monto de contrato
        Task<TournamentSponsor> LinkSponsorToTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount);

        // Desvincular un sponsor de un torneo
        Task UnlinkSponsorFromTournamentAsync(int tournamentId, int sponsorId);

        // Listar vínculos (y datos asociados) de torneos para un sponsor
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
    }
}
