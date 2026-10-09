using RequestAuditApp.Models;

namespace RequestAuditApp.Data;

public class InMemoryAuditDatabase
{
    private readonly object _sync = new();
    private readonly List<AuditRecord> _records = new();
    private int _nextId = 1;

    public void Insert(AuditRecord record)
    {
        lock (_sync)
        {
            var savedRecord = record.Copy();
            savedRecord.Id = _nextId++;
            _records.Add(savedRecord);
        }
    }

    public IReadOnlyList<AuditRecord> GetAll()
    {
        lock (_sync)
        {
            return _records.OrderByDescending(record => record.Id)
                .Select(record => record.Copy())
                .ToList();
        }
    }
}
