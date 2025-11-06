using Capstone_Project_v0._1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Capstone_Project_v0._1;

public static class MauiProgram
{
    //  CreateMauiApp
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Configure the core MAUI app and fonts
        builder
            .UseMauiApp<App>()  // Registers the main App class (App.xaml.cs)
            .ConfigureFonts(fonts =>
            {
                // Adds custom fonts available throughout the app
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        //  Add Blazor support (for Razor UI pages)
        builder.Services.AddMauiBlazorWebView(); // Enables Blazor Hybrid (web-like components inside MAUI)

#if DEBUG
        //  Developer tools (enabled only in Debug mode)
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        //  Setup SQLite database path
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bookstore.db");
        // FileSystem.AppDataDirectory points to the platform-specific local storage folder.

        //  Register the EF Core database context
        builder.Services.AddDbContext<LibraryContext>(opt =>
            opt.UseSqlite($"Data Source={dbPath}"));
        // Tells EF Core to use a SQLite database located at the specified path.
        // LibraryContext defines the schema (Books, UserBooks).


        //  Register Repository for dependency injection
        builder.Services.AddScoped<LibraryRepository>();

        //  Build the MAUI app
        var app = builder.Build();

        //  Initialize SQLite engine (required for MAUI)
        Batteries.Init();

        //  Ensure database exists (create if missing)
        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            ctx.Database.EnsureCreated(); // Creates the database/tables if they don't exist yet.
        }

        //  Return the fully configured app
        return app;
    }
}