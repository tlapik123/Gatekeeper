// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Mail;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using gatekeeper.GkConfig.Parsing;
using gatekeeper.GkLogging;
using gatekeeper.GkConvertors;
using gatekeeper.GkReactionPerms.Database;
using gatekeeper.GkReactionPerms.EventHandler;
using gatekeeper.GkReactionPerms.Module;
using gatekeeper.GkVerification.Handlers;
using gatekeeper.GkVerification.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("Starting the bot!");

// create parser with settings file
IGkConfigParser parser = new GkConfigFileParser(args[0]);

var config = await parser.ParseConfigAsync();

// initialize the discord client
var discord = new DiscordClient(
    new DiscordConfiguration {
        Token = config.Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.AllUnprivileged,
        MinimumLogLevel = LogLevel.Debug,
    }
);

// use interactivity - used mainly for creating reaction perms
discord.UseInteractivity(new InteractivityConfiguration {
    ResponseBehavior = InteractionResponseBehavior.Respond,
    ResponseMessage = "Bad interaction.",
});


// services for various command modules
var services = new ServiceCollection()
    // verification module
    .AddSingleton<Random>()
    .AddSingleton<HttpClient>()
    .AddSingleton(_ => new SmtpClient(config.EmailServer) {
        Credentials = new NetworkCredential(config.EmailUsername, config.EmailPassword),
    })
    .AddSingleton<IGkSisHandler, GkSisHandler>()
    .AddSingleton<IGkMailHandler>(sp => new GkMailHandler(sp.GetRequiredService<SmtpClient>(), config.EmailUsername))
    .AddSingleton<IGkHashHandler>(_ => new GkHashHandler(config.HashAlg, config.Salt))
    .AddSingleton<IGkFileOfHashesHandler>(_ => new GkFileOfHashesHandler(config.SaveFile))
    // role module
    .AddSingleton<IArgumentConverter<DiscordMessage>, DiscordMessageConverter>()
    .AddSingleton<IArgumentConverter<DiscordEmoji>, DiscordEmojiConverter>()
    .AddSingleton<IArgumentConverter<DiscordChannel>, DiscordChannelConverter>()
    .AddSingleton<IArgumentConverter<bool>, GkBoolConvertor>()
    .AddSingleton<IGkReactionPermsDatabase, GkReactionPermsDatabase>()
    .BuildServiceProvider();

// use commands next
var commands = discord.UseCommandsNext(
    new CommandsNextConfiguration {
        StringPrefixes = new[] { "~", },
        Services = services,
    }
);

// register custom parser
commands.RegisterConverter(services.GetRequiredService<IArgumentConverter<bool>>());
commands.RegisterConverter(ActivatorUtilities.GetServiceOrCreateInstance<GkEmojiChannelPairConvertor>(services));

// register command modules
commands.RegisterCommands<GkVerificationModule>();
commands.RegisterCommands<GkReactionPermsModule>();

var roleHandler = ActivatorUtilities.GetServiceOrCreateInstance<GkReactionHandler>(services);

// register reaction handling
discord.MessageReactionAdded += roleHandler.MessageReactionAdded;
discord.MessageReactionRemoved += roleHandler.MessageReactionRemoved;

// add more command logging
commands.CommandErrored += GkCommandLogging.CommandErrored;
commands.CommandExecuted += GkCommandLogging.CommandExecuted;

// connect client
await discord.ConnectAsync();

// sleep main forever
await Task.Delay(-1);