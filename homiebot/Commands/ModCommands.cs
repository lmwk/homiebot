using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Core.Services;
using DSharpPlus;

namespace Core.Commands
{
    class ModCommands : BaseCommandModule
    {
        private IInfractionService _infractionService;
        private IGuildService _guildService;

        public ModCommands(IInfractionService infractionService, IGuildService guildService)
        {
            _infractionService = infractionService;
            _guildService = guildService;
        }
        
        [Command("mute")]

        public async Task Mute(CommandContext ctx, ulong id,TimeSpan timeSpan, [RemainingText] string reason)
        {
            try
            {
                var member = await ctx.Guild.GetMemberAsync(id);
                var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageRoles);
                var guilddata = await _guildService.GetGuild(ctx.Guild.Id);
                if (caller)
                {
                    var role = ctx.Guild.GetRole(guilddata.muteroleid);
                    await member.GrantRoleAsync(role, reason);
                    await ctx.Channel.SendMessageAsync($"{member.Username} was muted by {ctx.Member.Nickname}");
                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{member.Username} was muted by {ctx.Member.Nickname} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };
                    await _infractionService.CreateInfraction(reason, "TempMute", id, ctx.Guild.Id,
                        DateTime.Now.Add(timeSpan));
                    await modlog(ctx, embed);
                    await member.SendMessageAsync(embed: embed);
                }else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }
        
        [Command("warn")]
        
        public async Task Warn(CommandContext ctx, ulong id, [RemainingText] string reason)
        {
            try
            {
                var member = await ctx.Guild.GetMemberAsync(id);
                var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageMessages);
                if (caller)
                {
                    var role = ctx.Guild.GetRole(779218379713544202);
                    await member.GrantRoleAsync(role, reason);
                    await ctx.Channel.SendMessageAsync($"{member.Username} was warned by {ctx.Member.Nickname}");
                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{member.Username} was warned by {ctx.Member.Username} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };
                    await _infractionService.CreateInfraction(reason, "Warn", id, ctx.Guild.Id, DateTime.Now.Add(TimeSpan.FromDays(365)));
                    await modlog(ctx, embed);
                    await member.SendMessageAsync(embed: embed);
                }else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

        [Command("bonk")]
        [RequirePermissions(Permissions.BanMembers)]

        public async Task ban(CommandContext ctx, ulong id, [RemainingText] string reason)
        {
            try
            {
                var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.BanMembers);
                var member = await ctx.Guild.GetMemberAsync(id);
                if (caller) {
                    await ctx.Guild.BanMemberAsync(member, 0, reason);
                    await ctx.Channel.SendMessageAsync($"{member.Username} was banned by {ctx.Member.Nickname}");
                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{member.Username} was banned by {ctx.Member.Nickname} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };
                    await modlog(ctx, embed);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

        [Command("unbonk")]
        [RequirePermissions(Permissions.BanMembers)]
        
        public async Task unban(CommandContext ctx, ulong id, [RemainingText] string reason)
        {
            try
            {
                var user = await ctx.Client.GetUserAsync(id);
                var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.BanMembers);

                if (caller)
                {
                    await user.UnbanAsync(ctx.Guild, reason);

                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{user.Username} was unbanned by {ctx.Member.Nickname} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };

                    await modlog(ctx, embed);
                }else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

        [Command("kick")]
        [RequirePermissions(Permissions.KickMembers)]

        public async Task kick(CommandContext ctx, ulong id, [RemainingText] string reason)
        {
            var person = await ctx.Guild.GetMemberAsync(id);
            var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.KickMembers);
            try
            {

                if (caller)
                {
                    await person.RemoveAsync(reason);

                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{person.Username} was kicked by {ctx.Member.Nickname} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };

                    await modlog(ctx, embed);
                    
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

        [Command("bbonk")]

        public async Task bbonk(CommandContext ctx, ulong id, TimeSpan timeSpan, [RemainingText] string reason)
        {
            try
            {
                var member = await ctx.Guild.GetMemberAsync(id);
                var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageRoles);
                var guilddata = await _guildService.GetGuild(ctx.Guild.Id);
                if (caller)
                {
                    var role = ctx.Guild.GetRole(guilddata.botbanroleid);
                    await member.GrantRoleAsync(role, reason);
                    await ctx.Channel.SendMessageAsync($"{member.Username} was muted by {ctx.Member.Nickname}");
                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = ctx.Member.DisplayName,
                        },
                        Description = $"{member.Username} was bot banned by {ctx.Member.Nickname} because \n`{reason}`",
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                        {
                            Height = 300,
                            Width = 300,
                            Url = ctx.Member.AvatarUrl
                        },
                        Color = DiscordColor.DarkRed,
                        Timestamp = ctx.Message.Timestamp
                    };
                    await _infractionService.CreateInfraction(reason, "BotBan", id, ctx.Guild.Id,
                        DateTime.Now.Add(timeSpan));
                    await modlog(ctx, embed);
                    await member.SendMessageAsync(embed: embed);
                }else
                {
                    await ctx.Channel.SendMessageAsync("You do not have the permissions required to use this command");
                }
            }catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e.ToString()}`");
            }
        }

        [Command("c")]
        [RequirePermissions(Permissions.ManageMessages)]

        public async Task clear(CommandContext ctx, int amount = 10, [RemainingText] string reason = "not specified")
        {
            var caller = ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageMessages);
            if (caller) {
                var messages = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, amount);
                await ctx.Channel.DeleteMessagesAsync(messages, reason);
                await ctx.Message.DeleteAsync("Clear command");
            }
        }

        public async Task modlog(CommandContext ctx, DiscordEmbedBuilder embed)
        {
            var modlog = ctx.Guild.GetChannel(796087001630900268);
            await modlog.SendMessageAsync(embed:embed);
        }
    }
}
