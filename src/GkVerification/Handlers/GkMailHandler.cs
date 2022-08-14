using System.Net.Mail;

namespace gatekeeper.GkVerification.Handlers; 

public class GkMailHandler : IGkMailHandler {
    
    private readonly SmtpClient _client;
    private readonly string _from;

    public GkMailHandler(SmtpClient client, string from) => (_client, _from) = (client, from);
    
    public async Task SendVerificationCodeAsync(string email, int code) {
        await _client.SendMailAsync(_from, email, "Here is your code from MFF server!", code.ToString());
    }
}