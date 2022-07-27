using Elasticsearch.Net;
using Shared.Model.ElasticSearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Shared.Model.Config;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using Shared.Model.Repositories.BaseRepository;
using Shared.Model.Repositories.CarInformationRepository;
using Shared.Model.Repositories.ParkingAreaRepository;
using Shared.Model.Repositories.ParkingSpaceRepository;
using Shared.Model.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsAPI
{
    public class Startup
    {
        private ILoggerFactory LoggerFactory { get; set; }
        private RabbitMqConfiguration RabbitMqConfiguration { get; set; }

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

            RabbitMqConfiguration = new RabbitMqConfiguration(Configuration["RabbitMQ:Server"], Configuration["RabbitMQ:Port"], Configuration["RabbitMQ:Username"], Configuration["RabbitMQ:Password"], Configuration["RabbitMQ:QueueName"], Configuration["RabbitMQ:VHost"]);

            var ES_URI = Configuration["elasticsearch:url"];
            var pool = new StaticConnectionPool(new List<Uri> { new Uri(ES_URI) });
            var connectionSettings = new ConnectionSettings(pool);
            var elasticSearchConfiguration = new ElasticSearchConfiguration(connectionSettings);
            var rabbitMqQueues = new AppConfig
            {
                WPFManageQueue = Configuration["RabbitMQ:Queues:WPFManageQueue"]

            };
            services.AddTransient<IPersistenceFactory, PersistenceFactory>(builder =>
            {
                return new PersistenceFactory(elasticSearchConfiguration,  LoggerFactory, RabbitMqConfiguration, rabbitMqQueues);
            });
            services.AddTransient<ICarRepository, CarRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IParkingSpaceRepository, ParkingSpaceRepository>();
            services.AddTransient<IParkingAreaRepository, ParkingAreaRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = Configuration["Jwt:Audience"],
            ValidIssuer = Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))

        };
    });

            services.AddMvc();
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
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
