namespace devpodcasts.server.api.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        // Log request information
        logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

        // Capture request body for logging (if required)
        if (context.Request.ContentLength != null && context.Request.ContentLength > 0 &&
            context.Request.ContentType != null &&
            context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            logger.LogInformation($"Request Body: {body}");
            context.Request.Body.Position = 0;
        }

        // Proceed with the request pipeline
        await next(context);

        // Log response information
        logger.LogInformation($"Response: {context.Response.StatusCode}");
    }
}

