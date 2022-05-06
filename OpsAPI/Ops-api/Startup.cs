using Elasticsearch.Net;
using FPT.akaSAFE.Shared.Model.ElasticSearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Nest;
using Shared.Model.Config;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpsAPI
{
    public class Startup
    {
        private ILoggerFactory LoggerFactory { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
              ServerVersion.Parse("8.0.26-mysql")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ops_api", Version = "v1" });
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddHttpContextAccessor();
            var ES_URI = Configuration["elasticsearch:url"];
            var pool = new StaticConnectionPool(new List<Uri> { new Uri(ES_URI) });
            var connectionSettings = new ConnectionSettings(pool);
            var elasticSearchConfiguration = new ElasticSearchConfiguration(connectionSettings);
            services.AddTransient<IPersistenceFactory, PersistenceFactory>(builder =>
            {
                return new PersistenceFactory(elasticSearchConfiguration, LoggerFactory);
            });
            //services.AddTransient<IPersistenceFactory, PersistenceFactory>();
            //services.AddTransient<IElasticSearchClient, ElasticSearchClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ops_api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
