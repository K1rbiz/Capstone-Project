using Capstone_Project_v0._1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Capstone_Project_v0._1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bookstore.db");

        builder.Services.AddDbContext<LibraryContext>(opt =>
            opt.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<LibraryRepository>();

        var app = builder.Build();

        Batteries.Init();

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            ctx.Database.EnsureCreated();
        }

        return app;
    }
}