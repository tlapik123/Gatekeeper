namespace gatekeeper.GkReactionPerms.Database; 

// {(reactionId, reactionName) -> channelId}
using EmojiChannelPairs = Dictionary<(ulong, string), ulong>;
public interface IGkReactionPermsDatabaseInner {
    /// <summary>
    /// Tries to fetch channelID from the database. If no channelId is found for given message and reaction, then null is returned.
    /// </summary>
    /// <param name="messageId">MessageId to search.</param>
    /// <param name="reaction">Reaction to search.</param>
    /// <returns>
    /// ChannelId if found, null if not.
    /// </returns>
    public Task<ulong?> TryGetChannelAsync(ulong messageId, (ulong, string) reaction);

    /// <summary>
    /// Adds or updates the message in the database - updates if there is already such messageId entry.
    /// </summary>
    /// <param name="messageId">MessageId entry to add or update.</param>
    /// <param name="emojiToChannelDict">EmojiChannelPairs to update the entry with.</param>
    public Task AddOrUpdateMessageAsync(ulong messageId, EmojiChannelPairs emojiToChannelDict);

    /// <summary>
    /// Adds or updates single reaction-channel pair on given message - updates if the reaction entry is already there.
    /// IF no messageId is found in the database, then it throws KeyNotFoundException.
    /// TODO don't throw exceptions
    /// </summary>
    /// <param name="messageId">MessageId to find.</param>
    /// <param name="reaction">Reaction to add or update.</param>
    /// <param name="channel">Channel to add or update.</param>
    /// <exception cref="KeyNotFoundException">No messageId was found in the database.</exception>
    public Task AddOrUpdateSinglePairAsync(ulong messageId, (ulong, string) reaction, ulong channel);
}