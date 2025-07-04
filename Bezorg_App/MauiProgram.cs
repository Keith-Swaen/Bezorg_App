﻿using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting; 
using Syncfusion.Maui.SignaturePad;
﻿using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bezorg_App.Services;

namespace Bezorg_App
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = default!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // JSON-config
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly
                 .GetManifestResourceStream("Bezorg_App.appsettings.json")
                             ?? throw new InvalidOperationException("Ontbrekend embedded appsettings.json");
            builder.Configuration.AddJsonStream(stream);

            // Bind ApiSettings
            builder.Services.Configure<ApiSettings>(
                       builder.Configuration.GetSection("ApiSettings"));

            // API service
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<IDeliveryStateService, DeliveryStateService>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureSyncfusionCore();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Build en sla ServiceProvider op
            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}
