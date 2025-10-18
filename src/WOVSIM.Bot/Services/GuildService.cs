using Discord;
using Microsoft.EntityFrameworkCore;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class GuildService : EntityService<Guild, ulong>
{
    public GuildService(IDbContextFactory<WovsimDbContext> dbContextFactory) : base(dbContextFactory)
    {
        
    }

    public async Task<Guild?> GetGuildAsync(ulong discordGuildId)
    {
        return await GetAsync(u => u.DiscordGuildId == discordGuildId, includes: q =>
            q.Include(g => g.GameGuild)
                .Include(g => g.MainGuild));
    }

    public async Task SwitchGuildModeAsync(ulong discordGuildId)
    {
        bool success = await UpdateAsync(
            g => g.DiscordGuildId == discordGuildId,
            g => g.IsGameGuild = !g.IsGameGuild
        );
        
        if (!success)
        {
            throw new InvalidOperationException("Guild not found");
        }
    }
}