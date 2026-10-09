using RequestAuditApp.Models;

namespace RequestAuditApp.Services;

public interface IAuditService
{
    Task SaveAsync(AuditRecord record);
    Task<IReadOnlyList<AuditRecord>> GetRecordsAsync();
}
