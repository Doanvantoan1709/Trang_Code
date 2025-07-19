using Microsoft.Extensions.FileProviders;
using BaseProject.Extensions;
using BaseProject.Api;
using Microsoft.OpenApi.Models;
using BaseProject.Api.Core.Middleware;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using BaseProject.Repository.AccountRepository;
using BaseProject.Service.AccountService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using Model;
using Microsoft.EntityFrameworkCore;
using BaseProject.Repository.TESTDB;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
        Console.WriteLine($"Connection String: {connectionString}"); // In ra để kiểm tra
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string 'MySqlConnection' is null or empty.");
        }

        builder.Services.AddDbContext<BaseProjectContext>(options =>
            options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString)));

        // Add services to the container.
        builder.Configuration.UseAppSettings();
        builder.Services.UseConfigurationServices();
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
            };
        });
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["RedisCacheUrl"];
        });
        //builder.Services.AddControllers();

        //Log.Logger = new LoggerConfiguration()
        //.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        //.Enrich.FromLogContext()
        //.CreateLogger();
        //builder.Host.UseSerilog();
        builder.Services.AddTransient<ExceptionHandlingMiddleware>();
        //builder.Services.AddTransient<ApiPermissionsMiddleware>();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Base Project API", Version = "v1" });

            // Tùy chỉnh tên Schema tránh trùng lặp
            c.CustomSchemaIds(type => type.FullName);
        });
        
        // Life time
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ITest, Test>();

        var app = builder.Build();

        app.UseCors(builder =>
        {
            builder.WithOrigins(AppSettings.AllowedOrigins.ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });

        app.UseDeveloperExceptionPage();
        //app.UseMiddleware<LoggingMiddleware>();

        //app.UseStaticFiles(new StaticFileOptions
        //{
        //    FileProvider = new PhysicalFileProvider(
        //           Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
        //    RequestPath = "/uploads",
        //});
        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        //app.UseMiddleware<ApiPermissionsMiddleware>();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var error = exceptionHandlerPathFeature?.Error;

                var result = JsonSerializer.Serialize(new { error = error?.Message, stackTrace = error?.StackTrace });
                await context.Response.WriteAsync(result);
            });
        });
        app.UseHttpsRedirection();
        app.MapControllers();
        //app.MapHub<ChatHub>("/chatHub");

        app.Run();
    }
}
