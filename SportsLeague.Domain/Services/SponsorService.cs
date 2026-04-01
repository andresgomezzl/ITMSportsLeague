using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(ISponsorRepository sponsorRepository, ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
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
