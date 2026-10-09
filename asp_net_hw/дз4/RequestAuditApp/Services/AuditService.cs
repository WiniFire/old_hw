using RequestAuditApp.Data;
using RequestAuditApp.Models;

namespace RequestAuditApp.Services;

public class AuditService : IAuditService
{
    private readonly InMemoryAuditDatabase _database;

    public AuditService(InMemoryAuditDatabase database)
    {
        _database = database;
    }

    public Task SaveAsync(AuditRecord record)
    {
        _database.Insert(record);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AuditRecord>> GetRecordsAsync()
    {
        return Task.FromResult(_database.GetAll());
    }
}
