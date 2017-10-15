using System.Threading.Tasks;
using Discord.Commands;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class ScraperModule : ModuleBase<SocketCommandContext>
    {
        private DownloadService _downloadService;

        public ScraperModule(DownloadService service)
        {
            _downloadService = service;
        }
        
        [Command("discmath"), Alias("disc")]
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