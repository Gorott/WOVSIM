using Discord.Interactions;

namespace WOVSIM.Bot.SlashCommands;

public class PingSlashCommand : InteractionModuleBase
{
    [SlashCommand("ping", "Pong!")]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!");
    }
}