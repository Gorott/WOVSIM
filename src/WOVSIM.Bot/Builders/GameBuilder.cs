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
    
    
    public async Task<ResponseModel> CreateGame(ContextModel context, string name)
    {
        ResponseModel response = new ResponseModel()
        {
            ResponseType = ResponseType.Embed
        };

        GameSession? existingGame = await _gameService.GetGameSessionForGuildId(context.DiscordGuild.Id);
        if (existingGame != null)
        {
            return ResponseModel.TextError("A game already exists in this server. Please finish or cancel the existing game before creating a new one.");
        }
        
        GameSession? game = await _gameService.CreateGameAsync(name, context);
        if (game == null)
        {
            return ResponseModel.TextError("Sorry, there was an error creating the game.");
        }
        game = await _gameService.GetGameSessionForSessionId(game.GameSessionId);
        await BuildGameInformationEmbed(response.Embed, game);
        response.Components = BuildGameInformationComponents(game);
        response.GameSessionId = game.GameSessionId;
        
        return response;
    }

    private async Task BuildGameInformationEmbed(EmbedBuilder embed, GameSession game)
    {
        embed.WithColor(game.Status switch
        {
            GameStatus.WaitingForPlayers => DiscordConstants.WaitingForPlayersColor,
            GameStatus.InProgress => DiscordConstants.InProgressColor,
            _ => DiscordConstants.CompletedColor
        });

        
        embed.WithTitle($"New Game created: {game.Name}");
        var discordUsers = (await Task.WhenAll(
                game.Players.Select(async p => await _userService.GetUserFromDiscordAsync(p.User.DiscordUserId))
            ))
            .Where(u => u != null)
            .Cast<IUser>()
            .ToList();

        
        string playerList = string.Join("\n", discordUsers.Select(u => u.Username));
        
        embed.AddField($"Players {game.Players.Count}/16", playerList, true);

    }

    private static ComponentBuilder BuildGameInformationComponents(GameSession game)
    {
        bool joinButtonDisabled = game.Players.Count >= 16 || game.Status != GameStatus.WaitingForPlayers;
        return new ComponentBuilder()
            .WithButton("Join", $"{InteractionConstants.Game.JoinGame}-{game.GameSessionId}", disabled: joinButtonDisabled);
    }

    public async Task<ResponseModel> JoinGame(ContextModel context, int gameSessionId)
    {
        ResponseModel response = new ResponseModel
        {
            ResponseType = ResponseType.Embed
        };
        
        GameSession? game = await _gameService.GetGameSessionForSessionId(gameSessionId);
        
        
        if (game == null)
        {
            return ResponseModel.TextError("Sorry, this game does not exist.");
        }


        
                
        if (game.Status != GameStatus.WaitingForPlayers)
        {
            return ResponseModel.TextError("Sorry, this game has already started.");
        }
        
        if (game.Players.Count >= 16)
        {
            return ResponseModel.TextError("Sorry, this game is already full.");
        }

        var userId = context.UserContext.UserId;
        if (game.Players.Any(p => p.UserId == userId))
        {
            return ResponseModel.TextError("You have already joined this game.");
        }

        
        await _gameService.AddPlayerToGame(context, gameSessionId);
        game = await _gameService.GetGameSessionForSessionId(gameSessionId);
        
        if (game == null)
        {
            return ResponseModel.TextError("Sorry, there was an error joining the game.");
        }
            
        
        await BuildGameInformationEmbed(response.Embed, game);
        response.Components = BuildGameInformationComponents(game);

        return response;
    }
}