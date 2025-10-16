using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using WOVSIM.Bot.Builders;
using WOVSIM.Bot.Configurations;
using WOVSIM.Bot.Handlers;
using WOVSIM.Bot.Models;
using WOVSIM.Bot.Services;
using WOVSIM.Persistence.EntityFramework;

namespace WOVSIM.Bot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
            BotSettings config = ConfigData.Data;

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "configs"))
                .AddJsonFile("config.json", true, true)
                .AddEnvironmentVariables();
            
            Configuration = configBuilder.Build();
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
            
            ServiceProvider provider = services.BuildServiceProvider();
            InitializeRequiredServices(provider);
            
            await provider.GetRequiredService<StartupService>().StartAsync();
            
            await Task.Delay(-1);
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

        private static void InitializeRequiredServices(ServiceProvider provider)
        {
            provider.GetRequiredService<InteractionHandler>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BotSettings>(Configuration);
            
            DiscordSocketClient client = ConfigureDiscordClient();
            
            RegisterCoreServices(services, client);
            RegisterHandlerServices(services);
            RegisterBuilderServices(services);
            RegisterFeatureServices(services);

            services.AddDbContextFactory<WovsimDbContext>(b =>
            {
                b.UseNpgsql(Configuration["Database:ConnectionString"]);
            });
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
                .AddSingleton(new InteractionService(client, new InteractionServiceConfig
                {
                    LogLevel = LogSeverity.Info,
                    DefaultRunMode = RunMode.Async,
                    ThrowOnError = false
                }))
                .AddSingleton<StartupService>()
                .AddSingleton(Configuration);
        }

        private static void RegisterHandlerServices(IServiceCollection services)
        {
            services
                .AddSingleton<InteractionHandler>();
        }

        private static void RegisterBuilderServices(IServiceCollection services)
        {
            services
                .AddSingleton<GameBuilder>();
        }

        private static void RegisterFeatureServices(IServiceCollection services)
        {
            services
                .AddSingleton<GameService>()
                .AddSingleton<UserService>();

            services.AddSingleton(new InteractiveConfig
                {
                    ReturnAfterSendingPaginator = true,
                    ProcessSinglePagePaginators = true
                })
                .AddSingleton<InteractiveService>();
        }
    }
}
