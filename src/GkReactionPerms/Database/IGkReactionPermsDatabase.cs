using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace gatekeeper.GkReactionPerms.Database;

/// <summary>
/// Generic permission database.
/// </summary>
public interface IGkReactionPermsDatabase {
    /// <summary>
    /// Gets the channel to do permission on from the database
    /// </summary>
    /// <param name="message">Discord message to find binding for</param>
    /// <param name="reaction">What reaction</param>
    /// <param name="guild">Guild to search the channel in</param>
    /// <returns>
    /// What channel to apply permissions on or null when no channel was found.
    /// </returns>
    public Task<DiscordChannel?> TryGetChannelAsync(DiscordMessage message, DiscordEmoji reaction, DiscordGuild guild);

    /// <summary>
    /// Adds or updates message that user reacts to - {emoji, channel} pairs
    /// </summary>
    /// <param name="message">Message to update bindings for.</param>
    /// <param name="emojiChannelPairs">Emoji to Channel bindings to update or add.</param>
    public Task AddOrUpdateMessageAsync(DiscordMessage message, IEnumerable<(DiscordEmoji, DiscordChannel)> emojiChannelPairs);

    /// <summary>
    /// Adds or updates single pair in the database - this keeps all the other pairs untouched.
    /// </summary>
    /// <param name="message">Reaction message to update the binding on.</param>
    /// <param name="emoji">Emoji to add or update.</param>
    /// <param name="channel">Channel to add or update.</param>
    /// <returns></returns>
    public Task AddOrUpdateSinglePair(DiscordMessage message, DiscordEmoji emoji, DiscordChannel channel);
}