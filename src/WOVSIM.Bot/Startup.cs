using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Services;

namespace WOVSIM.Bot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly DiscordSocketClient _client;

        public Startup(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "configs"))
                .AddJsonFile("config.json", true, true)
                .AddEnvironmentVariables();
            
            Configuration = configBuilder.Build();

            // Configure Discord client
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged |
                                 GatewayIntents.MessageContent
            });

            _client.Log += LogAsync;
        }

        public static async Task RunAsync(string[] args)
        {
            Startup startup = new Startup(args);
            await startup.RunAsync();
        }

        private async Task RunAsync()
        {
            ConfigureLogging();
            
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            IServiceProvider provider = services.BuildServiceProvider();
            
            await provider.GetRequiredService<StartupService>().StartAsync();
            
            await Task.Delay(-1); // Keep alive
        }

        private void ConfigureLogging()
        {
            LogEventLevel consoleLevel = LogEventLevel.Warning;
            LogEventLevel logLevel = LogEventLevel.Information;
#if DEBUG
            consoleLevel = LogEventLevel.Verbose;
            logLevel = LogEventLevel.Information;
#endif

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .Enrich.WithExceptionDetails()
                .Enrich.WithEnvironmentVariable("INSTANCE_NAME")
                .Enrich.WithMachineName()
                .WriteTo.Console(consoleLevel)
                .CreateLogger();

            Log.Information("WOVSIM Bot starting up..");

        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BotSettings>(Configuration);
            
            DiscordSocketClient client = ConfigureDiscordClient();
            
            RegisterCoreServices(services, client);
        }

        private static DiscordSocketClient ConfigureDiscordClient()
        {
            DiscordSocketConfig config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.MessageContent |
                                 GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.Guilds |
                                 GatewayIntents.GuildVoiceStates,
                ConnectionTimeout = 60000
            };
            
            return new DiscordSocketClient(config);
        }

        private void RegisterCoreServices(IServiceCollection services, DiscordSocketClient client)
        {
            services.AddSingleton(client)
                .AddSingleton<StartupService>();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine($"[{log.Severity}] {log.Source}: {log.Message}");
            if (log.Exception != null)
                Console.WriteLine(log.Exception);
            return Task.CompletedTask;
        }
    }
}
