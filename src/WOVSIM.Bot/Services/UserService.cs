using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class UserService
{

    private readonly IDbContextFactory<WovsimDbContext> _contextFactory;
    private readonly DiscordSocketClient _client;

    public UserService(
        IDbContextFactory<WovsimDbContext> contextFactory,
        DiscordSocketClient client
    )
    {
        _contextFactory = contextFactory;
        _client = client;
    }

    public async Task<User> GetUserAsync(IUser discordUser)
    {
        return await GetUserAsync(discordUser.Id);
    }

    public async Task<User> GetUserAsync(ulong discordUserId)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        
        var user = await db.Users.FirstOrDefaultAsync(u => u.DiscordUserId == discordUserId);
        if (user != null)
        {
            return user;
        }
        
        var newUser = new User { DiscordUserId = discordUserId };

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return newUser;
    }


    public async Task<IUser> GetUserFromDiscordAsync(ulong discordUserId)
    {
        IUser user = await _client.GetUserAsync(discordUserId);
        
        if (user == null)
        {
            return await _client.Rest.GetUserAsync(discordUserId);
        }
        
        return user;
    }
    

    private async Task<User> AddOrUpdateUser(IUser discordUser)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();

        var existingUser = await db.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.DiscordUserId == discordUser.Id);

        if (existingUser != null) return existingUser;
        var newUser = new User { DiscordUserId = discordUser.Id };

        await db.Users.AddAsync(newUser);
        await db.SaveChangesAsync();

        return newUser;
    }


}