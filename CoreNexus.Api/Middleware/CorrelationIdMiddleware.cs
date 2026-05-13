using Serilog.Context;

namespace CoreNexus.Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string Header = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var correlationId =
            context.Request.Headers[Header].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        // Guardar en contexto HTTP (IMPORTANTE)
        context.Items[Header] = correlationId;

        // Exponer en response
        context.Response.Headers[Header] = correlationId;

        // Logging scope global (Serilog)
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}