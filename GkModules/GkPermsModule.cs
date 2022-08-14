using System.Globalization;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using gatekeeper.GkPermsHandler;

namespace gatekeeper.GkModules;

/// <summary>
/// Houses commands regarding permission management.
/// </summary>
public class GkPermsModule : BaseCommandModule {
    private readonly IGkReactionPermsDatabase _reactionPermsDatabase;
    private readonly IDictionary<Type, IArgumentConverter> _convertors;

    public GkPermsModule(
        IArgumentConverter<DiscordMessage> messageConverter,
        IArgumentConverter<DiscordEmoji> emojiConverter,
        IArgumentConverter<DiscordChannel> channelConverter,
        IArgumentConverter<bool> boolConverter,
        IGkReactionPermsDatabase reactionPermsDatabase
    ) {
        _reactionPermsDatabase = reactionPermsDatabase;
        _convertors = new Dictionary<Type, IArgumentConverter> {
            { messageConverter.GetType(), messageConverter },
            { emojiConverter.GetType(), emojiConverter },
            { channelConverter.GetType(), channelConverter },
            { boolConverter.GetType(), boolConverter },
        };
    }

    [Command("react_to_perms")]
    [Description(
        "Assigns given reactions to be linked to given permissions on given channel and given message. Uses interations.")]
    [RequireGuild]
    public async Task SetReactionToPermissionsInteraction(CommandContext ctx) {
        var interactivity = ctx.Client.GetInteractivity();

        await ctx.RespondAsync(@"Stating creation process!
What message will users be reacting to? (Enter valid message ID)"
        );

        Dictionary<DiscordEmoji, DiscordChannel> changeDict = new();

        // TODO: let the user end in any moment?
        if (await TryGetResponseAsync<DiscordMessage>(interactivity, ctx) is not { } mRes) {
            await TimedOutRespondAsync(ctx.Message);
            return;
        }

        var (messageChain, message) = mRes;
        // Get the emoji-channel pairs
        while (true) {
            await messageChain.RespondAsync("What emoji will they use? (Enter an emoji)");
            if (await TryGetResponseAsync<DiscordEmoji>(interactivity, ctx) is not { } eRes) {
                await TimedOutRespondAsync(messageChain);
                return;
            }

            (messageChain, var tmpEmoji) = eRes;

            await messageChain.RespondAsync("What channel will they have access to? (Enter a channel)");
            if (await TryGetResponseAsync<DiscordChannel>(interactivity, ctx) is not { } cRes) {
                await TimedOutRespondAsync(messageChain);
                return;
            }

            (messageChain, var tmpChannel) = cRes;

            changeDict[tmpEmoji] = tmpChannel;

            await messageChain.RespondAsync("Specify more emoji -> channel pairs?");
            if (await TryGetResponseAsync<bool>(interactivity, ctx) is not { } bRes) {
                await TimedOutRespondAsync(messageChain);
                return;
            }

            (messageChain, var tmpBool) = bRes;
            if (!tmpBool) {
                await messageChain.RespondAsync("OK! No more emoji channel pairs!");
                break;
            }
        }
        // add binding to database
        await _reactionPermsDatabase.AddOrUpdateChannel(message.Id, changeDict);

        // indicate user with reaction to the message from bot and with message
        foreach (var emoji in changeDict.Keys) {
            await message.CreateReactionAsync(emoji);
        }

        await messageChain.RespondAsync("All set!");
    }

    [Command("react_to_perms_c")]
    [Description(
        "Assigns given reactions to be linked to given permissions on given channel and given message. Uses command arguments.")]
    [RequireGuild]
    public async Task SetReactionToPermissionsCommand(
        CommandContext ctx,
        DiscordMessage bindingMessage,
        [RemainingText] Dictionary<DiscordEmoji, DiscordChannel> bindings
    ) {
        await ctx.RespondAsync("Setting up given bindings!");

        await _reactionPermsDatabase.AddOrUpdateChannel(bindingMessage.Id, bindings);
        // indicate user with reaction to the message from bot and with message
        foreach (var emoji in bindings.Keys) {
            await bindingMessage.CreateReactionAsync(emoji);
        }

        await ctx.RespondAsync("All set!");
    }

    /// <summary>
    /// Tries to get response from the user convertable to given Type. 
    /// </summary>
    /// <param name="interactivity">Interactivity</param>
    /// <param name="ctx">Command context</param>
    /// <typeparam name="T">
    /// Only response convertible to this Type will be accepted.
    /// </typeparam>
    /// <returns>
    /// Tuple of DiscordMessage and Type result - if the response didn't time out.
    /// Null - response timed out.
    /// </returns>
    private async Task<(DiscordMessage, T)?> TryGetResponseAsync<T>(InteractivityExtension interactivity,
        CommandContext ctx) {
        T? retType = default;
        // get correct convertor
        if (TryGetConvertor<T>() is not { } convertor) return null;

        // chain is null only if TimedOut is true.
        var interactivityResult = await interactivity.WaitForMessageAsync(m => {
            var taskRes = convertor.ConvertAsync(m.Content, ctx).Result;
            if (!taskRes.HasValue) return false;
            retType = taskRes.Value;
            return true;
        });
        if (interactivityResult.TimedOut || retType is null) return null;

        return (interactivityResult.Result, retType);
    }

    /// <summary>
    /// Tries to get convertor from available convertors that were given to this module.
    /// </summary>
    /// <typeparam name="T">Type of thing that the convertor will convert. (IArgumentConverter of T)</typeparam>
    /// <returns>
    /// IArgumentConverter of T - a convertor that can convert to type T. 
    /// </returns>
    private IArgumentConverter<T>? TryGetConvertor<T>() {
        return _convertors.TryGetValue(typeof(IArgumentConverter<T>), out var convertor)
            ? (IArgumentConverter<T>)convertor
            : null;
    }

    /// <summary>
    /// Inform user about the time out.
    /// </summary>
    /// <param name="message">Message to respond to.</param>
    private static async Task TimedOutRespondAsync(DiscordMessage message) {
        await message.RespondAsync("No valid response was given, timed-out! Closing the interactivity!");
    }
}