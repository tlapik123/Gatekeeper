using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace gatekeeper.GkConvertors;

public class GkBoolConvertor : IArgumentConverter<bool> {
    /// <summary>
    /// Converts string to a bool value with some custom rules for english "yes" and "no" (and more variants).
    /// Variants:
    ///     yes, y, t -> true
    ///     no, n, f -> false
    /// </summary>
    /// <param name="value">String value to covert</param>
    /// <param name="_">Command context</param>
    /// <returns>
    /// Either empty optional if the string could not be found, otherwise optional with parsed bool value.
    /// </returns>
    public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx) {
        if (bool.TryParse(value, out var boolean)) {
            return Task.FromResult(Optional.FromValue(boolean));
        }

        return value.ToLower() switch {
            "yes" or "y" or "t" => Task.FromResult(Optional.FromValue(true)),
            "no" or "n" or "f" => Task.FromResult(Optional.FromValue(false)),
            _ => Task.FromResult(Optional.FromNoValue<bool>()),
        };
    }
}