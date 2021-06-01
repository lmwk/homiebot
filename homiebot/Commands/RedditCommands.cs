using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using Reddit;
using DSharpPlus.Entities;

namespace Core.Commands
{
    public class RedditCommands : BaseCommandModule
    {

        [Command("reddit")]
        [Description("This is a command that pulls a post in hot from a specified subreddit. Just do ??reddit <subreddit name> and in 1-2 seconds you'll see a post sent in an embed")]
        [Cooldown(60, 600, CooldownBucketType.Global)]

        public async Task getPost(CommandContext ctx, string subredditname)
        {
            try
            {
                var config = bot.configJson;

                var random = new Random();

                var r = new RedditClient(config.appId, config.Refresh, config.secret, config.access);

                var subreddit = r.Subreddit(subredditname).About();

                var post = subreddit.Posts.Hot[random.Next(0, 100)];

                var embed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = post.Listing.Author,
                    },
                };
                var m1 = await new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
                var m2 = await new DiscordMessageBuilder()
                    .WithContent(post.Listing.URL)
                    .SendAsync(ctx.Channel);
                await ctx.Message.DeleteAsync();
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

    }
}
