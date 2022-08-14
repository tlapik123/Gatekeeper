namespace gatekeeper.GkVerification.Handlers; 

public interface IGkHashHandler {
    /// <summary>
    /// Compute backwards hash of ukco.
    /// </summary>
    /// <param name="ukco">Ukco to hash.</param>
    /// <returns>Hashed hex string.</returns>
    public string ComputeHashFromUkco(string ukco);
}