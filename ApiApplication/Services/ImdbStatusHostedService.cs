using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.Services
{
    public class ImdbStatusHostedService : IImdbStatusHostedService
    {
        private readonly IServiceProvider _services;
        private Timer _timer;

        public bool Up { get; private set; }
        public DateTime LastCall { get; private set; } = DateTime.Now;

        public ImdbStatusHostedService(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IImdbService>();
                    
                    service.Find("tt0411008", out var _);

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

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
