using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using WOVSIM.Bot.Models;

namespace WOVSIM.Bot.Services;

public class StartupService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _client;
    private readonly BotSettings _settings;
    
    public StartupService(
        IServiceProvider provider,
        DiscordSocketClient client,
        IOptions<BotSettings> settings
    ) {
        _provider = provider;
        _client = client;
        _settings = settings.Value;
    }

    public async Task StartAsync()
    {
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
        
        Log.Information("Starting Discord client");
        await _client.StartAsync();
    }
}