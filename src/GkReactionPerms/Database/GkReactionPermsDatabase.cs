using DSharpPlus.Entities;

namespace gatekeeper.GkReactionPerms.Database;

using EmojiChannelPairs = Dictionary<(ulong, string), ulong>;

public class GkReactionPermsDatabase : IGkReactionPermsDatabase {
    private readonly IGkReactionPermsDatabaseInner _database;

    public GkReactionPermsDatabase(IGkReactionPermsDatabaseInner database) => _database = database;

    public async Task<DiscordChannel?> TryGetChannelAsync(
        DiscordMessage message,
        DiscordEmoji reaction,
        DiscordGuild guild
    ) {
        if (await _database.TryGetChannelAsync(message.Id, (reaction.Id, reaction.Name)) is { } channelId) {
            // this can also return null, for example if the channel was deleted after binding was created
            return guild.GetChannel(channelId);
        }

        return null;
    }

    public Task AddOrUpdateMessageAsync(
        DiscordMessage message,
        IEnumerable<(DiscordEmoji, DiscordChannel)> emojiChannelPairs
    ) {
        return _database.AddOrUpdateMessageAsync(message.Id, ConvertToIdDict(emojiChannelPairs));
    }

    public Task AddOrUpdateSinglePair(DiscordMessage message, DiscordEmoji emoji, DiscordChannel channel) {
        return _database.AddOrUpdateSinglePairAsync(message.Id, (emoji.Id, emoji.Name), channel.Id);
    }

    /// <summary>
    /// Converts discord Emoji - Channel pairs to format that the database can recognize.
    /// </summary>
    /// <param name="emojiChannelPairs">Emoji Channel pairs to convert.</param>
    /// <returns></returns>
    private EmojiChannelPairs ConvertToIdDict(
        IEnumerable<(DiscordEmoji, DiscordChannel)> emojiChannelPairs
    ) {
        EmojiChannelPairs retDict = new();
        foreach (var (emoji, channel) in emojiChannelPairs) {
            retDict[(emoji.Id, emoji.Name)] = channel.Id;
        }

        return retDict;
    }
}