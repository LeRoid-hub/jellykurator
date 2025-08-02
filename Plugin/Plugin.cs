using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Jellykurator.Configuration;
using Jellyfin.Data.Enums;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Jellykurator;

/// <summary>
/// The main plugin.
/// </summary>
public class JellykuratorPlugin : BasePlugin<PluginConfiguration>, IHasWebPages, IDisposable
{
    private readonly ILogger<JellykuratorPlugin> _logger;
    private readonly HttpClient _httpClient;
    private readonly Timer _exportTimer;
    private readonly ILibraryManager _libraryManager;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="JellykuratorPlugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="loggerFactory">Instance of the <see cref="ILoggerFactory"/> interface.</param>
    public JellykuratorPlugin(
        IApplicationPaths applicationPaths, 
        IXmlSerializer xmlSerializer,
        ILoggerFactory loggerFactory,
        ILibraryManager libraryManager)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = loggerFactory.CreateLogger<JellykuratorPlugin>();
        _httpClient = new HttpClient();
        _libraryManager = libraryManager;
        
        _logger.LogInformation("Jellykurator plugin initialized, starting export timer...");
        
        // Start the timer immediately and repeat every minute
        _exportTimer = new Timer(DoExport, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    /// <inheritdoc />
    public override string Name => "Jellykurator";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("b3d86c4e-cedd-4c3d-a871-33da1aa692b2");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static JellykuratorPlugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }

    private async Task SendMediaItemsAsync(List<object> mediaItems, string url, string token) {
        var json = JsonSerializer.Serialize(mediaItems, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var request = new HttpRequestMessage(HttpMethod.Post, url + "/v1/upload")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Jellykurator: Successfully sent media items to {Url}", url);
        }
        else
        {
            _logger.LogError("Jellykurator: Failed to send media items. Status code: {StatusCode}, Reason: {ReasonPhrase}",
                response.StatusCode, response.ReasonPhrase);
        }
    }

    private void DoExport(object? state)
    {
        try
        {
            _logger.LogInformation("Jellykurator: Running export at {Time}", DateTime.Now);
            var config = Configuration;
            var url = config.Host;
            var token = config.Token;

            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogWarning("Jellykurator: No export URL configured, skipping HTTP request");
                return;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Jellykurator: No export token configured, skipping HTTP request");
                return;
            }

            _logger.LogInformation("Jellykurator: Exporting data to {Url} with token {Token}", url, token);
            
            // Your export logic here
            // This runs in a background thread automatically
            var mediaItems = new List<object>();
            
            var movies = _libraryManager.GetItemList(new InternalItemsQuery
            {
                IncludeItemTypes = new[] { BaseItemKind.Movie},
                Recursive = true
            }).OfType<Movie>();
            
            foreach (var movie in movies) {
                var providerIds = new Dictionary<string, object>();
                if (movie.ProviderIds != null) {
                    foreach (var provider in movie.ProviderIds)
                    {
                        providerIds[provider.Key] = provider.Value;
                    }
                }
            
                mediaItems.Add(new
                {
                    type = "Movie",
                    id = movie.Id.ToString(),
                    name = movie.Name,
                    originalTitle = movie.OriginalTitle,
                    metadata_refs = providerIds
                });
            }
            
            var series = _libraryManager.GetItemList(new InternalItemsQuery
            {
                IncludeItemTypes = new[] { BaseItemKind.Series },
                Recursive = true
            }).OfType<Series>();

            foreach (var serie in series) {
                var providerIds = new Dictionary<string, object>();
                if (serie.ProviderIds != null) {
                    foreach (var provider in serie.ProviderIds)
                    {
                        providerIds[provider.Key] = provider.Value;
                    }
                }

                var seasons = _libraryManager.GetItemList(new InternalItemsQuery
                {
                    AncestorIds = new[] { serie.Id },
                    IncludeItemTypes = new[] { BaseItemKind.Season },
                    Recursive = true
                }).OfType<Season>().OrderBy(s => s.IndexNumber ?? 0);
                var seasonData = new List<object>();
                foreach (var season in seasons) {
                    var episodes = _libraryManager.GetItemList(new InternalItemsQuery{
                        AncestorIds = new[] { season.Id },
                        IncludeItemTypes = new[] { BaseItemKind.Episode },
                        Recursive = true
                    }).OfType<Episode>().OrderBy(e => e.IndexNumber ?? 0);

                    var episodeData = episodes.Select(episode => {

                        var episodeProviderIds = new Dictionary<string, object>();
                        if (episode.ProviderIds != null) {
                            foreach (var provider in episode.ProviderIds)
                            {
                                episodeProviderIds[provider.Key] = provider.Value;
                            }
                        }

                        return new {
                            episode_number = episode.IndexNumber ?? 0,
                            name = episode.Name,
                            id = episode.Id.ToString(),
                            metadata_refs = episodeProviderIds
                        };
                    }).ToList();
                    
                    seasonData.Add(new 
                    {
                        season_number = season.IndexNumber ?? 0,
                        name = season.Name,
                        id = season.Id.ToString(),
                        episodes = episodeData
                    });
                }

                mediaItems.Add(new
                {
                    type = "Series",
                    id = serie.Id.ToString(),
                    name = serie.Name,
                    originalTitle = serie.OriginalTitle,
                    metadata_refs = providerIds,
                    seasons = seasonData
                });
            }

            var samplesToLog = Math.Min(3, mediaItems.Count);
            for (int i = 0; i < samplesToLog; i++)
            {
                _logger.LogInformation("Jellykurator: Sample item {Index}: {@Item}", i + 1, mediaItems[i]);
            }
            _logger.LogInformation("Jellykurator: Found {MovieCount} movies", movies.Count());
            _logger.LogInformation("Jellykurator: Found {SeriesCount} series", series.Count());
            _logger.LogInformation("Jellykurator: Export completed");

            SendMediaItemsAsync(mediaItems, url, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Jellykurator: Error during export");
        }
    }

    /// <summary>
    /// Disposes the plugin resources.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the plugin resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _exportTimer?.Dispose();
            _logger.LogInformation("Jellykurator: Export timer disposed");
            _disposed = true;
        }
    }
}