namespace gatekeeper.GkVerificationHandlers;

/// <summary>
/// Handles reading and writing to the hash file
/// </summary>
public interface IGkFileOfHashesHandler {
    /// <summary>
    /// Checks if file contains certain hash.
    /// </summary>
    /// <param name="hash">Hash to check</param>
    /// <returns>
    /// Return value indicates whether there was hash in the file.
    /// </returns>
    public Task<bool> ContainsHashAsync(string hash);

    /// <summary>
    /// Tries to add hash to the file.
    /// </summary>
    /// <param name="hash">Hash to add.</param>
    /// <returns>
    /// True: hash was added
    /// False: there is already same hash.
    /// </returns>
    public Task<bool> TryAddHashAsync(string hash);

    /// <summary>
    /// Tries to remove hash from the file.
    /// </summary>
    /// <param name="hash">Hash to remove.</param>
    /// <returns>
    /// False: File didn't contain the hash.
    /// True: Hash was deleted. 
    /// </returns>
    public Task<bool> TryRemoveHashAsync(string hash);
}