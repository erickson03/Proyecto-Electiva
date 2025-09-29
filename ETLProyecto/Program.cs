using ETLProyecto.Data.Connections;
using ETLProyecto.Services.Implementations;
using ETLProyecto.Services.Interfaces;
using ETLProyecto.Services.Orchestrators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Configuración de appsettings.json
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Servicios de base de datos
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Servicios ETL con interfaces
        services.AddSingleton<IProductoService, ProductoService>();
        services.AddSingleton<IClienteService, ClienteService>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<ICsvReaderService, CsvReaderService>();

        // Runner ETL
        services.AddTransient<EtlRunner>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

// Ejecutar pipeline ETL
var runner = host.Services.GetRequiredService<EtlRunner>();
await runner.RunAsync();
