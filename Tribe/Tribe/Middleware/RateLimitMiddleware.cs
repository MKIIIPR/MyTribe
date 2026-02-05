using System.Collections.Concurrent;

namespace Tribe.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, (DateTime LastReset, int RequestCount)> _requestCounts = new();
        private const int MaxRequests = 100;
        private const int TimeWindowSeconds = 60;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.ToString();

            // Rate limit nur für Shop-API-Endpunkte
            if (path.StartsWith("/api/shop"))
            {
                var key = $"{clientIp}:{path}";
                var now = DateTime.UtcNow;

                if (_requestCounts.TryGetValue(key, out var data))
                {
                    var timeSinceReset = now - data.LastReset;
                    if (timeSinceReset.TotalSeconds > TimeWindowSeconds)
                    {
                        _requestCounts[key] = (now, 1);
                    }
                    else if (data.RequestCount >= MaxRequests)
                    {
                        _logger.LogWarning($"Rate limit exceeded for {key}");
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded" });
                        return;
                    }
                    else
                    {
                        _requestCounts[key] = (data.LastReset, data.RequestCount + 1);
                    }
                }
                else
                {
                    _requestCounts.TryAdd(key, (now, 1));
                }
            }

            await _next(context);
        }
    }
}
