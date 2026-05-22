namespace SportsLeague.Domain.Entities
{
    public class MatchLineup : AuditBase
    {
        // Foreign Keys
        public int MatchId { get; set; }
        public int PlayerId { get; set; }

        // Información alineación
        public bool IsStarter { get; set; }
        public string Position { get; set; } = string.Empty;

        // Navigation Properties
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
