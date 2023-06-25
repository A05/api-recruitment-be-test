using Microsoft.Extensions.Hosting;
using System;

namespace ApiApplication.Domain
{
    public interface IImdbStatusHostedService : IHostedService, IDisposable
    {
        bool Up { get; }        
        DateTime LastCall { get; }
    }
}
