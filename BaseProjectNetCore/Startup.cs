using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Service.Core.Mapper;
using BaseProject.Extensions;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using BaseProject.Api.Filter;
using Model;
using Repository.Common;
using Service.Common;

namespace BaseProject.Api
{
    public static class Startup
    {

        public static void UseConfigurationServices(this IServiceCollection services)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(opts =>
            {
                opts.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                opts.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type =ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme,

                            }
                        }, new string[]{ }
                    }
                });
            });

            //services.AddSignalR();
            services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 1048576000; });

            // Database
            //services.AddDbContext<BaseProjectContext>(options =>
            //{
            //    var connectionString = AppSettings.Connections.PostgreSQLConnection;
            //    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("BaseProject.Model"));
            //});

            services.AddDbContext<BaseProjectContext>(options =>
            {
                var connectionString = AppSettings.Connections.MySqlConnection;
                options.UseMySql(connectionString,
                    Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly("BaseProject.Model"));
            });


            //services.AddDbContext<BaseProjectContext>(options =>
            //{
            //    var connectionString = AppSettings.Connections.DefaultConnection;
            //    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("BaseProject.Model"));
            //});

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var connectionString = AppSettings.Connections.MongoDBConnection.ConnectionString;
                var settings = MongoClientSettings.FromConnectionString(connectionString);

                return new MongoClient(settings);
            });

            services.AddDependencyInjection();

            services.AddIdentity<AppUser, AppRole>()
                 .AddEntityFrameworkStores<BaseProjectContext>()
                 .AddDefaultTokenProviders();

            //services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // }).AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters()
            //     {
            //         ValidateIssuer = false,
            //         ValidateAudience = false,
            //         RequireExpirationTime = true,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.AuthSetting.Key)),
            //         ValidateIssuerSigningKey = true,
            //     };
            // });

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = AppSettings.ConnectionStrings.DistCacheConnectionString;
            //    options.InstanceName = "BaseProject_";
            //});

            services.Configure<IdentityOptions>(options =>
             {
                 // Password settings.
                 options.Password.RequireDigit = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;
                 options.Password.RequiredLength = 6;
                 options.Password.RequiredUniqueChars = 0;

                 // Lockout settings.
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(AppSettings.AuthSetting.SecondsExpires);
                 options.Lockout.MaxFailedAccessAttempts = 5;
                 options.Lockout.AllowedForNewUsers = true;

                 // User settings.
                 options.User.AllowedUserNameCharacters =
                 "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                 options.User.RequireUniqueEmail = true;

                 //SignIn settings
                 options.SignIn.RequireConfirmedEmail = true;
             });

            services.ConfigureApplicationCookie(options =>
             {
                 // Cookie settings
                 options.Cookie.HttpOnly = true;
                 options.ExpireTimeSpan = TimeSpan.FromSeconds(AppSettings.AuthSetting.SecondsExpires);
                 options.LoginPath = "/Identity/Account/Login";
                 options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                 options.SlidingExpiration = true;
             });

            //services.AddAuthentication()
            //     .AddGoogle(options =>
            //     {
            //         options.ClientId = AppSettings.ExternalAuth.GoogleAuth.ClientId;
            //         options.ClientSecret = AppSettings.ExternalAuth.GoogleAuth.ClientSecret;
            //         options.CallbackPath = "/google";
            //     });
        }

        private static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IMapper, Mapper>();
            services.AddScoped<DbContext, BaseProjectContext>();
            services.AddScoped<LogActionFilter>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            var repositoryTypes = typeof(IRepository<>).Assembly.GetTypes()
                 .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("BaseProject.Repository") && x.Name.EndsWith("Repository"));
            foreach (var intf in repositoryTypes.Where(t => t.IsInterface))
            {
                var impl = repositoryTypes.FirstOrDefault(c => c.IsClass && intf.Name.Substring(1) == c.Name);
                if (impl != null) services.AddScoped(intf, impl);
            }

            services.AddScoped(typeof(IService<>), typeof(Service<>));
            var serviceTypes = typeof(IService<>).Assembly.GetTypes()
                 .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.StartsWith("BaseProject.Service") && x.Name.EndsWith("Service"));
            foreach (var intf in serviceTypes.Where(t => t.IsInterface))
            {
                var impl = serviceTypes.FirstOrDefault(c => c.IsClass && intf.Name.Substring(1) == c.Name);
                if (impl != null) services.AddScoped(intf, impl);
            }
        }
    }

}
