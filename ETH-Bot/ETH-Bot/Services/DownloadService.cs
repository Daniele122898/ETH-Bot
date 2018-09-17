using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;

namespace ETH_Bot.Services
{
    public class DownloadService
    {

        public async Task DownloadLinAlg(SocketCommandContext context)
        {
            string url = "http://igl.ethz.ch/teaching/linear-algebra/la2017/";
            var scraperData = ScraperService.ScrapeLinAlg(url);

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Linear Algebra",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions.",
                ThumbnailUrl = Utility.EthLogo,
                Url = "http://igl.ethz.ch/teaching/linear-algebra/la2017/"
            };

            for (int i = 0; i < scraperData.Exercises.Count; i++)
            {
                var exLink = url+scraperData.Exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < scraperData.Solutions.Count)
                {
                    solLink = url+scraperData.Solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        public async Task DownloadEprog(SocketCommandContext context)
        {
            var scraperData = ScraperService.ScrapeEprog();
            
            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Introduction to Programming",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises. Since this scraper does not include the extra materials [click here to view them](http://www.lst.inf.ethz.ch/education/einfuehrung-in-die-programmierung-i--252-0027-.html)",
                ThumbnailUrl = Utility.EthLogo,
                Url = "http://www.lst.inf.ethz.ch/education/einfuehrung-in-die-programmierung-i--252-0027-.html"
            };

            for (int i = 0; i < scraperData.Exercises.Count; i++)
            {
                var exLink = scraperData.Exercises[i].Attributes["href"].Value;
                
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink})";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        public async Task DownloadAlgDat(SocketCommandContext context)
        {
            string url = "https://www.cadmo.ethz.ch/education/lectures/HS17/DA/";

            var scraperData = ScraperService.ScrapeAlgDat(url);

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Algorithms and Datastructures",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions. To also get the coding exercises [click here](https://www.cadmo.ethz.ch/education/lectures/HS17/DA/index.html)",
                ThumbnailUrl = Utility.EthLogo,
                Url = "https://www.cadmo.ethz.ch/education/lectures/HS17/DA/index.html"
            };

            for (int i = 0; i < scraperData.Exercises.Count; i++)
            {
                var exLink = url+scraperData.Exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < scraperData.Solutions.Count)
                {
                    solLink = url+scraperData.Solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb.Build());
        }
        public async Task DownloadDiscMath(SocketCommandContext context)
        {
            string url = "http://www.crypto.ethz.ch/teaching/lectures/DM17/";

            var scraperData = ScraperService.ScrapeDiscMath(url);

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Discrete Math",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions",
                ThumbnailUrl = Utility.EthLogo,
                Url = "http://www.crypto.ethz.ch/teaching/lectures/DM17/"
            };

            for (int i = 0; i < scraperData.Exercises.Count; i++)
            {
                //href += $"{url+node.Attributes["href"].Value} \n";
                var exLink = url+scraperData.Exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < scraperData.Solutions.Count)
                {
                    solLink = url+scraperData.Solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb.Build());
        }
    }
}