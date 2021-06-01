using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Core.Commands
{
    public class Rolecommands : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"the latency of the bot is: {ctx.Client.Ping}");
        }

        [Command("role")]

        public static async Task Role(CommandContext ctx, ulong roleid)
        {
            try
            {
                ulong[] idblacklist = new ulong[] { 780616204582125568, 780636673905393714, 780640589863190549, 781207638268313620, 755833569523400814, 780640331829084200, 781050663122501642, 780609809917018123, 780493862024118313, 790003093923430400, 780605289283190794, 782437460725989428, 791822492901441548, 808202582883172373,798609227366662145, 780636673905393714 };

                for (int i = 0; i < idblacklist.Length; i++)
                {
                    if(roleid == idblacklist[i])
                    {
                        await ctx.Channel.SendMessageAsync("You cannot obtain this role via bot command, ask a moderator or the owner");
                        return;
                    }
                }

                var role = ctx.Guild.GetRole(roleid);
                var reactemoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                var noemoji = DiscordEmoji.FromName(ctx.Client, ":x:");
                var bot = await ctx.Guild.GetMemberAsync(791803142341656597);

                var author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = bot.Username,
                    IconUrl = bot.AvatarUrl
                };

                var embed = new DiscordEmbedBuilder
                {
                    Author = author,
                    Description = $"Would you like to have the role {role.Name}?, if so then react with the smh emoji, if no react with X",
                    Color = role.Color,
                    Title = $"assign role {role.Name}?",
                };

                var message = await ctx.Channel.SendMessageAsync(embed: embed);
                await message.CreateReactionAsync(reactemoji);
                await message.CreateReactionAsync(noemoji);

                var interactivity = ctx.Client.GetInteractivity();
                var result = await interactivity.WaitForReactionAsync(x => x.Message == message && x.Emoji == reactemoji || x.Emoji == noemoji && x.User == ctx.Member).ConfigureAwait(false);

                if(result.Result.Emoji == reactemoji)
                {
                    await ctx.Member.GrantRoleAsync(role);
                    await message.DeleteAsync();
                }
                else if (result.Result.Emoji == noemoji)
                {
                    await ctx.Member.RevokeRoleAsync(role);
                    await message.DeleteAsync();
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("It seems something went wrong, please try the command again!");
                    await message.DeleteAsync();
                }
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(e.ToString());
            }
        }

        [Command("Roleinfo")]

        public static async Task Roleinfo(CommandContext ctx, ulong roleid)
        {
            try
            {
                DiscordRole role = ctx.Guild.GetRole(roleid);
                var bot = await ctx.Guild.GetMemberAsync(791803142341656597);
                var author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "homieBot",
                    IconUrl = bot.AvatarUrl
                };

                var embed = new DiscordEmbedBuilder
                {
                    Author = author,

                    Title = $"info obtained from the role: {role.Name}",

                    Description = $"The name of the role is: {role.Name},\n\n " +
                    $"The role is mentionable: {role.IsMentionable},\n\n " +
                    $"The Id of the role is: {role.Id},\n\n " +
                    $"The role is hoisted: {role.IsHoisted},\n\n " +
                    $"The permissions of this role are {role.Permissions.ToPermissionString()},\n\n " +
                    $"The position of this role in the role hierarchy is: {role.Position}",

                    Color = role.Color,
                };

                var message = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(e.ToString());
            }
        }
    }
}
