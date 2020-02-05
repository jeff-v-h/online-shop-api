using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PLP.Social.DataService.common.Models;
using PLP.Social.DataService.domain.Frameworks;
using PLP.Social.DataService.domain.Managers;
using PLP.Social.DataService.domain.Models.ModelMappers;
using PLP.Social.DataService.domain.Models.ViewModels;
using OnlineShopApi.presentation.Middlewares;
using OnlineShopApi.presentation.RequestModels;
using System.Text;

namespace OnlineShopApi.presentation
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .WithOrigins("http://localhost:4200", "https://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                        //.AllowCredentials();
                    });
            });

            // configure strongly typed settings objects
            var authSettingsSection = Configuration.GetSection("AuthSettings");
            services.Configure<AuthSettings>(authSettingsSection);
            services.Configure<DatabaseSettings>(options => Configuration.GetSection("DatabaseSettings").Bind(options));

            // configure jwt authentication
            var authSettings = authSettingsSection.Get<AuthSettings>();
            var key = Encoding.ASCII.GetBytes(authSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Auto Mapper configurations (to map models to view models)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SocialProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.CreateMap<NewUser, UserVM>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PLP Social Dataservice", Version = "v1" });
            });

            // Add services in data layer via ServiceManager and the domain layer
            ServiceManager.InjectServices(services);
            services.AddScoped<ISocialManager, SocialManager>();
            services.AddScoped<IUserManager, UserManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PLP Social Dataservice V1");
                // Set RoutePrefix to an empty string to serve swagger UI as the app's root
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
