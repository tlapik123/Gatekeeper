using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using gatekeeper.GkVerification.Handlers;

namespace gatekeeper.GkVerification.Module;

public record struct VerificationData(string Hash, int Code, int NumOfTries);

/// <summary>
/// Houses commands regarding verification.
/// </summary>
public class GkVerificationModule : BaseCommandModule {
    // UserId -> hash, code, tries 
    private static readonly Dictionary<ulong, VerificationData> ActiveVerifications = new();
    private const int StartNumOfTries = 5;

    private readonly IGkSisHandler _sisHandler;
    private readonly IGkMailHandler _mailHandler;
    private readonly IGkHashHandler _hashHandler;
    private readonly IGkFileOfHashesHandler _fileOfHashesHandler;
    private readonly Random _rnd;

    public GkVerificationModule(IGkSisHandler sisHandler, IGkMailHandler mailHandler, IGkHashHandler hashHandler,
        IGkFileOfHashesHandler fileOfHashesHandler, Random rnd) {
        _sisHandler = sisHandler;
        _mailHandler = mailHandler;
        _hashHandler = hashHandler;
        _fileOfHashesHandler = fileOfHashesHandler;
        _rnd = rnd;
    }

    [Command("verify")]
    [Description("Starts the verification process.")]
    public async Task VerifyCommand(CommandContext ctx, int ukco) {
        await ctx.RespondAsync("Starting verification process!");
        var hash = _hashHandler.ComputeHashFromUkco(ukco.ToString());
        var userId = ctx.User.Id;
        // check if the user is already verified or doing verification
        if (!ActiveVerifications.ContainsKey(userId) && !await _fileOfHashesHandler.ContainsHashAsync(hash)) {
            if (await _sisHandler.GetEmailOfMffAsync(ukco) is { } email) {
                // create verification code
                var code = _rnd.Next(999_999, 9_999_999);
                await _mailHandler.SendVerificationCodeAsync(email, code);

                ActiveVerifications[ctx.User.Id] = new(hash, code, StartNumOfTries);

                await ctx.RespondAsync("Verification code sent!");
                return;
            }
        }

        await ctx.RespondAsync("Something went wrong!");
    }

    [Command("code")]
    [Description("Checks code which was sent to your email after verification.")]
    public async Task CodeCommand(CommandContext ctx, int code) {
        var userId = ctx.User.Id;
        if (ActiveVerifications.TryGetValue(userId, out var data)) {
            if (data.Code == code) {
                // TODO: give user the role 
                // add ukco to file - there is no checking - already happened in `verify`
                await _fileOfHashesHandler.TryAddHashAsync(data.Hash);
                // cleanup
                ActiveVerifications.Remove(userId);
                await ctx.RespondAsync("Verification process successful");
            } else {
                var numOfTriesNew = data.NumOfTries - 1;
                if (numOfTriesNew != 0) {
                    ActiveVerifications[userId] = data with { NumOfTries = numOfTriesNew, };
                } else {
                    ActiveVerifications.Remove(userId);
                }

                await ctx.RespondAsync($"Bad code! Remaining tries{numOfTriesNew}.");
            }
        } else {
            // warn user there is no code registered for this ID
            await ctx.RespondAsync(
                "There is no code registered for this user. You can start verification process by using `verify` command.");
        }
    }

    [Command("add_ukco")]
    [Description("Adds provided ukco to the file, without any checking or verification process.")]
    public async Task AddUkcoCommand(CommandContext ctx, int ukco) {
        var hash = _hashHandler.ComputeHashFromUkco(ukco.ToString());
        if (await _fileOfHashesHandler.TryAddHashAsync(hash)) {
            await ctx.RespondAsync("Added ukco to database!");
        }

        await ctx.RespondAsync("There already is ukco in database!");
    }
}