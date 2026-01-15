using AutoReacto.Core.Extensions;
using AutoReacto.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AutoReacto;

/// <summary>
/// Entry point for the AutoReacto Discord bot.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog early for startup logging
        Log.Logger = LoggerFactory.CreateLogger();

        try
        {
            Log.Information("Starting AutoReacto Discord Bot...");
            
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    services.AddAutoReactoServices(context.Configuration);
                })
                .Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
