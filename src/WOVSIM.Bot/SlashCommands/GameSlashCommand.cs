using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using Serilog;
using WOVSIM.Bot.Builders;
using WOVSIM.Bot.Extensions;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Resources;
using WOVSIM.Bot.Services;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.SlashCommands;

[Group("game", "Game commands")]
public class GameSlashCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GameBuilder _gameBuilder;
    private readonly UserService _userService;
    private readonly GameService _gameService;

    private InteractiveService Interactivity { get; }

    public GameSlashCommand(
        GameBuilder gameBuilder,
        UserService userService,
        InteractiveService interactivity,
        GameService gameService
    )
    {
        _gameBuilder = gameBuilder;
        _userService = userService;
        Interactivity = interactivity;
        _gameService = gameService;
    }
    
    [ComponentInteraction($"{InteractionConstants.Game.JoinGame}-*", true)]
    public async Task JoinGame(string gameSessionIdString)
    {
        int gameSessionId = int.Parse(gameSessionIdString);
        ResponseModel response = await _gameBuilder.JoinGame(new ContextModel(Context, await _userService.GetUserAsync(Context.User)), gameSessionId);
        if (response.CommandResponse == CommandResponse.Error) {
            await Context.Interaction.RespondAsync(response.Text, ephemeral: true);
            return;
        } 
        
        await Context.UpdateInteractionEmbed(response);
        
    }

    [SlashCommand("create", "Create a new Wolvesville game lobby")]
    [CommandContextType(InteractionContextType.Guild)]
    public async Task CreateGameAsync([Summary("Name", "The name of your game")] string name)
    {
        await DeferAsync();
        User user = await _userService.GetUserAsync(Context.User);
        ContextModel context = new ContextModel(Context, user);
        ResponseModel response = await _gameBuilder.CreateGame(context, user.UserId, name);

        var responseId = await Context.SendFollowUpResponse(Interactivity, response);

        if (responseId.HasValue && response.GameSessionId.HasValue)
        {
            await _gameService.AddResponseId(response.GameSessionId.Value, responseId.Value);
        }
    }
}