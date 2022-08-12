using System.Diagnostics.CodeAnalysis;
using DSharpPlus.Entities;

namespace gatekeeper.GkPermsHandler;

public class GkReactionPermsDatabase : IGkReactionPermsDatabase {
    // reaction message id -> {reaction -> channel}
    private readonly Dictionary<ulong, Dictionary<DiscordEmoji, DiscordChannel>>
        _database = new();

    public Task<bool> TryGetChannel(ulong messageId, DiscordEmoji reaction,
        [MaybeNullWhen(false)] out DiscordChannel channelToApplyPerm) {
        channelToApplyPerm = null;
        return Task.FromResult(_database.TryGetValue(messageId, out var secondDict)
                               && secondDict.TryGetValue(reaction, out channelToApplyPerm));
    }

    public Task AddOrUpdateChannel(ulong messageId, Dictionary<DiscordEmoji, DiscordChannel> emojiToChannelDict) {
        _database[messageId] = emojiToChannelDict;
        return Task.CompletedTask;
    }
}