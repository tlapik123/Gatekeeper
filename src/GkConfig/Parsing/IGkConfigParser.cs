using gatekeeper.GkConfig.Data;

namespace gatekeeper.GkConfig.Parsing; 

/// <summary>
/// Represents generic config parser.
/// </summary>
public interface IGkConfigParser {
    
    /// <summary>
    /// Get the config record.
    /// </summary>
    /// <returns>
    /// Returns <see cref="GkConfig"/>.
    /// </returns>
    public Task<GkConfig.Data.GkConfig> ParseConfigAsync();
}