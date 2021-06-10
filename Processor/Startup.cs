using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Processor.Entities;
using Processor.Managers;
using Processor.Managers.Interfaces;
using Processor.Services;
using System.Collections.Concurrent;
using TwitterStatistics.Managers;
using Processor.Client;

namespace Processor
{
    public class Startup
    {
        private const string TwitterServiceConfigSection = "TwitterService";
        private const string TwitterServiceConfigBearerToken = "TwitterService:BearerToken";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TwitterServiceOptions>(Configuration.GetSection(TwitterServiceConfigSection));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Processor", Version = "v1" });
            });

            var bearerToken = Configuration[$"{TwitterServiceConfigBearerToken}"];
            services.AddSingleton((s)=> { return new HttpClientWrapper(bearerToken); });

            services.AddSingleton<ConcurrentQueue<string>>();
            services.AddSingleton<TweetStat>();
            services.AddHostedService<QueueBackgroundService>();
            services.AddScoped<IStatisticsManager, StatisticsManager>();
            services.AddSingleton<ITweetManager, TweetManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Processor v1"));
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
