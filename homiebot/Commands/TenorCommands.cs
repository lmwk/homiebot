using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using TenorSharp;
using TenorSharp.Enums;

namespace Core.Commands
{
    public class TenorCommands : BaseCommandModule
    {
        string key = bot.configJson.tenorkey;
        Random rand = new Random();

        [Command("tenor")]
        [Cooldown(100, 3600, CooldownBucketType.Global)]

        public async Task tenorsearch(CommandContext ctx, string searchterm)
        {
            try
            {
                var client = new TenorClient(key, mediaFilter0: MediaFilter.off);
                var result = client.Search(searchterm).GifResults[rand.Next(0, 20)];
                await ctx.Channel.SendMessageAsync(result.Url.AbsoluteUri);
                await ctx.Message.DeleteAsync();
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(e.ToString());
            }
        }
    }
}
