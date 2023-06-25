using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiApplication
{
    public class ExecutionTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExecutionTimeMiddleware> _logger;

        public ExecutionTimeMiddleware(RequestDelegate next, ILogger<ExecutionTimeMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var r = context.Request;
                var url = $"{r.Scheme}://{r.Host}{r.PathBase}{r.Path}{r.QueryString}";
                string message = $"{r.Method} on {url} took {stopwatch.ElapsedMilliseconds} ms.";

                _logger.LogInformation(message);
            }
        }
    }
}
