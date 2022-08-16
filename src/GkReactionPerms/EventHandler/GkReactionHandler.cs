using DSharpPlus;
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
        if (await _reactionPermsDatabase.TryGetChannelAsync(e.Message, e.Emoji, e.Guild) is { } discordChannel &&
            e.User is DiscordMember member) {
            // try to update existing overwrite
            if (await TryUpdateUserOverwriteAsync(discordChannel, member, ap => ap | Permissions.AccessChannels)) {
                return;
            }
            // there wasn't any existing overwrite create new
            await discordChannel.AddOverwriteAsync(member, Permissions.AccessChannels);
        }
    }

    /// <summary>
    /// Fired when reaction is removed.
    /// Checks for reaction messages in the database and updates users accordingly. 
    /// </summary>
    public async Task MessageReactionRemoved(DiscordClient _, MessageReactionRemoveEventArgs e) {
        if (e.User.IsBot) return;
        if (await _reactionPermsDatabase.TryGetChannelAsync(e.Message, e.Emoji, e.Guild) is { } discordChannel &&
            e.User is DiscordMember member) {
            await TryUpdateUserOverwriteAsync(discordChannel, member, ap => ap ^ Permissions.AccessChannels);
            // there wasn't any existing already - dont change anything
        }
    }

    private async Task<bool> TryUpdateUserOverwriteAsync(DiscordChannel channel, DiscordMember member,
        Func<Permissions, Permissions> changeAllowedPermsFunc) {
        foreach (var overwrite in channel.PermissionOverwrites) {
            if (member.Equals(await overwrite.GetMemberAsync())) {
                await overwrite.UpdateAsync(changeAllowedPermsFunc(overwrite.Allowed));
                return true;
            }
        }

        return false;
    }
}