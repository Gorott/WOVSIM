using System.Security.Cryptography;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Fergun.Interactive;
using Serilog;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Resources;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Bot.Extensions;

public static class InteractionContextExtensions
{

    public static async Task HandleCommandException(this IInteractionContext context, Exception exception,
        string message = null, bool sendReply = true, bool deferFirst = false)
    {
        string referenceId = GenerateRandomCode();

        var commandName = context.Interaction switch
        {
            SocketSlashCommand socketSlashCommand => socketSlashCommand.CommandName,
            SocketUserCommand socketUserCommand => socketUserCommand.CommandName,
            SocketInteraction socketInteraction => "ButtonInteraction",
            _ => null
        };
        
        Log.Error(exception, "SlashCommandUsed: Error {referenceId} | {discordUserName} / {discordUserId} | {guildName} / {guildId} | {commandResponse} ({message}) | {messageContent}",
            referenceId, context.User?.Username, context.User?.Id, context.Guild?.Name, context.Guild?.Id, CommandResponse.Error, message, commandName);

        if (sendReply)
        {
            if (deferFirst)
            {
                await context.Interaction.DeferAsync(ephemeral: true);
            }

            if (exception?.Message != null && exception.Message.Contains("50013: Missing Permissions", StringComparison.OrdinalIgnoreCase))
            {
                await context.Interaction.FollowupAsync("Error while replying: The bot is missing permissions.\n" +
                                                        "Make sure it has permission to 'Embed Links' and 'Attach Images'.",
                    ephemeral: true);
            } else
            {
                await context.Interaction.FollowupAsync(
                    $"Sorry, something went wrong while trying to process `{commandName}`. Please try again later.\n" +
                    $"*Reference id: `{referenceId}`*", ephemeral: true);
            }
        }
    }
    
        public static async Task SendResponse(this IInteractionContext context, InteractiveService interactiveService,
        ResponseModel response, bool ephemeral = false, ResponseModel extraResponse = null)
    {
        var embeds = new[] { response.Embed.Build() };
        if (extraResponse != null)
        {
            embeds = [response.Embed.Build(), extraResponse.Embed?.Build()];
        }

        switch (response.ResponseType)
        {
            case ResponseType.Text:
                await context.Interaction.RespondAsync(response.Text, allowedMentions: AllowedMentions.None,
                    ephemeral: ephemeral, components: response.Components?.Build());
                break;
            case ResponseType.Embed:
                await context.Interaction.RespondAsync(null, embeds,
                    ephemeral: ephemeral, components: response.Components?.Build());
                break;
            case ResponseType.ComponentsV2:
                await context.Interaction.RespondAsync(ephemeral: ephemeral,
                    components: response.ComponentsV2?.Build(),
                    allowedMentions: AllowedMentions.None);
                break;
            case ResponseType.Paginator:
                _ = interactiveService.SendPaginatorAsync(
                    response.ComponentPaginator.Build(),
                    (SocketInteraction)context.Interaction,
                    TimeSpan.FromMinutes(5),
                    ephemeral: ephemeral);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
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
            case ResponseType.ComponentsV2:
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
    
            
    public static async Task UpdateInteractionEmbed(this IInteractionContext context, ResponseModel response, InteractiveService interactiveService = null, bool defer = true)
    {
        var message = (context.Interaction as SocketMessageComponent)?.Message;

        if (message == null)
        {
            return;
        }

        await context.ModifyMessage(message, response, defer);
    }

    public static async Task ModifyMessage(this IInteractionContext context, IUserMessage message,
        ResponseModel response, bool defer = true, bool interactionEdit = false)
    {
        await message.ModifyAsync(m =>
        {
            m.Components = response.ResponseType == ResponseType.ComponentsV2
                ? response.ComponentsV2?.Build()
                : response.Components?.Build();
            m.Embed = response.ResponseType == ResponseType.ComponentsV2 ? null : response.Embed?.Build();
            m.AllowedMentions = AllowedMentions.None;
        });

        if (defer)
        {
            await context.Interaction.DeferAsync();
        }
    }

    public static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int size = 8;
        
        var data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }

        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = Math.Abs(BitConverter.ToInt32(data, i * 4));
            var index = rnd % chars.Length;
            
            result.Append(chars[index]);
        }
        
        return result.ToString();
    }
}