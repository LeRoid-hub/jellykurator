using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Jellykurator.Configuration;
/// <summary>
/// Plugin configuration
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Default constructor for the plugin configuration.
    /// </summary>
    public PluginConfiguration()
    {
        Host = "";
        Token = "";
    }

    /// <summary>
    /// The host URL for the Jellykurator service.
    /// </summary>
    public string Host { get; set; }
    
    /// <summary>
    /// The authentication token for the Jellykurator service.
    /// </summary>
    public string Token { get; set; }
}
