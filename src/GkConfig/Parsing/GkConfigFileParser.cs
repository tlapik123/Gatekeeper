using System.Security.Cryptography;

namespace gatekeeper.GkConfig.Parsing;

/// <summary>
/// Loading settings from `=` delimited file.
/// </summary>
public class GkConfigFileParser : IGkConfigParser {
    private readonly string _settingsFile;

    private readonly List<string> _neededSettings = new() {
        "token",
        "email_server",
        "email_username",
        "email_password",
        "hash_algorithm",
        "hash_salt",
        "hash_save_file",
    };

    /// <summary>
    /// <see cref="GkConfigFileParser"/> requires settings file to read.
    /// </summary>
    /// <param name="settingsFile">File to read the settings from.</param>
    public GkConfigFileParser(string settingsFile) => _settingsFile = settingsFile;

    public async Task<Data.GkConfig> ParseConfigAsync() {
        if (await TryParseAsDict() is { } dict && AreAllSettingsThere(dict)) {
            // NOTE not the nicest, maybe do enum?
            return new Data.GkConfig(
                dict[_neededSettings[0]],
                dict[_neededSettings[1]],
                dict[_neededSettings[2]],
                dict[_neededSettings[3]],
                TryParseHashAlg(dict[_neededSettings[4]]),
                dict[_neededSettings[5]],
                dict[_neededSettings[6]]
            );
        }
        throw new ArgumentException("Setting file doesn't have all the required arguments! Or there is a syntax error near some `=`!");
    }

    private HashAlgorithm TryParseHashAlg(string gottenAlg) => gottenAlg switch {
        "sha256" => SHA256.Create(),
        "sha512" => SHA512.Create(),
        "md5" => MD5.Create(),
        _ => throw new ArgumentOutOfRangeException()
    };
    
    private bool AreAllSettingsThere(IReadOnlyDictionary<string, string> parsedDict) {
        return _neededSettings.All(parsedDict.ContainsKey);
    }

    private async Task<Dictionary<string, string>?> TryParseAsDict() {
        Dictionary<string, string> retDict = new();
        using var sr = new StreamReader(_settingsFile);

        var allContent = await sr.ReadToEndAsync();
        var wholeSplit = allContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in wholeSplit) {
            var tmpSplit = s.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (tmpSplit.Length != 2) return null;
            retDict[tmpSplit[0]] = tmpSplit[1];
        }

        return retDict;
    }
}