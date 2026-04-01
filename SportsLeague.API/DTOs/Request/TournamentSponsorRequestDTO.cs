namespace SportsLeague.API.DTOs.Request
{
    public class TournamentSponsorRequestDTO
    {
        // Id del torneo al que se vincula el sponsor
        public int TournamentId { get; set; }

        // Id del sponsor que se vincula
        public int SponsorId { get; set; }

        // Monto del contrato acordado
        public decimal ContractAmount { get; set; }
    }
}
