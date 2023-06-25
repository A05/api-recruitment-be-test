using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ApiApplication
{
    internal interface IExceptionHandler
    {
        Task<bool> HandleAsync(string controller, Exception exception, HttpContext context);
    }
}
