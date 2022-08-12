using System.Diagnostics.CodeAnalysis;
using DSharpPlus.Entities;

namespace gatekeeper.GkPermsHandler;

/// <summary>
/// Generic permission database.
/// </summary>
public interface IGkReactionPermsDatabase {
    /// <summary>
    /// Gets the channel to do permission on from the database
    /// </summary>
    /// <param name="messageId">ID of the message reacted to</param>
    /// <param name="reaction">What reaction</param>
    /// <param name="channelToApplyPerm">What channel to apply permissions on</param>
    /// <returns>
    /// True: there is such channel for such message and reaction
    /// False: there is no such channel
    /// </returns>
    public Task<bool> TryGetChannel(ulong messageId, DiscordEmoji reaction, [MaybeNullWhen(false)] out DiscordChannel channelToApplyPerm);
    
    /// <summary>
    /// Adds or updates entry in the database
    /// </summary>
    /// <param name="messageId">Id of the messages users will react to</param>
    /// <param name="emojiToChannelDict">emoji -> what channel permission to change binding</param>
    public Task AddOrUpdateChannel(ulong messageId, Dictionary<DiscordEmoji, DiscordChannel> emojiToChannelDict);
}