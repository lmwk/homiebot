using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Core.Commands
{
    class UserCommands : BaseCommandModule
    {

        [Command("user")]

        public async Task Getinfo(CommandContext ctx, ulong userid)
        {
            try
            {
                var user = await ctx.Guild.GetMemberAsync(userid);
                var thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl, Height = 80, Width = 80 };
                var smh = DiscordEmoji.FromGuildEmote(ctx.Client, 791819587464200212);

                var author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = user.Username,
                };
                var embed = new DiscordEmbedBuilder
                {
                    Author = author,
                    Color = user.Color,
                    Title = $"user info of {user.Username}",
                    Thumbnail = thumbnail,
                    Description = $"**Id**\n{user.Id}\n\n" +
                    $"**Joined**\n" +
                    $"Joined on: {user.JoinedAt.ToLocalTime()}\n\n" +
                    $"**Account created**\n" +
                    $"This account was created on: {user.CreationTimestamp.ToLocalTime()}\n\n" +
                    $"Started boosting: {user.PremiumSince.ToString()}\n\n" +
                    $"Joined server at: {user.JoinedAt.ToString()}"
                    
                };
                await ctx.Channel.SendMessageAsync(embed: embed);

                var message = await new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
                await user.SendMessageAsync($"{ctx.Member.Username} is stalking you {smh.Url}");

            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(e.ToString());
            }
        }

    }
}
