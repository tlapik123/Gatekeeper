namespace gatekeeper.GkReactionPerms.Database;

using EmojiChannelPairs = Dictionary<(ulong, string), ulong>;

public class GkReactionPermsDatabaseInner : IGkReactionPermsDatabaseInner {
    /// <summary>
    /// reaction messageId -> {(reactionId, reactionName) -> channelId}
    /// </summary>
    private readonly Dictionary<ulong, EmojiChannelPairs>
        _database = new();

    public Task<ulong?> TryGetChannelAsync(ulong messageId, (ulong,string) reaction) {
        if (_database.TryGetValue(messageId, out var secondDict)
            && secondDict.TryGetValue(reaction, out var channelToApplyPerm)) {
            return Task.FromResult<ulong?>(channelToApplyPerm);
        }

        return Task.FromResult<ulong?>(null);
    }

    public Task AddOrUpdateMessageAsync(ulong messageId, EmojiChannelPairs emojiToChannelDict) {
        _database[messageId] = emojiToChannelDict;
        return Task.CompletedTask;
    }

    public Task AddOrUpdateSinglePairAsync(ulong messageId, (ulong,string) reaction, ulong channel) {
        // TODO make more functions that add or update, not only one
        _database[messageId][reaction] = channel;
        return Task.CompletedTask;
    }

    /*private void Serialize() {
        string jsonString = JsonSerializer.Serialize(_database);
        Console.WriteLine(jsonString);
    }*/
}