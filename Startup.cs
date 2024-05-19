using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using TinyUrl.Cache;
using TinyUrl.Repositories;

namespace TinyUrlSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton<IMongoClient>(provider =>
            {
                var connectionString = Configuration.GetConnectionString("MongoDb");
                if (connectionString == null)
                {
                    throw new InvalidOperationException("MongoDb connection string is null.");
                }
                return new MongoClient(connectionString);
            });
            services.AddSingleton<IMongoDatabase>(provider =>
            {
                var client = provider.GetService<IMongoClient>();
                if (client == null)
                {
                    throw new InvalidOperationException("MongoClient is null.");
                }
                var databaseName = Configuration?.GetValue<string>("MongoDbSettings:DatabaseName");
                if (databaseName == null)
                {
                    throw new InvalidOperationException("Database name is null.");
                }
                return client.GetDatabase(databaseName);
            });

            // Register the cache as a singleton
            services.AddSingleton<LRUCache<string, string>>(provider =>
            {
                var capacity = 1000; // Adjust capacity as needed
                return new LRUCache<string, string>(capacity);
            });

            // Register UrlRepository
            services.AddSingleton<UrlRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
