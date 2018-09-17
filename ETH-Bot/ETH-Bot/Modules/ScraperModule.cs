using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ETH_Bot.Services;
using Microsoft.EntityFrameworkCore.Internal;

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
                semester.Classes.ForEach(x=> classes += x.Name+"\n");
                if (string.IsNullOrWhiteSpace(classes)) continue;
                eb.AddField(x =>
                {
                    x.IsInline = true;
                    x.Name = semester.Name;
                    x.Value = classes;
                });
            }

            await ReplyAsync("", embed: eb.Build());
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
