using Microsoft.AspNetCore.Builder;

namespace ApiApplication
{
    public static class ExecutionTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseExecutionTime(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExecutionTimeMiddleware>();
        }
    }
}
