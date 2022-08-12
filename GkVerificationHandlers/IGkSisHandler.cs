namespace gatekeeper.GkVerificationHandlers; 


/*
 * MFF
 *  - studies -> verified
 *
 * Other
 *  - * -> invader
 */

/// <summary>
/// Handles getting info of SIS.
/// </summary>
public interface IGkSisHandler {
    /// <summary>
    /// Gets email off SIS.
    /// </summary>
    /// <param name="ukco">User ukco to get the email from</param>
    /// <returns>
    /// If the user is from MFF and has visible email then it will return said email.
    /// Else returns null
    /// </returns>
    public Task<string?> GetEmailOfMffAsync(int ukco);
}