using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using ETH_Bot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ETH_Bot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        
        public async Task MainAsync(string[] args)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });
            _client.Log += Log;
            
            //configure Config service
            ConfigService.ConstructLazy();
            //create serviceprovider
            var serviceProvider = ConfigureServices();
            
            //setup command handler
            await serviceProvider.GetRequiredService<CommandHandler>().InitializeAsync(serviceProvider);
            serviceProvider.GetRequiredService<ReminderService>().Initialize();
            serviceProvider.GetRequiredService<SubscribeService>().Initialize();

            //get token
            string token = ConfigService.LazyGet("token", true);
            //Connect to Discord
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            //Hang indefinitely
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(_client);
            services.AddScoped<CommandService>();
            services.AddScoped<DownloadService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<InteractiveService>();
            services.AddSingleton<SubscribeService>();
            services.AddSingleton<ReminderService>();
            // Disabled to fix MemLeak issues
            //services.AddDbContext<EthContext>(o =>
                //o.UseMySql(ConfigService.LazyGet("connectionString", true)), ServiceLifetime.Transient);

            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }

        private Task Log(LogMessage m)
        {
            switch (m.Severity)
            {
                case LogSeverity.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogSeverity.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogSeverity.Critical: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case LogSeverity.Verbose: Console.ForegroundColor = ConsoleColor.White; break;
            }
            
            Console.WriteLine(m.ToString());
            if(m.Exception != null)
                Console.WriteLine(m.Exception);
            Console.ForegroundColor = ConsoleColor.Gray;

            return Task.CompletedTask;
        }
    }
}