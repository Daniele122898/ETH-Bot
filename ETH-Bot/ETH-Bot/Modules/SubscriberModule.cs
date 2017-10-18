using System.Threading.Tasks;
using Discord.Commands;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class SubscriberModule : ModuleBase<SocketCommandContext>
    {
        private SubscribeService _subscribeService;

        public SubscriberModule(SubscribeService service)
        {
            _subscribeService = service;
        }

        [Command("subscribe"), Alias("sub")]
        public async Task Subscribe()
        {
            await _subscribeService.Subscribe(Context);
        }
    }
}