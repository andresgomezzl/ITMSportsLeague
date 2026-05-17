using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        IMatchLineupRepository matchLineupRepository,
        ILogger<MatchLineupService> logger)
    {
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _matchLineupRepository = matchLineupRepository;
        _logger = logger;
    }

    public async Task<MatchLineup> RegisterAsync(int matchId, MatchLineup lineup)
    {
        // V1 + V6: el partido debe existir y estar Scheduled
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

        // V2: el jugador debe existir
        var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
        if (player == null)
            throw new KeyNotFoundException($"No se encontró el jugador con ID {lineup.PlayerId}");

        // V3: el jugador debe pertenecer al HomeTeam o AwayTeam
        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
            throw new InvalidOperationException("El jugador no pertenece a ninguno de los equipos del partido");

        // V4: no puede registrarse dos veces en la misma alineación
        var exists = await _matchLineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (exists)
            throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

        // Validación de posición
        if (string.IsNullOrWhiteSpace(lineup.Position))
            throw new InvalidOperationException("La posición es obligatoria");

        lineup.MatchId = matchId;
        lineup.Position = lineup.Position.Trim().ToUpperInvariant();

        // V5: máximo 11 titulares por equipo por partido
        if (lineup.IsStarter)
        {
            var teamLineups = await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, player.TeamId);
            var startersCount = teamLineups.Count(x => x.IsStarter);

            if (startersCount >= 11)
                throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
        }

        _logger.LogInformation(
            "Registering lineup for match {MatchId}, player {PlayerId}",
            matchId, lineup.PlayerId);

        var created = await _matchLineupRepository.CreateAsync(lineup);

        // Retorna con navegación cargada para respuesta de API
        return await _matchLineupRepository.GetByIdWithDetailsAsync(created.Id) ?? created;
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    public async Task DeleteAsync(int matchId, int id)
    {
        var lineup = await _matchLineupRepository.GetByIdAsync(id);
        if (lineup == null || lineup.MatchId != matchId)
            throw new KeyNotFoundException($"No se encontró la alineación con ID {id} para el partido {matchId}");

        _logger.LogInformation("Deleting lineup {LineupId} from match {MatchId}", id, matchId);
        await _matchLineupRepository.DeleteAsync(id);
    }
}