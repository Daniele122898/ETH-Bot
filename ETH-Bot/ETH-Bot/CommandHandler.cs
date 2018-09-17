using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ETH_Bot.Services;

namespace ETH_Bot
{
    public class CommandHandler
    {

        private IServiceProvider _serviceProvider;
        private DiscordSocketClient _client;
        private readonly CommandService _commandService;
        
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService commandService)
        {
            _serviceProvider = provider;
            _client = client;
            _commandService = commandService;
            
            _client.MessageReceived += HandleCommandsAsync;
            _commandService.Log += HandleErrorAsync;
        }
        
        public async Task InitializeAsync(IServiceProvider provider)
        {
            _serviceProvider = provider;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommandsAsync(SocketMessage m)
        {
            try
            {
                var message = m as SocketUserMessage;
                if (message == null) return;
                if (message.Author.IsBot) return;
                //if (!(message.Channel is SocketGuildChannel)) return;
                
                //prefix ends and command starts
                string prefix = ">";
                
                int argPos = prefix.Length-1;
                if(!(message.HasStringPrefix(prefix, ref argPos)|| message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                    return;
                
                //create Context
                var context = new SocketCommandContext(_client,message);

                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
                if (!result.IsSuccess)
                {
                    await HandleErrorAsync(result, context);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private async Task HandleErrorAsync(IResult result, SocketCommandContext context, CommandException exception = null)
        {
            switch (result.Error)
            {
                case CommandError.Exception:
                    if (exception != null)
                    {
                        await SentryService.SendMessage(
                            $"**Exception**\n{exception.InnerException.Message}\n```\n{exception.InnerException}```");
                    }
                    break;
                case CommandError.BadArgCount:
                    await context.Channel.SendMessageAsync("", embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2], result.ErrorReason).Build());
                    break;
                case CommandError.UnknownCommand:
                    break;
                case CommandError.ParseFailed:
                    await context.Channel.SendMessageAsync($"", embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2], $"Couldn't parse entered value! Make sure you enter the requested data type").WithDescription("If a whole number is asked then please provide one etc.").Build());
                    break;
                default:
                    await context.Channel.SendMessageAsync($"", embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2], $"{result.ErrorReason}").Build());
                    break;
            }
        }
        
        private async Task HandleErrorAsync(LogMessage logMessage)
        {
            var commandException = logMessage.Exception as CommandException;
            if(commandException == null) return;
            await HandleErrorAsync(ExecuteResult.FromError(commandException),
                (SocketCommandContext) commandException.Context, commandException);
        }
    }
}