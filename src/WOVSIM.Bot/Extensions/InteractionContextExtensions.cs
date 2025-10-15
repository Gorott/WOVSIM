using Discord;
using Fergun.Interactive;
using WOVSIM.Bot.Models;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Bot.Extensions;

public static class InteractionContextExtensions
{
    public static async Task<ulong?> SendFollowUpResponse(this IInteractionContext context,
        InteractiveService interactiveService, ResponseModel response, bool ephemeral = false)
    {
        ulong? responseId = null;

        
        switch (response.ResponseType)
        {
            case ResponseType.Text:
                 var text = await context.Interaction.FollowupAsync(response.Text, allowedMentions: AllowedMentions.None,
                    ephemeral: ephemeral, components: response.Components?.Build());
                 responseId = text.Id;
                break;
            case ResponseType.Embed:
                var embed = await context.Interaction.FollowupAsync(null, [response.Embed.Build()], ephemeral: ephemeral, components: response.Components?.Build());
                responseId = embed.Id;
                break;
            case ResponseType.ComponetsV2:
                var components = await context.Interaction.FollowupAsync(null, components: response.ComponentsV2?.Build(),
                    allowedMentions: AllowedMentions.None);
                responseId = components.Id;
                break;
            case ResponseType.ComponentsPaginator:
                _ = await interactiveService.SendPaginatorAsync(
                    response.ComponentPaginator.Build(),
                    context.Interaction,
                    TimeSpan.FromMinutes(5),
                    ephemeral: ephemeral);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return responseId;
    }
}