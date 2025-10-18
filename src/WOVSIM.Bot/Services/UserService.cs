using System.Linq.Expressions;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WOVSIM.Persistence.Domain.Models;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot.Services;

public class UserService : EntityService<User, ulong>
{
    
    private readonly DiscordSocketClient _client;

    public UserService(
        IDbContextFactory<WovsimDbContext> contextFactory,
        DiscordSocketClient client
    ) : base(contextFactory)
    {
        _client = client;
    }

    public async Task<User> GetUserAsync(IUser discordUser)
    {
        return await GetUserAsync(discordUser.Id);
    }

    public async Task<User> GetUserAsync(ulong discordUserId, CancellationToken ct = default)
    {

        return await GetOrCreateAsync(u => u.DiscordUserId == discordUserId, () => new User { DiscordUserId = discordUserId }, ct);
    }


    public async Task<IUser?> GetUserFromDiscordAsync(ulong discordUserId, CancellationToken ct = default)
    {
        IUser user = _client.GetUser(discordUserId);
        if (user != null)
        {
            return user;
        }

        try
        {
            var options = new RequestOptions { CancelToken = ct };
            return await _client.Rest.GetUserAsync(discordUserId, options).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }


}