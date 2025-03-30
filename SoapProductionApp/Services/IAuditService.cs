namespace SoapProductionApp.Services
{
    public interface IAuditService
    {
        Task LogAsync(string actionType, string entityName, int? entityId = null, object? before = null, object? after = null);
    }
}
