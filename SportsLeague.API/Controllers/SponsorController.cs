using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;
    private readonly ILogger<SponsorController> _logger;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper,
        ILogger<SponsorController> logger)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null)
            return NotFound(new { message = $"Patrocinador con ID {id} no encontrado" });

        var dto = _mapper.Map<SponsorResponseDTO>(sponsor);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var created = await _sponsorService.CreateAsync(sponsor);
            var response = _mapper.Map<SponsorResponseDTO>(created);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // POST api/Sponsor/{id}/tournaments - Vincula un sponsor a un torneo
    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult<TournamentSponsorResponseDTO>> LinkToTournament(int id, TournamentSponsorRequestDTO dto)
    {
        try
        {
            // id path es sponsorId para claridad
            var created = await _sponsorService.LinkSponsorToTournamentAsync(dto.TournamentId, id, dto.ContractAmount);
            var response = _mapper.Map<TournamentSponsorResponseDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = id }, response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET api/Sponsor/{id}/tournaments - Lista los torneos vinculados a un sponsor
    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournamentsBySponsor(int id)
    {
        var links = await _sponsorService.GetTournamentsBySponsorAsync(id);
        var dto = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(links);
        return Ok(dto);
    }

    // DELETE api/Sponsor/{id}/tournaments/{tournamentId} - Desvincula sponsor de torneo
    [HttpDelete("{id}/tournaments/{tournamentId}")]
    public async Task<ActionResult> UnlinkFromTournament(int id, int tournamentId)
    {
        try
        {
            await _sponsorService.UnlinkSponsorFromTournamentAsync(tournamentId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
