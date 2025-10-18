using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WOVSIM.Bot.Models;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class GameService : EntityService<GameSession, int>
{
    private readonly UserService _userService;

    public GameService(
        IDbContextFactory<WovsimDbContext> contextFactory,
        UserService userService
    ) : base(contextFactory)
    {
        _userService = userService;
    }

    public async Task<GameSession?> CreateGameAsync(string name, ContextModel context, CancellationToken ct = default)
    {
        if (await GetGameSessionForGuildId(context.DiscordGuild.Id) is not null)
        {
            return null;
        }

        await using var db = await _contextFactory.CreateDbContextAsync(ct);
        var user = await _userService.GetUserAsync(context.UserContext.DiscordUserId);

        var session = new GameSession
        {
            StarterUserId = user.UserId,
            DiscordGuildId = context.DiscordGuild.Id,
            Name = name,
            Players = new List<Player> { new Player { UserId = user.UserId } }
        };

        db.GameSessions.Add(session);
        await db.SaveChangesAsync(ct);
        return session;
    }


    public async Task<GameSession?> GetGameSessionForSessionId(int gameSessionId)
    {
        return await GetAsync(g => g.GameSessionId == gameSessionId, includes: q =>
            q.Include(g => g.Players)
                .ThenInclude(p => p.User)
        );
    }

    public async Task<GameSession?> GetGameSessionForGuildId(ulong guildId)
    {
        return await GetAsync(g => g.DiscordGuildId == guildId, includes: q =>
            q.Include(g => g.Players)
                .ThenInclude(p => p.User)
        );
    }

    public async Task AddResponseId(int gameSessionId, ulong responseId)
    {
        bool success = await UpdateAsync(
            g => g.GameSessionId == gameSessionId,
            g => g.GameInformationResponseId = responseId
        );
        

        if (!success)
        {
            throw new InvalidOperationException("Game session not found.");
        }
        
    }

    public async Task AddPlayerToGame(ContextModel context, int gameSessionId, CancellationToken ct = default)
    {
        User user = await _userService.GetUserAsync(context.UserContext.DiscordUserId, ct);
        bool success = await UpdateAsync(
            g => g.GameSessionId == gameSessionId,
            g =>
            {
                g.Players ??= new List<Player>();

                if (g.Players.Any(p => p.UserId == user.UserId))
                {
                    return;
                }

                g.Players.Add(new Player { UserId = user.UserId, User = user });
            },
            ct,
            g => g.Players
        );

        if (!success)
        {
            throw new InvalidOperationException("Game session not found.");
        }
    }
}