using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using gatekeeper.GkPermsHandler;

namespace gatekeeper.GkModules;

/// <summary>
/// Houses commands regarding permission management.
/// </summary>
public class GkRoleManagementModule : BaseCommandModule {
    private readonly IArgumentConverter<DiscordMessage> _messageConverter;
    private readonly IArgumentConverter<DiscordEmoji> _emojiConverter;
    private readonly IArgumentConverter<DiscordChannel> _channelConverter;
    private readonly IArgumentConverter<bool> _boolConverter;
    private readonly IGkReactionPermsDatabase _reactionPermsDatabase;

    public GkRoleManagementModule(
        IArgumentConverter<DiscordMessage> messageConverter,
        IArgumentConverter<DiscordEmoji> emojiConverter,
        IArgumentConverter<DiscordChannel> channelConverter,
        IArgumentConverter<bool> boolConverter,
        IGkReactionPermsDatabase reactionPermsDatabase
    ) {
        _messageConverter = messageConverter;
        _emojiConverter = emojiConverter;
        _channelConverter = channelConverter;
        _boolConverter = boolConverter;
        _reactionPermsDatabase = reactionPermsDatabase;
    }

    [Command("reaction_to_access_perms")]
    [Description("Assigns given reactions to be linked to given permissions on given channel and given message")]
    [RequireGuild]
    public async Task ReactionToAccessPermsCommand(CommandContext ctx) {
        var interactivity = ctx.Client.GetInteractivity();

        await ctx.RespondAsync(@"Stating creation process!
What message will users be reacting to? (Enter valid message ID)"
        );

        DiscordMessage? message = null;
        Dictionary<DiscordEmoji, DiscordChannel> changeDict = new();
        
        // TODO: let the user end in any moment?

        var chain = (await interactivity.WaitForMessageAsync(m => {
            var taskRes = _messageConverter.ConvertAsync(m.Content, ctx).Result;
            if (!taskRes.HasValue) return false;
            message = taskRes.Value;
            return true;
        })).Result;


        while (true) {
            DiscordEmoji? tmpEmoji = null;

            await chain.RespondAsync("What emoji will they use? (Enter an emoji)");
            chain = (await interactivity.WaitForMessageAsync(m => {
                var taskRes = _emojiConverter.ConvertAsync(m.Content, ctx).Result;
                if (!taskRes.HasValue) return false;
                tmpEmoji = taskRes.Value;
                return true;
            })).Result;

            DiscordChannel? tmpChannel = null;

            await chain.RespondAsync("What channel will they have access to? (Enter a channel)");
            chain = (await interactivity.WaitForMessageAsync(m => {
                var taskRes = _channelConverter.ConvertAsync(m.Content, ctx).Result;
                if (!taskRes.HasValue) return false;
                tmpChannel = taskRes.Value;
                return true;
            })).Result;
            
            // TODO: null when timeout
            changeDict[tmpEmoji] = tmpChannel;

            await chain.RespondAsync("Specify more emoji -> channel pairs?");
            chain = (await interactivity.WaitForMessageAsync(_ => true)).Result;
            if (await _boolConverter.ConvertAsync(chain.Content, ctx) is { HasValue: true, Value: false, }) break;
        }
        await _reactionPermsDatabase.AddOrUpdateChannel(message.Id, changeDict);
        // indicate user with reaction to the message from bot and with message
        foreach (var emoji in changeDict.Keys) {
            await message.CreateReactionAsync(emoji);
        }
        
        await chain.RespondAsync("All set!");
    }
}