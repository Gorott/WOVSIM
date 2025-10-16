using Discord;
using Fergun.Interactive.Pagination;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Bot.Models;

public class ResponseModel
{
    public ResponseModel()
    {
        Embed = new EmbedBuilder();
        EmbedAuthor = new EmbedAuthorBuilder();
        EmbedFooter = new EmbedFooterBuilder();
        ComponentsV2 = new ComponentBuilderV2();
        ComponentsContainer = new ContainerBuilder();

        ComponentsV2.WithContainer(ComponentsContainer);
    }
    
    
    public EmbedAuthorBuilder EmbedAuthor { get; set; }
    public EmbedBuilder Embed { get; set; }
    public EmbedFooterBuilder EmbedFooter { get; set; }
    public ComponentBuilder Components { get; set; }
    
    public string Text { get; set; }
    public int? GameSessionId { get; set; }
    
    
    public ComponentBuilderV2 ComponentsV2 { get; set; }
    public ContainerBuilder ComponentsContainer { get; set; }
    
    
    public ResponseType ResponseType { get; set; }
    
    public ComponentPaginatorBuilder ComponentPaginator { get; set; }
    
    public CommandResponse CommandResponse { get; set; }
    
}