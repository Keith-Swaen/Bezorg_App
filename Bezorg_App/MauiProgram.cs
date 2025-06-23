using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bezorg_App
{
    public static class MauiProgram
    {
        // ServiceProvider beschikbaar stellen voor DI-opvraging elders in de app
        public static IServiceProvider Services { get; private set; } = default!;
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Laad het embedded JSON-configuratiebestand (appsettings.json)
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly
                 .GetManifestResourceStream("Bezorg_App.appsettings.json")
                             ?? throw new InvalidOperationException("Missing embedded appsettings.json");
            builder.Configuration.AddJsonStream(stream);

            // Bind de sectie "ApiSettings" naar onze POCO en registreer bij DI
            builder.Services
                   .Configure<ApiSettings>(
                       builder.Configuration.GetSection("ApiSettings"));

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            Services = app.Services;
            return builder.Build();
        }
    }
}
