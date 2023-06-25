using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiApplication.Domain
{
    public class ImdbStatusHostedService : BackgroundService, IImdbStatusHostedService
    {
        private readonly IServiceProvider _services;
        
        public bool Up { get; private set; }
        public DateTime LastCall { get; private set; } = DateTime.Now;

        public ImdbStatusHostedService(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var _timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            do
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IImdbService>();

                        var (_, _) = await service.FindAsync("tt0411008");

                        Up = true;
                    }
                }
                catch
                {
                    Up = false;
                }
                finally
                {
                    LastCall = DateTime.Now;
                }
            } 
            while (await _timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
