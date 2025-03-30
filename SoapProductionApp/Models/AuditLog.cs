namespace SoapProductionApp.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;
        public int? EntityId { get; set; }

        public string ActionType { get; set; } = string.Empty;
        public string? BeforeJson { get; set; }
        public string? AfterJson { get; set; }
    }
}
