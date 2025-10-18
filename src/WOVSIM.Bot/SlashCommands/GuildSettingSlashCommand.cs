using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using WOVSIM.Bot.Builders;
using WOVSIM.Bot.Services;

namespace WOVSIM.Bot.SlashCommands;

public class GuildSettingSlashCommand
{
    private readonly GuildService _guildService;
    private readonly GuildSettingBuilder _guildSettingBuilder;
    
    private InteractiveService Interactivity { get; }
    
    public GuildSettingSlashCommand(
        GuildService guildService,
        GuildSettingBuilder guildSettingBuilder,
        InteractiveService interactivity
    )
    {
        _guildService = guildService;
        _guildSettingBuilder = guildSettingBuilder;
        Interactivity = interactivity;
    }
    [SlashCommands("configure", "Set up server settings for the bot")]
    [CommandContextType(InteractionContextType.Guild)]
    public async Task ConfigureServerAsync()
    {
        await DeferAsync(ephemeral: true);
        // Implementation for configuring server settings goes here
    }
}