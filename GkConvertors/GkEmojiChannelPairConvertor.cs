using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace gatekeeper.GkConvertors;

using EmojiChannelPairs = Dictionary<DiscordEmoji, DiscordChannel>;

public class GkEmojiChannelPairConvertor : IArgumentConverter<EmojiChannelPairs> {
    private readonly IArgumentConverter<DiscordEmoji> _emojiConvertor;
    private readonly IArgumentConverter<DiscordChannel> _channelConvertor;

    public GkEmojiChannelPairConvertor(IArgumentConverter<DiscordEmoji> emojiConvertor,
        IArgumentConverter<DiscordChannel> channelConvertor) =>
        (_emojiConvertor, _channelConvertor) = (emojiConvertor, channelConvertor);
    
    /// <summary>
    /// Converts string to the Dictionary of emoji to channel pairs.
    /// The string format has to be:
    ///     emoji:channel, emoji:channel, ...
    /// separated by any number of spaces.
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="ctx">Command context</param>
    /// <returns>
    /// Empty optional - thing could not be converted.
    /// NON-empty optional - thing was converted successfully.
    /// </returns>
    public async Task<Optional<EmojiChannelPairs>> ConvertAsync(string value, CommandContext ctx) {
        var retPairs = new Dictionary<DiscordEmoji, DiscordChannel>();
        var keyValues = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var keyValue in keyValues) {
            var pair = keyValue.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (pair.Length != 2) {
                return Optional.FromNoValue<EmojiChannelPairs>();
            }

            var emojiStr = pair[0].Trim();
            var channelStr = pair[1].Trim();
            if (await _emojiConvertor.ConvertAsync(emojiStr, ctx) is not { HasValue: true } emojiOpt ||
                await _channelConvertor.ConvertAsync(channelStr, ctx) is not { HasValue: true } channelOpt) {
                return Optional.FromNoValue<EmojiChannelPairs>();
            }

            retPairs[emojiOpt.Value] = channelOpt.Value;
        }

        return retPairs;
    }
}