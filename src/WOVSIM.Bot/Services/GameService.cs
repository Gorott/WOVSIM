using System.Collections;
using Microsoft.EntityFrameworkCore;
using WOVSIM.Bot.Models;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class GameService
{
    private readonly IDbContextFactory<WovsimDbContext> _contextFactory;
    private readonly UserService _userService;
    public GameService(
        IDbContextFactory<WovsimDbContext> contextFactory,
        UserService userService
        )
    {
        _contextFactory = contextFactory;
        _userService = userService;
    }

    public async Task<GameSession> CreateGameAsync(string name, ContextModel context)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();

        var user = await _userService.GetUserAsync(context.UserContext.DiscordUserId);
        
        db.Attach(user);

        var creator = new Player
        {
            UserId = user.UserId,
            User = user
        };

        var session = new GameSession
        {
            StarterUserId = user.UserId,
            DiscordGuildId = context.DiscordGuild.Id,
            Name = name,
            Players = new List<Player> { creator }
        };

        db.GameSessions.Add(session);

        await db.SaveChangesAsync();



        return session;
    }

    public async Task<GameSession> GetGameSessionForSessionId(int gameSessionId)
    {
        await using WovsimDbContext db = await _contextFactory.CreateDbContextAsync();
        return await db.GameSessions
            .FirstOrDefaultAsync(g =>
                g.GameSessionId == gameSessionId &&
                (g.Status == GameStatus.InProgress || g.Status == GameStatus.WaitingForPlayers));
    }

    public async Task<GameSession> GetGameSessionForGuildId(ulong guildId)
    {
        await using WovsimDbContext db = await _contextFactory.CreateDbContextAsync();
        return db.GameSessions
            .FirstOrDefault(g => g.DiscordGuildId == guildId && (g.Status == GameStatus.InProgress || g.Status == GameStatus.WaitingForPlayers));
    }

    public async Task AddResponseId(int gameSessionId, ulong responseId)
    {
        await using WovsimDbContext db = await _contextFactory.CreateDbContextAsync();
        
        GameSession game = await db.GameSessions.FindAsync(gameSessionId);
        
        game.GameInformationResponseId = responseId;
        
        db.Update(game);
        await db.SaveChangesAsync();
    }
}