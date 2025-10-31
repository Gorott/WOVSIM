using Discord;
using Discord.Interactions;
using Serilog;
using WOVSIM.Bot.Builders;
using WOVSIM.Bot.Extensions;
using WOVSIM.CodeRunner;
using WOVSIM.CodeRunner.Interfaces;

namespace WOVSIM.Bot.SlashCommands;


public class SubmitCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICodeRunnerService _runner;
    private readonly ChallengeBuilder _challengeBuilder;

    public SubmitCommand(
        ICodeRunnerService runner,
        ChallengeBuilder challengeBuilder
        )
    {
        _runner = runner;
        _challengeBuilder = challengeBuilder;
    }

    [SlashCommand("submit", "Submit code for testing")]
    public async Task SubmitAsync(ProgrammingLanguage language, string? code = null)
    {
        await DeferAsync();

        try
        {
            
            
            var reply = await _challengeBuilder.SubmitCodeAsync(language, code);

            if (reply.Stream != null)
            {
                await FollowupWithFileAsync(
                    reply.Stream,
                    reply.FileName,
                    components: reply.ComponentsV2.Build(),
                    flags: MessageFlags.ComponentsV2);
            }
            else
            {
                await FollowupAsync(
                    components: reply.ComponentsV2.Build(),
                    flags: MessageFlags.ComponentsV2);
            }
        } catch (Exception ex)
        {
            Log.Error(ex, "Error while running code submission");
            await FollowupAsync("An error occurred while processing your submission. Please try again later.");
            return;
        }
        
    }
}
