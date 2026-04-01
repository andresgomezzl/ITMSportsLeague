namespace SportsLeague.API.DTOs.Response
{
    public class TournamentSponsorResponseDTO
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int SponsorId { get; set; }
        public decimal ContractAmount { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Información anidada del sponsor para el response (útil al listar sponsors de un torneo)
        public SponsorResponseDTO? Sponsor { get; set; }
        // Información anidada del torneo para el response (útil al listar torneos de un sponsor)
        public TournamentResponseDTO? Tournament { get; set; }
    }
}
