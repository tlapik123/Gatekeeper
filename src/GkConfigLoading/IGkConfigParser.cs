using gatekeeper.GkDataStructures;

namespace gatekeeper.GkConfigLoading; 

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
    public Task<GkConfig> ParseConfigAsync();
}