using System.Text;

namespace gatekeeper.GkVerificationHandlers;

public class GkFileOfHashesHandler : IGkFileOfHashesHandler {
    private readonly string _filePath;

    public GkFileOfHashesHandler(string relativePath) {
        _filePath = relativePath;
        
        // create the file if it doesnt exist
        if (!File.Exists(_filePath)) {
            File.Create(_filePath);
        }
    }

    public async Task<bool> ContainsHashAsync(string hash) {
        using StreamReader sr = new(_filePath);
        while (sr.Peek() >= 0) {
            if (await sr.ReadLineAsync() == hash) {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> TryAddHashAsync(string hash) {
        if (await ContainsHashAsync(hash)) return false;

        await using StreamWriter sw = new(_filePath, append: true);
        await sw.WriteLineAsync(hash);
        return true;
    }

    public async Task<bool> TryRemoveHashAsync(string hash) {
        StringBuilder sb = new();
        var containsHash = false;
        using (StreamReader sr = new(_filePath)) {
            while (await sr.ReadLineAsync() is { } line) {
                if (line != hash) sb.AppendLine(line);
                else containsHash = true;
            }
        }

        if (!containsHash) return false;

        await using StreamWriter sw = new(_filePath, append: false);
        await sw.WriteAsync(sb);
        return true;
    }
}