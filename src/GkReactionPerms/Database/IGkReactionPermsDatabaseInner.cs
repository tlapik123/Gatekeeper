namespace gatekeeper.GkReactionPerms.Database; 

using EmojiChannelPairs = Dictionary<(ulong, string), ulong>;
public interface IGkReactionPermsDatabaseInner {
    public Task<ulong?> TryGetChannelAsync(ulong messageId, (ulong, string) reaction);

    public Task AddOrUpdateMessageAsync(ulong messageId, EmojiChannelPairs emojiToChannelDict);

    public Task AddOrUpdateSinglePairAsync(ulong messageId, (ulong, string) reaction, ulong channel);
}