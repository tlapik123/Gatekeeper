using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using gatekeeper.GkReactionPerms.Database;

namespace gatekeeper.GkReactionPerms.EventHandler;

public sealed class GkReactionHandler {
    private readonly IGkReactionPermsDatabase _reactionPermsDatabase;

    public GkReactionHandler(IGkReactionPermsDatabase reactionPermsDatabase) =>
        _reactionPermsDatabase = reactionPermsDatabase;

    /// <summary>
    /// Fired when reaction is added.
    /// Checks for reaction messages in the database and updates users accordingly. 
    /// </summary>
    public async Task MessageReactionAdded(DiscordClient _, MessageReactionAddEventArgs e) {
        if (e.User.IsBot) return;
        if (await _reactionPermsDatabase.TryGetChannelAsync(e.Message, e.Emoji, e.Guild) is {} discordChannel && e.User is DiscordMember member) {
            await discordChannel.AddOverwriteAsync(member, Permissions.AccessChannels);
        }
    }

    /// <summary>
    /// Fired when reaction is removed.
    /// Checks for reaction messages in the database and updates users accordingly. 
    /// </summary>
    public async Task MessageReactionRemoved(DiscordClient _, MessageReactionRemoveEventArgs e) {
        if (e.User.IsBot) return;
        if (await _reactionPermsDatabase.TryGetChannelAsync(e.Message, e.Emoji, e.Guild) is {} discordChannel && e.User is DiscordMember member) {
            // add empty member - this overwrites any permission in the channel
            // TODO only disallow access, leave rest of the perms intact
            await discordChannel.AddOverwriteAsync(member);
        }
    }
}