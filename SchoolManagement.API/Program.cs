using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API;
using SchoolManagement.API.Middleware;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Persistence;
using System.Threading.RateLimiting;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        //startup.ConfigureMenuServices(builder.Services);

        

        var app = builder.Build();
        startup.Configure(app, app.Environment);

        // Database Migration and Seeding
        await InitializeDatabaseAsync(app);

        

        //// Health Check Endpoint
        //app.UseHealthChecks("/health");

        app.Run();
    }

    private static async Task InitializeDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SchoolManagementDbContext>();

        try
        {
            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed initial data
            await MenuDataSeeder.SeedAsync(context);

            app.Logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}