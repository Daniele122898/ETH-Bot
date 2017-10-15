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

        [Command("github"), Alias("git")]
        public async Task GitHub()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], $"Find me here").WithUrl("https://github.com/Daniele122898/ETH-Bot"));
        }

        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], "Simple Help").AddField(
                x =>
                {
                    x.IsInline = false;
                    x.Name = "Scraper";
                    x.Value = "`>linalg`\n" +
                              "`>discmath`\n" +
                              "`>algdat`\n" +
                              "`>eprog`";
                }).AddField(x =>
            {
                x.IsInline = false;
                x.Name = "Other";
                x.Value = "`>ping`\n" +
                          "`>github`";
            }).WithThumbnailUrl(Utility.EthLogo));
        }
    }
}