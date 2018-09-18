using System;
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
            HtmlDocument doc;
            HtmlNodeCollection nodes;
            try
            {
                doc = web.Load(url);
                nodes = doc.DocumentNode.SelectNodes(xpath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await ReplyAsync("", embed: Utility.ResultFeedback(
                        Utility.RedFailiureEmbed, 
                        Utility.SuccessLevelEmoji[2], 
                        "Must Result in Node set. Maybe access to page is denied or the XPath is wrong!")
                    .Build());
                return;
            }
            
            string nodeTexts = "";
            if (nodes == null || nodes.Count == 0)
            {
                await ReplyAsync("", embed: Utility.ResultFeedback(
                    Utility.RedFailiureEmbed, 
                    Utility.SuccessLevelEmoji[2], 
                    "Couldn't find any nodes")
                .Build());
                return;
            }
            foreach (var node in nodes)
            {
                nodeTexts += node.InnerText + "\n";
            }

            if (nodeTexts.Length < 2000)
            {
                await ReplyAsync($"```\n{nodeTexts}\n```");
            }
            else
            {
                var tmp1 = nodeTexts.Substring(0, 1990);
                await ReplyAsync($"```\n{tmp1}\n```");
                int remaining = nodeTexts.Length - 1990;
                if (remaining <= 1990)
                {
                    var tmp2 = nodeTexts.Substring(1990, remaining);
                    await ReplyAsync($"```\n{tmp2}\n```");
                }
                else
                {
                    var tmp2 = nodeTexts.Substring(1990, 1990);
                    await ReplyAsync($"```\n{tmp2}\n```");
                    var tmp3 = nodeTexts.Substring((1990+1990), nodeTexts.Length -(1990*2));
                    await ReplyAsync($"```\n{tmp3}\n```");
                }
            }
        }
    }
}