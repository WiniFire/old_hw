namespace RequestAuditApp.Services;

public interface IRequestFilter
{
    bool ShouldAudit(HttpContext context);
}
