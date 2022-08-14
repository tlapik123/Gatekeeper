namespace gatekeeper.GkVerification.Handlers;

/// <summary>
/// Gets the email and sends the verification code to it. 
/// </summary>
public interface IGkMailHandler {
    /// <summary>
    /// Sends verification code to the specified email address.
    /// </summary>
    /// <param name="email">Address to send the code to</param>
    /// <param name="code">Code to send</param>
    public Task SendVerificationCodeAsync(string email, int code);
}