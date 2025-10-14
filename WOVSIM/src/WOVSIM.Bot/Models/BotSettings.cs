namespace WOVSIM.Bot.Models;

public class BotSettings
{
    public DiscordConfig Discord { get; set; }
}

public class DiscordConfig
{
    public string Token { get; set; }
}