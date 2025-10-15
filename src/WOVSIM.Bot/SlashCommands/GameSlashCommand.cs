using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using WOVSIM.Bot.Builders;
using WOVSIM.Bot.Extensions;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Services;
using WOVSIM.Domain.Enums;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.SlashCommands;

[Group("game", "Game commands")]
public class GameSlashCommand : InteractionModuleBase
{
    private readonly GameBuilder _gameBuilder;
    private readonly UserService _userService;
    private readonly GameService _gameService;

    private InteractiveService Interactivity { get; }
    public GameSlashCommand(
        GameBuilder gameBuilder,
        UserService userService,
        GameService gameService
        )
    {
        _gameBuilder = gameBuilder;
        _userService = userService;
        _gameService = gameService;
    }

    [SlashCommand("create", "Create a new Wolvesville game lobby")]
    [CommandContextType(InteractionContextType.Guild)]
    public async Task CreateGameAsync([Summary("Name", "The name of your game")] string name)
    {
        await DeferAsync();
        User user = await _userService.GetUserAsync(Context.User);
        ContextModel context = new ContextModel(Context, user);
        try
        {


            ResponseModel response = await _gameBuilder.CreateGame(context, user.UserId, name);

            var responseId = await Context.SendFollowUpResponse(Interactivity, response);

            if (responseId.HasValue && response.GameSessionId.HasValue)
            {
                await _gameService.AddResponseId(response.GameSessionId.Value, responseId.Value);
            }
        }
        catch (Exception e)
        {
            ResponseModel response = new ResponseModel
            {
                ResponseType = ResponseType.Text,
            };

            response.Text = $"Something went wrong! Please try again later.\n```{e.StackTrace}```";
            await Context.SendFollowUpResponse(Interactivity, response);
        }
    }
}