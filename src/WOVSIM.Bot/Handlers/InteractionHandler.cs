using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using WOVSIM.Bot.Services;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.Handlers;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _provider;
    private readonly UserService _userService;
    
    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactionService,
        IServiceProvider provider,
        UserService userService
    )
    {
        _client = client;
        _interactionService = interactionService;
        _provider = provider;
        _userService = userService;
        _client.SlashCommandExecuted += SlashCommandExecuted;
    }

    private async Task SlashCommandExecuted(SocketInteraction socketInteraction)
    {
        if (socketInteraction is not SocketSlashCommand socketSlashCommand)
        {
            return;
        }

        _ = Task.Run(() => ExecuteSlashCommand(socketInteraction, socketSlashCommand));
    }
    
    private async Task ExecuteSlashCommand(SocketInteraction socketInteraction, SocketSlashCommand socketSlashCommand)
    {
        SocketInteractionContext context = new SocketInteractionContext(_client, socketInteraction);
        User user = await _userService.GetUserAsync(socketInteraction.User.Id);

        SearchResult<SlashCommandInfo> commandSearch = _interactionService.SearchSlashCommand(socketSlashCommand);

        if (!commandSearch.IsSuccess)
        {
            Log.Error("Someone tried to execute a non-existent slash command! {slashCommand}", socketSlashCommand.CommandName);
            return;
        }

        ICommandInfo command = commandSearch.Command;
        
        if (!await CommandEnabled(context, command))
        {
            return;
        }

        await _interactionService.ExecuteCommandAsync(context, _provider);
    }

    private static async Task<bool> CommandEnabled(SocketInteractionContext context, ICommandInfo command)
    {
        return true;
    }
}