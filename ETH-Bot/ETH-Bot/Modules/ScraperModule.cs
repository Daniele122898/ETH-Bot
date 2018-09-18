using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ETH_Bot.Data.Entities.SubEntities;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class ScraperModule : ModuleBase<SocketCommandContext>
    {
        private DownloadService _downloadService;
        private SemesterService _semesterService;

        public ScraperModule(DownloadService service, SemesterService semesterService)
        {
            _downloadService = service;
            _semesterService = semesterService;
        }

        [Command("available"), Alias("av")]
        public async Task AvailableScrapes()
        {
            var av = _semesterService.SemesterData;
            var eb = new EmbedBuilder()
            {
                Color = Utility.BlueInfoEmbed,
                Title = "All Available Semesters and Classes",
                Footer = Utility.RequestedBy(Context.User),
                ThumbnailUrl = Utility.EthLogo,
            };
            foreach (var semester in av)
            {
                string classes = "";
                semester.Lectures.ForEach(x=> classes += x.Name+"\n");
                if (string.IsNullOrWhiteSpace(classes)) continue;
                eb.AddField(x =>
                {
                    x.IsInline = false;
                    x.Name = semester.Name;
                    x.Value = classes;
                });
            }

            await ReplyAsync("", embed: eb.Build());
        }

        [Command("scrape"), Alias("s")]
        public async Task Scrape(string semester, [Remainder] string course)
        {
            course = course.Trim();
            // find shit in saved jsons
            var av = _semesterService.SemesterData;
            bool foundSem = false;
            bool foundClass = false;
            foreach (var semester1 in av)
            {
                if (!semester1.Name.Equals(semester, StringComparison.OrdinalIgnoreCase)) continue;
                foundSem = true;
                foreach (var @class in semester1.Lectures)
                {
                    if (!@class.Name.Equals(course, StringComparison.OrdinalIgnoreCase) && @class.Alias.All(x=> !x.Equals(course, StringComparison.OrdinalIgnoreCase) )) 
                        continue;

                    foundClass = true;
                    // scrape class
                    ScraperData data = new ScraperData();
                    try
                    {
                        data = ScraperService.Scrape(@class.Url, @class.Xpath, @class.Exercise, @class.Solution,
                            @class.HasExercise, @class.HasSolution);
                    }
                    catch (Exception)
                    {
                        await ReplyAsync("", embed: Utility.ResultFeedback(
                                Utility.RedFailiureEmbed,
                                Utility.SuccessLevelEmoji[2], 
                                "The xPath or URL is wrong in the JSON file. Please report!")
                            .Build());
                        return;
                    }
                    
                    var eb = new EmbedBuilder()
                    {
                        Color = Utility.ETHBlue,
                        Title = @class.Name,
                        Footer = Utility.RequestedBy(Context.User),
                        Description = "These are all the exercises and solutions.",
                        ThumbnailUrl = Utility.EthLogo,
                        Url = @class.Url
                    };
                    
                    for (int i = 0; i < data.Exercises.Count; i++)
                    {
                        var exLink = (@class.Relative ? 
                                         (string.IsNullOrWhiteSpace(@class.Addition) ? 
                                             @class.Url : 
                                             @class.Url.Replace(@class.Addition, "")) : "") 
                                     + data.Exercises[i].Attributes["href"].Value;
                        string solLink = null;
                        if (i < data.Solutions.Count)
                        {
                            solLink = (@class.Relative ? 
                                          (string.IsNullOrWhiteSpace(@class.Addition) ? 
                                              @class.Url :
                                              @class.Url.Replace(@class.Addition, "")) : "") 
                                      + data.Solutions[i].Attributes["href"].Value;
                        }
                        eb.AddField(x =>
                        {
                            x.Name = $"Exercise {i}";
                            x.IsInline = true;
                            x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                        });
                    }

                    await ReplyAsync("", embed: eb.Build());
                }
            }

            if (!foundSem || !foundClass)
            {
                await ReplyAsync("", embed: Utility.ResultFeedback(
                    Utility.RedFailiureEmbed,
                    Utility.SuccessLevelEmoji[2], 
                    "Couldn't find Semester or Class!")
                .Build());
            }
        }
        
        [Command("discmath"), Alias("disc", "disk", "discmat", "diskmat", "diskmath")]
        public async Task DiscMath()
        {
            await _downloadService.DownloadDiscMath(Context);
        }
        
        [Command("algdat"), Alias("alg" ,"dat", "a&d" ,"ad")]
        public async Task AlgDat()
        {
            await _downloadService.DownloadAlgDat(Context);
        }
        
        [Command("eprog")]
        public async Task Eprog()
        {
            await _downloadService.DownloadEprog(Context);
        }
        
        [Command("linalg"), Alias("lin")]
        public async Task LinAlg()
        {
            await _downloadService.DownloadLinAlg(Context);
        }
    }
}
