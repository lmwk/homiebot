using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Core.Commands
{
    public class PollCommand : BaseCommandModule
    {
        [Command("poll")]

        public async Task CreatePoll(CommandContext ctx,TimeSpan timeSpan, params DiscordEmoji[] Emojioptions)
        {
            try
            {
                var interactivity = ctx.Client.GetInteractivity();

                var embed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = ctx.Member.DisplayName,
                    },
                    Color = ctx.Member.Color,
                    Title = "Poll",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = ctx.Member.AvatarUrl,
                        Height = 400,
                        Width = 400
                    },
                };
                await ctx.Channel.SendMessageAsync("What is the description you would like?");
                var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel & x.Author == ctx.User);

                embed.Description = message.Result.Content;
                var PollEmbed = await ctx.Channel.SendMessageAsync(embed: embed);

                for (int i = 0; i < Emojioptions.Length; i++)
                {
                    await PollEmbed.CreateReactionAsync(Emojioptions[i]);
                }

                var result = await interactivity.CollectReactionsAsync(PollEmbed, timeSpan);

                var results = result.Select(x => $"{x.Emoji}: {x.Total}");

                var resuls = string.Join("\n", results);

                for (int i = 0; i < Emojioptions.Length; i++)
                {
                    await PollEmbed.DeleteReactionsEmojiAsync(Emojioptions[i]);
                }

                embed.Description = $"Poll Ended: {message.Result.Content}";
                await PollEmbed.DeleteAsync();
                var Pollend = await ctx.Channel.SendMessageAsync(embed: embed);
                await ctx.Channel.SendMessageAsync($"The results for {Pollend.JumpLink}\n{resuls}");
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.Message}`");
            }
        }
    }
}
