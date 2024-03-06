using devpodcasts.server.api.Middlewares;

namespace devpodcasts.server.api.Extensions;

public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}