using ApiApplication.Auth;
using ApiApplication.Database;
using ApiApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace ApiApplication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IImdbStatusHostedService>(sp => new ImdbStatusHostedService(sp));
            services.AddHostedService(sp => (ImdbStatusHostedService) sp.GetService<IImdbStatusHostedService>());

            services.AddHttpClient();

            services.AddTransient<IImdbService, ImdbService>(sp =>
            {
                var apiKey = Configuration["ImdbApiKey"];
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new ImdbService(apiKey, httpClientFactory);
            });

            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<IShowtimesRepository, ShowtimesRepository>();

            services.AddDbContext<CinemaDbContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddSingleton<ICustomAuthenticationTokenService, CustomAuthenticationTokenService>();
            services.AddAuthentication(options =>
            {
                options.AddScheme<CustomAuthenticationHandler>(CustomAuthenticationSchemeOptions.AuthenticationScheme, CustomAuthenticationSchemeOptions.AuthenticationScheme);
                options.RequireAuthenticatedSignIn = true;                
                options.DefaultScheme = CustomAuthenticationSchemeOptions.AuthenticationScheme;
            });

            services.AddAutoMapper(typeof(Startup));

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = new SnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                });

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }

            app.UseHttpsRedirection();
            app.UseExecutionTime();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SampleData.Initialize(app);
        }      
    }
}
