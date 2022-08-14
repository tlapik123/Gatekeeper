using DSharpPlus.Entities;

namespace gatekeeper.GkReactionPerms.Database;

/// <summary>
/// Generic permission database.
/// </summary>
public interface IGkReactionPermsDatabase {
    /// <summary>
    /// Gets the channel to do permission on from the database
    /// </summary>
    /// <param name="messageId">ID of the message reacted to</param>
    /// <param name="reaction">What reaction</param>
    /// <returns>
    /// What channel to apply permissions on or null when no channel was found.
    /// </returns>
    public Task<DiscordChannel?> TryGetChannel(ulong messageId, DiscordEmoji reaction);
    
    /// <summary>
    /// Adds or updates entry in the database
    /// </summary>
    /// <param name="messageId">Id of the messages users will react to</param>
    /// <param name="emojiToChannelDict">emoji -> what channel permission to change binding</param>
    public Task AddOrUpdateChannel(ulong messageId, IReadOnlyDictionary<DiscordEmoji, DiscordChannel> emojiToChannelDict);
}