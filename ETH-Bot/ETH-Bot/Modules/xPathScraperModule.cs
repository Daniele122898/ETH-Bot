using System.Threading.Tasks;
using Discord.Commands;
using ETH_Bot.Services;
using HtmlAgilityPack;

namespace ETH_Bot.Modules
{
    public class xPathScraperModule : ModuleBase<SocketCommandContext>
    {
        [Command("xpath", RunMode = RunMode.Async)]
        public async Task TestXpath(string url, string xpath)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes(xpath);
            string nodeTexts = "";
            if (nodes == null || nodes.Count == 0)
            {
                await ReplyAsync("", embed: Utility.ResultFeedback(
                    Utility.RedFailiureEmbed, 
                    Utility.SuccessLevelEmoji[2], 
                    "Couldn't find any nodes")
                .Build());
            }
            foreach (var node in nodes)
            {
                nodeTexts += node.InnerText + "\n";
            }

            await ReplyAsync($"```\n{nodeTexts}\n```");
        }
    }
}