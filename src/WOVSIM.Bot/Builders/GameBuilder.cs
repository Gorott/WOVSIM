using Discord;
using Serilog;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Resources;
using WOVSIM.Bot.Services;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.Builders;

public class GameBuilder
{
    private readonly UserService _userService;
    private readonly GameService _gameService;

    public GameBuilder(
        UserService userService,
        GameService gameService
    )
    {
        _userService = userService;
        _gameService = gameService;
    }
    
    
    public async Task<ResponseModel> CreateGame(ContextModel context, int userId, string name)
    {
        ResponseModel response = new ResponseModel()
        {
            ResponseType = ResponseType.Embed
        };

        GameSession existingGame = await _gameService.GetGameSessionForGuildId(context.DiscordGuild.Id);
        if (existingGame != null)
        {
            response.Embed.WithDescription("Sorry, there is already a game in progress.");
            return response;
        }
        
        GameSession game = await _gameService.CreateGameAsync(name, context);
        await BuildGameInformationEmbed(response.Embed, game);
        response.Components = BuildGameInformationComponents(game);
        response.GameSessionId = game.GameSessionId;
        
        return response;
    }

    private async Task BuildGameInformationEmbed(EmbedBuilder embed, GameSession game)
    {
        embed.WithColor(DiscordConstants.WaitingForPlayersColor);
        
        embed.WithTitle($"New Game created: {game.Name}");
        ICollection<IUser> discordUsers = await Task.WhenAll(
            game.Players.Select(async p => await _userService.GetUserFromDiscordAsync(p.User.DiscordUserId))
        );
        
        string playerList = string.Join("\n", discordUsers.Select(u => u.Username));


        embed.AddField($"Players {game.Players.Count}/16", playerList, true);

    }

    private static ComponentBuilder BuildGameInformationComponents(GameSession game)
    {
        bool joinButtonDisabled = game.Players.Count >= 16 && game.Status != GameStatus.WaitingForPlayers;
        return new ComponentBuilder()
            .WithButton("Join", $"{InteractionConstants.Game.JoinGame}-{game.GameSessionId}", disabled: joinButtonDisabled);
    }

    public async Task<ResponseModel> JoinGame(ContextModel context, int gameSessionId)
    {
        ResponseModel response = new ResponseModel
        {
            ResponseType = ResponseType.Embed
        };
        
        GameSession game = await _gameService.GetGameSessionForSessionId(gameSessionId);
        
        if (game == null)
        {
            response.ResponseType = ResponseType.Text;
            response.CommandResponse = CommandResponse.Error;
            response.Text = "Sorry, this game does not exist.";
            return response;
        }

        var userId = context.UserContext.UserId;

        if (game.Players.Any(p => p.UserId == userId))
        {
            response.ResponseType = ResponseType.Text;
            response.CommandResponse = CommandResponse.Error;
            response.Text = "You are already in this game.";
            return response;
        }
        
        await _gameService.AddPlayerToGame(context, gameSessionId);
            
        
        await BuildGameInformationEmbed(response.Embed, game);
        response.Components = BuildGameInformationComponents(game);

        return response;
    }
}