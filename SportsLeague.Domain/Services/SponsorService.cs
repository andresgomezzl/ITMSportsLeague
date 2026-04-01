using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentRepository tournamentRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentRepository = tournamentRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

        return sponsor;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        // Validación de negocio nombre único
        var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
        if (exists)
        {
            _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
            throw new InvalidOperationException($"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
        }

        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _sponsorRepository.ExistsByNameAsync(name);
    }

    // Vincular un sponsor a un torneo con validaciones
    public async Task<TournamentSponsor> LinkSponsorToTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount)
    {
        // Validar monto
        if (contractAmount <= 0)
        {
            throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");
        }

        // Validar que el sponsor existe
        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
        if (sponsor == null)
        {
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");
        }

        // Validar que el torneo existe
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");
        }

        // Verificar que no exista la vinculación
        var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing != null)
        {
            throw new InvalidOperationException("El patrocinador ya está vinculado a este torneo");
        }

        // Crear vínculo
        var link = new TournamentSponsor
        {
            TournamentId = tournamentId,
            SponsorId = sponsorId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };

        var created = await _tournamentSponsorRepository.CreateAsync(link);
        return created;
    }

    // Desvincular un sponsor de un torneo
    public async Task UnlinkSponsorFromTournamentAsync(int tournamentId, int sponsorId)
    {
        var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing == null)
        {
            throw new KeyNotFoundException("La vinculación no existe");
        }

        await _tournamentSponsorRepository.DeleteAsync(existing.Id);
    }

    // Listar torneos vinculados a un sponsor
    public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");
        }

        // Validar nombre único si cambió
        if (!string.Equals(existing.Name, sponsor.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (exists)
            {
                throw new InvalidOperationException($"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }
        }

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebsiteUrl = sponsor.WebsiteUrl;
        existing.Category = sponsor.Category;

        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");
        }

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }
}
