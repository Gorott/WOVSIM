namespace WOVSIM.Bot.Models;

public class BotSettings
{
    public DatabaseConfig Database { get; set; }
    public DiscordConfig Discord { get; set; }
}

public class DiscordConfig
{
    public string Token { get; set; }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; }
}