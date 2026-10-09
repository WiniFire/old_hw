namespace RequestAuditApp.Services;

public class AdminRequestFilter : IRequestFilter
{
    public bool ShouldAudit(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/admin")) return true;

        var header = context.Request.Headers["X-Audit"];
        return header.Count == 1
            && string.Equals(header[0]?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }
}
