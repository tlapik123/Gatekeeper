// inspired by https://github.com/DSharpPlus/Example-Bots/blob/master/DSPlus.Examples.CSharp.Ex02/Program.cs
// bot example done by Emzi0767

using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace gatekeeper.GkLogging;

/// <summary>
/// Very simple logging extension for commands.
/// </summary>
public static class GkCommandLogging {
    private static readonly EventId BotEventId = new(22, "Gatekeeper");
    
    /// <summary>
    /// Logs that command successfully finished.
    /// </summary>
    public static Task CommandExecuted(CommandsNextExtension _, CommandExecutionEventArgs e) {
        // let's log the name of the command and user
        e.Context.Client.Logger.LogInformation(BotEventId,
            $"{e.Context.User.Username} successfully finished '{e.Command.QualifiedName}'");
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Logs what error was fired and what command was errored.
    /// </summary>
    public static Task CommandErrored(CommandsNextExtension _, CommandErrorEventArgs e) {
        // let's log the error details
        e.Context.Client.Logger.LogError(BotEventId,
            $"{e.Context.User.Username} tried executing '{e.Command.QualifiedName}' but it errored: {e.Exception.GetType()}: {e.Exception.Message}",
            DateTime.Now);
        return Task.CompletedTask;
    }
}