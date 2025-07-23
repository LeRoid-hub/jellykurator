using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Jellykurator;

/// <summary>
/// Background service for exporting data.
/// </summary>
public class ExportService : BackgroundService 
{
    private readonly ILogger<ExportService> _logger;
    private readonly JellykuratorPlugin _plugin;

    // Define LoggerMessage delegates for better performance
    private static readonly Action<ILogger, Exception?> LogStarted =
        LoggerMessage.Define(LogLevel.Information, new EventId(1, "ServiceStarted"), "Jellykurator ExportService started!");
    
    private static readonly Action<ILogger, Exception?> LogExporting =
        LoggerMessage.Define(LogLevel.Information, new EventId(2, "Exporting"), "Jellykurator: Exporting data...");
    
    private static readonly Action<ILogger, Exception?> LogError =
        LoggerMessage.Define(LogLevel.Error, new EventId(3, "ExportError"), "Jellykurator: Error occurred while exporting data");
    
    private static readonly Action<ILogger, Exception?> LogStopped =
        LoggerMessage.Define(LogLevel.Information, new EventId(4, "ServiceStopped"), "Jellykurator ExportService stopped");

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportService"/> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger{ExportService}"/> interface.</param>
    /// <param name="plugin">Instance of the <see cref="JellykuratorPlugin"/>.</param>    
    public ExportService(
        ILogger<ExportService> logger,
        JellykuratorPlugin plugin)
    {
        _logger = logger;
        _plugin = plugin;
    }

    /// <summary>
    /// Executes the background service.    
    /// </summary>
    /// <param name="stoppingToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogStarted(_logger, null);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                LogExporting(_logger, null);
                
                // Simulate some work
                await Task.Delay(2000, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogError(_logger, ex);
            }

            // Wait 1 minute before next run
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
        }

        LogStopped(_logger, null);
    }
}