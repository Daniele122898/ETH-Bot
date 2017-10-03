using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], $"Pong! {Context.Client.Latency} ms :ping_pong:"));
        }
    }
}