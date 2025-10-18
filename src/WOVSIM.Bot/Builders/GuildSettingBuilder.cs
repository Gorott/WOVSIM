using System.Text;
using Discord;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Services;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.Builders;

public class GuildSettingBuilder
{
    private readonly GuildService _guildService;
    public GuildSettingBuilder(GuildService guildService)
    {
        _guildService = guildService;
    }
    public async Task<ResponseModel> GetGuildSettings(ContextModel context, GuildPermissions guildPermissions)
    {
        var response = new ResponseModel { ResponseType = ResponseType.Embed };

        var guild = await _guildService.GetGuildAsync(context.DiscordGuild.Id);
        
        if (guild == null)
        {
            return ResponseModel.TextError("This server is not configured for Wolvesville Simulation. Please contact a server administrator to set up the bot.");
        }

        response.Embed.WithTitle($"Wolvesville Simulation configuration for {context.DiscordGuild.Name}");
        response.Embed.WithFooter($"{guild.DiscordGuildId}\nUse '/settings' to modify personal settings.");

        var sb = new StringBuilder();
        sb.AppendLine($"**Is Game Server:** {(guild.MainGuildId != null ? "Yes" : "No")}");

        if (guild.MainGuildId != null)
        {
            sb.AppendLine($"**Main Server:** {guild.MainGuild?.Name ?? "(unknown)"} ({guild.MainGuild?.DiscordGuildId})");
        }
        else
        {
            var game = guild.GameGuild;
            sb.AppendLine($"**Game Server Linked:** {(game != null ? $"{game.Name} ({game.DiscordGuildId})" : "None")}");
        }

        response.Embed.AddField("Server Linking", sb.ToString(), inline: false);
        return response;
    }

    
}