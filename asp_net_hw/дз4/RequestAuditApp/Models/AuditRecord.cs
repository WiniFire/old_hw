namespace RequestAuditApp.Models;

public class AuditRecord
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string User { get; set; } = "Anonymous";
    public DateTime Timestamp { get; set; }
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }

    public AuditRecord Copy()
    {
        return (AuditRecord)MemberwiseClone();
    }
}
