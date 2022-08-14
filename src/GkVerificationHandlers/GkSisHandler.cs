using System.Net;

namespace gatekeeper.GkVerificationHandlers;

public class GkSisHandler : IGkSisHandler {
    private const string IncompleteSearchUri =
        "https://is.cuni.cz/studium/kdojekdo/index.php?do=hledani&koho=s&fakulta=11320&prijmeni=&jmeno=&login=&r_zacatek=Z&sustav=&sobor_mode=text&sims_mode=text&sdruh=&svyjazyk=&pocet=50&vyhledej=Vyhledej&sidos=";
    private const string IncompleteProfileUri = "https://is.cuni.cz/studium/kdojekdo/index.php?do=detail&si=";
    private const string ProfileSearch = "?do=detail&amp;si=";
    private const string MailSearch = "href=\"mailto:";
    private const char EndSymbol = '>';
    
    private const string ErrorPrompt = "Počet studentů, které nemáte právo vyhledat:";
    
    private readonly HttpClient _client;

    public GkSisHandler(HttpClient client) => _client = client;

    public async Task<string?> GetEmailOfMffAsync(int ukco) {
        if (await GetUsefulStringAsync(IncompleteSearchUri + ukco, ProfileSearch) is { } si) {
            if (await GetUsefulStringAsync(IncompleteProfileUri + si, MailSearch) is { } email) {
                return WebUtility.HtmlDecode(email);
            }
        }
        // TODO: get to error searching

        return null;
    }
    
    /// <summary>
    /// Gets needed string from the uri.
    /// </summary>
    /// <param name="uriToSearch">Uri to search the string in</param>
    /// <param name="toMatch">String to match in uri</param>
    /// <returns>
    /// Returns the string behind the <paramref name="toMatch"/> string.
    /// </returns>
    private async Task<string?> GetUsefulStringAsync(string uriToSearch, string toMatch) {
        var responseBody = await _client.GetStringAsync(uriToSearch);

        if (responseBody.IndexOf(toMatch, StringComparison.Ordinal) is var idx && idx != -1) {
            var startIdx = idx + toMatch.Length;
            var endIdx = responseBody.IndexOf(EndSymbol, startIdx);
            var tmpSplit = responseBody.Substring(startIdx, endIdx - startIdx).Split('"');
            return tmpSplit[0];
        }

        return null;
    }
}