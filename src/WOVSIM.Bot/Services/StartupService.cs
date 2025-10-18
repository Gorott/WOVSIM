using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using WOVSIM.Bot.Models;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class StartupService
{
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _client;
    private readonly IDbContextFactory<WovsimDbContext> _contextFactory;
    private readonly BotSettings _settings;
    private readonly InteractionService _interactionService;
    
    public StartupService(
        IServiceProvider provider,
        DiscordSocketClient client,
        IDbContextFactory<WovsimDbContext> contextFactory,
        IOptions<BotSettings> settings,
        InteractionService interactionService,
        InteractionService interactions
        
    ) {
        _provider = provider;
        _client = client;
        _contextFactory = contextFactory;
        _interactionService = interactionService;
        _interactions = interactions;
        _settings = settings.Value;
        
    }

    public async Task StartAsync()
    {
        await using WovsimDbContext context = await _contextFactory.CreateDbContextAsync();
        try
        {
            Log.Information("Ensuring database is up to date");
            await context.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Something went wrong while creating/updating the database!");
            throw;
        }


        Log.Information("Starting bot...");
        
        string discordToken = _settings.Discord.Token;
        if (string.IsNullOrWhiteSpace(discordToken))
        {
            throw new Exception("Please enter the bots token into the `/configs/config.json` file.");
        }
        
        Log.Information("Logging into Discord");
        await _client.LoginAsync(TokenType.Bot, discordToken);
        
        Log.Information("Setting Discord user status");
        await _client.SetStatusAsync(UserStatus.DoNotDisturb);
        
        Log.Information("Loading interaction modules");
        var modules = await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        Log.Information("Loaded {Count} modules", modules.Count());


        
        Log.Information("Starting Discord client");
        await _client.StartAsync();

        
        Log.Information("Waiting until client is connected");
        while (_client.ConnectionState != ConnectionState.Connected)
        {
            await Task.Delay(100);
        }
        Log.Information("Client is connected");

        await RegisterSlashCommands();
    }

    private async Task RegisterSlashCommands()
    {
        Log.Information("Starting slash command registration");
        
        Log.Information("Registering slash commands globally");
        await _interactionService.RegisterCommandsGloballyAsync();
    }
}