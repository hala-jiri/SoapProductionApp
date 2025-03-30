using SoapProductionApp.Data;
using SoapProductionApp.Extensions;
using SoapProductionApp.Models;

namespace SoapProductionApp.Services
{
    public class AuditService: IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string actionType, string entityName, int? entityId = null, object? before = null, object? after = null)
        {
            var user = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown";

            var log = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                UserName = user,
                ActionType = actionType,
                EntityName = entityName,
                EntityId = entityId,
                BeforeJson = before?.ToSafeJson(),
                AfterJson = after?.ToSafeJson()
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
