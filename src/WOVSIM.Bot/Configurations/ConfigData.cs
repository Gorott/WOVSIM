using Newtonsoft.Json;
using WOVSIM.Bot.Models;

namespace WOVSIM.Bot.Configurations;

public static class ConfigData
{
    private const string ConfigFolder = "configs";
    private const string ConfigFile = "config.json";

    public static BotSettings Data { get; }
    static ConfigData()
    {
        if (!Directory.Exists(ConfigFolder))
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        if (!File.Exists(Path.Combine(ConfigFolder, ConfigFile)))
        {
            Data = new BotSettings
            {
                Discord = new DiscordConfig
                {
                    Token = "YOUR_DISCORD_BOT_TOKEN"
                }
            };

            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
            
            Console.WriteLine("Created new bot configuration file with default values. \n" +
                              $"Please set your API keys in {ConfigFolder}/{ConfigFile} before running the bot again. \n \n" +
                              "Exiting in 10 seconds...",
                ConsoleColor.Red);

            Thread.Sleep(10000);
            Environment.Exit(0);;
        }
        else
        {
            string json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
            Data = JsonConvert.DeserializeObject<BotSettings>(json);
        }
    }
}