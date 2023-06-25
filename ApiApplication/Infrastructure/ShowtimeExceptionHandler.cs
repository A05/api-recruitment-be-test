using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ApiApplication
{
    internal class ShowtimeExceptionHandler : IExceptionHandler
    {
        public Task<bool> HandleAsync(string controller, Exception exception, HttpContext context)
        {
            if (!controller.Equals("showtime", StringComparison.InvariantCultureIgnoreCase))
                return Task.FromResult(false);

            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return Task.FromResult(true);
        }
    }
}
