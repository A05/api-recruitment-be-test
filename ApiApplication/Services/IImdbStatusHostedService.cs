using Microsoft.Extensions.Hosting;
using System;

namespace ApiApplication.Services
{
    public interface IImdbStatusHostedService : IHostedService, IDisposable
    {
        bool Up { get; }        
        DateTime LastCall { get; }
    }
}
