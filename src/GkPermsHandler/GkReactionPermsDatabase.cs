using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DSharpPlus.Entities;

namespace gatekeeper.GkPermsHandler;

public class GkReactionPermsDatabase : IGkReactionPermsDatabase {
    // reaction message id -> {reaction -> channel}
    private readonly Dictionary<ulong, IReadOnlyDictionary<DiscordEmoji, DiscordChannel>>
        _database = new();

    public Task<DiscordChannel?> TryGetChannel(ulong messageId, DiscordEmoji reaction) {
        if (_database.TryGetValue(messageId, out var secondDict)
            && secondDict.TryGetValue(reaction, out var channelToApplyPerm)) {
            return Task.FromResult<DiscordChannel?>(channelToApplyPerm);
        }

        return Task.FromResult<DiscordChannel?>(null);
    }

    public Task AddOrUpdateChannel(ulong messageId, IReadOnlyDictionary<DiscordEmoji, DiscordChannel> emojiToChannelDict) {
        _database[messageId] = emojiToChannelDict;
        Serialize();
        return Task.CompletedTask;
    }

    private void Serialize() {
        string jsonString = JsonSerializer.Serialize(_database);
        Console.WriteLine(jsonString);
    }
}