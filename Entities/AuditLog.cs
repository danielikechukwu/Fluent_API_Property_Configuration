namespace EF_FluentAPIPropertyConfiguration.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }

        public string TableName { get; set; } = null!;

        public string ActionType { get; set; } = null!;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; } 

        public string PerformedBy { get; set; } = null!;

        public DateTime ActionTime { get; set; }

        public string? CorrelationId { get; set; }
    }
}
