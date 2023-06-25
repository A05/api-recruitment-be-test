using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiApplication
{
    internal class DefaultExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<DefaultExceptionHandler> _logger;

        public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException();
        }

        public Task<bool> HandleAsync(string controller, Exception exception, HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var r = context.Request;
            var url = $"{r.Scheme}://{r.Host}{r.PathBase}{r.Path}{r.QueryString}";
            _logger.LogError(exception, $"Failed to execute {r.Method} on {url}.");

            return Task.FromResult(true);
        }
    }
}
